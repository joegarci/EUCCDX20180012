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
    public class RoleCreateOrUpdateViewModel : NotifyPropertyChanged
    {
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private const string message = "Se presento un error controlado comuniquese con el administrador.";
        public CommonFunctions CommonFunctions;
        public Roles_CreateOrUpdate viewRole;

        #region Constructor
        public RoleCreateOrUpdateViewModel(Role objRole, Roles_CreateOrUpdate view)
        {
            CommonFunctions = new CommonFunctions();
            CurrentRole = objRole;
            viewRole = view;
            Search();
        }
        #endregion

        #region Properties
        private Role currentRole;
        public Role CurrentRole
        {
            get => currentRole;
            set { currentRole = value; RaisePropertyChanged("CurrentRole"); }
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
                    editCommand = new RelayCommand(p => Update());
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
                    closeCommand = new RelayCommand(p => OnClose());
                }

                return closeCommand;
            }
        }
        #endregion

        #region Function
        public void OnClose()
        {
            viewRole.Close();
        }
        public void Search()
        {
            try
            {
                using (ModelDB modeldb = new ModelDB())
                {
                    CurrentRole = modeldb.Roles.Where(p => p.IdRole == CurrentRole.IdRole).FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                Log.Fatal("Se ha presento un error al actualizar el usuario en la view model RoleCreateOrUpdateViewModel función Search.", ex);
                MessageBox.Show(message, "Mensaje Informativo", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        public void Update()
        {
            try
            {
                using (ModelDB modeldb = new ModelDB())
                {
                    Role objRoleUpdate = modeldb.Roles.Find(CurrentRole.IdRole);
                    objRoleUpdate.NameRole = CurrentRole.NameRole;
                    objRoleUpdate.DescriptionRole = CurrentRole.DescriptionRole;
                    CommonFunctions common = new CommonFunctions();
                    if (common.IsValid(modeldb))
                    {
                        modeldb.Entry(objRoleUpdate).State = EntityState.Modified;
                        modeldb.SaveChanges();
                        MessageBox.Show("Registro actualizado correctamente", "Mensaje Informativo", MessageBoxButton.OK, MessageBoxImage.Information);
                        OnClose();
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Fatal("Se ha presento un error al actualizar el usuario en la view model RoleCreateOrUpdateViewModel función Update.", ex);
                MessageBox.Show(message, "Mensaje Informativo", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        #endregion
    }
}
