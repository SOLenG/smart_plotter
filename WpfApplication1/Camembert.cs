using System;
using System.Collections;
using System.Collections.Generic;
using OxyPlot;
using OxyPlot.Series;

namespace HomePlotter
{
    internal class Camembert
    {

        public PlotModel ModelP1 { get; set; }

        public string SourceImg { get; set; }

        public Camembert()
        {
            ModelP1 = new PlotModel { Title = "Camembert" };
            SourceImg = Program.ImageHouse;
            var serieEmpty = EmptyCamembert();

            ModelP1.Series.Add(serieEmpty);

        }

        public Camembert(ICollection dateWeek)
        {
            ModelP1 = new PlotModel { Title = "Camembert" };
            SourceImg = Program.ImageHouse;

            if (dateWeek.Count <= 0)
            {
                var serieEmpty = EmptyCamembert();

                ModelP1.Series.Add(serieEmpty);
                return;
            }

            dynamic seriesP1 = new PieSeries { StrokeThickness = 2.0, InsideLabelPosition = 0.8, AngleSpan = 360, StartAngle = 0, };
            var datas = new Dictionary<string, double>();

            foreach (var day in dateWeek)
            {
                foreach (var room in TreatmentData.PresenceByRoomHouresDictionary[day.ToString()].Keys)
                {
                    var roomTime = TreatmentData.PresenceByRoomHouresDictionary[day.ToString()][room];
                    
                    if (datas.ContainsKey(room))
                    {
                        datas[room] = datas[room] + roomTime;
                    }
                    else
                    {
                        datas.Add(room, roomTime);
                    }

                }
            }

            foreach (var room in datas.Keys)
            {
                seriesP1.Slices.Add(new PieSlice(room, ((datas[room] * 5.0 / 60.0) / dateWeek.Count)) {IsExploded = true });
            }
            
            ModelP1.Series.Add(seriesP1);

        }

        private static PieSeries EmptyCamembert()
        {
            dynamic seriesP1 = new PieSeries { StrokeThickness = 2.0, InsideLabelPosition = 0.8, AngleSpan = 360, StartAngle = 0 };

            seriesP1.Slices.Add(new PieSlice("no data", 1) { IsExploded = false, Fill = OxyColors.PaleVioletRed });

            return seriesP1;
        }
    }
}