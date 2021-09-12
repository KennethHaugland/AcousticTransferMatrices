using System;
using System.Collections;

namespace AcousticTransferMatrices.CalculationLayer.Integration
{
    [Serializable]
    public class QuaderatureItem : IComparer, IComparable
    {
        public QuaderatureItem()
        {

        }
        public double Weight { get; set; }
        public double Abscissa { get; set; }

        public int Compare(object x, object y)
        {
            QuaderatureItem a = (QuaderatureItem)x;
            QuaderatureItem b = (QuaderatureItem)y;
            return a.Abscissa.CompareTo(b.Abscissa);
        }

        public int CompareTo(object obj)
        {
            QuaderatureItem b = (QuaderatureItem)obj;
            return Abscissa.CompareTo(b.Abscissa);
        }
    }
}
