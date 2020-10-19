using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

using Yuki.Mvvm.Attributes;
using Yuki.Mvvm.Commands;

namespace Yuki.Mvvm.ViewModels
{
    /// <summary>
    ///     The base View Model implementing <see cref="INotifyPropertyChanging"/> and
    ///     <see cref="INotifyPropertyChanged"/>.
    /// </summary>
    public abstract class ViewModel : INotifyPropertyChanging, INotifyPropertyChanged
    {
        /// <summary>
        ///     The <see cref="IReadOnlyCollection{T}"/> of property names who only have a getter methods.
        /// </summary>
        private readonly IReadOnlyCollection<string> _getOnlyProperties;

        /// <summary>
        ///     The <see cref="IReadOnlyCollection{T}"/> of property names who have been explicitly declared as a
        ///     dependent property with the <see cref="DependentAttribute"/>.
        /// </summary>
        private readonly IReadOnlyCollection<string> _explicitlyDependentProperties;

        /// <summary>
        ///     The <see cref="Dictionary{TKey,TValue}"/> of properties and values, where the Key is the property name
        ///     and the value is the properties value.
        /// </summary>
        private readonly Dictionary<string, object> _propertyValues;

        /// <summary>
        ///     The event raised when the value of a property is about to change.
        /// </summary>
        public event PropertyChangingEventHandler PropertyChanging;

        /// <summary>
        ///     The event raised when the value of a property has changed.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        ///     Initializes a new instance of the <see cref="ViewModel"/> class.
        /// </summary>
        protected ViewModel()
        {
            _propertyValues = new Dictionary<string, object>();
            _getOnlyProperties = FindGetOnlyProperties().ToList().AsReadOnly();
            _explicitlyDependentProperties = FindDependentProperties().ToList().AsReadOnly();
        }

        /// <summary>
        ///     Gets an <see cref="ICollection{T}"/> of property names who do not have setter methods.
        /// </summary>
        /// <returns>
        ///     The <see cref="ICollection{T}"/> of property names.
        /// </returns>
        private ICollection<string> FindGetOnlyProperties() =>
            GetType().GetProperties()
                     .Where(p => p.SetMethod == null)
                     .Select(p => p.Name).ToList();

        /// <summary>
        ///     Gets an <see cref="ICollection{T}"/> of property names who have a <see cref="DependentAttribute"/>.
        /// </summary>
        /// <returns>
        ///     The <see cref="ICollection{T}"/> of property names.
        /// </returns>
        private ICollection<string> FindDependentProperties() =>
            GetType().GetProperties()
                     .Where(p => p.GetCustomAttribute<DependentAttribute>() != null)
                     .Select(p => p.Name)
                     .ToList();

        /// <summary>
        ///     Gets the value of the property.
        /// </summary>
        /// <param name="propertyName">
        ///     The name of the property.
        /// </param>
        /// <typeparam name="TValue">
        ///     The type of value expected.
        /// </typeparam>
        /// <returns>
        ///     The value of the property.
        /// </returns>
        protected TValue Get<TValue>([CallerMemberName] string propertyName = null)
        {
            // Ensure we have a property name
            EnsurePropertyNameIsValid(propertyName);

            // If we have a value, return it
            // ReSharper disable once AssignNullToNotNullAttribute
            //  ^ Already checking in EnsurePropertyNameIsValid
            if (_propertyValues.ContainsKey(propertyName)) return (TValue)_propertyValues[propertyName];

            // Otherwise use the default value
            return default;
        }

        /// <summary>
        ///     Gets the value of the property or uses the given default value.
        /// </summary>
        /// <param name="defaultValue">
        ///     The default value to use if the property has not been registered yet.
        /// </param>
        /// <param name="propertyName">
        ///     The name of the property.
        /// </param>
        /// <typeparam name="TValue">
        ///     The type of value expected.
        /// </typeparam>
        /// <returns>
        ///     The value of the property.
        /// </returns>
        protected TValue Get<TValue>(TValue defaultValue, [CallerMemberName] string propertyName = null)
        {
            // Ensure we have a property name
            EnsurePropertyNameIsValid(propertyName);

            // If we have a value, return it
            // ReSharper disable once AssignNullToNotNullAttribute
            //  ^ Already checking in EnsurePropertyNameIsValid
            if (_propertyValues.ContainsKey(propertyName))
            {
                return (TValue)_propertyValues[propertyName];
            }
            else
            {
                // Otherwise, set it to our default
                Set(defaultValue, propertyName);
            }

            // Return the value
            return Get<TValue>(propertyName);
        }

        /// <summary>
        ///     Sets the value of the property.
        /// </summary>
        /// <param name="value">
        ///        The value to set the property to.
        /// </param>
        /// <param name="propertyName">
        ///     The name of the property.
        /// </param>
        /// <typeparam name="TValue">
        ///        The type of value being set.
        /// </typeparam>
        protected void Set<TValue>(TValue value, [CallerMemberName] string propertyName = null)
        {
            // Ensure we have a property name
            EnsurePropertyNameIsValid(propertyName);

            // If the value is there, update it
            OnPropertyChanging(propertyName);

            // Add a new value, or update an existing one
            _propertyValues[propertyName] = value;

            // Notify
            OnPropertyChanged(propertyName);

            // Also notify any dependent properties
            List<string> dependentProperties = _getOnlyProperties.ToList();
            dependentProperties.AddRange(_explicitlyDependentProperties);
            foreach (string dependentPropertyName in dependentProperties)
            {
                OnPropertyChanged(dependentPropertyName);
            }
        }

        /// <summary>
        ///        Method called to raise the <see cref="PropertyChanging"/> event.
        /// </summary>
        /// <param name="propertyName">
        ///        The name of the property whose value is about to change.
        /// </param>
        protected void OnPropertyChanging([CallerMemberName] string propertyName = null)
        {
            // Ensure we have a property name
            EnsurePropertyNameIsValid(propertyName);

            // Invoke the event
            PropertyChanging?.Invoke(this, new PropertyChangingEventArgs(propertyName));
        }

        /// <summary>
        ///        Method called to raise the <see cref="PropertyChanged"/> event.
        /// </summary>
        /// <param name="propertyName">
        ///        The name of the property whose value has changed.
        /// </param>
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            // Ensure we have a property name
            EnsurePropertyNameIsValid(propertyName);

            // Invoke the event
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

            // Raise the CanExecuteChanged method on any RelayCommands
            foreach (KeyValuePair<string, object> property in _propertyValues)
            {
                if (property.Value is INotifyCanExecuteChanged command) command.RaiseCanExecuteChanged();
            }
        }

        /// <summary>
        ///     Ensures that the given property name is valid. If the name of the property is invalid, then an
        ///     <see cref="Exception"/> will be thrown.
        /// </summary>
        /// <param name="propertyName">
        ///        The name of the property.
        /// </param>
        private void EnsurePropertyNameIsValid(string propertyName)
        {
            if (string.IsNullOrEmpty(propertyName))
            {
                throw new ArgumentNullException(
                    nameof(propertyName),
                    "The property name cannot be null or empty");
            }
        }
    }
}
