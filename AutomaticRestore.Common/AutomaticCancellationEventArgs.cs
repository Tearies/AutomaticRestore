namespace AutomaticRestore.Common
{
    public class AutomaticCancellationEventArgs
    {
        public CancellationResons CancellationReson { get; private set; }

        public AutomaticCancellationEventArgs(CancellationResons cancellationReson)
        {
            CancellationReson = cancellationReson;
        }
    }
}