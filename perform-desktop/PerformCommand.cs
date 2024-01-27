using System.ComponentModel;
using System.Windows.Input;

namespace perform_desktop;

public class PerformCommand : ICommand
{
    private readonly MainViewModel _viewModel;

    public PerformCommand(MainViewModel viewModel)
    {
        _viewModel = viewModel;

        _viewModel.PropertyChanged += OnViewModelPropertyChanged;
    }

    private void OnViewModelPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(MainViewModel.SelectedMove))
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    public bool CanExecute(object? parameter)
    {
        return _viewModel.SelectedMove != null && _viewModel.CanPerform(_viewModel.SelectedMove.Key);
    }

    public void Execute(object? parameter)
    {
        if (_viewModel.SelectedMove != null) _viewModel.Perform(_viewModel.SelectedMove.Key);
    }

    public event EventHandler? CanExecuteChanged;
}