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
    public class UserViewModel : NotifyPropertyChanged
    {
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private const string message = "Se presento un error controlado comuniquese con el administrador.";
        public Users_CreateOrUpdate ViewCreateOrUpdate;
        public CommonFunctions CommonFunctions;
        public Users ViewUsers;

        #region Constructor
        public UserViewModel()
        {
            ListUsers = new ObservableCollection<User>();
            CommonFunctions = new CommonFunctions();
            CurrentUser = new User();
            ViewUsers = new Users();
            GetAll();
        }
        #endregion

        #region Properties
        private ObservableCollection<User> listUsers;
        public ObservableCollection<User> ListUsers
        {
            get => listUsers;
            set { listUsers = value; RaisePropertyChanged("ListUsers"); }
        }

        private User currentUser;
        public User CurrentUser
        {
            get => currentUser;
            set { currentUser = value; RaisePropertyChanged("CurrentUser"); }
        }
        #endregion

        #region Command
        private ICommand editCommand;
        public ICommand EditCommand
        {
            get
            {
                if (editCommand == null)
                {
                    editCommand = new RelayCommand(p => NewView("Edit"));
                }

                return editCommand;
            }
        }

        private ICommand newCommand;
        public ICommand NewCommand
        {
            get
            {
                if (newCommand == null)
                {
                    newCommand = new RelayCommand(p => NewView("Add"));
                }

                return newCommand;
            }
        }
        #endregion

        #region Function
        public void GetAll()
        {
            try
            {
                using (ModelDB modeldb = new ModelDB())
                {
                    ListUsers = new ObservableCollection<User>(modeldb.Users.ToList());
                }
            }
            catch (Exception ex)
            {
                Log.Fatal("Se ha presento un error al actualizar el usuario en la view model UserCreateOrUpdateViewModel función AddRoles.", ex);
                MessageBox.Show(message, "Mensaje Informativo", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        public void NewView(string Type)
        {
            if (Type == "Add") { CurrentUser = new User(); }
            ViewCreateOrUpdate = new Users_CreateOrUpdate();
            UserCreateOrUpdateViewModel userCreateOrUpdateViewModel = new UserCreateOrUpdateViewModel(CurrentUser, ViewCreateOrUpdate);
            ViewCreateOrUpdate.DataContext = userCreateOrUpdateViewModel;
            ViewCreateOrUpdate.ShowDialog();
            ViewCreateOrUpdate.Close();
            GetAll();
        }
        #endregion
    }
}