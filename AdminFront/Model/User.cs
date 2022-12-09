using MIA.Services;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MIA.Model
{
    public partial class User : NotifyPropertyChanged
    {
        #region Fields
        private string _NameUser;
        private bool _ActiveUser;
        private string _ComputerNameUser;
        private string _FullName;
        private byte _IdType;
        #endregion

        #region Properties
        public User()
        {
            AuditTickets = new HashSet<AuditTicket>();
            UsersRoles = new HashSet<UsersRole>();
            UsersAssistants = new HashSet<UsersAssistants>();
        }
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public byte IdUser { get; set; }

        [Required(ErrorMessage = "Debe ingresar el usuario de red")]
        [StringLength(10)]
        public string NameUser
        {
            get => _NameUser;
            set { _NameUser = value; RaisePropertyChanged("NameUser"); }
        }
        public bool ActiveUser
        {
            get => _ActiveUser;
            set { _ActiveUser = value; RaisePropertyChanged("ActiveUser"); }
        }
        [Required]
        [StringLength(100)]
        public string ComputerNameUser
        {
            get => _ComputerNameUser;
            set { _ComputerNameUser = value; RaisePropertyChanged("ComputerNameUser"); }
        }
        public string FullName
        {
            get => _FullName;
            set { _FullName = value; RaisePropertyChanged("FullName"); }
        }
        public byte IdType
        {
            get => _IdType;
            set { _IdType = value; RaisePropertyChanged("IdType"); }
        }
        #endregion

        #region VirtualKey
        public virtual UsersTypes TypeUser { get; set; }

        public virtual ICollection<AuditTicket> AuditTickets { get; set; }

        public virtual ICollection<UsersRole> UsersRoles { get; set; }

        public virtual ICollection<UsersAssistants> UsersAssistants { get; set; }
        #endregion
    }
}
