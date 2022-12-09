namespace MIA.Model
{
    using System;
    using System.ComponentModel.DataAnnotations;

    public partial class AuditTicket
    {
        [Key]
        public int IdAuditTicket { get; set; }

        public string IdTicket { get; set; }

        public byte IdUser { get; set; }

        public DateTime StartDateAuditTicket { get; set; }

        public DateTime? EndDateAuditTicket { get; set; }

        public byte StartStateAuditTicket { get; set; }

        public byte EndStateAuditTicket { get; set; }

        [StringLength(500)]
        public string DescriptionAuditTicket { get; set; }

        public virtual State State { get; set; }

        public virtual State State1 { get; set; }

        public virtual Ticket Ticket { get; set; }

        public virtual User User { get; set; }
    }
}
