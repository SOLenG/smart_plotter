using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using static System.IO.Path;

namespace HomePlotter
{
    using static Console;
    using static File;

    public class TreatmentData
    {
        public static IEnumerable<Capteur> Capteurs { get; private set; } = new List<Capteur>();

        public static IEnumerable<Netatmo> Netatmos { get; private set; } = new List<Netatmo>();

        public static Dictionary<string, Capteur> CapteurDictionary { get; private set; } =
            new Dictionary<string, Capteur>();

        public static Dictionary<string, List<Netatmo>> NetatmosDictionary { get; private set; } =
            new Dictionary<string, List<Netatmo>>();

        public static Dictionary<string, List<Netatmo>> NetatmosByDateDictionary { get; private set; } =
            new Dictionary<string, List<Netatmo>>();

        public static Dictionary<string, Dictionary<string, double>> PresenceByRoomHouresDictionary { get; private set; } =
            new Dictionary<string, Dictionary<string, double>>();

        /**
         * 
         */

        public void LoadCapteurs()
        {
            var capteurfilepath = Program.Capteurfilepath;

            if (!Exists(capteurfilepath))
            {
                WriteLine($"{capteurfilepath} not exist");
                //ReadKey(true);
                return;
            }


            var xd = XDocument.Load(capteurfilepath);
            //Run query
            Capteurs = from data in xd.Descendants("capteur")
                let grandeur = new Grandeur()
                {
                    Abreviation =
                        (data.Element("grandeur") != null)
                            ? data.Element("grandeur").Attribute("abreviation").Value
                            : null,
                    Name = (data.Element("grandeur") != null) ? data.Element("grandeur").Attribute("nom").Value : null,
                    Unite =
                        (data.Element("grandeur") != null) ? data.Element("grandeur").Attribute("unite").Value : null
                }
                let valeur = new Valeur()
                {
                    Type = (data.Element("valeur") != null) ? data.Element("valeur").Attribute("type").Value : null,
                    Max = (data.Element("valeur") != null) ? data.Element("valeur").Attribute("max").Value : null,
                    Min = (data.Element("valeur") != null) ? data.Element("valeur").Attribute("min").Value : null
                }
                select new Capteur()
                {
                    Id = data.Element("id").Value,
                    Type = data.Attribute("type").Value,
                    Lieu = data.Element("lieu").Value,
                    Description = data.Element("description").Value,
                    Box = data.Element("box").Value,
                    Grandeur = grandeur,
                    Valeur = valeur,
                    Seuils = (from seuil in data.Descendants("seuils")
                        select new Seuil()
                        {
                            Description = seuil.Element("seuil").Attribute("description").Value,
                            Valeur = seuil.Element("seuil").Attribute("valeur").Value
                        }).ToList()
                };
            foreach (var capteur in Capteurs)
            {
                if (CapteurDictionary.ContainsKey(capteur.Id))
                {
                    CapteurDictionary[capteur.Id] = capteur;
                }
                else
                {
                    CapteurDictionary.Add(capteur.Id, capteur);
                }
            }
        }

        public void TimePresenceByRoom(ArrayList dateWeek)
        {
            foreach (var day in dateWeek)
            {
                TimePresenceByRoom(day.ToString());
            }
        }

        public void TimePresenceByRoom(string day)
        {
            if (PresenceByRoomHouresDictionary.ContainsKey(day)) return;

            var netatmos = NetatmosByDateDictionary[day];
            var datas = new Dictionary<string, double>();

            foreach (var netatmo in netatmos)
            {
                if (!CapteurDictionary.ContainsKey(netatmo.CapteurId)) continue;
                if (!IsPresent(netatmo)) continue;
                
                var room = CapteurDictionary[netatmo.CapteurId].Lieu;

                if (datas.ContainsKey(room))
                {
                    datas[room] = datas[room] + 5 / 60;
                }
                else
                {
                    datas.Add(room, 5 / 60);
                }
            }

            PresenceByRoomHouresDictionary[day] = datas;
        }

        private static bool IsPresent(Netatmo netatmo)
        {
            if (!CapteurDictionary.ContainsKey(netatmo.CapteurId) || CapteurDictionary[netatmo.CapteurId].Seuils.Count <= 0 ) return false;

            return Convert.ToDouble(netatmo.Value) >= Convert.ToDouble(CapteurDictionary[netatmo.CapteurId].Seuils.First().Valeur);
        }

        /**
         * 
         */

        public void LoadAllEntrees()
        {
            var netatmoDirPath = Program.NetatmoDirPath;

            try
            {
                foreach (var filesName in Directory.GetFiles(netatmoDirPath))
                {
                    if (GetExtension(filesName) == ".dt")
                    {
                        LoadDatasNetatmo(filesName);
                    }
                }
            }
            catch (DirectoryNotFoundException)
            {
                return;
            }
        }

        /**
         * @var string filepath : chemin absolue du fichier cible
         */

        private static void LoadDatasNetatmo(string filePath)
        {
            Netatmos = from line in ReadLines(filePath)
                let datas = Regex.Split(line, "(?:^|,)(\"(?:[^\"]+|\"\")*\"|[^,]*)")
                let date = Convert.ToDateTime(datas[1].Replace("\"", ""))
                let lastDatas = Regex.Split(datas[2], " ")
                let id = lastDatas[1]
                let value = lastDatas[2]
                where CapteurDictionary.ContainsKey(id) && CapteurDictionary[id].Seuils.Count > 0
                select new Netatmo()
                {
                    CapteurId = id,
                    Date = date,
                    Value = value
                };

            foreach (var netatmo in Netatmos)
            {
                if (NetatmosDictionary.ContainsKey(netatmo.CapteurId))
                {
                    NetatmosDictionary[netatmo.CapteurId].Add(netatmo);
                }
                else
                {
                    var dataNetatmo = new List<Netatmo> {netatmo};
                    NetatmosDictionary.Add(netatmo.CapteurId, dataNetatmo);
                }
                if (NetatmosByDateDictionary.ContainsKey(netatmo.Date.ToString("yyyyMMdd")))
                {
                    NetatmosByDateDictionary[netatmo.Date.ToString("yyyyMMdd")].Add(netatmo);
                }
                else
                {
                    var dataNetatmo = new List<Netatmo> {netatmo};
                    NetatmosByDateDictionary.Add(netatmo.Date.ToString("yyyyMMdd"), dataNetatmo);
                }
            }
        }
    }
}