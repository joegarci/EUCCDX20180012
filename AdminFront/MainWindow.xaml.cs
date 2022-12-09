using MIA.Helpers;
using MIA.Menu;
using MIA.Model;
using MIA.ViewModel;
using log4net;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using MenuItem = MIA.Menu.MenuItem;
using System.Security.Cryptography;
using System.IO;
using System.Text;
using System.Configuration;

namespace MIA
{
    public partial class MainWindow : Window
    {
        /// <summary>
        /// Log de error de la aplicación: \bin\Debug\MiAsistenteEnProcesos_LogdeErrores.log cuando no esya instalada
        /// //**********************************************************************************************************
        /// Log de error de la aplicación: \bin\Debug\MiAsistenteEnProcesos_LogdeErrores.log cuando esta instalada
        /// C:\Users\UserName\AppData\Local\Apps\2.0\VW342WME.KWK\P051R4JT.BA8\gaap..tion_[GUID ID]
        /// </summary
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private CommonFunctions commonModel;
        public MainWindow()
        {
            this.FontFamily = new FontFamily("Nunito");
            InitializeComponent();
            ValidateDbConnection();
        }
       
        private void ValidateDbConnection()
        {
            try
            {
                commonModel = new CommonFunctions();
                using (ModelDB modeldb = new ModelDB())
                {
                    var dataList = modeldb.Roles.ToList();
                    DataContext = new MainWindowViewModel();

                    if (commonModel.GetUser(Environment.UserName) is null)
                    {
                        MessageBox.Show("No se encuentra el usuario: " + Environment.UserName + " creado en base de datos comuniqueselo al administrador.", "Importante Usuario No Encontrado", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                        App.Current.Shutdown();
                    }
                }
            }
            catch (Exception exc)
            {
                if (exc.InnerException != null && exc.InnerException.InnerException != null)
                {
                    string msg = exc.InnerException.InnerException.Message;
                    Log.Fatal(msg, exc);
                }
                Application.Current.Shutdown();
            }
        }
        private void UIElement_OnPreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            ListBox lstBoxMenu = (ListBox)sender;
            MenuItem menuSelected = (MenuItem)lstBoxMenu.SelectedItems[0];
            Type typeOption = menuSelected.Content.GetType();
            var nameOption = typeOption.Name;

            switch (nameOption)
            {
                case "AdminUsersFront":
                    Views.AdminUsersFront adminUsersFront = (Views.AdminUsersFront)menuSelected.Content;
                    AdminFrontViewModel adminFrontViewModel = new AdminFrontViewModel();
                    adminUsersFront.DataContext = adminFrontViewModel;
                    break;
                case "Dashboard":
                    Views.Dashboard dashboard = (Views.Dashboard)menuSelected.Content;
                    dashboard.Clear();
                    dashboard.Refresh();
                    break;
                case "EmailParameters":
                    Views.EmailParameters email = (Views.EmailParameters)menuSelected.Content;
                    EmailViewModel emailViewModel = new EmailViewModel();
                    email.DataContext = emailViewModel;
                    break;
                case "Tickets":
                    Views.Tickets tickets = (Views.Tickets)menuSelected.Content;
                    TicketViewModel ticketViewModel = new TicketViewModel();
                    tickets.DataContext = ticketViewModel;
                    break;
                case "Users":
                    Views.Users users = (Views.Users)menuSelected.Content;
                    UserViewModel userViewModel = new UserViewModel();
                    users.DataContext = userViewModel;
                    break;
                case "Roles":
                    Views.Roles roles = (Views.Roles)menuSelected.Content;
                    RoleViewModel roleViewModel = new RoleViewModel();
                    roles.DataContext = roleViewModel;
                    break;
                case "RolesUsers":
                    Views.RolesUsers rolesUsers = (Views.RolesUsers)menuSelected.Content;
                    RoleUserViewModel roleUserViewModel = new RoleUserViewModel();
                    rolesUsers.DataContext = roleUserViewModel;
                    break;
                case "AuditTickets":
                    Views.AuditTickets auditTickets = (Views.AuditTickets)menuSelected.Content;
                    AuditTicketViewModel auditTicketViewModel = new AuditTicketViewModel();
                    auditTickets.DataContext = auditTicketViewModel;
                    break;
                case "Parameters":
                    Views.Parameters parameters = (Views.Parameters)menuSelected.Content;
                    RpaParametersViewModel rpaParametersViewModel = new RpaParametersViewModel();
                    parameters.DataContext = rpaParametersViewModel;
                    break;
                default:
                    break;
            }
            DependencyObject dependencyObject = Mouse.Captured as DependencyObject;
            while (dependencyObject != null)
            {
                if (dependencyObject is ScrollBar)
                {
                    return;
                }

                dependencyObject = VisualTreeHelper.GetParent(dependencyObject);
            }
            MenuToggleButton.IsChecked = false;
        }
    }
}
