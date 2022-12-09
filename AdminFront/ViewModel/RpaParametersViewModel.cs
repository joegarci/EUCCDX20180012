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
    public class RpaParametersViewModel : NotifyPropertyChanged
    {
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private const string message = "Se presento un error controlado comuniquese con el administrador.";
        public Parameters_CreateOrUpdate ViewCreateOrUpdate;
        public CommonFunctions CommonFunctions;

        #region Constructor
        public RpaParametersViewModel()
        {
            ListAssistants = new ObservableCollection<Assistants>();
            ListParameters = new ObservableCollection<RpaParameter>();
            CommonFunctions = new CommonFunctions();
            CurrentAssistant = new Assistants();
            CurrentParameter = new RpaParameter();
            ListAssistants = new ObservableCollection<Assistants>(CommonFunctions.GetAssitant());
            currentAssistant.IdAssistant = CommonFunctions.GetAssitant().Select(X => X.IdAssistant).FirstOrDefault();
        }
        #endregion

        #region Properties
        private ObservableCollection<RpaParameter> listParameters;
        public ObservableCollection<RpaParameter> ListParameters
        {
            get => listParameters;
            set { listParameters = value; RaisePropertyChanged("ListParameters"); }
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

        private RpaParameter currentParameter;
        public RpaParameter CurrentParameter
        {
            get => currentParameter;
            set { currentParameter = value; RaisePropertyChanged("CurrentParameter"); }
        }
        #endregion

        #region Command
        private ICommand rpaparameterCommand;
        public ICommand RpaParameterCommand
        {
            get
            {
                if (rpaparameterCommand == null)
                {
                    rpaparameterCommand = new RelayCommand(p => OnUpdateRol());
                }

                return rpaparameterCommand;
            }
        }
        #endregion

        #region Function
        public void OnUpdateRol()
        {
            ViewCreateOrUpdate = new Parameters_CreateOrUpdate();
            ParameterCreateOrUpdateViewModel parameterCreateOrUpdateViewModel = new ParameterCreateOrUpdateViewModel(CurrentParameter, ViewCreateOrUpdate);
            ViewCreateOrUpdate.DataContext = parameterCreateOrUpdateViewModel;
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
                    ListParameters = new ObservableCollection<RpaParameter>(modeldb.RpaParameters.Where(p => p.IdAssistant == currentAssistant.IdAssistant).ToList());
                }
            }
            catch (Exception ex)
            {
                Log.Fatal("Se ha presento un error al actualizar el usuario en la view model RpaParametersViewModel función GetAll.", ex);
                MessageBox.Show(message, "Mensaje Informativo", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        #endregion
    }
}
