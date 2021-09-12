using System;

namespace AcousticTransferMatrices.Core.Extensions
{
    public class WeakSubscription<T> : IDisposable, IObserver<T>
    {
        private readonly WeakReference reference;
        private readonly IDisposable subscription;
        private bool disposed;

        public WeakSubscription(IObservable<T> observable, IObserver<T> observer)
        {
            reference = new WeakReference(observer);
            subscription = observable.Subscribe(this);
        }

        void IObserver<T>.OnCompleted()
        {
            IObserver<T> observer = (IObserver<T>)reference.Target;
            if (observer != null)
                observer.OnCompleted();
            else
                Dispose();
        }


        void IObserver<T>.OnError(Exception error)
        {
            var observer = (IObserver<T>)reference.Target;
            if (observer != null)
                observer.OnError(error);
            else
                Dispose();
        }

        void IObserver<T>.OnNext(T value)
        {
            var observer = (IObserver<T>)reference.Target;
            if (observer != null)
                observer.OnNext(value);
            else
                Dispose();
        }

        public void Dispose()
        {
            if (!disposed)
            {
                disposed = true;
                subscription.Dispose();
            }
        }
    }
}
