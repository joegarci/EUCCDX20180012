using MIA.Commands;
using MIA.Helpers;
using MIA.Model;
using MIA.Services;
using MIA.Views;
using log4net;
using System;
using System.Data.Entity;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace MIA.ViewModel
{
    public class UserCreateOrUpdateViewModel : NotifyPropertyChanged
    {
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private const string message = "Se presento un error controlado comuniquese con el administrador.";
        public CommonFunctions CommonFunctions;
        public Users_CreateOrUpdate viewUser;

        #region Constructor
        public UserCreateOrUpdateViewModel(User objUser, Users_CreateOrUpdate view)
        {
            CommonFunctions = new CommonFunctions();
            CurrentUser = objUser;
            viewUser = view;
            Search();
        }
        #endregion

        #region Properties
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
                    editCommand = new RelayCommand(p => UpdateOrCreate());
                }

                return editCommand;
            }
        }

        private ICommand closeCommand;
        public ICommand CloseCommand
        {
            get
            {
                if (closeCommand == null)
                {
                    closeCommand = new RelayCommand(p => viewUser.Close());
                }

                return closeCommand;
            }
        }
        #endregion

        #region Function
        public void Search()
        {
            try
            {
                using (ModelDB modeldb = new ModelDB())
                {
                    CurrentUser = modeldb.Users.Where(p => p.IdUser == CurrentUser.IdUser).FirstOrDefault();
                    if (CurrentUser == null)
                    {
                        CurrentUser = new User();
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Fatal("Se ha presento un error al actualizar el usuario en la view model UserCreateOrUpdateViewModel función Search.", ex);
                MessageBox.Show(message, "Mensaje Informativo", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        public void UpdateOrCreate()
        {
            using (ModelDB modeldb = new ModelDB())
            {
                if (CurrentUser.IdUser == 0)
                {
                    Add();
                }
                else
                {
                    Update();
                }
            }
            viewUser.Close();
        }
        public void Update()
        {
            try
            {
                using (ModelDB modeldb = new ModelDB())
                {
                    CommonFunctions common = new CommonFunctions();
                    if (common.IsValid(modeldb))
                    {
                        modeldb.Entry(CurrentUser).State = EntityState.Modified;
                        modeldb.SaveChanges();
                        MessageBox.Show("Registro actualizado correctamente", "Mensaje Informativo", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Fatal("Se ha presento un error al actualizar el usuario en la view model UserCreateOrUpdateViewModel función Update.", ex);
                MessageBox.Show(message, "Mensaje Informativo", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        public void Add()
        {
            try
            {
                using (ModelDB modeldb = new ModelDB())
                {
                    CurrentUser.IdType = modeldb.TypeUsers.Where(p => p.NameUserType == "RPA").FirstOrDefault().IdType;
                    modeldb.Users.Add(CurrentUser);
                    modeldb.SaveChanges();
                    AddRoles(CurrentUser.IdUser);
                    MessageBox.Show("Registro Ingresado correctamente", "Mensaje Informativo", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                Log.Fatal("Se ha presento un error al actualizar el usuario en la view model UserCreateOrUpdateViewModel función Add.", ex);
                MessageBox.Show(message, "Mensaje Informativo", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        public void AddRoles(byte id)
        {
            try
            {
                using (ModelDB modeldb = new ModelDB())
                {
                    foreach (Assistants item in modeldb.Assistants.ToList())
                    {
                        UsersAssistants objAssistant = new UsersAssistants
                        {
                            IdUser = id,
                            ActiveUserAssistant = true,
                            IdAssistant = item.IdAssistant
                        };
                        modeldb.UsersAssistants.Add(objAssistant);
                        modeldb.SaveChanges();
                    }
                    foreach (Role item in modeldb.Roles.ToList())
                    {
                        UsersRole objRole = new UsersRole
                        {
                            IdRole = item.IdRole,
                            IdUser = id,
                            MaxIterationsUserRole = 0,
                            ActiveUserRole = false,
                        };
                        modeldb.UsersRoles.Add(objRole);
                        modeldb.SaveChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Fatal("Se ha presento un error al actualizar el usuario en la view model UserCreateOrUpdateViewModel función AddRoles.", ex);
                MessageBox.Show(message, "Mensaje Informativo", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        #endregion
    }
}