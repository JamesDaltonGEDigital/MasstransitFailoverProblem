namespace MassTransitReceive
{
    public class Counter : ICounter
    {
        int count;
        static readonly SemaphoreSlim semaphore = new SemaphoreSlim(1, 1);

        public int Count
        {
            get
            {
                semaphore.Wait();
                try
                {
                    return count;
                }
                finally
                {
                    semaphore.Release();
                }
            }
        }

        public void Clear()
        {
            semaphore.Wait();
            try
            {
                count = 0;
            }
            finally
            {
                semaphore.Release();
            }
        }

        public void Increment()
        {
            semaphore.Wait();
            try
            {
                count++;
            }
            finally
            {
                semaphore.Release();
            }
        }
    }
}