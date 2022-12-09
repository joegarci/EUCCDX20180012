using MIA.Services;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MIA.Model
{
    [Table("Roles")]
    public partial class Role : NotifyPropertyChanged
    {
        #region Fields
        private string _NameRole;
        private string _DescriptionRole;
        #endregion


        public Role()
        {
            UsersRoles = new HashSet<UsersRole>();
        }

        [Key]
        public byte IdRole { get; set; }

        public string CodRole { get; set; }

        [Required]
        [StringLength(50)]
        public string NameRole
        {
            get => _NameRole;
            set { _NameRole = value; RaisePropertyChanged("NameRole"); }
        }

        [Required]
        [StringLength(250)]
        public string DescriptionRole
        {
            get => _DescriptionRole;
            set { _DescriptionRole = value; RaisePropertyChanged("DescriptionRole"); }
        }

        public byte IdAssistant { get; set; }

        public int RolId { get; set; }

        public virtual ICollection<UsersRole> UsersRoles { get; set; }

        public virtual Assistants Assistants { get; set; }
    }
}
