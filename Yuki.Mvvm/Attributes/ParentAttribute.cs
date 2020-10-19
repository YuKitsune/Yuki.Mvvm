using System;
using System.Linq;

namespace Yuki.Mvvm.Attributes
{
    /// <summary>
    ///     The <see cref="Attribute"/> used to explicitly declare a property as a parent to another property.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class ParentAttribute : Attribute
    {
        /// <summary>
        ///     Gets the names of the properties which the target property is a parent to.
        /// </summary>
        public string[] DependantPropertyNames { get; }

        /// <summary>
        ///     Initializes a new instance of the <see cref="ParentAttribute"/> class.
        /// </summary>
        /// <param name="dependantPropertyNames">
        ///     The names of the parent properties.
        /// </param>
        public ParentAttribute(params string[] dependantPropertyNames)
        {
            // If there are any property names to check, then make sure none of them are empty or whitespace.
            if (dependantPropertyNames != null &&
                dependantPropertyNames.Length != 0 &&
                dependantPropertyNames.Any(string.IsNullOrWhiteSpace))
            {
                throw new ArgumentException("One or more property names were null or whitespace.");
            }
            
            DependantPropertyNames = dependantPropertyNames;
        }
    }
}