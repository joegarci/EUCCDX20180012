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
    internal class ParameterCreateOrUpdateViewModel : NotifyPropertyChanged
    {
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private const string message = "Se presento un error controlado comuniquese con el administrador.";
        public Parameters_CreateOrUpdate viewParameter;
        public CommonFunctions CommonFunctions;

        #region Constructor
        public ParameterCreateOrUpdateViewModel(RpaParameter objParameter, Parameters_CreateOrUpdate view)
        {
            CommonFunctions = new CommonFunctions();
            CurrentParameter = objParameter;
            viewParameter = view;
            Search();
        }
        #endregion

        #region Properties
        private RpaParameter currentParameter;
        public RpaParameter CurrentParameter
        {
            get => currentParameter;
            set { currentParameter = value; RaisePropertyChanged("CurrentParameter"); }
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
            viewParameter.Close();
        }
        public void Search()
        {
            try
            {
                using (ModelDB modeldb = new ModelDB())
                {
                    CurrentParameter = modeldb.RpaParameters.Where(p => p.IdParameter == CurrentParameter.IdParameter).FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                Log.Fatal("Se ha presento un error al actualizar el usuario en la view model ParameterCreateOrUpdateViewModel función Search.", ex);
                MessageBox.Show(message, "Mensaje Informativo", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        public void Update()
        {
            try
            {
                using (ModelDB modeldb = new ModelDB())
                {
                    RpaParameter objRpa = modeldb.RpaParameters.Find(CurrentParameter.IdParameter);
                    objRpa.NameParameter = CurrentParameter.NameParameter;
                    objRpa.ValueParameter = CurrentParameter.ValueParameter;
                    CommonFunctions common = new CommonFunctions();
                    if (common.IsValid(modeldb))
                    {
                        modeldb.Entry(objRpa).State = EntityState.Modified;
                        modeldb.SaveChanges();
                        MessageBox.Show("Registro actualizado correctamente", "Mensaje Informativo", MessageBoxButton.OK, MessageBoxImage.Information);
                        OnClose();
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Fatal("Se ha presento un error al actualizar el usuario en la view model ParameterCreateOrUpdateViewModel función Update.", ex);
                MessageBox.Show(message, "Mensaje Informativo", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        #endregion
    }
}
