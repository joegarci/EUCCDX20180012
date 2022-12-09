namespace MIA.ViewModel
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Clase que contiene las propiedades para la vista de Tickets
    /// </summary>
    public class Tickets_ViewModel
    {
        public string IdTicket { get; set; }
        public string State { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime? ExecutionDate { get; set; }
        public bool Locked { get; set; }
        public bool Priority { get; set; }
        public List<string> NextStatus { get; set; }
        public string SelectName { get; set; }
    }
}
