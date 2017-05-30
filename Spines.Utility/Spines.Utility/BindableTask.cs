// This file is licensed to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using System.Threading.Tasks;

namespace Spines.Utility
{
  /// <summary>
  /// Runs an asynchronous operation and exposes the status through properties.
  /// </summary>
  /// <typeparam name="TResult">The type of the result of the asynchronous operation.</typeparam>
  public class BindableTask<TResult> : INotifyPropertyChanged
  {
    /// <summary>
    /// Creates a new instance of BindableTask and awaits the completion of the task.
    /// </summary>
    /// <param name="task">The asynchronous operation.</param>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1804:RemoveUnusedLocals", MessageId = "ignored")]
    public BindableTask(Task<TResult> task)
    {
      Validate.NotNull(task, nameof(task));
      _task = task;
      if (!task.IsCompleted)
      {
        // Starts the task, but doesn't care about the result.
        // ReSharper disable once UnusedVariable
        var ignored = WatchTaskAsync(task);
      }
    }

    /// <summary>
    /// The result. default(TResult) if the task has not completed successfully yet or was aborted.
    /// </summary>
    public TResult Result => _task.Status == TaskStatus.RanToCompletion ? _task.Result : default(TResult);

    /// <summary>
    /// Whether the task has been completed.
    /// </summary>
    public bool IsCompleted => _task.IsCompleted;

    /// <summary>
    /// Whether the task has successfully completed.
    /// </summary>
    public bool HasSucceeded => _task.Status == TaskStatus.RanToCompletion;

    /// <summary>
    /// Whether the task has been canceled.
    /// </summary>
    public bool IsCanceled => _task.IsCanceled;

    /// <summary>
    /// Whether the task has been aborted with an error.
    /// </summary>
    public bool IsFaulted => _task.IsFaulted;

    /// <summary>
    /// The message of the exception that caused the task to fault.
    /// </summary>
    public string ErrorMessage => _task.Exception?.InnerException?.Message;

    /// <summary>
    /// Id invoked when a public property changes its value.
    /// </summary>
    public event PropertyChangedEventHandler PropertyChanged;

    private readonly Task<TResult> _task;

    private async Task WatchTaskAsync(Task task)
    {
      try
      {
        await task;
      }
      catch
      {
        // Exceptions will be examined through the task instance.
      }
      PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsCompleted)));
      if (task.IsCanceled)
      {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsCanceled)));
      }
      else if (task.IsFaulted)
      {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsFaulted)));
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ErrorMessage)));
      }
      else
      {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(HasSucceeded)));
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Result)));
      }
    }
  }
}