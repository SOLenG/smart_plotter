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
        public string Salle { get; set; }
        public string Chambre_Beatrice { get; set; }
        public string Chambre_Alain { get; set; }

        public Camembert()
        {
            ModelP1 = new PlotModel {Title = "Camembert"};
            SourceImg = Program.ImageHouse;
            var serieEmpty = EmptyCamembert();

            ModelP1.Series.Add(serieEmpty);
        }

        public List<string> WeekList(DateTime date)
        {
            var dateWeek = new List<string> { date.ToString("yyyyMMdd")};

            for (var i = 0; i < 7; i++)
            {
                dateWeek.Add(date.AddDays(i).ToString("yyyyMMdd"));
            }

            return dateWeek;
        }

        public Camembert(DateTime date)
        {
            var dateWeeks = new List<List<string>>()
            {
                WeekList(date.AddDays(-7)),
                WeekList(date),
                WeekList(date.AddDays(+7))
            };

            var dateWeek = dateWeeks[1];

            ModelP1 = new PlotModel { Title = "Camembert" };
            SourceImg = Program.ImageHouse;

            if (dateWeek.Count <= 0)
            {
                var serieEmpty = EmptyCamembert();

                ModelP1.Series.Add(serieEmpty);
                return;
            }

            var t = new TreatmentData();

            foreach (var week in dateWeeks)
            {
                t.TimePresenceByRoom(week);
            }

            dynamic seriesP1 = new PieSeries
            {
                StrokeThickness = 2.0,
                InsideLabelPosition = 0.8,
                AngleSpan = 360,
                StartAngle = 0,
            };
            var datas = new Dictionary<string, double>();

            foreach (var day in dateWeek)
            {
                foreach (var room in TreatmentData.PresenceByRoomHouresDictionary[day].Keys)
                {
                    var roomTime = TreatmentData.PresenceByRoomHouresDictionary[day][room];

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
                var val = (datas[room]*5.0/60.0)/dateWeek.Count;
                var hours = (int) val;
                var min = (int) ((val - hours)*60);
                var propertyName = room.Replace(" ", "_");
                if (this.GetType().GetProperty(propertyName) != null)
                {
                    Console.WriteLine(this.GetType().GetProperty(propertyName));
                    this.GetType().GetProperty(propertyName).SetValue(this, hours + "h " + min + "min ");
                }
                seriesP1.Slices.Add(new PieSlice(room, val) {IsExploded = true});
            }

            ModelP1.Series.Add(seriesP1);
        }

        private static PieSeries EmptyCamembert()
        {
            dynamic seriesP1 = new PieSeries
            {
                StrokeThickness = 2.0,
                InsideLabelPosition = 0.8,
                AngleSpan = 360,
                StartAngle = 0
            };

            seriesP1.Slices.Add(new PieSlice("no data", 1) {IsExploded = false, Fill = OxyColors.PaleVioletRed});

            return seriesP1;
        }
    }
}