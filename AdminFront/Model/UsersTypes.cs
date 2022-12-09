namespace MIA.Model
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("UsersTypes")]
    public partial class UsersTypes
    {
        public UsersTypes()
        {
            Users = new HashSet<User>();
        }
        [Key]
        public byte IdType { get; set; }

        public string CodUserType { get; set; }
        public string NameUserType { get; set; }
        public string DescriptionUserType { get; set; }

        public virtual ICollection<User> Users { get; set; }
    }
}
