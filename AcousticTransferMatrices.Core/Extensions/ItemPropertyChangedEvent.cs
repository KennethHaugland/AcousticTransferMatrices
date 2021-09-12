using System.Reflection;

namespace AcousticTransferMatrices.Core.Extensions
{
    public class ItemPropertyChangedEvent<TSender>
    {
        public TSender Sender { get; set; }
        public PropertyInfo Property { get; set; }
        public bool HasOld { get; set; }
        public object OldValue { get; set; }
        public object NewValue { get; set; }

        public override string ToString()
        {
            return string.Format("Sender: {0}, Property: {1}, HasOld: {2}, OldValue: {3}, NewValue: {4}", this.Sender, this.Property, this.HasOld, this.OldValue, this.NewValue);
        }
    }
}
