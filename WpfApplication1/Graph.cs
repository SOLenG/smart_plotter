using System;
using System.Linq;
using System.Text;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;

namespace HomePlotter
{
    internal class Graph
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

            var unite = TreatmentData.Capteurs.FirstOrDefault(element => element.Id == id).Grandeur.Unite;
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
            var ordonnee = new LinearAxis
            {
                Title = "ppm|%|C°|mm|dB|mbar",
                Position = AxisPosition.Left
            };
            if (unite != null)
                ordonnee.Title = unite;

            /** Ajout des axes au PlotModel */
            GraphModel.Axes.Add(abscisse);
            GraphModel.Axes.Add(ordonnee);
        }

        private void GenerateData()
        {
            foreach (var capteur in TreatmentData.Capteurs)
            {
                if (!TreatmentData.NetatmosDictionary.ContainsKey(capteur.Id)) { continue; }

                var donnees = new LineSeries();
                
                foreach (var netatmo in TreatmentData.NetatmosDictionary[capteur.Id])
                {
                    donnees.Points.Add(new DataPoint(DateTimeAxis.ToDouble(netatmo.Date), Convert.ToDouble(netatmo.Value)));
                }

                GraphModel.Series.Add(donnees);
            }
        }

        public void GenerateDataByDay(StringBuilder st, string id)
        {
            if (!TreatmentData.NetatmosDictionary.ContainsKey(id)) return;

            DateTime dt;
            try
            {
                 dt = DateTime.Parse(st.ToString());
            }
            catch (Exception)
            {
                return;
            }

            var donnees = new LineSeries();
            foreach (var netatmo in TreatmentData.NetatmosDictionary[id])
            {
                if (netatmo.Date.Day == dt.Day)
                {
                    donnees.Points.Add(new DataPoint(DateTimeAxis.ToDouble(netatmo.Date), Convert.ToDouble(netatmo.Value)));
                }
            }

            GraphModel.Series.Add(donnees);
        }
    }
}
