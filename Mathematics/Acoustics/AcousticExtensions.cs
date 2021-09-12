using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mathematics.Acoustics
{
    public static class AcousticExtensions
    {
        //   static Vector<double> RefValues_1_3 = new double[] { 33, 36, 39, 42, 45, 48, 51, 52, 53, 54, 55, 56, 56, 56, 56, 56 };
        //  static Vector<double> RefValues_1_1 = new double[] { 36, 45, 52, 55, 56 };

        //  Vector<double> C1_50_3150_1_3 = new double[] { -40, -36, -33, -29, -26, -23, -21, -19, -17, -15, -13, -12, -11, -10, -9, -9, -9, -9, -9 };
        //  Vector<double> C1_100_3150_1_3 = new double[] { -29, -26, -23, -21, -19, -17, -15, -13, -12, -11, -10, -9, -9, -9, -9, -9 };

        public static (int Rw, int C1, int C2) Rw_11<T>(this Vector<double> input)
        {
            Vector<double> RefValues_1_1 = new double[] { 36, 45, 52, 55, 56 };
            Vector<double> C1_100_3150_1_1 = new double[] { -21, -14, -8, -5, -4 };
            Vector<double> C2_100_3150_1_1 = new double[] { -14, -10, -7, -4, -6 };
            RefValues_1_1 += 42;
            int i = 0;
            Vector<double> Ref_1_1;
            do
            {
                Ref_1_1 = RefValues_1_1 - i;
                i++;
            } while (input.DifferenceSum(Ref_1_1).Positive >= 10);
            var temp_C1 = 10 ^ ((C1_100_3150_1_1 - input) / 10);
            double C1 = Math.Round(temp_C1.Sum() - 10 * Math.Log10(temp_C1.Sum()), 0);

            var temp_C2 = 10 ^ ((C2_100_3150_1_1 - input) / 10);
            double C2 = Math.Round(temp_C2.Sum() - 10 * Math.Log10(temp_C2.Sum()), 0);
            return ((int)Ref_1_1[2], (int)C1, (int)C2);
        }

        /// <summary>
        /// Frequiency from 100 to 3150 Hz
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="input"></param>
        /// <returns>Rw, C_1 and C_2 </returns>
        public static (int Rw, int C1, int C2) Rw_13<T>(this Vector<double> input)
        {
            Vector<double> RefValues_1_3 = new double[] { 33, 36, 39, 42, 45, 48, 51, 52, 53, 54, 55, 56, 56, 56, 56, 56 };
            Vector<double> C1_100_3150_1_3 = new double[] { -29, -26, -23, -21, -19, -17, -15, -13, -12, -11, -10, -9, -9, -9, -9, -9 };
            Vector<double> C2_100_3150_1_3 = new double[] { -20, -20, -18, -16, -15, -14, -13, -12, -11, -9, -8, -9, -10, -11, -13, -15 };
            RefValues_1_3 += 42;

            int i = 0;
            Vector<double> Ref_1_3;
            do
            {
                Ref_1_3 = RefValues_1_3 - i;
                i++;
            } while (input.DifferenceSum(Ref_1_3).Positive >= 32);

            var temp_C1 = 10 ^ ((C1_100_3150_1_3 - input) / 10);
            double C1 = Math.Round(temp_C1.Sum() - 10 * Math.Log10(temp_C1.Sum()), 0);

            var temp_C2 = 10 ^ ((C2_100_3150_1_3 - input) / 10);
            double C2 = Math.Round(temp_C2.Sum() - 10 * Math.Log10(temp_C2.Sum()), 0);

            return ((int)Ref_1_3[7], (int)C1, (int)C2);
        }

        public static (double Positive, double Negative) DifferenceSum<T>(this Vector<T> vector, Vector<T> Rw)
        {
            if (vector.Length() == Rw.Length())
            {
                Vector<T> vectorRw = Rw - vector;
                double PositiveSum = 0;
                double NegativeSum = 0;
                foreach (T variable in vectorRw.Items)
                {
                    if (double.TryParse(variable.ToString(), out double test))
                    {
                        if (test > 0)
                            PositiveSum += test;
                        else if (test < 0)
                            NegativeSum += test;
                    }
                }
                return (PositiveSum, NegativeSum);
            }
            throw new Exception("Not able to ");
        }
    }
}
