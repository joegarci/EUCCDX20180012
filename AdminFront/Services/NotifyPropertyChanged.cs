using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace MIA.Services
{
    public class NotifyPropertyChanged : INotifyPropertyChanged
    {
        protected void RaisePropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        public event PropertyChangedEventHandler PropertyChanged;
    }
}
