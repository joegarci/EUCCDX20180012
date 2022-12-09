using MIA.Services;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MIA.ViewData
{
    public class AuditTicketData : NotifyPropertyChanged
    {

        private DateTime? _StartDate;
        private DateTime? _EndDate;

        #region Properties
        public string IdTicket { get; set; }
        public string NameUser { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}")]
        [Column(TypeName = "DateTime2")]
        public DateTime? StartDate
        {
            get => _StartDate;
            set
            {
                if (value != _StartDate)
                {
                    _StartDate = value;
                    RaisePropertyChanged("StartDate");
                }
            }
        }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}")]
        [Column(TypeName = "DateTime2")]
        public DateTime? EndDate
        {
            get => _EndDate;
            set
            {
                if (value != _EndDate)
                {
                    _EndDate = value;
                    RaisePropertyChanged("EndDate");
                }
            }
        }
        public string StartState { get; set; }
        public string EndState { get; set; }
        public string Description { get; set; }

        #endregion
    }
}
