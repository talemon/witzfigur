using System.Windows.Input;

namespace perform_desktop;

public class ClearLogCommand(MainViewModel model) : ICommand
{
    private MainViewModel _model = model;

    public bool CanExecute(object? parameter)
    {
        return true;
    }

    public void Execute(object? parameter)
    {
        _model.ClearLog();
    }

    public event EventHandler? CanExecuteChanged;
}