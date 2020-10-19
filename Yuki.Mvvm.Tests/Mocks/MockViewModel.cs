using System;
using System.Threading.Tasks;

using Yuki.Mvvm.Attributes;
using Yuki.Mvvm.Commands;
using Yuki.Mvvm.ViewModels;

namespace Yuki.Mvvm.Tests.Mocks
{
    /// <summary>
    ///     The mocked <see cref="ViewModel"/>.
    /// </summary>
    public class MockViewModel : ViewModel
    {
        /// <summary>
        ///     Gets or sets a <see cref="Guid"/>.
        /// </summary>
        public Guid Guid
        {
            get => Get<Guid>();
            set => Set(value);
        }

        /// <summary>
        ///     Gets or sets an <see cref="int"/>.
        /// </summary>
        public int Integer
        {
            get => Get<int>();
            set => Set(value);
        }

        /// <summary>
        ///     Gets or sets a <see cref="TestClass"/>.
        /// </summary>
        public TestClass Class
        {
            get => Get<TestClass>();
            set => Set(value);
        }

        /// <summary>
        ///     Gets or sets the invalid property.
        /// </summary>
        public string InvalidProperty
        {
            get => Get<string>(null);
            set => Set(value, string.Empty);
        }

        /// <summary>
        ///     Gets the <see cref="int"/> which is implicitly dependant on <see cref="Integer"/>.
        /// </summary>
        public int ImplicitlyDependantInteger => Integer * 2;

        /// <summary>
        ///     Gets or sets the <see cref="int"/> which is explicitly dependant on <see cref="Integer"/>.
        /// </summary>
        [Dependant(nameof(Integer))]
        public int ExplicitlyDependantInteger
        {
            get => Integer * 5;

            // ReSharper disable once ValueParameterNotUsed
            set
            {
                // Do nothing
            }
        }

        /// <summary>
        ///     Gets the <see cref="RelayCommand"/> for incrementing the <see cref="Integer"/> property.
        /// </summary>
        public RelayCommand IncrementIntegerCommand => Get(new RelayCommand(IncrementInteger));

        /// <summary>
        ///     Gets the <see cref="RelayCommand"/> for incrementing the <see cref="Integer"/> property.
        /// </summary>
        public AsyncCommand IncrementIntegerAsyncCommand => Get(new AsyncCommand(IncrementIntegerAsync));

        /// <summary>
        ///     Increments the <see cref="Integer"/> property.
        /// </summary>
        public void IncrementInteger() => Integer++;

        /// <summary>
        ///     Gets the <see cref="Guid"/> of the <see cref="MockEvent"/> which was received.
        /// </summary>
        public Guid ReceivedMockEventId { get; private set; }

        /// <summary>
        ///     Gets the <see cref="Guid"/> of the <see cref="MockEvent"/> which was received asynchronously.
        /// </summary>
        public Guid ReceivedAsyncMockEventId { get; private set; }

        /// <summary>
        ///     Gets the <see cref="Guid"/> of the <see cref="MockEvent"/> which was received, whose handler was
        ///     was automatically subscribed.
        /// </summary>
        public Guid ReceivedAutoMockEventId { get; private set; }

        /// <summary>
        ///     Gets the <see cref="Guid"/> of the <see cref="MockEvent"/> which was received asynchronously, whose
        ///     handler was was automatically subscribed.
        /// </summary>
        public Guid ReceivedAutoAsyncMockEventId { get; private set; }

        /// <summary>
        ///     Increments the <see cref="Integer"/> property as an asynchronous operation.
        /// </summary>
        /// <returns>
        ///     The <see cref="Task"/> representing the asynchronous operation.
        /// </returns>
        public async Task IncrementIntegerAsync()
        {
            await Task.Yield();
            IncrementInteger();
        }
    }
}
