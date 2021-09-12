using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AcousticTransferMatrices.Core.Attribute
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public sealed class TextBoxTypeAttribute : System.Attribute
    {
        /// <summary>
        /// Gets or sets the order.
        /// </summary>
        /// <value>The order.</value>
        public TextBoxType TextBoxType { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyOrderAttribute"/> class.
        /// </summary>
        /// <param name="order">The order.</param>
        public TextBoxTypeAttribute(TextBoxType textBoxType)
        {
            TextBoxType = textBoxType;
        }
    }

    public enum TextBoxType
    {
        ReadOnly,
        Masked
    }
}
