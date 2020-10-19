using System;

namespace Yuki.Mvvm.Commands
{
    /// <summary>
    ///     A command whose sole purpose is to relay its functionality to other objects by invoking delegates.
    /// </summary>
    /// <typeparam name="T">
    ///     The type of parameter.
    /// </typeparam>
    public class RelayCommand<T> : BaseCommand<T>
    {
        /// <summary>
        ///     The <see cref="Action{T}"/> to execute.
        /// </summary>
        private readonly Action<T> _execute;

        /// <summary>
        ///     The <see cref="Func{TParam, TResult}"/> to determine whether or not the <see cref="_execute"/> can be
        ///     executed.
        /// </summary>
        private readonly Func<T, bool> _canExecute;

        /// <summary>
        ///     Initializes a new instance of the <see cref="RelayCommand"/> class.
        /// </summary>
        /// <param name="execute">
        ///     The <see cref="Action{T}"/> to execute.
        /// </param>
        /// <param name="canExecute">
        ///     The <see cref="Func{TParam, TResult}"/> to determine whether or not the <paramref name="execute"/> can
        ///     be executed.
        /// </param>
        public RelayCommand(Action<T> execute, Func<T, bool> canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute), "The action to execute cannot be null.");
            _canExecute = canExecute;
        }

        /// <summary>
        ///     Determines whether the current <see cref="RelayCommand"/> can be executed in its current state.
        /// </summary>
        /// <returns>
        ///     <c>true</c> if the current <see cref="RelayCommand"/> can be executed; otherwise, <c>false</c>.
        /// </returns>
        public override bool CanExecute(T parameter) => _canExecute?.Invoke(parameter) ?? true;

        /// <summary>
        ///     Executes the current <see cref="RelayCommand"/>.
        /// </summary>
        public override void Execute(T parameter)
        {
            if (CanExecute(parameter)) _execute(parameter);
        }
    }
}
