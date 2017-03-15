﻿// This file is licensed to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows.Input;

namespace Spines.MachiKae.Commands
{
  internal class DelegateCommand : ICommand
  {
    public DelegateCommand(Action<object> execute)
      : this(execute, null)
    {
    }

    public DelegateCommand(Action<object> execute, Predicate<object> canExecute)
    {
      _execute = execute;
      _canExecute = canExecute;
    }

    public event EventHandler CanExecuteChanged;

    public bool CanExecute(object parameter)
    {
      return _canExecute == null || _canExecute(parameter);
    }

    public void Execute(object parameter)
    {
      _execute(parameter);
    }

    public void RaiseCanExecuteChanged()
    {
      CanExecuteChanged?.Invoke(this, EventArgs.Empty);
    }

    private readonly Predicate<object> _canExecute;
    private readonly Action<object> _execute;
  }
}