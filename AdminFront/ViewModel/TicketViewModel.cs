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
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace MIA.ViewModel
{
    public class TicketViewModel : NotifyPropertyChanged
    {
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private const string message = "Se presento un error controlado comuniquese con el administrador.";
        public DateTime dtLastDays = DateTime.Now.AddDays(-5);
        public CommonFunctions CommonFunctions;

        #region Constructor
        public TicketViewModel()
        {
            CurrentTicket = new TicketData();
            CurrentAssistant = new Assistants();
            CommonFunctions = new CommonFunctions();
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

        private ObservableCollection<TicketData> listTicket;
        public ObservableCollection<TicketData> ListTicket
        {
            get { return listTicket; }
            set { listTicket = value; RaisePropertyChanged("ListTicket"); }
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

        private TicketData currentTicket;
        public TicketData CurrentTicket
        {
            get { return currentTicket; }
            set { currentTicket = value; RaisePropertyChanged("CurrentTicket"); }
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

        private ICommand commandUpdate;
        public ICommand CommandUpdate
        {
            get
            {
                if (commandUpdate == null)
                {
                    commandUpdate = new RelayCommand(p => Update());
                }
                return commandUpdate;
            }
        }

        private ICommand commandDownlaod;
        public ICommand CommandDownlaod
        {
            get
            {
                if (commandDownlaod == null)
                {
                    commandDownlaod = new RelayCommand(p => DownloadReport());
                }
                return commandDownlaod;
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
                    ListTicket = new ObservableCollection<TicketData>((from item in (from a in modeldb.Tickets
                                                                                     join b in modeldb.States on a.IdState equals b.IdState
                                                                                     where a.CreationDate >= dtLastDays && b.IdAssistant == currentAssistant.IdAssistant
                                                                                     select new { a.IdTicket, b.NameState, a.CreationDate, a.ExecutionDate, a.Locked, a.Priority }).ToList()
                                                                       let row = new TicketData
                                                                       {
                                                                           IdTicket = item.IdTicket,
                                                                           State = item.NameState,
                                                                           Locked = item.Locked,
                                                                           Priority = item.Priority,
                                                                           CreationDate = item.CreationDate,
                                                                           NextStatus = CommonFunctions.GetNextStates(currentAssistant.IdAssistant),
                                                                           ExecutionDate = item.ExecutionDate,
                                                                           SelectName = ""
                                                                       }
                                                                       select row).ToList());
                }
            }
            catch (Exception ex)
            {
                Log.Fatal("Se ha presento un error al actualizar el usuario en la view model TicketViewModel función GetAll.", ex);
                MessageBox.Show(message, "Mensaje Informativo", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        public void Search()
        {
            try
            {
                string All = "";
                foreach (var item in ListState.Where(item => item.IsSelect == true))
                {
                    All = All + "," + item.IdState.ToString();
                }

                using (ModelDB modeldb = new ModelDB())
                {
                    ListTicket = new ObservableCollection<TicketData>((from item in (from a in modeldb.Tickets
                                                                                     join b in modeldb.States on a.IdState equals b.IdState
                                                                                     where a.CreationDate >= dtLastDays && (a.IdTicket == currentTicket.IdTicket || All.Contains(a.IdState.ToString())) && b.IdAssistant == currentAssistant.IdAssistant
                                                                                     select new { a.IdTicket, b.NameState, a.CreationDate, a.ExecutionDate, a.Locked, a.Priority }).ToList()
                                                                       let row = new TicketData
                                                                       {
                                                                           IdTicket = item.IdTicket,
                                                                           State = item.NameState,
                                                                           Locked = item.Locked,
                                                                           Priority = item.Priority,
                                                                           CreationDate = item.CreationDate,
                                                                           NextStatus = CommonFunctions.GetNextStates(currentAssistant.IdAssistant),
                                                                           ExecutionDate = item.ExecutionDate,
                                                                           SelectName = ""
                                                                       }
                                                                       select row).ToList());
                }
            }
            catch (Exception ex)
            {
                Log.Fatal("Se ha presento un error al actualizar el usuario en la view model TicketViewModel función Search.", ex);
                MessageBox.Show(message, "Mensaje Informativo", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        public void Update()
        {
            try
            {
                foreach (var item in ListTicket.Where(item => item.SelectName != "" || item.Priority == true))
                {
                    using (ModelDB modeldb = new ModelDB())
                    {
                        AuditTicket auditTicket = new AuditTicket();
                        Ticket objTicket = modeldb.Tickets.Find(item.IdTicket);
                        if (item.SelectName != "")
                        {
                            auditTicket.IdTicket = objTicket.IdTicket;
                            auditTicket.IdUser = modeldb.Users.Where(p => p.NameUser == Environment.UserName).FirstOrDefault().IdUser;
                            auditTicket.StartDateAuditTicket = DateTime.Now;
                            auditTicket.EndDateAuditTicket = DateTime.Now;
                            auditTicket.StartStateAuditTicket = objTicket.IdState;
                            objTicket.IdState = byte.Parse(modeldb.States.Where(S => S.NameState == item.SelectName).Select(S => S.IdState).FirstOrDefault().ToString());
                            auditTicket.EndStateAuditTicket = objTicket.IdState;
                            auditTicket.DescriptionAuditTicket = "Modificado por pantalla Front";
                            modeldb.AuditTickets.Add(auditTicket);
                        }
                        objTicket.Priority = item.Priority;
                        CommonFunctions common = new CommonFunctions();
                        if (common.IsValid(modeldb))
                        {
                            modeldb.Entry(objTicket).State = EntityState.Modified;
                            modeldb.SaveChanges();
                        }
                    }
                }
                MessageBox.Show("Informaciín Modificada Correctamente", "Mensaje Informativo", MessageBoxButton.OK, MessageBoxImage.Information);
                GetAll();
            }
            catch (Exception ex)
            {
                Log.Fatal("Se ha presento un error al actualizar el usuario en la view model TicketViewModel función Update.", ex);
                MessageBox.Show(message, "Mensaje Informativo", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        public void DownloadReport()
        {
            List<TicketData> modelTicket;
            try
            {
                using (ModelDB modeldb = new ModelDB())
                {
                    modelTicket = (from item in (from a in modeldb.Tickets
                                                 join b in modeldb.States on a.IdState equals b.IdState
                                                 where a.CreationDate >= dtLastDays && b.IdAssistant == currentAssistant.IdAssistant
                                                 select new { a.IdTicket, b.NameState, a.CreationDate, a.ExecutionDate, a.Locked, a.Priority }).Take(80).ToList()
                                   let row = new TicketData
                                   {
                                       IdTicket = item.IdTicket,
                                       State = item.NameState,
                                       Locked = item.Locked,
                                       Priority = item.Priority,
                                       CreationDate = item.CreationDate,
                                       NextStatus = CommonFunctions.GetNextStates(currentAssistant.IdAssistant),
                                       ExecutionDate = item.ExecutionDate,
                                       SelectName = ""
                                   }
                                   select row).ToList();
                }
                if (modelTicket.Count > 0)
                {
                    DataTable table = new DataTable();
                    table = CommonFunctions.ConvertToDataTable(modelTicket);
                    table.Columns["IdTicket"].ColumnName = "Ticket";
                    table.Columns["State"].ColumnName = "Estados";
                    table.Columns["CreationDate"].ColumnName = "Fecha Creación";
                    table.Columns.Remove("ExecutionDate");
                    table.Columns["Locked"].ColumnName = "Bloqueado";
                    table.Columns.Remove("Priority");
                    table.Columns.Remove("NextStatus");
                    table.Columns.Remove("SelectName");
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
            catch (Exception ex)
            {
                Log.Fatal("Se ha presento un error al actualizar el usuario en la view model TicketViewModel función DownloadReport.", ex);
                MessageBox.Show(message, "Mensaje Informativo", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        #endregion

    }
}
