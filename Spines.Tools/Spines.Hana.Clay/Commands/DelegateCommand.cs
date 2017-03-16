// This file is licensed to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows.Input;

namespace Spines.MachiKae.Commands
{
  /// <summary>
  /// Wraps an action with the ICommand interface.
  /// </summary>
  internal class DelegateCommand : ICommand
  {
    /// <summary>
    /// Creates a new Instance of DelegateCommand.
    /// </summary>
    /// <param name="execute">The action that will be executed by the command.</param>
    public DelegateCommand(Action<object> execute)
      : this(execute, null)
    {
    }

    /// <summary>
    /// Creates a new Instance of DelegateCommand.
    /// </summary>
    /// <param name="execute">The action that will be executed by the command.</param>
    /// <param name="canExecute">Called to determine whether the command can be executed.</param>
    public DelegateCommand(Action<object> execute, Predicate<object> canExecute)
    {
      _execute = execute;
      _canExecute = canExecute;
    }

    /// <summary>
    /// Notifies about changes in the executability of the command.
    /// </summary>
    public event EventHandler CanExecuteChanged;

    /// <summary>
    /// Can the command be executed with the given parameter?
    /// </summary>
    /// <param name="parameter">The parameter that would be passed to the execute method.</param>
    /// <returns>Whether the command can be executed.</returns>
    public bool CanExecute(object parameter)
    {
      return _canExecute == null || _canExecute(parameter);
    }

    /// <summary>
    /// Executes the action.
    /// </summary>
    /// <param name="parameter">The parameter that is passed to the action.</param>
    public void Execute(object parameter)
    {
      _execute(parameter);
    }

    /// <summary>
    /// Notifies about changes in the executability of the command.
    /// </summary>
    public void RaiseCanExecuteChanged()
    {
      CanExecuteChanged?.Invoke(this, EventArgs.Empty);
    }

    private readonly Predicate<object> _canExecute;
    private readonly Action<object> _execute;
  }
}