using MIA.Commands;
using MIA.Helpers;
using MIA.Model;
using MIA.Services;
using MIA.Views;
using log4net;
using System;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace MIA.ViewModel
{
    public class AdminFrontUserCreateViewModel : NotifyPropertyChanged
    {
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private const string message = "Se presento un error controlado comuniquese con el administrador.";
        public CommonFunctions CommonFunctions;
        public FrontUser_Create viewUser;

        #region Constructor
        public AdminFrontUserCreateViewModel(User objUser, FrontUser_Create view)
        {
            CommonFunctions = new CommonFunctions();
            CurrentUser = objUser;
            viewUser = view;
            ListUsersTypes = new ObservableCollection<UsersTypes>(CommonFunctions.GetTypeUsers());
        }
        #endregion

        #region Properties
        private ObservableCollection<UsersTypes> listUsersTypes;
        public ObservableCollection<UsersTypes> ListUsersTypes
        {
            get { return listUsersTypes; }
            set { listUsersTypes = value; RaisePropertyChanged("ListUsersTypes"); }
        }

        private User currentUser;
        public User CurrentUser
        {
            get { return currentUser; }
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
        public void Update()
        {
            try
            {
                using (ModelDB modeldb = new ModelDB())
                {
                    CommonFunctions common = new CommonFunctions();
                    if (common.IsValid(modeldb))
                    {
                        modeldb.Entry(currentUser).State = EntityState.Modified;
                        modeldb.SaveChanges();
                        MessageBox.Show("Registro Actualizado correctamente", "Mensaje Informativo", MessageBoxButton.OK, MessageBoxImage.Information);
                        viewUser.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Fatal("Se ha presento un error al actualizar el usuario en la view model AdminFrontUserCreateViewModel función Update.", ex);
                MessageBox.Show(message, "Mensaje Informativo", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }
        public void Add()
        {
            try
            {
                using (ModelDB modeldb = new ModelDB())
                {
                    if (modeldb.Users.Where(p => p.NameUser == currentUser.NameUser).Count() == 0)
                    {
                        CurrentUser = null;
                        modeldb.Users.Add(CurrentUser);
                        modeldb.SaveChanges();
                        AddAssistant(currentUser.IdUser);
                        MessageBox.Show("Registro Ingresado correctamente", "Mensaje Informativo", MessageBoxButton.OK, MessageBoxImage.Information);
                        viewUser.Close();
                    }
                    else
                    {
                        MessageBox.Show("El usuario de red: " + currentUser.NameUser + " ya esta creado en base de datos", "Mensaje Informativo", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Fatal("Se ha presento un error al actualizar el usuario en la view model AdminFrontUserCreateViewModel función Add.", ex);
                MessageBox.Show(message, "Mensaje Informativo", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        public void AddAssistant(byte id)
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
                }
            }
            catch (Exception ex)
            {
                Log.Fatal("Se ha presento un error al actualizar el usuario en la view model AdminFrontUserCreateViewModel función AddAssistant.", ex);
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
        }
        #endregion
    }
}
