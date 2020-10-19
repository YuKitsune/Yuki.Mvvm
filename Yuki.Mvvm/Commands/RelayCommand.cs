using System;

namespace Yuki.Mvvm.Commands
{
    /// <summary>
    ///     A command whose sole purpose is to relay its functionality to other objects by invoking delegates.
    /// </summary>
    public class RelayCommand : BaseCommand
    {
        /// <summary>
        ///     The <see cref="Action"/> to execute.
        /// </summary>
        private readonly Action _execute;

        /// <summary>
        ///     The <see cref="Func{TResult}"/> to determine whether or not the <see cref="_execute"/> can be executed.
        /// </summary>
        private readonly Func<bool> _canExecute;

        /// <summary>
        ///     Initializes a new instance of the <see cref="RelayCommand"/> class.
        /// </summary>
        /// <param name="execute">
        ///     The <see cref="Action"/> to execute.
        /// </param>
        /// <param name="canExecute">
        ///     The <see cref="Func{TResult}"/> to determine whether or not the <paramref name="execute"/> can be
        ///     executed.
        /// </param>
        public RelayCommand(Action execute, Func<bool> canExecute = null)
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
        public override bool CanExecute() => _canExecute?.Invoke() ?? true;

        /// <summary>
        ///     Executes the current <see cref="RelayCommand"/>.
        /// </summary>
        public override void Execute()
        {
            if (CanExecute()) _execute();
        }
    }
}
