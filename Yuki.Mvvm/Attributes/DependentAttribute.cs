using System;

namespace Yuki.Mvvm.Attributes
{
    /// <summary>
    ///     The <see cref="Attribute"/> used to explicitly declare a property as dependent on another property.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class DependentAttribute : Attribute
    {
    }
}
