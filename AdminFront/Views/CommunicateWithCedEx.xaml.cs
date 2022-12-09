using System.Diagnostics;
using System.Globalization;
using System.Resources;
using System.Windows;
using System.Windows.Controls;

namespace MIA.Views
{
    public partial class CommunicateWithCedEx : UserControl
    {
        public CommunicateWithCedEx()
        {
            InitializeComponent();
        }
        private void OpenTeams_Click(object sender, RoutedEventArgs e)
        {
            var rm = new ResourceManager(typeof(Properties.Resources));
            string message = rm.GetString("URL_Comunicate_Con_Cedex", CultureInfo.CreateSpecificCulture("es-CO"));
            Process.Start(message);
        }
    }
}
