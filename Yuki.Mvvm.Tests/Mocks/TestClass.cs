using System;

namespace Yuki.Mvvm.Tests.Mocks
{
    /// <summary>
    ///     A class used to test the <see cref="MockViewModel"/>.
    /// </summary>
    public class TestClass
    {
        /// <summary>
        ///     Gets the <see cref="System.Guid"/> of the current <see cref="TestClass"/>.
        /// </summary>
        public Guid Guid { get; }

        /// <summary>
        ///     Initializes a new instance of the <see cref="TestClass"/> class.
        /// </summary>
        public TestClass() => Guid = Guid.NewGuid();
    }
}
