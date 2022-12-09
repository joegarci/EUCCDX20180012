namespace MIA.Model
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public class UsersAssistants
    {
        [Key]
        [Column(Order = 1)]
        public byte IdUser { get; set; }
        [Key]
        [Column(Order = 2)]
        public byte IdAssistant { get; set; }

        public bool ActiveUserAssistant { get; set; }

        public virtual Assistants Assistants { get; set; }

        public virtual User User { get; set; }

    }
}
