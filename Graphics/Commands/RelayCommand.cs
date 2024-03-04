using System.Windows.Input;

namespace Charts.Commands
{
    /// <summary>
    /// Represents a command that doesn't require a parameter.
    /// </summary>
    public class RelayCommand : ICommand
    {
        private readonly Action _execute;
        private readonly Func<bool> _canExecute;

        /// <summary>
        /// Initializes a new instance of the <see cref="RelayCommand"/> class.
        /// </summary>
        /// <param name="execute">The action to be executed.</param>
        /// <param name="canExecute">A predicate to determine if the command can be executed.</param>
        public RelayCommand(Action execute, Func<bool> canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        /// <inheritdoc/>
        public bool CanExecute(object parameter)
            => _canExecute?.Invoke() ?? true;

        /// <inheritdoc/>
        public void Execute(object parameter) => _execute();

        /// <inheritdoc/>
        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }
    }
}
