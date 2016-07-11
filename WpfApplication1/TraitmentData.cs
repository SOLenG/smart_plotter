using System;
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

        public static Dictionary<string, List<Netatmo>> DicNetatmos { get; private set; } = new Dictionary<string, List<Netatmo>>();

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
                       select new Netatmo()
                       {
                           CapteurId = id,
                           Date = date,
                           Value = value
                       };

            foreach (var netatmo in Netatmos)
            {
                if (DicNetatmos.ContainsKey(netatmo.CapteurId))
                {
                    DicNetatmos[netatmo.CapteurId].Add(netatmo);
                }
                else
                {
                    var dataNetatmo = new List<Netatmo> {netatmo};
                    DicNetatmos.Add(netatmo.CapteurId, dataNetatmo);
                }
            }
        }
    }
}