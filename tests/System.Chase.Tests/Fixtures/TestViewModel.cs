namespace System.Chase.Tests.Fixtures
{
    public class TestViewModel : ObservableObject
    {
        private string _name;
        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }

        public string LastErrorMessage { get; private set; }

        public void SafeActionRunner(Action action)
        {
            RunSafe(action);
        }

        public void UnsafeActionRunner(Action action)
        {
            action();
        }
        
        protected override void OnError(Exception ex)
        {
            LastErrorMessage = ex.Message;
        }
    }
}
