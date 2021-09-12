using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AcousticTransferMatrices.Core.Acoustics
{
    [Serializable]
    public class FreqValues : IComparable
    {
        public FreqValues()
        {

        }

        public FreqValues(double f, double v)
        {
            Freq = f;
            Value.Add(v);
        }

        public FreqValues(double f, List<double> v)
        {
            Freq = f;
            Value = v;
        }
        public double Freq { get; set; }

        public double Angle { get; set; }
        public List<double> Value { get; set; } = new List<double>();

        public int CompareTo(object obj)
        {
            FreqValues test = (FreqValues)obj;
            if (test.Freq > this.Freq)
                return -1;
            else if (test.Freq < this.Freq)
                return 1;
            else
                return 0;
        }
    }
}
