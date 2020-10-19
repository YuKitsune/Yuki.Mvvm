using System;
using System.Linq;

namespace Yuki.Mvvm.Attributes
{
    /// <summary>
    ///     The <see cref="Attribute"/> used to explicitly declare a property as Dependant on another property.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class DependantAttribute : Attribute
    {
        /// <summary>
        ///     Gets the names of the properties which the target property is dependant on.
        /// </summary>
        public string[] ParentPropertyNames { get; }

        /// <summary>
        ///     Initializes a new instance of the <see cref="DependantAttribute"/> class.
        /// </summary>
        /// <param name="parentPropertyNames">
        ///     The names of the parent properties.
        /// </param>
        public DependantAttribute(params string[] parentPropertyNames)
        {
            // If there are any property names to check, then make sure none of them are empty or whitespace.
            if (parentPropertyNames != null &&
                parentPropertyNames.Length != 0 &&
                parentPropertyNames.Any(string.IsNullOrWhiteSpace))
            {
                throw new ArgumentException("One or more property names were null or whitespace.");
            }
            
            ParentPropertyNames = parentPropertyNames;
        }
    }
}
