using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mathematics.Acoustics
{
    [Serializable]
    public class OctaveBand
    {

        private int pOctaveBandNumber = 3;
        public int OctaveBandNumber
        {
            get { return pOctaveBandNumber; }
            set { pOctaveBandNumber = value; }
        }

        public double LowFrequencyLimit { get; set; } = 25;
        public double HighFrequencyLimit { get; set; } = 20000;

        public OctaveBand() { }

        public OctaveBand(double f_low, double f_high)
        {
            LowFrequencyLimit = f_low;
            HighFrequencyLimit = f_high;
        }

        public OctaveBand(double f_low, double f_high, int octaveband)
        {
            LowFrequencyLimit = f_low;
            HighFrequencyLimit = f_high;
            OctaveBandNumber = octaveband;
        }

        #region "Octaveband"
        public double[] Preferrd_Oktaveband_Values = { 1, 1.25, 1.6, 2, 2.5, 3.15, 4, 5, 6.3, 8 };

        public double[] Returns_Oktveband_13(double low, double high)
        {

            double lowest = Math.Floor(Math.Log10(low));
            double highest = Math.Floor(Math.Log10(high));

            List<double> result = new List<double>();
            for (int i = (int)lowest; i <= (int)highest; i++)
            {
                double mult = Math.Pow(10, (double)i);
                for (int j = 0; j < Preferrd_Oktaveband_Values.Length; j++)
                {
                    double val = Preferrd_Oktaveband_Values[j] * mult;
                    if ((val >= low) && (val <= high))
                    {
                        result.Add(val);
                    }
                }
            }
            return result.ToArray();
        }

        public double[] Returns_Oktaveband_11(double low, double high)
        {
            List<double> result = new List<double>();
            for (int i = 0; i < 15; i++)
            {
                double val = Preferrd_Oktaveband_Values[3 * i % 10] * Math.Pow(10, Math.Floor(3 * (double)i / 10));
                if ((val >= low) && (val <= high))
                {
                    result.Add(val);
                }
            }

            return result.ToArray();
        }

        public double[] GetOctavebandFrequencies()
        {
            List<double> Result = new List<double>();
            if (OctaveBandNumber == 1)
                return Returns_Oktaveband_11(LowFrequencyLimit, HighFrequencyLimit);
            else if (OctaveBandNumber == 3)
                return Returns_Oktveband_13(LowFrequencyLimit, HighFrequencyLimit);
            else
            {
                bool IsEven = OctaveBandNumber % 2 == 0;
                double fr = 1000;
                int tStart;
                int tEnd;
                if (IsEven)
                {
                    tStart = (int)(Math.Floor(Math.Log10(LowFrequencyLimit / fr)) / (3 / (10 * (double)OctaveBandNumber)));
                    tEnd = (int)(Math.Ceiling(Math.Log10(HighFrequencyLimit / fr)) / (3 / (10 * (double)OctaveBandNumber)));
                }
                else
                {
                    tStart = (((int)(Math.Floor(Math.Log10(LowFrequencyLimit / fr)) / (3 / (10 * (double)OctaveBandNumber)))) - 1) / 2;
                    tEnd = (((int)(Math.Ceiling(Math.Log10(HighFrequencyLimit / fr)) / (3 / (20 * (double)OctaveBandNumber)))) - 1) / 2;
                }
                // Modulus takes a long time to calcualte so just do it once
        

                //for (int i = -(10 * OctaveBandNumber); i < 10 * OctaveBandNumber; i++)
                for (int i = tStart; i <= tEnd; i++)
                {
                    double OctavePower;
                    if (!IsEven)
                    {
                        OctavePower = 3 * (double)i / (10 * (double)OctaveBandNumber);
                    }
                    else
                    {
                        OctavePower = 3 * (2 * (double)i + 1) / (20 * (double)OctaveBandNumber);
                    }
                    double fm = fr * Math.Pow(10, OctavePower);

                    double fH = fm * Math.Pow(10, 3 / (20 * (double)OctaveBandNumber));
                    double fL = fm * Math.Pow(10, -3 / (20 * (double)OctaveBandNumber));

                    if (fm >= LowFrequencyLimit && fm <= HighFrequencyLimit)
                    {
                        Result.Add(fm);
                    }
                }


                return Result.ToArray();
            }
        }

        public double[] Logspace(double start, double end, int count)
        {
            double d = count, p = end / start;
            double[] result = Enumerable.Range(0, count).Select(i => start * Math.Pow(p, i / d)).ToArray();
            result[result.Length - 1] = end;
            return result;
        }
        #endregion
    }
}
