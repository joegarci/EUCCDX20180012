using MIA.Services;
using System.ComponentModel.DataAnnotations;

namespace MIA.Model
{
    public partial class RpaParameter : NotifyPropertyChanged
    {
        #region Fields
        private string _NameParameter;
        private string _ValueParameter;
        private bool _IsVisibleParameter;
        private string _DescriptionParameter;
        #endregion

        #region Properties
        [Key]
        public byte IdParameter { get; set; }
        public string CodParameter { get; set; }
        [Required]
        [StringLength(50)]
        public string NameParameter
        {
            get => _NameParameter;
            set { _NameParameter = value; RaisePropertyChanged("NameParameter"); }
        }
        [Required]
        public string ValueParameter
        {
            get => _ValueParameter;
            set { _ValueParameter = value; RaisePropertyChanged("ValueParameter"); }
        }
        public bool IsVisibleParameter
        {
            get => _IsVisibleParameter;
            set { _IsVisibleParameter = value; RaisePropertyChanged("IsVisibleParameter"); }
        }
        public string DescriptionParameter
        {
            get => _DescriptionParameter;
            set { _DescriptionParameter = value; RaisePropertyChanged("DescriptionParameter"); }
        }
        public byte IdAssistant { get; set; }
        public virtual Assistants Assistants { get; set; }
        #endregion
    }
}
