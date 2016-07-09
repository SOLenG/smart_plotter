using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace WpfApplication1
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Program.init();
            
            // for generate the textBox values 
            generateTextBoxSalle();
            // for generate the date
            generateTextBoxDate();
            this.DataContext = new Graph();
        }

        private void generateTextBoxSalle()
        {
            foreach (Capteur capteur in TreatmentData.capteurs)
            {
                comboSalle.Items.Add(capteur.id);
            }
        }

        private void generateTextBoxDate()
        {
            /*string[] list = files.getDateDirecotry();
            foreach (string file in list)
            {
                comboDate.Items.Add(file);
            }*/
            foreach (Capteur capteur in TreatmentData.capteurs)
            {
                if (!TreatmentData.dicNetatmos.ContainsKey(capteur.id)) { continue; }
                

                foreach (Netatmo netatmo in TreatmentData.dicNetatmos[capteur.id])
                {
                    if (!comboDate.Items.Contains(netatmo.date.ToString("yyyy/MM/dd")))
                        comboDate.Items.Add(netatmo.date.ToString("yyyy/MM/dd"));
                }
               
            }
        }

        private void comboSalle_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            StringBuilder stringBuilder = new StringBuilder(comboDate.Text);
            DataContext = new Graph(stringBuilder, comboSalle.Text);
        }
    }
}
