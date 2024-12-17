using System.Collections.Concurrent;

namespace Content.Universe.Entities
{
    public static class HandleAllocator
    {
        private static ulong nextHandle = 1;
        private static ConcurrentQueue<ulong> handleQueue = new ConcurrentQueue<ulong>();

        public static Handle AllocateHandle()
        {
            if (handleQueue.TryDequeue(out ulong handle))
            {
                return new Handle(handle, FreeHandle);
            }
            return new Handle(nextHandle++, FreeHandle);
        }

        public static void FreeHandle(ulong handle)
        {
            handleQueue.Enqueue(handle);
        }
    }
}
