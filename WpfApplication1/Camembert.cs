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
            var dateWeek = new List<string> {date.ToString("yyyyMMdd")};

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

            ModelP1 = new PlotModel {Title = "Camembert"};
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
            checkVariation(dateWeeks);

            /**
             * Set le block le moyenne de temps passé dans les pieces dans la semaine selectyionné
             */
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

        /**
         * Attend un List<List<string>> de 3 element minimum
         */

        private static void checkVariation(IReadOnlyList<List<string>> weeksList)
        {
            if (weeksList.Count < 3)
                return;


            for (var i = 0; i < 7; i++)
            {
                var datePrev = weeksList[0][i];
                var dateCur = weeksList[1][i];
                var dateNext = weeksList[2][i];
                var dates = new List<string>
                {
                    datePrev,
                    dateCur,
                    dateNext
                };
                var variations = new List<Dictionary<string, double>>();
                var rooms = new List<string>();
                var j = 0;
                foreach (var day in dates)
                {
                    variations.Add(new Dictionary<string, double>());
                    foreach (var room in TreatmentData.PresenceByRoomHouresDictionary[day].Keys)
                    {
                        if (!rooms.Contains(room))
                            rooms.Add(room);
                    }
                    j++;
                }

                foreach (var room in rooms)
                {
                    var val1 = 0.0;
                    var val2 = 0.0;
                    var val3 = 0.0;

                    if (TreatmentData.PresenceByRoomHouresDictionary.ContainsKey(datePrev) &&
                        TreatmentData.PresenceByRoomHouresDictionary[datePrev].ContainsKey(room))
                    {
                        val1 = TreatmentData.PresenceByRoomHouresDictionary[datePrev][room];
                    }
                    if (TreatmentData.PresenceByRoomHouresDictionary.ContainsKey(dateCur) &&
                        TreatmentData.PresenceByRoomHouresDictionary[dateCur].ContainsKey(room))
                    {
                        val2 = TreatmentData.PresenceByRoomHouresDictionary[dateCur][room];
                    }
                    if (TreatmentData.PresenceByRoomHouresDictionary.ContainsKey(dateNext) &&
                        TreatmentData.PresenceByRoomHouresDictionary[dateNext].ContainsKey(room))
                    {
                        val3 = TreatmentData.PresenceByRoomHouresDictionary[dateNext][room];
                    }

                    if (val1/val2*100 >= 20.0)
                    {
                        Console.Write(0);
                    }
                    if (val2/val3*100 >= 20.0)
                    {
                        Console.Write(2);
                    }
                    if (val3/val1*100 >= 20.0)
                    {
                        Console.Write(4);
                    }
                }
            }
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