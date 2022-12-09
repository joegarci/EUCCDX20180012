using MIA.Services;
using System;
using System.Collections.Generic;

namespace MIA.ViewData
{
    public class TicketData : NotifyPropertyChanged
    {
        private string _SelectName;
        private bool _Priority;

        #region Fields
        public string IdTicket { get; set; }
        public string State { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime? ExecutionDate { get; set; }
        public bool Locked { get; set; }

        public bool Priority
        {
            get => _Priority;
            set
            {
                _Priority = value; RaisePropertyChanged("Priority");
            }
        }
        public List<string> NextStatus { get; set; }
        public string SelectName
        {
            get => _SelectName;
            set { _SelectName = value; RaisePropertyChanged("NameUser"); }
        }
        #endregion

    }
}
