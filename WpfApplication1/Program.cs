namespace WpfApplication1
{

    public class Program
    {
        public static string capteurfilepath = "c:\\ecole\\capteurs.xtim";
        public static string netatmoDirPath = "c:\\ecole\\netatmo\\";
        public static string entreeDirPath = "c:\\ecole\\entree\\";
        public static string salleDirPath = "c:\\ecole\\salle\\";

        public static void init()
        {
            TreatmentData treatment = new TreatmentData();
            treatment.loadCapteurs();
            treatment.loadAllEntrees();
        }
    }
}