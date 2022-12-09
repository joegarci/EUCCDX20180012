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
using EmailParameters = MIA.Model.EmailParameters;

namespace MIA.ViewModel
{
    public class EmailUpdateViewModel : NotifyPropertyChanged
    {
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private const string message = "Se presento un error controlado comuniquese con el administrador.";
        public CommonFunctions CommonFunctions;
        public Email_Update viewEmail;

        #region Constructor
        public EmailUpdateViewModel(EmailParameters objEmail, Email_Update view)
        {
            CommonFunctions = new CommonFunctions();
            CurrentEmail = objEmail;
            viewEmail = view;
            Search();
        }
        #endregion

        #region Properties
        private EmailParameters currentEmail;
        public EmailParameters CurrentEmail
        {
            get => currentEmail;
            set { currentEmail = value; RaisePropertyChanged("CurrentEmail"); }
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
            viewEmail.Close();
        }
        public void Search()
        {
            try
            {
                using (ModelDB modeldb = new ModelDB())
                {
                    CurrentEmail = modeldb.EmailParameters.Where(p => p.IdEmailParameter == CurrentEmail.IdEmailParameter).FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                Log.Fatal("Se ha presento un error al actualizar el usuario en la view model EmailUpdateViewModel función Search.", ex);
                MessageBox.Show(message, "Mensaje Informativo", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        public void Update()
        {
            try
            {
                using (ModelDB modeldb = new ModelDB())
                {
                    EmailParameters objEmail = modeldb.EmailParameters.Find(CurrentEmail.IdEmailParameter);
                    objEmail.SubjectEmailParameter = CurrentEmail.SubjectEmailParameter;
                    objEmail.TOEmailParameter = CurrentEmail.TOEmailParameter;
                    objEmail.CCEmailParameter = CurrentEmail.CCEmailParameter;
                    objEmail.BCCEmailParameter = CurrentEmail.BCCEmailParameter;
                    objEmail.BodyEmailParameter = CurrentEmail.BodyEmailParameter;
                    CommonFunctions common = new CommonFunctions();
                    if (common.IsValid(modeldb))
                    {
                        modeldb.Entry(objEmail).State = EntityState.Modified;
                        modeldb.SaveChanges();
                        MessageBox.Show("Registro actualizado correctamente", "Mensaje Informativo", MessageBoxButton.OK, MessageBoxImage.Information);
                        OnClose();
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Fatal("Se ha presento un error al actualizar el usuario en la view model EmailUpdateViewModel función Update.", ex);
                MessageBox.Show(message, "Mensaje Informativo", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        #endregion
    }
}
