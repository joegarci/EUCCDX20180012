using MIA.Services;
using System.ComponentModel.DataAnnotations;

namespace MIA.Model
{
    public partial class EmailParameters : NotifyPropertyChanged
    {

        #region fields
        private string _SubjectEmailParameter;
        private string _BodyEmailParameter;
        private string _CodEmailParameter;
        private string _CCEmailParameter;
        private string _BCCEmailParameter;
        private string _TOEmailParameter;
        private bool _IsHTMLEmailParameter;
        #endregion

        #region Properties
        [Key]
        public byte IdEmailParameter { get; set; }

        [Required(ErrorMessage = "Debe ingresar el Asunto")]
        public string SubjectEmailParameter
        {
            get => _SubjectEmailParameter;
            set { _SubjectEmailParameter = value; RaisePropertyChanged("SubjectEmailParameter"); }
        }

        [Required(ErrorMessage = "No se permite dejar el cuerpo de vacio")]
        public string BodyEmailParameter
        {
            get => _BodyEmailParameter;
            set { _BodyEmailParameter = value; RaisePropertyChanged("BodyEmailParameter"); }
        }

        public string CodEmailParameter
        {
            get => _CodEmailParameter;
            set { _CodEmailParameter = value; RaisePropertyChanged("CodEmailParameter"); }
        }

        public string CCEmailParameter
        {
            get => _CCEmailParameter;
            set { _CCEmailParameter = value; RaisePropertyChanged("CCEmailParameter"); }
        }

        public string BCCEmailParameter
        {
            get => _BCCEmailParameter;
            set { _BCCEmailParameter = value; RaisePropertyChanged("BCCEmailParameter"); }
        }

        [Required(ErrorMessage = "Debe ingresar el destinatario")]
        public string TOEmailParameter
        {
            get => _TOEmailParameter;
            set { _TOEmailParameter = value; RaisePropertyChanged("TOEmailParameter"); }
        }

        public bool IsHTMLEmailParameter
        {
            get => _IsHTMLEmailParameter;
            set { _IsHTMLEmailParameter = value; RaisePropertyChanged("IsHTMLEmailParameter"); }
        }

        public byte IdAssistant { get; set; }
        #endregion
    }
}
