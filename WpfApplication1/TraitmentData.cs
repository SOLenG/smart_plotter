using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace WpfApplication1
{
    using static System.Console;
    using static System.IO.File;

    public class TreatmentData
    {

        private static IEnumerable<Capteur> ListCapteurs = new List<Capteur>();
        public static IEnumerable<Capteur> capteurs {
            get { return ListCapteurs; }
            private set { ListCapteurs = value; }
        }

        private static IEnumerable<Netatmo> Listnetatmos = new List<Netatmo>();
        public static IEnumerable<Netatmo> netatmos {
            get { return Listnetatmos; }
            private set { Listnetatmos = value; }
        }

        private static Dictionary<string, List<Netatmo>> dictionaryNetatmos = new Dictionary<string, List<Netatmo>>();

        public static Dictionary<string, List<Netatmo>> dicNetatmos
        {
            get { return dictionaryNetatmos; }
            private set { dictionaryNetatmos = value; }
        }
        /**
         * 
         */
        public void loadCapteurs()
        {
            String capteurfilepath = Program.capteurfilepath;

            if (!Exists(capteurfilepath))
            {
                WriteLine($"{capteurfilepath} not exist");
                ReadKey(true);
                return;
            }
            

            XDocument xd = XDocument.Load(capteurfilepath);
            //Run query
            ListCapteurs = from data in xd.Descendants("capteur")
                let Grandeur = new Grandeur()
                {
                    abreviation =
                        (data.Element("grandeur") != null)
                            ? data.Element("grandeur").Attribute("abreviation").Value
                            : null,
                    name = (data.Element("grandeur") != null) ? data.Element("grandeur").Attribute("nom").Value : null,
                    unite =
                        (data.Element("grandeur") != null) ? data.Element("grandeur").Attribute("unite").Value : null
                }
                let Valeur = new Valeur()
                {
                    type = (data.Element("valeur") != null) ? data.Element("valeur").Attribute("type").Value : null,
                    max = (data.Element("valeur") != null) ? data.Element("valeur").Attribute("max").Value : null,
                    min = (data.Element("valeur") != null) ? data.Element("valeur").Attribute("min").Value : null
                }
                select new Capteur()
                {
                    id = data.Element("id").Value,
                    type = data.Attribute("type").Value,
                    lieu = data.Element("lieu").Value,
                    description = data.Element("description").Value,
                    box = data.Element("box").Value,
                    grandeur = Grandeur,
                    valeur = Valeur,
                    seuils = (from seuil in data.Descendants("seuils")
                        select new Seuil()
                        {
                            description = seuil.Element("seuil").Attribute("description").Value,
                            valeur = seuil.Element("seuil").Attribute("valeur").Value
                        }).ToList()


                };
        }

        /**
         * 
         */
        public void loadAllEntrees()
        {
            String netatmoDirPath = Program.netatmoDirPath;

            foreach (string filesName in Directory.GetFiles(netatmoDirPath))
            {
                if (Path.GetExtension(filesName) == ".dt")
                {
                    loadDatasNetatmo(filesName);
                }
            }
        }

        /**
         * @var string filepath : chemin absolue du fichier cible
         */
        private void loadDatasNetatmo(string filePath)
        {
            Listnetatmos = from line in ReadLines(filePath)
                           let datas = Regex.Split(line, "(?:^|,)(\"(?:[^\"]+|\"\")*\"|[^,]*)")
                           let _date = Convert.ToDateTime(datas[1].Replace("\"", "")) 
                           let lastDatas = Regex.Split(datas[2], " ")
                           let _id = lastDatas[1]
                           let _value = lastDatas[2]
                       select new Netatmo()
                       {
                           capteurId = _id,
                           date = _date,
                           value = _value
                       };

            foreach (var netatmo in Listnetatmos)
            {
                if (dictionaryNetatmos.ContainsKey(netatmo.capteurId))
                {
                    dictionaryNetatmos[netatmo.capteurId].Add(netatmo);
                }
                else
                {
                    List<Netatmo> dataNetatmo = new List<Netatmo>();
                    dataNetatmo.Add(netatmo);
                    dictionaryNetatmos.Add(netatmo.capteurId, dataNetatmo);
                }
            }
        }
    }
}