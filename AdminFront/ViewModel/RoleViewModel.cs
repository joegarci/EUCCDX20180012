using MIA.Commands;
using MIA.Helpers;
using MIA.Model;
using MIA.Services;
using MIA.Views;
using log4net;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace MIA.ViewModel
{
    public class RoleViewModel : NotifyPropertyChanged
    {
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private const string message = "Se presento un error controlado comuniquese con el administrador.";
        public Roles_CreateOrUpdate ViewCreateOrUpdate;
        public CommonFunctions CommonFunctions;

        #region Constructor
        public RoleViewModel()
        {
            ListAssistants = new ObservableCollection<Assistants>();
            ListRoles = new ObservableCollection<Role>();
            CommonFunctions = new CommonFunctions();
            CurrentAssistant = new Assistants();
            CurrentRole = new Role();
            ListAssistants = new ObservableCollection<Assistants>(CommonFunctions.GetAssitant());
            currentAssistant.IdAssistant = CommonFunctions.GetAssitant().Select(X => X.IdAssistant).FirstOrDefault();
        }
        #endregion

        #region Properties
        private ObservableCollection<Role> listRoles;
        public ObservableCollection<Role> ListRoles
        {
            get => listRoles;
            set { listRoles = value; RaisePropertyChanged("ListRoles"); }
        }

        private ObservableCollection<Assistants> listassistants;
        public ObservableCollection<Assistants> ListAssistants
        {
            get => listassistants;
            set { listassistants = value; RaisePropertyChanged("ListAssistants"); }
        }

        private Assistants currentAssistant;
        public Assistants CurrentAssistant
        {
            get
            {
                if (currentAssistant != null)
                {
                    GetAll();
                }
                return currentAssistant;
            }
            set { currentAssistant = value; RaisePropertyChanged("CurrentAssistant"); }
        }

        private Role currentRole;
        public Role CurrentRole
        {
            get => currentRole;
            set { currentRole = value; RaisePropertyChanged("CurrentRole"); }
        }
        #endregion

        #region Command
        private ICommand roleCommand;
        public ICommand RoleCommand
        {
            get
            {
                if (roleCommand == null)
                {
                    roleCommand = new RelayCommand(p => OnUpdateRol());
                }

                return roleCommand;
            }
        }
        #endregion

        #region Function
        public void OnUpdateRol()
        {
            ViewCreateOrUpdate = new Roles_CreateOrUpdate();
            RoleCreateOrUpdateViewModel roleCreateOrUpdateViewModel = new RoleCreateOrUpdateViewModel(CurrentRole, ViewCreateOrUpdate);
            ViewCreateOrUpdate.DataContext = roleCreateOrUpdateViewModel;
            ViewCreateOrUpdate.ShowDialog();
            ViewCreateOrUpdate.Close();
            GetAll();
        }
        public void GetAll()
        {
            try
            {
                using (ModelDB modeldb = new ModelDB())
                {
                    ListRoles = new ObservableCollection<Role>(modeldb.Roles.Where(p => p.IdAssistant == currentAssistant.IdAssistant).ToList());
                }
            }
            catch (Exception ex)
            {
                Log.Fatal("Se ha presento un error al actualizar el usuario en la view model RoleViewModel función GetAll.", ex);
                MessageBox.Show(message, "Mensaje Informativo", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        #endregion
    }
}
