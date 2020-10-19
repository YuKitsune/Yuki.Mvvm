using System;
using System.Windows.Input;

namespace Yuki.Mvvm.Commands
{
    /// <summary>
    ///     The base <see cref="ICommand"/>.
    /// </summary>
    public abstract class BaseCommand : ICommand, INotifyCanExecuteChanged
    {
        /// <summary>
        ///     The event raised when the return value of the <see cref="CanExecute"/> method has changed.
        /// </summary>
        public event EventHandler CanExecuteChanged;

        /// <summary>
        ///     Determines whether the current <see cref="BaseCommand"/> can be executed in its current state.
        /// </summary>
        /// <returns>
        ///     <c>true</c> if the current <see cref="BaseCommand"/> can be executed; otherwise, <c>false</c>.
        /// </returns>
        public abstract bool CanExecute();

        /// <summary>
        ///     Executes the current <see cref="BaseCommand"/>.
        /// </summary>
        public abstract void Execute();

        /// <summary>
        ///     Determines whether the current <see cref="BaseCommand"/> can be executed in its current state.
        /// </summary>
        /// <param name="parameter">
        ///     Unused.
        /// </param>
        /// <returns>
        ///     <c>true</c> if the current <see cref="BaseCommand"/> can be executed; otherwise, <c>false</c>.
        /// </returns>
        bool ICommand.CanExecute(object parameter) => CanExecute();

        /// <summary>
        ///     Executes the current <see cref="BaseCommand"/>.
        /// </summary>
        /// <param name="parameter">
        ///     Unused.
        /// </param>
        void ICommand.Execute(object parameter) => Execute();

        /// <summary>
        ///     Raises the <see cref="CanExecuteChanged"/> event.
        /// </summary>
        public void RaiseCanExecuteChanged() => CanExecuteChanged?.Invoke(this, new EventArgs());
    }
}
