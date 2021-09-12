using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AcousticTransferMatrices.Core.Attribute
{
    /// <summary>
    /// Specifies the order of property.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public sealed class ShowPropertyGrid : System.Attribute
    {
        /// <summary>
        /// Gets or sets the order.
        /// </summary>
        /// <value>The order.</value>
        public bool Show { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyOrderAttribute"/> class.
        /// </summary>
        /// <param name="order">The order.</param>
        public ShowPropertyGrid(bool show = true)
        {
            Show = show;
        }
    }
}
