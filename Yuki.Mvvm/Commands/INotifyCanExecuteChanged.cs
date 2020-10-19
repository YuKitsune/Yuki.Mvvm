using System;
using System.Windows.Input;

namespace Yuki.Mvvm.Commands
{
    /// <summary>
    ///     The interface to implement allowing for the invocation of the CanExecuteChanged event.
    /// </summary>
    internal interface INotifyCanExecuteChanged
    {
        /// <summary>
        ///     The event to raise once the value of the <see cref="ICommand.CanExecute"/> method may have been changed.
        /// </summary>
        event EventHandler CanExecuteChanged;

        /// <summary>
        ///     Raises the <see cref="ICommand.CanExecuteChanged"/> event.
        /// </summary>
        void RaiseCanExecuteChanged();
    }
}
