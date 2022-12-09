using MIA.Commands;
using MIA.Helpers;
using MIA.Model;
using MIA.Services;
using MIA.ViewData;
using log4net;
using System;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace MIA.ViewModel
{
    public class RoleUserViewModel : NotifyPropertyChanged
    {
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private const string message = "Se presento un error controlado comuniquese con el administrador.";
        public CommonFunctions CommonFunctions;

        #region Constructor
        public RoleUserViewModel()
        {
            ListAssistants = new ObservableCollection<Assistants>();
            listRoleUsers = new ObservableCollection<RoleUserData>();
            ListUsers = new ObservableCollection<User>();
            CommonFunctions = new CommonFunctions();
            CurrentAssistant = new Assistants();
            CurrentUser = new User();
            CurrentRole = new RoleUserData();
            ListAssistants = new ObservableCollection<Assistants>(CommonFunctions.GetAssitant());
            currentAssistant.IdAssistant = CommonFunctions.GetAssitant().Select(X => X.IdAssistant).FirstOrDefault();
        }
        #endregion

        #region Properties
        private ObservableCollection<RoleUserData> listRoleUsers;
        public ObservableCollection<RoleUserData> ListRoleUser
        {
            get => listRoleUsers;
            set { listRoleUsers = value; RaisePropertyChanged("ListRoleUser"); }
        }

        private ObservableCollection<User> listUsers;
        public ObservableCollection<User> ListUsers
        {
            get => listUsers;
            set { listUsers = value; RaisePropertyChanged("ListUsers"); }
        }

        private ObservableCollection<Assistants> listAssistants;
        public ObservableCollection<Assistants> ListAssistants
        {
            get => listAssistants;
            set { listAssistants = value; RaisePropertyChanged("Assistants"); }
        }

        private Assistants currentAssistant;
        public Assistants CurrentAssistant
        {
            get
            {
                if (currentAssistant != null)
                {
                    ListRoleUser = new ObservableCollection<RoleUserData>();
                    GetAll();
                }
                return currentAssistant;
            }
            set { currentAssistant = value; RaisePropertyChanged("CurrentAssistant"); }
        }

        private User currentUser;
        public User CurrentUser
        {
            get
            {
                if (currentUser != null)
                {
                    SelectRole();
                }
                return currentUser;
            }
            set { currentUser = value; RaisePropertyChanged("CurrentUser"); }
        }

        private RoleUserData currentRole;
        public RoleUserData CurrentRole
        {
            get => currentRole;
            set { currentRole = value; RaisePropertyChanged("CurrentRole"); }
        }
        #endregion

        #region Command
        private ICommand isCheck;
        public ICommand IsCheck
        {
            get
            {
                if (isCheck == null)
                {
                    isCheck = new RelayCommand(p => UpdateActive());
                }
                return isCheck;
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
                    ListUsers = new ObservableCollection<User>((from a in modeldb.Users
                                                                join b in modeldb.UsersAssistants on a.IdUser equals b.IdUser
                                                                where b.IdAssistant == currentAssistant.IdAssistant && a.NameUser.ToUpper().Contains("RPA") && a.ActiveUser == true
                                                                select a).ToList());
                }
            }
            catch (Exception ex)
            {
                Log.Fatal("Se ha presento un error al actualizar el usuario en la view model RoleUserViewModel función GetAll.", ex);
                MessageBox.Show(message, "Mensaje Informativo", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        public void UpdateActive()
        {
            try
            {
                using (ModelDB modeldb = new ModelDB())
                {
                    var objUsersRoles = modeldb.UsersRoles.Where(a => a.IdRole == currentRole.IdRole && a.IdUser == currentUser.IdUser).FirstOrDefault();
                    objUsersRoles.ActiveUserRole = currentRole.Active != true;
                    modeldb.Entry(objUsersRoles).State = EntityState.Modified;
                    modeldb.SaveChanges();
                    SelectRole();
                }
            }
            catch (Exception ex)
            {
                Log.Fatal("Se ha presento un error al actualizar el usuario en la view model RoleUserViewModel función UpdateActive.", ex);
                MessageBox.Show(message, "Mensaje Informativo", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        public void SelectRole()
        {
            try
            {
                ListRoleUser = new ObservableCollection<RoleUserData>();
                using (ModelDB modeldb = new ModelDB())
                {
                    foreach (var objGrid in from item in (from a in modeldb.UsersRoles
                                                          join b in modeldb.Roles on a.IdRole equals b.IdRole
                                                          where a.IdUser == currentUser.IdUser && b.IdAssistant == currentAssistant.IdAssistant
                                                          select new { a.IdUser, b.IdRole, b.NameRole, a.ActiveUserRole, b.DescriptionRole }).ToList()
                                            let objGrid = new RoleUserData
                                            {
                                                IdRole = item.IdRole,
                                                IdUser = item.IdUser,
                                                NameRol = item.NameRole,
                                                Active = item.ActiveUserRole,
                                                Description = item.DescriptionRole
                                            }
                                            select objGrid)
                    {
                        ListRoleUser.Add(objGrid);
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Fatal("Se ha presento un error al actualizar el usuario en la view model RoleUserViewModel función SelectRole.", ex);
                MessageBox.Show(message, "Mensaje Informativo", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        #endregion
    }
}
