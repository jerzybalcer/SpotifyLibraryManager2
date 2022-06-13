﻿using System;
using System.Windows.Input;

namespace SpotifyLibraryManager.Helpers
{
    public class Command : ICommand
    {
        readonly Action<object> _execute;
        readonly Predicate<object> _canExecute;

        public Command(Action<object> execute)
        : this(execute, null)
        {
        }

        public Command(Action<object> execute, Predicate<object> canExecute)
        {
            if (execute == null)
                throw new ArgumentNullException(nameof(execute));

            _execute = execute;
            _canExecute = canExecute;
        }

        public bool CanExecute(object parameter)
        {
            return _canExecute == null ? true : _canExecute(parameter);
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public void Execute(object parameter)
        {
            _execute(parameter);
        }
    }
}
