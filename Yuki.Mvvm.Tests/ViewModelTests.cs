using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using NUnit.Framework;
using Yuki.Mvvm.Commands;
using Yuki.Mvvm.Tests.Mocks;
using Yuki.Mvvm.ViewModels;

namespace Yuki.Mvvm.Tests
{
    /// <summary>
    ///     The <see cref="ViewModel"/> tests.
    /// </summary>
    [TestFixture]
    public class ViewModelTests
    {
        /// <summary>
        ///     Ensures that property values can be set, and retrieved.
        /// </summary>
        [Test]
        public void CanGetAndSetPropertyValues()
        {
            // Arrange
            Guid setGuid = Guid.NewGuid();
            int setInt = 1;
            TestClass setClass = new TestClass();

            // Act
            MockViewModel viewModel = new MockViewModel { Guid = setGuid, Integer = setInt, Class = setClass };
            Guid getGuid = viewModel.Guid;
            int getInt = viewModel.Integer;
            TestClass getClass = viewModel.Class;

            // Assert
            Assert.AreEqual(setGuid, getGuid);
            Assert.AreEqual(setInt, getInt);
            Assert.AreEqual(setClass, getClass);
        }

        /// <summary>
        ///     Ensures that the default values can be retrieved when no value has been set.
        /// </summary>
        [Test]
        public void CanGetDefaultValues()
        {
            // Arrange
            MockViewModel viewModel = new MockViewModel();

            // Act / Assert
            Assert.AreEqual(viewModel.Guid, default(Guid));
            Assert.AreEqual(viewModel.Integer, 0);
            Assert.IsNull(viewModel.Class);
        }

        /// <summary>
        ///     Ensures that the Get and Set methods will throw an <see cref="ArgumentNullException"/> when the property
        ///     name is not provided.
        /// </summary>
        [Test]
        public void GetAndSetMethodsThrowWithoutPropertyName()
        {
            // Arrange
            MockViewModel viewModel = new MockViewModel();

            // Act / Assert
            Assert.Throws<ArgumentNullException>(() => Console.WriteLine(viewModel.InvalidProperty));
            Assert.Throws<ArgumentNullException>(() => viewModel.InvalidProperty = "Test");
        }

        /// <summary>
        ///     Ensures that the Get method sets the default value if one is not already present, and does not duplicate
        ///     new objects.
        /// </summary>
        [Test]
        public void DefaultGettersWork()
        {
            // Arrange
            MockViewModel viewModel = new MockViewModel();

            // Act / Assert
            for (int i = 1; i <= 10; i++)
            {
                viewModel.IncrementIntegerCommand.Execute();
                Assert.AreEqual(i, viewModel.Integer);
            }
        }

        /// <summary>
        ///     Ensures the <see cref="INotifyPropertyChanging.PropertyChanging"/> and
        ///     <see cref="INotifyPropertyChanged.PropertyChanged"/> events are raised correctly.
        /// </summary>
        [Test]
        public void PropertyUpdateEventsAreRaised()
        {
            // Arrange
            // Set up the ViewModel
            int startingValue = 2;
            int finishingValue = 5;
            MockViewModel viewModel = new MockViewModel { Integer = startingValue };

            // Some variables to track whether or not the events were raised
            List<string> propertiesChanging = new List<string>();
            List<string> propertiesChanged = new List<string>();
            bool canExecuteChangedWasCalled = false;

            // Local function to handle the CanExecuteChanged event
            void OnCanExecuteChanged(object sender, EventArgs args)
            {
                if (sender is RelayCommand testCommand)
                {
                    // Ensure the value has been set
                    Assert.AreEqual(viewModel.IncrementIntegerCommand, testCommand);

                    // Set these for later
                    canExecuteChangedWasCalled = true;
                }
                else
                {
                    // Not the RelayCommand, fail!
                    Assert.Fail();
                }
            }

            // Local function to handle the PropertyChanging event
            void OnPropertyChanging(object sender, PropertyChangingEventArgs args)
            {
                if (sender is MockViewModel testViewModel)
                {
                    // Ensure the value hasn't been set yet
                    Assert.AreEqual(startingValue, testViewModel.Integer);

                    // Update the list
                    propertiesChanging.Add(args.PropertyName);
                }
                else
                {
                    // Not the ViewModel, fail!
                    Assert.Fail();
                }
            }

            // Local function to handle the PropertyChanged event
            void OnPropertyChanged(object sender, PropertyChangedEventArgs args)
            {
                if (sender is MockViewModel testViewModel)
                {
                    // Ensure the value has been set
                    Assert.AreEqual(finishingValue, testViewModel.Integer);

                    // Update the list
                    propertiesChanged.Add(args.PropertyName);
                }
                else
                {
                    // Not the ViewModel, fail!
                    Assert.Fail();
                }
            }

            // Setup the events
            viewModel.IncrementIntegerCommand.CanExecuteChanged += OnCanExecuteChanged;
            viewModel.PropertyChanging += OnPropertyChanging;
            viewModel.PropertyChanged += OnPropertyChanged;

            try
            {
                // Act
                viewModel.Integer = finishingValue;
            }
            finally
            {
                // Clean up, just in case
                viewModel.IncrementIntegerCommand.CanExecuteChanged -= OnCanExecuteChanged;
                viewModel.PropertyChanging -= OnPropertyChanging;
                viewModel.PropertyChanged -= OnPropertyChanged;
            }

            // Assert
            Assert.IsTrue(canExecuteChangedWasCalled);
            Assert.Contains(nameof(viewModel.Integer), propertiesChanging);
            Assert.Contains(nameof(viewModel.Integer), propertiesChanged);
            Assert.Contains(nameof(viewModel.ImplicitlyDependentInteger), propertiesChanged);
            Assert.Contains(nameof(viewModel.ExplicitlyDependentInteger), propertiesChanged);
        }
    }
}
