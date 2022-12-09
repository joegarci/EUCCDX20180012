namespace MIA.Model
{
    using MIA.Services;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public partial class State : NotifyPropertyChanged
    {
        public State()
        {
            AuditTickets = new HashSet<AuditTicket>();
            AuditTickets1 = new HashSet<AuditTicket>();
            Tickets = new HashSet<Ticket>();
        }

        [Key]
        public byte IdState { get; set; }

        public string CodState { get; set; }

        [Required]
        [StringLength(50)]
        public string NameState { get; set; }

        public byte IdAssistant { get; set; }


        public virtual ICollection<AuditTicket> AuditTickets { get; set; }

        public virtual ICollection<AuditTicket> AuditTickets1 { get; set; }

        public virtual ICollection<Ticket> Tickets { get; set; }
        public virtual Assistants Assistants { get; set; }
    }
}
