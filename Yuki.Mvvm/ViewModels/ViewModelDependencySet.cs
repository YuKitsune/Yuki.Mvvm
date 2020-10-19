using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Yuki.Mvvm.Attributes;

namespace Yuki.Mvvm.ViewModels
{
    /// <summary>
    ///     A set of property dependencies for a specific <see cref="ViewModel"/>.
    /// </summary>
    internal class ViewModelDependencySet
    {
        /// <summary>
        ///     Gets the <see cref="Type"/> of <see cref="ViewModel"/>.
        /// </summary>
        public Type ViewModelType { get; }
        
        /// <summary>
        ///     Gets the names of properties which are implicitly dependant on others (get-only).
        /// </summary>
        public IReadOnlyCollection<string> ImplicitlyDependantPropertyNames { get; }
        
        /// <summary>
        ///     Gets the <see cref="PropertyDependencyInfo"/>s.
        /// </summary>
        public IReadOnlyCollection<PropertyDependencyInfo> DependencyInfo { get; }
        
        /// <summary>
        ///     Initializes a new instance of the <see cref="ViewModelDependencySet"/> class.
        /// </summary>
        /// <param name="viewModelType">
        ///     The <see cref="Type"/> of <see cref="ViewModel"/>.
        /// </param>
        /// <param name="implicitlyDependantPropertyNames">
        ///     The names of properties which only have a getter, and do not have a <see cref="DependantAttribute"/> or
        ///     a <see cref="ParentAttribute"/>.
        /// </param>
        /// <param name="dependencyInfo">
        ///     A <see cref="ICollection{T}"/> of <see cref="PropertyDependencyInfo"/> for the <see cref="ViewModel"/>.
        /// </param>
        public ViewModelDependencySet(
            Type viewModelType,
            ICollection<string> implicitlyDependantPropertyNames,
            ICollection<PropertyDependencyInfo> dependencyInfo)
        {
            ViewModelType = viewModelType;
            ImplicitlyDependantPropertyNames = new ReadOnlyCollection<string>(implicitlyDependantPropertyNames.ToList());
            DependencyInfo = new ReadOnlyCollection<PropertyDependencyInfo>(dependencyInfo.ToList());
        }

        /// <summary>
        ///     Gets the names of all properties which may depend on a given property.
        /// </summary>
        /// <param name="propertyName">
        ///     The name of the property.
        /// </param>
        /// <returns>
        ///     The names of properties which may depend on the given property.
        /// </returns>
        public string[] GetDependenciesFor(string propertyName)
        { 
            List<string> propertyNames = ImplicitlyDependantPropertyNames.ToList();
            propertyNames.AddRange(
                DependencyInfo
                    .Where(d => d.ParentPropertyName == propertyName)
                    .SelectMany(d => d.DependantPropertyNames));

            // Filter out any duplicates, and make sure we're not returning the given property name in the list
            return propertyNames.Distinct().Except(new []{ propertyName }).ToArray();
        }
    }
}