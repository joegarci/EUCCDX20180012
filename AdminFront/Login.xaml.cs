using log4net;
using MIA.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace MIA
{
    /// <summary>
    /// Lógica de interacción para Login.xaml
    /// </summary>
    public partial class Login : Window
    {
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private const string message = "Se presento un error controlado comuniquese con el administrador.";
        public Login()
        {
            InitializeComponent();
            // Setear nombre de usuario actual.
            tbUsername.Text = Environment.UserName;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(pbPassword.Password))
            {
                _ = MessageBox.Show("Por favor digite la contraseña para continuar.", "Mensaje Informativo", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
            else
            {
                try
                {
                    // Consumo de API
                    bool isValid = Authentication.ValidateLogin(tbUsername.Text, pbPassword.Password, out string error);
                    // Validación de respuesta
                    if (!string.IsNullOrEmpty(error) || !isValid)
                    {
                        if (!string.IsNullOrEmpty(error))
                        {
                            Log.Fatal("Se ha presentado un error al realizar la validación de directorio activo a través de la API. " + error);
                        }
                        _ = MessageBox.Show("La autenticación no fue correcta para el usuario " + Environment.UserName, "Mensaje Informativo", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    else
                    {
                        // Si la validación es correcta se procede con el inicio de la aplicación.
                        OnStartup();
                    }

                }
                catch (Exception ex)
                {
                    Log.Fatal("Se ha presentado un error al realizar la validación de directorio activo a través de la API.", ex);
                    _ = MessageBox.Show(message, "Mensaje Informativo", MessageBoxButton.OK, MessageBoxImage.Error);
                    Application.Current.Shutdown();
                }
            }
        }

        /// <summary>
        /// Inicia la aplicación.
        /// </summary>
        protected void OnStartup()
        {
            //initialize the splash screen and set it as the application main window
            SplashScreen splashScreen = new SplashScreen();
            Application.Current.MainWindow = splashScreen;
            splashScreen.Show();

            //in order to ensure the UI stays responsive, we need to
            //do the work on a different thread
            Task.Factory.StartNew(() =>
            {
                //simulate some work being done
                System.Threading.Thread.Sleep(3000);

                //once we're done we need to use the Dispatcher
                //to create and show the main window
                Dispatcher.Invoke(() =>
                {
                    //initialize the main window, set it as the application main window
                    //and close the splash screen
                    MainWindow mainWindow = new MainWindow();
                    Application.Current.MainWindow = mainWindow;
                    mainWindow.Show();
                    Close();
                    splashScreen.Close();
                });
            });
        }

        /// <summary>
        /// Si el usuario lo desea puede cancelar el inicio de sesión.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}
