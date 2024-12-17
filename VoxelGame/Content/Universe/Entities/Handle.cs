namespace Content.Universe.Entities
{
    public class Handle : IDisposable
    {
        public ulong Value { get; private set; }
        private Action<ulong> returnHandler;
        public Handle(ulong value, Action<ulong> returnHandler)
        {
            Value = value;
            this.returnHandler = returnHandler;
        }
        public void Dispose()
        {
            returnHandler(Value);
        }
    }
}
