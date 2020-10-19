using System;
using System.Threading.Tasks;

namespace Yuki.Mvvm.Extensions
{
    /// <summary>
    ///     The <see cref="Task"/> extension methods.
    /// </summary>
    public static class TaskExtensions
    {
        /// <summary>
        ///     Fires-and-forgets the given <see cref="Task"/> and handles any exceptions using the <paramref name="onError"/>
        ///     <see cref="Action{TError}"/>.
        /// </summary>
        /// <param name="task">
        ///     The <see cref="Task"/> to invoke.
        /// </param>
        /// <param name="onError">
        ///     The <see cref="Action{TError}"/> to handle any exceptions.
        /// </param>
        public static async void FireAndForgetSafeAsync(this Task task, Action<Exception> onError)
        {
            try
            {
                await task.ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                onError(ex);
            }
        }
    }
}