// This file is licensed to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Spines.Utility
{
  /// <summary>
  /// Asynchronous command that supports cancellation.
  /// </summary>
  /// <typeparam name="TResult"></typeparam>
  public class AsyncCommand<TResult> : IAsyncCommand<TResult>, INotifyPropertyChanged
  {
    /// <summary>
    /// Creates a new instance of AsyncCommand.
    /// </summary>
    /// <param name="execute">The operation to execute.</param>
    /// <param name="canExecute">Used to check whether the command can be executed.</param>
    public AsyncCommand(Func<object, CancellationToken, Task<TResult>> execute, Func<object, bool> canExecute)
    {
      _execute = execute;
      _canExecute = canExecute;
      _cancelCommand = new CancelAsyncCommand();
    }

    /// <summary>
    /// Creates a new instance of AsyncCommand.
    /// </summary>
    /// <param name="execute">The operation to execute.</param>
    public AsyncCommand(Func<object, CancellationToken, Task<TResult>> execute)
    {
      _execute = execute;
      _cancelCommand = new CancelAsyncCommand();
    }

    /// <inheritdoc />
    public ICommand CancelCommand => _cancelCommand;

    /// <inheritdoc />
    public BindableTask<TResult> Execution
    {
      get { return _execution; }
      set
      {
        if (value == _execution)
        {
          return;
        }
        _execution = value;
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Execution)));
      }
    }

    /// <summary>
    /// Is invoked when a property changes.
    /// </summary>
    public event PropertyChangedEventHandler PropertyChanged;

    /// <inheritdoc />
    public event EventHandler CanExecuteChanged;

    /// <inheritdoc />
    public bool CanExecute(object parameter)
    {
      return _canExecute?.Invoke(parameter) ?? true;
    }

    /// <inheritdoc />
    public async void Execute(object parameter)
    {
      await ExecuteAsync(parameter);
    }

    /// <inheritdoc />
    public async Task ExecuteAsync(object parameter)
    {
      if (!CanExecute(parameter))
      {
        return;
      }
      _cancelCommand.NotifyCommandStarting();
      Execution = new BindableTask<TResult>(_execute(parameter, _cancelCommand.Token));
      RaiseCanExecuteChanged();
      await Execution.Completion;
      _cancelCommand.NotifyCommandFinished();
      RaiseCanExecuteChanged();
    }

    /// <summary>
    /// Invokes the CanExecuteChanged event.
    /// </summary>
    protected void RaiseCanExecuteChanged()
    {
      CanExecuteChanged?.Invoke(this, EventArgs.Empty);
    }

    private readonly Func<object, CancellationToken, Task<TResult>> _execute;
    private readonly Func<object, bool> _canExecute;
    private readonly CancelAsyncCommand _cancelCommand;
    private BindableTask<TResult> _execution;

    private sealed class CancelAsyncCommand : ICommand
    {
      public CancellationToken Token => _cts.Token;

      public event EventHandler CanExecuteChanged;

      public void NotifyCommandStarting()
      {
        _commandExecuting = true;
        if (!_cts.IsCancellationRequested)
        {
          return;
        }
        _cts = new CancellationTokenSource();
        RaiseCanExecuteChanged();
      }

      public void NotifyCommandFinished()
      {
        _commandExecuting = false;
        RaiseCanExecuteChanged();
      }

      private CancellationTokenSource _cts = new CancellationTokenSource();
      private bool _commandExecuting;

      private void RaiseCanExecuteChanged()
      {
        CanExecuteChanged?.Invoke(this, EventArgs.Empty);
      }

      bool ICommand.CanExecute(object parameter)
      {
        return _commandExecuting && !_cts.IsCancellationRequested;
      }

      void ICommand.Execute(object parameter)
      {
        _cts.Cancel();
        RaiseCanExecuteChanged();
      }
    }
  }
}