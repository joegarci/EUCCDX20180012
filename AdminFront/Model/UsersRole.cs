namespace MIA.Model
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public partial class UsersRole
    {
        [Key]
        [Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public byte IdUser { get; set; }

        [Key]
        [Column(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public byte IdRole { get; set; }

        public short MaxIterationsUserRole { get; set; }

        public bool ActiveUserRole { get; set; }

        public string RunnerName { get; set; }

        public virtual Role Role { get; set; }

        public virtual User User { get; set; }
    }
}
