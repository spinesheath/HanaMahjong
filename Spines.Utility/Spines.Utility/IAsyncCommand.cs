// This file is licensed to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Threading.Tasks;
using System.Windows.Input;

namespace Spines.Utility
{
  /// <summary>
  /// Command with an asynchronous execute method.
  /// </summary>
  /// <typeparam name="TResult">The type of the result of the execution.</typeparam>
  public interface IAsyncCommand<TResult> : ICommand
  {
    /// <summary>
    /// Cancels the operation.
    /// </summary>
    ICommand CancelCommand { get; }

    /// <summary>
    /// Access to the asynchronous execution of the command.
    /// </summary>
    BindableTask<TResult> Execution { get; }

    /// <summary>
    /// Executes the command asynchronously.
    /// </summary>
    /// <param name="parameter">Data used by the command. Can be null.</param>
    /// <returns>The executed operation.</returns>
    Task ExecuteAsync(object parameter);
  }
}