using System.Windows.Input;

namespace System.Chase
{
    public abstract class SimpleCommand : SafeMethodRunner, ICommand
    {
        public abstract bool CanExecute(object parameter);
        public event EventHandler CanExecuteChanged;
        public abstract void Execute(object parameter);
        public void RaiseCanExecuteChanged() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);
    }
}
