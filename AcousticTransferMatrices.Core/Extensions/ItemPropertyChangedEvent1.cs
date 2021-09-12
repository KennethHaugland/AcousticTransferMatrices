using System.Reflection;

namespace AcousticTransferMatrices.Core.Extensions
{
    public class ItemPropertyChangedEvent<TSender, TProperty>
    {
        public TSender Sender { get; set; }
        public PropertyInfo Property { get; set; }
        public bool HasOld { get; set; }
        public TProperty OldValue { get; set; }
        public TProperty NewValue { get; set; }
    }
}
