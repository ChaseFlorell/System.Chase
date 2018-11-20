using System.Threading.Tasks;

namespace System.Chase.Internal
{
    internal sealed class BusyHelper : IDisposable
    {
        private readonly int _delayInMs;
        private readonly ObservableObject _viewModel;
        private bool _delayed;
        private Guid _tracker;

        internal BusyHelper(ObservableObject viewModel, int delayInMs)
        {
            _viewModel = viewModel;
            if (delayInMs <= 0)
            {
                StartBusy();
            }
            else
            {
                _delayed = true;
                _delayInMs = delayInMs;
                Task.Delay(delayInMs).ContinueWith(_ =>
                {
                    StartBusy();
                    _delayed = false;
                }, TaskContinuationOptions.ExecuteSynchronously);
            }

            void StartBusy()
            {
                _tracker = new Guid();
                _viewModel.BusyLocks.Add(_tracker);
                _viewModel.RaisePropertyChanged(nameof(ObservableObject.IsBusy));
                _viewModel.RaisePropertyChanged(nameof(ObservableObject.IsNotBusy));
            }
        }

        public async void Dispose()
        {
            while (_delayed) await Task.Delay(_delayInMs).ConfigureAwait(false);
            _viewModel.BusyLocks.Remove(_tracker);
            _viewModel.RaisePropertyChanged(nameof(ObservableObject.IsBusy));
            _viewModel.RaisePropertyChanged(nameof(ObservableObject.IsNotBusy));
        }
    }
}
