using System.Diagnostics;
using System.Windows.Input;

namespace perform_desktop;

public class RelayCommand(Action<object?> execute, Predicate<object?>? canExecute) : ICommand
{
    public RelayCommand(Action<object?> execute) : this(execute, null) { }

    #region ICommand Members 
    [DebuggerStepThrough]
    public bool CanExecute(object? parameter)
    {
        return canExecute?.Invoke(parameter) ?? true;
    }
    public event EventHandler? CanExecuteChanged
    {
        add => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }
    public void Execute(object? parameter) { execute(parameter); }
    #endregion // ICommand Members 
}