using System;
using System.Collections;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace HomePlotter
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Program.Init();

            // for generate the textBox values 
            GenerateTextBoxSalle();
            // for generate the date
            GenerateTextBoxDate();
            DataContext = new Graph();
        }

        private void GenerateTextBoxSalle()
        {
            foreach (var capteur in TreatmentData.Capteurs)
            {
                comboSalle.Items.Add(capteur.Id);
            }
        }

        private void GenerateTextBoxDate()
        {
            foreach (var capteur in TreatmentData.Capteurs)
            {
                if (!TreatmentData.NetatmosDictionary.ContainsKey(capteur.Id))
                {
                    continue;
                }

                foreach (var netatmo in TreatmentData.NetatmosDictionary[capteur.Id])
                {
                    if (!ComboDate.Items.Contains(netatmo.Date.ToString("yyyy/MM/dd")))
                        ComboDate.Items.Add(netatmo.Date.ToString("yyyy/MM/dd"));
                }
            }
        }

        private void comboSalle_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            var stringBuilder = new StringBuilder(ComboDate.Text);
            DataContext = new Graph(stringBuilder, comboSalle.Text);
        }

        private void Calendar_SelectedDatesChanged(object sender, SelectionChangedEventArgs e)
        {
            // ... Get reference.
            var calendar = sender as Calendar;

            // ... See if a date is selected.
            if (calendar.SelectedDate.HasValue)
            {

                var date = calendar.SelectedDate.Value;
                Title = date.ToShortDateString();
                DataContext = new Camembert(date);
                return;
            }

            DataContext = new Camembert();
        }

        private void btnCam_Click(object sender, RoutedEventArgs e)
        {
            DataContext = new Camembert();
        }
    }
}