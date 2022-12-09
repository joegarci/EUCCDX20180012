using MIA.Model;
using System.ComponentModel.DataAnnotations.Schema;

namespace MIA.ViewModel
{
    [NotMapped]
    public class FrontUser_ViewModel
    {
        public int IdUser { get; set; }
        public string NameUser { get; set; }
        public string FullName { get; set; }
    }
}
