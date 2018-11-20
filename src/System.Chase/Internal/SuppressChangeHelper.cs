namespace System.Chase.Internal
{
    internal sealed class SuppressChangeHelper : IDisposable
    {
        private readonly ObservableObject _viewModel;
        private static readonly object _lock = new object();
            
        internal SuppressChangeHelper(ObservableObject viewModel)
        {
            _viewModel = viewModel;
            _viewModel.ChangeNotificationsSuppressed = true;
        }

        public void Dispose()
        {
            lock (_lock)
            {
                _viewModel.ChangeNotificationsSuppressed = false;
                
                foreach (var property in _viewModel.SuppressedChangedProperties)
                    _viewModel.RaisePropertyChanged(property);
                
                _viewModel.SuppressedChangedProperties.Clear();
            }
        }
    }
}
