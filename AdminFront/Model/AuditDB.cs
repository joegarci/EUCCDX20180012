using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MIA.Model
{
    [Table("AuditDB")]
    public class AuditDB
    {
        [Key]
        public int IdAuditDB { get; set; }
        public byte IdUser { get; set; }
        public string TableNameAuditDB { get; set; }
        public DateTime DateOperationAuditDB { get; set; }
        public string OperationTypeAuditDB { get; set; }
        public string OldValueAuditDB { get; set; }
        public string NewValueAuditDB { get; set; }
    }
}
