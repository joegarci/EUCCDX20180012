using MIA.Commands;
using MIA.Model;
using MIA.Services;
using MIA.ViewData;
using MIA.Views;
using log4net;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace MIA.ViewModel
{
    public class AdminFrontViewModel : NotifyPropertyChanged
    {
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private const string message = "Se presento un error controlado comuniquese con el administrador.";
        public FrontUser_Create ViewCreateOrUpdate;

        #region Constructor
        public AdminFrontViewModel()
        {
            CurrentUser = new User();
            GetAll();
        }
        #endregion

        #region Properties
        private ObservableCollection<User> listUsers;
        public ObservableCollection<User> ListUsers
        {
            get { return listUsers; }
            set { listUsers = value; RaisePropertyChanged("ListUsers"); }
        }

        private ObservableCollection<AdminFrontData> listAssistants;
        public ObservableCollection<AdminFrontData> ListAssistants
        {
            get { return listAssistants; }
            set { listAssistants = value; RaisePropertyChanged("ListAssistants"); }
        }

        private User currentUser;
        public User CurrentUser
        {
            get
            {
                if (currentUser != null)
                {
                    GetUserAssistant();
                }
                return currentUser;
            }
            set { currentUser = value; RaisePropertyChanged("CurrentUser"); }
        }

        private AdminFrontData currentAssistant;
        public AdminFrontData CurrentAssistant
        {
            get { return currentAssistant; }
            set { currentAssistant = value; RaisePropertyChanged("CurrentAssistant"); }
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

        private ICommand deleteCommand;
        public ICommand DeleteCommand
        {
            get
            {
                if (deleteCommand == null)
                {
                    deleteCommand = new RelayCommand(p => Delete());
                }
                return deleteCommand;
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
        #endregion

        #region Function
        public void UpdateActive()
        {
            try
            {
                using (ModelDB modeldb = new ModelDB())
                {
                    UsersAssistants objUser = modeldb.UsersAssistants.Where(a => a.IdUser == currentUser.IdUser && a.IdAssistant == currentAssistant.IdAssistant).FirstOrDefault();
                    objUser.ActiveUserAssistant = currentAssistant.Active != true;
                    modeldb.Entry(objUser).State = EntityState.Modified;
                    modeldb.SaveChanges();
                    GetUserAssistant();
                }
            }
            catch (Exception ex)
            {
                Log.Fatal("Se ha presento un error al actualizar el usuario en la view model AdminFrontViewModel función UpdateActive.", ex);
                MessageBox.Show(message, "Mensaje Informativo", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        public void GetAll()
        {
            try
            {
                using (ModelDB modeldb = new ModelDB())
                {
                    ListUsers = new ObservableCollection<User>(modeldb.Users.Where(p => !p.NameUser.Contains("RPA")).ToList());
                }
            }
            catch (Exception ex)
            {
                Log.Fatal("Se ha presento un error al actualizar el usuario en la view model AdminFrontViewModel función GetAll.", ex);
                MessageBox.Show(message, "Mensaje Informativo", MessageBoxButton.OK, MessageBoxImage.Error);

            }
        }
        public void GetUserAssistant()
        {
            try
            {
                using (ModelDB modeldb = new ModelDB())
                {
                    ListAssistants = new ObservableCollection<AdminFrontData>(from item in (from a in modeldb.Users
                                                                                            join b in modeldb.UsersAssistants on a.IdUser equals b.IdUser
                                                                                            join c in modeldb.Assistants on b.IdAssistant equals c.IdAssistant
                                                                                            where a.IdUser == currentUser.IdUser
                                                                                            select new { a.IdUser, b.IdAssistant, c.NameAssistant, b.ActiveUserAssistant }).ToList()
                                                                              let objGrid = new AdminFrontData
                                                                              {
                                                                                  IdUser = item.IdUser,
                                                                                  IdAssistant = item.IdAssistant,
                                                                                  AssistantName = item.NameAssistant,
                                                                                  Active = item.ActiveUserAssistant
                                                                              }
                                                                              select objGrid);
                }
            }
            catch (Exception ex)
            {
                Log.Fatal("Se ha presento un error al actualizar el usuario en la view model AdminFrontViewModel función GetUserAssistant.", ex);
                MessageBox.Show(message, "Mensaje Informativo", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        public void Delete()
        {
            try
            {
                using (ModelDB modeldb = new ModelDB())
                {
                    User dataList = modeldb.Users.Find(currentUser.IdUser);
                    if (dataList.UsersAssistants.Count != 0)
                    {
                        List<UsersAssistants> lstUsersRoles = modeldb.UsersAssistants.Where(a => a.IdUser == currentUser.IdUser).ToList();
                        foreach (var item in lstUsersRoles)
                        {
                            modeldb.UsersAssistants.Remove(item);
                            modeldb.SaveChanges();
                        }
                    }
                    modeldb.Users.Remove(dataList);
                    modeldb.SaveChanges();
                    GetAll();
                }
            }
            catch (Exception ex)
            {
                Log.Fatal("Se ha presento un error al actualizar el usuario en la view model AdminFrontViewModel función Delete.", ex);
                MessageBox.Show(message, "Mensaje Informativo", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        public void NewView(string Type)
        {
            if (Type == "Add") { CurrentUser = new User(); }
            ViewCreateOrUpdate = new FrontUser_Create();
            AdminFrontUserCreateViewModel frontUserCreateViewModel = new AdminFrontUserCreateViewModel(CurrentUser, ViewCreateOrUpdate);
            ViewCreateOrUpdate.DataContext = frontUserCreateViewModel;
            ViewCreateOrUpdate.ShowDialog();
            ViewCreateOrUpdate.Close();
            GetAll();
        }
        #endregion
    }
}
