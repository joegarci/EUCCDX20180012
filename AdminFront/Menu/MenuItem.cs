namespace MIA.Menu
{
    using System;
    using System.ComponentModel;
    using System.Windows;
    using System.Windows.Controls;
    /// <summary>
    /// Clase contiene la definición del objecto MenuItem
    /// </summary>
    public class MenuItem : INotifyPropertyChanged
    {
        #region Propiedades Privadas
        /// <summary>
        /// Propiedad que contiene el nombre de la opcion del Menú
        /// </summary>
        private string _name;
        /// <summary>
        /// Propiedad que contiene el icono que se mostrara en el Menú
        /// </summary>
        private string _icon;
        /// <summary>
        /// Propiedad que contiene el objecto de la vista que se cargará para la opcion del Menú
        /// </summary>
        private object _content;
        /// <summary>
        /// Propiedad que contiene el valor del objecto de Visibilidad del Scroll Horizontal 
        /// </summary>
        private ScrollBarVisibility _horizontalScrollBarVisibilityRequirement;
        /// <summary>
        /// Propiedad que contiene el valor del objecto de Visibilidad del Scroll Vertical 
        /// </summary>
        private ScrollBarVisibility _verticalScrollBarVisibilityRequirement;
        /// <summary>
        /// Propiedad de contiene el valor del objecto de Margenes 
        /// </summary>
        private Thickness _marginRequirement = new Thickness(16);
        #endregion

        /// <summary>
        /// Metodo que estable los valores para el nombre, icono y contenido del menú.
        /// </summary>
        /// <param name="name">Nombre de la opcion del Menú</param>
        /// <param name="content">Objecto de la vista que se cargará para la opcion del Menú</param>
        /// <param name="icon">Nombre del Icono que se mostrara en el Menú</param>
        public MenuItem(string name, object content, string icon)
        {
            _name = name;
            _icon = icon;
            Content = content;
        }

        /// <summary>
        /// Propiedad que obtiene / establece el valor del icono
        /// </summary>
        public string Icon
        {
            get => _icon;
            set => this.MutateVerbose(ref _icon, value, RaisePropertyChanged());
        }

        /// <summary>
        /// Propiedad que obtiene / establece el valor del nombre
        /// </summary>
        public string Name
        {
            get => _name;
            set => this.MutateVerbose(ref _name, value, RaisePropertyChanged());
        }

        /// <summary>
        /// Propiedad que obtiene / establece el valor del contenido
        /// </summary>
        public object Content
        {
            get => _content;
            set => this.MutateVerbose(ref _content, value, RaisePropertyChanged());
        }

        /// <summary>
        /// Propiedad que obtiene / establece el valor del objecto de Visibilidad del Scroll Horizontal 
        /// </summary>
        public ScrollBarVisibility HorizontalScrollBarVisibilityRequirement
        {
            get => _horizontalScrollBarVisibilityRequirement;
            set => this.MutateVerbose(ref _horizontalScrollBarVisibilityRequirement, value, RaisePropertyChanged());
        }

        /// <summary>
        /// Propiedad que obtiene / establece el valor del objecto de Visibilidad del Scroll Vertical
        /// </summary>
        public ScrollBarVisibility VerticalScrollBarVisibilityRequirement
        {
            get => _verticalScrollBarVisibilityRequirement;
            set => this.MutateVerbose(ref _verticalScrollBarVisibilityRequirement, value, RaisePropertyChanged());
        }

        /// <summary>
        /// Propiedad que obtiene / establece el valor del objecto de Margen
        /// </summary>
        public Thickness MarginRequirement
        {
            get => _marginRequirement;
            set => this.MutateVerbose(ref _marginRequirement, value, RaisePropertyChanged());
        }

        /// <summary>
        /// Evento que proporciona la visualizacion de los cambios efectuados en el objecto.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Metodo que se usa para notificar a la UI o elementos enlazados que los datos han cambiado.
        /// </summary>
        /// <returns></returns>
        private Action<PropertyChangedEventArgs> RaisePropertyChanged()
        {
            return args => PropertyChanged?.Invoke(this, args);
        }
    }
}
