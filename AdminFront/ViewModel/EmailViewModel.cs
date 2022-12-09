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
using EmailParameters = MIA.Model.EmailParameters;

namespace MIA.ViewModel
{
    public class EmailViewModel : NotifyPropertyChanged
    {
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private const string message = "Se presento un error controlado comuniquese con el administrador.";
        public CommonFunctions CommonFunctions;
        public Email_Update ViewEmailUpdate;

        #region Constructor
        public EmailViewModel()
        {
            ListAssistants = new ObservableCollection<Assistants>();
            ListEmails = new ObservableCollection<EmailParameters>();
            CommonFunctions = new CommonFunctions();
            CurrentAssistant = new Assistants();
            CurrentEmail = new EmailParameters();
            ListAssistants = new ObservableCollection<Assistants>(CommonFunctions.GetAssitant());
            currentAssistant.IdAssistant = CommonFunctions.GetAssitant().Select(X => X.IdAssistant).FirstOrDefault();

        }
        #endregion

        #region Properties
        private ObservableCollection<EmailParameters> listEmails;
        public ObservableCollection<EmailParameters> ListEmails
        {
            get => listEmails;
            set { listEmails = value; RaisePropertyChanged("ListEmails"); }
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

        private EmailParameters currentEmail;
        public EmailParameters CurrentEmail
        {
            get => currentEmail;
            set { currentEmail = value; RaisePropertyChanged("CurrentEmail"); }
        }
        #endregion

        #region Command
        private ICommand emailCommand;
        public ICommand EmailCommand
        {
            get
            {
                if (emailCommand == null)
                {
                    emailCommand = new RelayCommand(p => OnUpdateRol());
                }
                return emailCommand;
            }
        }
        #endregion

        #region Function
        public void OnUpdateRol()
        {
            ViewEmailUpdate = new Email_Update();
            EmailUpdateViewModel emailUpdateViewModel = new EmailUpdateViewModel(CurrentEmail, ViewEmailUpdate);
            ViewEmailUpdate.DataContext = emailUpdateViewModel;
            ViewEmailUpdate.ShowDialog();
            ViewEmailUpdate.Close();
            GetAll();
        }
        public void GetAll()
        {
            try
            {
                using (ModelDB modeldb = new ModelDB())
                {
                    ListEmails = new ObservableCollection<EmailParameters>(modeldb.EmailParameters.Where(p => p.IdAssistant == currentAssistant.IdAssistant).ToList());
                }
            }
            catch (Exception ex)
            {
                Log.Fatal("Se ha presento un error al actualizar el usuario en la view model EmailUpdateViewModel función GetAll.", ex);
                MessageBox.Show(message, "Mensaje Informativo", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        #endregion
    }
}
