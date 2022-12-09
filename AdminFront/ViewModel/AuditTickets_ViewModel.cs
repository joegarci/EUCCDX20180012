namespace MIA.ViewModel
{
    using System;

    /// <summary>
    /// Clase que contiene las propiedades para la vista de Auditoria
    /// </summary>
    /// 
    public class AuditTickets_ViewModel
    {
        public string IdTicket { get; set; }
        public string NameUser { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string StartState { get; set; }
        public string EndState { get; set; }
        public string Description { get; set; }
    }
}
