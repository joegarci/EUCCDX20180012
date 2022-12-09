using MIA.Model;
using System.ComponentModel.DataAnnotations.Schema;

namespace MIA.ViewModel
{
    [NotMapped]
    public class Check_ViewModel : State
    {
        private bool _IsSelect;
        public bool IsSelect
        {
            get => _IsSelect;
            set { _IsSelect = value; RaisePropertyChanged("IsSelect"); }
        }
    }
}
