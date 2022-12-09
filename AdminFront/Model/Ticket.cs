namespace MIA.Model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public partial class Ticket
    {
        public Ticket()
        {
            AuditTickets = new HashSet<AuditTicket>();
        }

        [Key]
        public string IdTicket { get; set; }

        public byte IdState { get; set; }

        public DateTime CreationDate { get; set; }

        public DateTime? ExecutionDate { get; set; }

        public bool Locked { get; set; }

        public bool Priority { get; set; }

        public virtual ICollection<AuditTicket> AuditTickets { get; set; }

        public virtual State State { get; set; }
    }
}
