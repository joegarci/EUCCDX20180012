namespace MIA.Model
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    public class Assistants
    {
        public Assistants()
        {
            UsersAssistants = new HashSet<UsersAssistants>();
            RpaParameters = new HashSet<RpaParameter>();
            Roles = new HashSet<Role>();
        }
        [Key]
        public byte IdAssistant { get; set; }
        public string CodAssistant { get; set; }
        public string NameAssistant { get; set; }

        public virtual ICollection<UsersAssistants> UsersAssistants { get; set; }
        public virtual ICollection<RpaParameter> RpaParameters { get; set; }
        public virtual ICollection<Role> Roles { get; set; }
    }
}
