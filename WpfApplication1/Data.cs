using System;

namespace HomePlotter
{
    public struct Entree
    {
        //-rw-r--r-- 1 maison staff 32143 Mar 12 14:11:09 2014 Images/00626E48B85D(SergeEntree)_1_20140201080803_35.jpg
        public string Rules;
        public string Location;
        public string Name;
        public string Number;
        public string Date;
    }

    public struct Netatmo
    {
        // "31/01/2014 00:03:07" temperaturesalle 21,8
        public DateTime Date;
        public string CapteurId;
        public string Value;
    }

    public struct Salle
    {
        //-rw-r--r-- 1 maison staff 36637 Feb  1 08:38:43 2014 20140201073842/Frame1.jpg
        public string Rules;
        public string Location;
        public string Name;
        public string Number;
        public string Date;
    }
}