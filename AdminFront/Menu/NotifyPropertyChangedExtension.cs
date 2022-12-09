namespace MIA.Menu
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;
    /// <summary>
    /// Clase de extensión de la interfaz INotifyPropertyChanged que es usado para devolver cambios observables en las propiedades conocidas.
    /// </summary>
    public static class NotifyPropertyChangedExtension
    {
        /// <summary>
        /// Método que permite establecer el nuevo valor al campo que esta cambiando
        /// </summary>
        /// <typeparam name="TField">Tipo del campo que esta cambiando.</typeparam>
        /// <param name="instance">Instancia de la interfaz INotifyPropertyChanged que esta activa.</param>
        /// <param name="field">Valor del campo que esta cambiando.</param>
        /// <param name="newValue">Nuevo valor del campo que esta cambiando.</param>
        /// <param name="raise">Encapsula un método que tiene un único parámetro y no devuelve un valor.</param>
        /// <param name="propertyName">Nombre de la propiedad que esta cambiando.</param>
        public static void MutateVerbose<TField>(this INotifyPropertyChanged instance, ref TField field, TField newValue, Action<PropertyChangedEventArgs> raise, [CallerMemberName] string propertyName = null)
        {
            if (instance is null)
            {
                throw new ArgumentNullException(nameof(instance));
            }

            if (EqualityComparer<TField>.Default.Equals(field, newValue))
            {
                return;
            }

            field = newValue;
            raise?.Invoke(new PropertyChangedEventArgs(propertyName));
        }
    }
}
