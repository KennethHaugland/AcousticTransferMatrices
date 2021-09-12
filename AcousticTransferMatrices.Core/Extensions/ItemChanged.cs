using System.Collections.Specialized;

namespace AcousticTransferMatrices.Core.Extensions
{
    public class ItemChanged<T>
    {
        public T Item { get; set; }
        public bool Added { get; set; }
        public NotifyCollectionChangedEventArgs EventArgs { get; set; }
    }
}
