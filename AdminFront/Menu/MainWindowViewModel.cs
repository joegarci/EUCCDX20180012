namespace MIA.Menu
{
    using MIA.Helpers;
    using MIA.Views;
    using System;

    /// <summary>
    /// Clase que contiene las propiedades para la vista del menu
    /// </summary>
    public class MainWindowViewModel
    {
        /// <summary>
        /// Propiedad que contiene el detalle de cada una de las opciones de la apliación
        /// </summary>
        public MenuItem[] MenuItems { get; }
        /// <summary>
        /// Metodo que instancia la propiedad 
        /// </summary>
        public MainWindowViewModel()
        {
            /// IMPORTANTE!!!
            /// Para agregar nuevos iconos asociados a nuevas opciones, 
            /// ingrese a la página https://materialdesignicons.com/ y
            /// copie el nombre.
            CommonFunctions common = new CommonFunctions();
            if (common.TypeUser(Environment.UserName) == "CodSuper")
            {
                MenuItems = new[]
                {
                    new MenuItem("Inicio", new Dashboard(), "ChartAreaspline"),
                    new MenuItem("Usuarios Front", new AdminUsersFront(), "databaserefresh")
                };
            }
            else
            {
                MenuItems = new[]
                {
                    new MenuItem("Inicio", new Dashboard(), "ChartAreaspline"),
                    new MenuItem("Correos", new EmailParameters(), "emaileditoutline"),
                    new MenuItem("Procesamiento", new Tickets(), "CursorDefaultClick"),
                    new MenuItem("Fuerza de Trabajo", new RolesUsers(), "Robot"),
                    new MenuItem("Auditoria", new AuditTickets(), "FileDocumentBoxSearch"),
                    new MenuItem("Actividades", new Roles(), "RobotIndustrial"),
                    new MenuItem("Máquinas", new Users(), "LaptopChromebook"),
                    new MenuItem("Parámetros", new Parameters(), "SettingsOutline"),
                    new MenuItem("Comunícate con la EVC de Evolución Digital", new CommunicateWithCedEx(), "chattyping")
                };
            }
        }
    }
}
