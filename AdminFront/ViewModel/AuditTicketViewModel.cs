using MIA.Commands;
using MIA.Helpers;
using MIA.Model;
using MIA.Services;
using MIA.ViewData;
using log4net;
using Microsoft.Win32;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace MIA.ViewModel
{
    public class AuditTicketViewModel : NotifyPropertyChanged
    {
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private const string message = "Se presento un error controlado comuniquese con el administrador.";
        public DateTime dtLastDays = DateTime.Now.AddDays(-5);
        public CommonFunctions CommonFunctions;

        #region Constructor
        public AuditTicketViewModel()
        {
            CurrentAssistant = new Assistants();
            CommonFunctions = new CommonFunctions();
            CurrentAudtitTicket = new AuditTicketData();
            ListAudtitTicket = new ObservableCollection<AuditTicketData>();
            ListAssistants = new ObservableCollection<Assistants>(CommonFunctions.GetAssitant());
            currentAssistant.IdAssistant = CommonFunctions.GetAssitant().Select(X => X.IdAssistant).FirstOrDefault();
        }
        #endregion

        #region Properties
        private ObservableCollection<Check_ViewModel> listState;
        public ObservableCollection<Check_ViewModel> ListState
        {
            get { return listState; }
            set { listState = value; RaisePropertyChanged("ListState"); }
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

        private AuditTicketData currentAudtitTicket;
        public AuditTicketData CurrentAudtitTicket
        {
            get { return currentAudtitTicket; }
            set
            {
                currentAudtitTicket = value;
                RaisePropertyChanged("CurrentAudtitTicket");
            }
        }

        private ObservableCollection<Assistants> listAssistants;
        public ObservableCollection<Assistants> ListAssistants
        {
            get { return listAssistants; }
            set { listAssistants = value; RaisePropertyChanged("ListAssistants"); }
        }

        private ObservableCollection<AuditTicketData> listAudtitTicket;
        public ObservableCollection<AuditTicketData> ListAudtitTicket
        {
            get { return listAudtitTicket; }
            set { listAudtitTicket = value; RaisePropertyChanged("ListAudtitTicket"); }
        }
        #endregion

        #region Command
        private ICommand commandSearch;
        public ICommand CommandSearch
        {
            get
            {
                if (commandSearch == null)
                {
                    commandSearch = new RelayCommand(p => Search());
                }
                return commandSearch;
            }
        }

        private ICommand commandDownload;
        public ICommand CommandDownload
        {
            get
            {
                if (commandDownload == null)
                {
                    commandDownload = new RelayCommand(p => DownloadReport());
                }
                return commandDownload;
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
                    ListState = new ObservableCollection<Check_ViewModel>(CommonFunctions.GetStates(currentAssistant.IdAssistant));
                    ListAudtitTicket = new ObservableCollection<AuditTicketData>((from item in (from a in modeldb.AuditTickets
                                                                                                join b in modeldb.States on a.StartStateAuditTicket equals b.IdState
                                                                                                join c in modeldb.Users on a.IdUser equals c.IdUser
                                                                                                where a.StartDateAuditTicket >= dtLastDays && b.IdAssistant == currentAssistant.IdAssistant
                                                                                                select new { a.IdTicket, c.NameUser, a.StartDateAuditTicket, a.EndDateAuditTicket, b.NameState, a.EndStateAuditTicket, a.DescriptionAuditTicket }).ToList()
                                                                                  let nameState = modeldb.States.Where(a => a.IdState == item.EndStateAuditTicket).Select(a => a.NameState).FirstOrDefault()
                                                                                  let row = new AuditTicketData
                                                                                  {
                                                                                      IdTicket = item.IdTicket,
                                                                                      NameUser = item.NameUser,
                                                                                      StartDate = item.StartDateAuditTicket,
                                                                                      EndDate = (DateTime)item.EndDateAuditTicket,
                                                                                      StartState = item.NameState,
                                                                                      EndState = nameState,
                                                                                      Description = item.DescriptionAuditTicket
                                                                                  }
                                                                                  select row).ToList());
                }
            }
            catch (Exception ex)
            {
                Log.Fatal("Se ha presento un error al actualizar el usuario en la view model AuditTicketViewModel función UpdateActive.", ex);
                MessageBox.Show(message, "Mensaje Informativo", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        public void Search()
        {
            try
            {
                using (ModelDB modeldb = new ModelDB())
                {
                    string All = "";
                    foreach (var item in ListState.Where(item => item.IsSelect == true))
                    {
                        All = All + "," + item.IdState.ToString();
                    }
                    ListAudtitTicket = new ObservableCollection<AuditTicketData>((from item in (from a in modeldb.AuditTickets
                                                                                                join b in modeldb.States on a.StartStateAuditTicket equals b.IdState
                                                                                                join c in modeldb.Users on a.IdUser equals c.IdUser
                                                                                                where (a.IdTicket == currentAudtitTicket.IdTicket || All.Contains(a.StartStateAuditTicket.ToString())) && b.IdAssistant == currentAssistant.IdAssistant
                                                                                                select new { a.IdTicket, c.NameUser, a.StartDateAuditTicket, a.EndDateAuditTicket, b.NameState, a.EndStateAuditTicket, a.DescriptionAuditTicket }).Take(80).ToList()
                                                                                  let nameState = modeldb.States.Where(a => a.IdState == item.EndStateAuditTicket).Select(a => a.NameState).FirstOrDefault()
                                                                                  let row = new AuditTicketData
                                                                                  {
                                                                                      IdTicket = item.IdTicket,
                                                                                      NameUser = item.NameUser,
                                                                                      StartDate = item.StartDateAuditTicket,
                                                                                      EndDate = (DateTime)item.EndDateAuditTicket,
                                                                                      StartState = item.NameState,
                                                                                      EndState = nameState,
                                                                                      Description = item.DescriptionAuditTicket
                                                                                  }
                                                                                  select row).ToList());
                }
            }
            catch (Exception ex)
            {
                Log.Fatal("Se ha presento un error al actualizar el usuario en la view model AuditTicketViewModel función Search.", ex);
                MessageBox.Show(message, "Mensaje Informativo", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        public void DownloadReport()
        {
            List<AuditTicketData> modelTicket;
            try
            {
                using (ModelDB modeldb = new ModelDB())
                {
                    modelTicket = (from item in (from a in modeldb.AuditTickets
                                                 join b in modeldb.States on a.StartStateAuditTicket equals b.IdState
                                                 join c in modeldb.Users on a.IdUser equals c.IdUser
                                                 where (a.StartDateAuditTicket >= currentAudtitTicket.StartDate && a.EndDateAuditTicket <= currentAudtitTicket.EndDate) && b.IdAssistant == currentAssistant.IdAssistant
                                                 select new
                                                 {
                                                     a.IdTicket,
                                                     c.NameUser,
                                                     a.StartDateAuditTicket,
                                                     a.EndDateAuditTicket,
                                                     b.NameState,
                                                     a.EndStateAuditTicket,
                                                     a.DescriptionAuditTicket
                                                 }).Take(80).ToList()
                                   let nameState = modeldb.States.Where(a => a.IdState == item.EndStateAuditTicket).Select(a => a.NameState).FirstOrDefault()
                                   let row = new AuditTicketData
                                   {
                                       IdTicket = item.IdTicket,
                                       NameUser = item.NameUser,
                                       StartDate = item.StartDateAuditTicket,
                                       EndDate = (DateTime)item.EndDateAuditTicket,
                                       StartState = item.NameState,
                                       EndState = nameState,
                                       Description = item.DescriptionAuditTicket
                                   }
                                   select row).ToList();
                    if (modelTicket.Count > 0)
                    {
                        DataTable table = new DataTable();
                        table = CommonFunctions.ConvertToDataTable(modelTicket);
                        table.Columns["IdTicket"].ColumnName = "Ticket";
                        table.Columns["NameUser"].ColumnName = "Nombre de Usuario";
                        table.Columns["StartDate"].ColumnName = "Fecha Inicial";
                        table.Columns["EndDate"].ColumnName = "Fecha Final";
                        table.Columns["StartState"].ColumnName = "Estado Inicial";
                        table.Columns["EndState"].ColumnName = "Estado Final";
                        table.Columns["Description"].ColumnName = "Descripción";
                        using (ExcelPackage pck = new ExcelPackage())
                        {
                            SaveFileDialog showdialog = new SaveFileDialog()
                            {
                                Title = "Guardar como",
                                Filter = "Libro de Excel | *.xlsx",
                                AddExtension = true
                            };
                            showdialog.ShowDialog();
                            if (showdialog.FileName != "")
                            {
                                ExcelWorksheet workSheet = pck.Workbook.Worksheets.Add("Reporte de proceso");
                                workSheet.Cells["A1"].LoadFromDataTable(table, true);
                                workSheet.Cells["C1:CN"].Style.Numberformat.Format = "mm/dd/yyyy hh:mm:ss AM/PM";
                                pck.SaveAs(new FileInfo(showdialog.FileName));
                                MessageBox.Show("Descarga Correcta", "Mensaje Informativo", MessageBoxButton.OK, MessageBoxImage.Information);
                            }
                        }
                    }
                    else
                    {
                        MessageBox.Show("No se encontro información para las fechas seleccionadas", "Mensaje Informativo", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Fatal("Se ha presento un error al actualizar el usuario en la view model AuditTicketViewModel función DownloadReport.", ex);
                MessageBox.Show(message, "Mensaje Informativo", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        #endregion
    }
}
