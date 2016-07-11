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
            this.DataContext = new Graph();
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
                if (!TreatmentData.DicNetatmos.ContainsKey(capteur.Id)) { continue; }
                
                foreach (var netatmo in TreatmentData.DicNetatmos[capteur.Id])
                {
                    if (!comboDate.Items.Contains(netatmo.Date.ToString("yyyy/MM/dd")))
                        comboDate.Items.Add(netatmo.Date.ToString("yyyy/MM/dd"));
                }
               
            }
        }

        private void comboSalle_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            var stringBuilder = new StringBuilder(comboDate.Text);
            DataContext = new Graph(stringBuilder, comboSalle.Text);
        }
    }
}
