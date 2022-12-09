using MIA.Services;

namespace MIA.ViewData
{
    public class AdminFrontData : NotifyPropertyChanged
    {
        private bool _Active;
        public int IdUser { get; set; }
        public int IdAssistant { get; set; }
        public string AssistantName { get; set; }
        public bool Active
        {
            get => _Active;
            set { _Active = value; RaisePropertyChanged("Active"); }
        }
    }
}
