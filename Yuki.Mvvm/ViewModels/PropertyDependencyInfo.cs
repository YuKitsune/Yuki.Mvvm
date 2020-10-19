using System;
using System.Collections.Generic;
using System.Linq;

namespace Yuki.Mvvm.ViewModels
{
    /// <summary>
    ///     The class containing information on a particular property and it's dependants.
    /// </summary>
    internal class PropertyDependencyInfo
    {
        /// <summary>
        ///     Gets the name of the parent property.
        /// </summary>
        public string ParentPropertyName { get; }
        
        /// <summary>
        ///     Gets the names of the properties (names) which are dependant on the parent property.
        /// </summary>
        public List<string> DependantPropertyNames { get; }

        /// <summary>
        ///     Initializes a new instance of the <see cref="PropertyDependencyInfo"/> class.
        /// </summary>
        /// <param name="parentPropertyName">
        ///     The name of the parent property.
        /// </param>
        /// <param name="dependantPropertyNames">
        ///     The <see cref="List{T}"/> of properties (names) which are dependant on the parent property.
        /// </param>
        public PropertyDependencyInfo(string parentPropertyName, params string[] dependantPropertyNames)
        {
            if (string.IsNullOrWhiteSpace(parentPropertyName)) throw new ArgumentNullException(nameof(parentPropertyName));
            if (dependantPropertyNames == null ||
                dependantPropertyNames.Length == 0)
            {
                throw new ArgumentException($"The {nameof(dependantPropertyNames)} cannot be null or empty.", nameof(parentPropertyName));
            }

            if (dependantPropertyNames.Any(string.IsNullOrWhiteSpace))
            {
                throw new ArgumentException($"The {nameof(dependantPropertyNames)} cannot contain a property name which is null or whitespace.", nameof(parentPropertyName));
            }

            ParentPropertyName = parentPropertyName;
            DependantPropertyNames = dependantPropertyNames.ToList();
        }
    }
}