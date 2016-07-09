using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;

namespace WpfApplication1
{
    class Graph
    {
        public PlotModel GraphModel { get; set; }

        public Graph()
        {

            /*GraphModel = new PlotModel();
            SetGraphAxesAndTitle();
            GenerateData();
            GraphModel.InvalidatePlot(true);*/ // Notifie au PlotView de reconstruire le graphique
        }

        public Graph(StringBuilder sb, string id)
        {
            GraphModel = new PlotModel();

            string unite = TreatmentData.capteurs.FirstOrDefault(element => element.id == id).grandeur.unite;
            SetGraphAxesAndTitle(id, unite);
            GenerateDataByDay(sb, id);
        }

        private void SetGraphAxesAndTitle(string id = null, string unite = null)
        {
            /** @var Title : Titre du graphique */
            GraphModel.Title = "Graphique général de toutes les données Netatmo récoltés";
            if (id != null && unite != null)
                GraphModel.Title = "Graphique de " + id + " l'unite est " + unite;

            /** Génération Axe X  */
          var abscisse = new DateTimeAxis()
          {
            MajorGridlineStyle = LineStyle.Solid,
            MinorGridlineStyle = LineStyle.Dot,
            IntervalLength = 80,
            Title = "Date",
            Position = AxisPosition.Bottom
          };

            /** Génération Axe Y  */
            LinearAxis ordonnee = new LinearAxis();
            ordonnee.Title = "ppm|%|C°|mm|dB|mbar";
            if (unite != null)
                ordonnee.Title = unite;
            ordonnee.Position = AxisPosition.Left;

            /** Ajout des axes au PlotModel */
            GraphModel.Axes.Add(abscisse);
            GraphModel.Axes.Add(ordonnee);
        }

        private void GenerateData()
        {
            foreach (Capteur capteur in TreatmentData.capteurs)
            {
                if (!TreatmentData.dicNetatmos.ContainsKey(capteur.id)) { continue; }

                LineSeries donnees = new LineSeries();
                
                foreach (Netatmo netatmo in TreatmentData.dicNetatmos[capteur.id])
                {
                    donnees.Points.Add(new DataPoint(DateTimeAxis.ToDouble(netatmo.date), Convert.ToDouble(netatmo.value)));
                }

                GraphModel.Series.Add(donnees);
            }
        }

        public void GenerateDataByDay(StringBuilder st, string id)
        {
            DateTime dt = DateTime.Parse(st.ToString());

            if (TreatmentData.dicNetatmos.ContainsKey(id))
            {

                LineSeries donnees = new LineSeries();
                foreach (Netatmo netatmo in TreatmentData.dicNetatmos[id])
                {
                    if (netatmo.date.Day == dt.Day)
                    {
                        donnees.Points.Add(new DataPoint(DateTimeAxis.ToDouble(netatmo.date), Convert.ToDouble(netatmo.value)));
                    }
                }

                GraphModel.Series.Add(donnees);
            }
        }
    }
}
