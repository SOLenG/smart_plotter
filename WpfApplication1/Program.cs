using System;

namespace HomePlotter
{
    using System.Configuration;

    public class Program
    {
        public static string Capteurfilepath = Environment.CurrentDirectory + ConfigurationManager.AppSettings["capteursPath"];
        public static string NetatmoDirPath = Environment.CurrentDirectory + ConfigurationManager.AppSettings["netatmoPath"];
        public static string EntreeDirPath = Environment.CurrentDirectory + ConfigurationManager.AppSettings["entreePath"];
        public static string SalleDirPath = Environment.CurrentDirectory + ConfigurationManager.AppSettings["sallePath"];

        public static void Init()
        {
            var treatment = new TreatmentData();
            treatment.LoadCapteurs();
            treatment.LoadAllEntrees();
        }
    }
}