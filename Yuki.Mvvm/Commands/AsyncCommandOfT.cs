using System;
using System.Threading.Tasks;
using Yuki.Mvvm.Extensions;

namespace Yuki.Mvvm.Commands
{
    /// <summary>
    ///     A command whose sole purpose is to relay its functionality to other objects by invoking delegates
    ///     asynchronously.
    /// </summary>
    /// <typeparam name="T">
    ///     The type of parameter.
    /// </typeparam>
    public class AsyncCommand<T> : BaseCommand<T>
    {
        /// <summary>
        ///     The <see cref="Func{TParam, TResult}"/> to execute.
        /// </summary>
        private readonly Func<T, Task> _execute;

        /// <summary>
        ///     The <see cref="Func{TParam, TResult}"/> to determine whether or not the <see cref="_execute"/> can be executed.
        /// </summary>
        private readonly Func<T, bool> _canExecute;

        /// <summary>
        ///     The <see cref="Action{T}"/> to handle <see cref="Exception"/>s.
        /// </summary>
        private readonly Action<Exception> _onException;

        /// <summary>
        ///     A value indicating whether or not the current <see cref="AsyncCommand"/> is executing.
        /// </summary>
        private bool _isExecuting;

        /// <summary>
        ///     Gets a value indicating whether or not the current <see cref="AsyncCommand"/> is executing.
        /// </summary>
        public bool IsExecuting
        {
            get => _isExecuting;
            private set
            {
                _isExecuting = value;
                RaiseCanExecuteChanged();
            }
        }

        /// <summary>
        ///     Gets a value indicating whether or not the current <see cref="AsyncCommand"/> can be executed whilst it
        ///     is already running.
        /// </summary>
        public bool CanExecuteConcurrently { get; }

        /// <summary>
        ///     Initializes a new instance of the <see cref="AsyncCommand{T}"/> class.
        /// </summary>
        /// <param name="execute">
        ///     The <see cref="Func{TParam, TResult}"/> to execute.
        /// </param>
        /// <param name="canExecute">
        ///     The <see cref="Func{TParam, TResult}"/> to determine whether or not the <paramref name="execute"/> can be
        ///     executed.
        /// </param>
        /// <param name="canExecuteConcurrently">
        ///     If set to <c>true</c>, then the <paramref name="execute"/> can still be executed whilst it is already
        ///     executing. Otherwise, the <paramref name="execute"/> can not be executed whilst it is already running.
        /// </param>
        /// <param name="onException">
        ///     The <see cref="Action{T}"/> to handle <see cref="Exception"/>s.
        /// </param>
        public AsyncCommand(
            Func<T, Task> execute,
            Func<T, bool> canExecute = null,
            bool canExecuteConcurrently = false,
            Action<Exception> onException = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute), "The function to execute cannot be null.");
            _canExecute = canExecute;
            IsExecuting = false;
            CanExecuteConcurrently = canExecuteConcurrently;
            _onException = onException;
        }

        /// <summary>
        ///     Determines whether the current <see cref="AsyncCommand"/> can be executed in its current state.
        /// </summary>
        /// <param name="parameter">
        ///     The <typeparamref name="T"/>.
        /// </param>
        /// <returns>
        ///     <c>true</c> if the current <see cref="AsyncCommand"/> can be executed; otherwise, <c>false</c>.
        /// </returns>
        public override bool CanExecute(T parameter)
        {
            // Can't run if we're already running and not allowed to run concurrently
            if (!CanExecuteConcurrently && IsExecuting) return false;

            return _canExecute?.Invoke(parameter) ?? true;
        }

        /// <summary>
        ///     Executes the current <see cref="AsyncCommand"/> as an asynchronous operation.
        /// </summary>
        /// <param name="parameter">
        ///     The <typeparamref name="T"/>.
        /// </param>
        /// <returns>
        ///     The <see cref="Task"/> representing the asynchronous operation.
        /// </returns>
        public async Task ExecuteAsync(T parameter)
        {
            if (CanExecute(parameter))
            {
                try
                {
                    IsExecuting = true;
                    await _execute(parameter);
                }
                finally
                {
                    IsExecuting = false;
                }
            }
        }

        /// <summary>
        ///     Executes the current <see cref="AsyncCommand"/>.
        /// </summary>
        /// <param name="parameter">
        ///     The <typeparamref name="T"/>.
        /// </param>
        public override void Execute(T parameter) => ExecuteAsync(parameter).FireAndForgetSafeAsync(_onException);
    }
}
