using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace PeekServiceMonitor.PropertyChanged
{
    public abstract class NotifyPropertyChangedBase : INotifyPropertyChanged, INotifyPropertyChanging
    {
        public event PropertyChangingEventHandler PropertyChanging;
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Sets the field to the specified value, raising the <see cref="PropertyChanged"/> event if necessary.
        /// </summary>
        /// <typeparam name="T">The type of field being set.</typeparam>
        /// <param name="field">A reference to the field being set.</param>
        /// <param name="value">The new value to which the field should be set.</param>
        /// <param name="propertyName">The name to be used when raising the <see cref="PropertyChanged"/> event. If null, the name of the caller
        /// will be used.</param>
        /// <returns><c>true</c> if the new value was different from the previous value; <c>false</c> otherwise.</returns>
        protected bool SetField<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) return false;

            OnPropertyChanging(propertyName);
            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }

        /// <summary>
        /// Raises the <see cref="PropertyChanging"/> event for a property with the specified name.
        /// </summary>
        /// <param name="propertyName"></param>
        protected void OnPropertyChanging(string propertyName)
        {
            PropertyChanging?.Invoke(this, new PropertyChangingEventArgs(propertyName));
        }

        /// <summary>
        /// Raises the <see cref="PropertyChanged"/> event for a property with the specified name.
        /// </summary>
        /// <param name="propertyName"></param>
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
