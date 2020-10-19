using System;
using System.Windows.Input;

namespace Yuki.Mvvm.Commands
{
    /// <summary>
    ///     The base <see cref="ICommand"/>.
    /// </summary>
    /// <typeparam name="T">
    ///     The type of parameter to be passed.
    /// </typeparam>
    public abstract class BaseCommand<T> : ICommand, INotifyCanExecuteChanged
    {
        /// <summary>
        ///     The event raised when the return value of the <see cref="CanExecute"/> method has changed.
        /// </summary>
        public event EventHandler CanExecuteChanged;

        /// <summary>
        ///     Determines whether the current <see cref="RelayCommand"/> can be executed in its current state.
        /// </summary>
        /// <param name="parameter">
        ///     The <typeparamref name="T"/>.
        /// </param>
        /// <returns>
        ///     <c>true</c> if the current <see cref="RelayCommand"/> can be executed; otherwise, <c>false</c>.
        /// </returns>
        public abstract bool CanExecute(T parameter);

        /// <summary>
        ///     Executes the current <see cref="RelayCommand"/>.
        /// </summary>
        /// <param name="parameter">
        ///     The <typeparamref name="T"/>.
        /// </param>
        public abstract void Execute(T parameter);

        /// <summary>
        ///     Determines whether the current <see cref="RelayCommand"/> can be executed in its current state.
        /// </summary>
        /// <param name="parameter">
        ///     The <typeparamref name="T"/>.
        /// </param>
        /// <returns>
        ///     <c>true</c> if the current <see cref="RelayCommand"/> can be executed; otherwise, <c>false</c>.
        /// </returns>
        bool ICommand.CanExecute(object parameter)
        {
            T typeSafeParameter = GetTypeSafeParameter(parameter);
            return CanExecute(typeSafeParameter);
        }

        /// <summary>
        ///     Executes the current <see cref="RelayCommand"/>.
        /// </summary>
        /// <param name="parameter">
        ///     The <typeparamref name="T"/>.
        /// </param>
        void ICommand.Execute(object parameter)
        {
            T typeSafeParameter = GetTypeSafeParameter(parameter);
            Execute(typeSafeParameter);
        }

        /// <summary>
        ///     Raises the <see cref="CanExecuteChanged"/> event.
        /// </summary>
        public void RaiseCanExecuteChanged() => CanExecuteChanged?.Invoke(this, new EventArgs());

        /// <summary>
        ///     Gets a type safe instance of the <paramref name="parameter"/>.
        /// </summary>
        /// <param name="parameter">
        ///     The parameter.
        /// </param>
        /// <returns>
        ///     The <c>default</c> version of <typeparamref name="T"/> if <paramref name="parameter"/> is null.
        ///     Otherwise, the <paramref name="parameter"/> is converted to a <typeparamref name="T"/>.
        ///     If the <paramref name="parameter"/> cannot be converted, then an <see cref="ArgumentException"/> is thrown.
        /// </returns>
        private T GetTypeSafeParameter(object parameter)
        {
            if (parameter == null) return default;

            if (parameter is T typeSafeParameter) return typeSafeParameter;

            throw new ArgumentException(
                $"The parameter type ({parameter.GetType()}) is not assignable from {typeof(T)}.",
                nameof(parameter));
        }
    }
}
