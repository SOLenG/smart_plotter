using System.Collections.Generic;

namespace HomePlotter
{
    public struct Capteur
    {
        public string Type;
        public string Id;
        public string Description;
        public string Box;
        public string Lieu;
        public Grandeur Grandeur;
        public Valeur Valeur;
        public List<Seuil> Seuils;
        
    }

    public struct Grandeur
    {
        public string Name;
        public string Unite;
        public string Abreviation;
    }

    public struct Valeur
    {
        public string Type;
        public string Min;
        public string Max;
    }

    public struct Seuil
    {
        public string Description;
        public string Valeur;
    }
}
