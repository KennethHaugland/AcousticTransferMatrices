using Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace AcousticTransferMatrices.Core.Acoustics
{

    public static class AcosuticMatrixHelpers
    {
        public static Matrix<Complex> PorousMatrix(Complex G, Complex Z, Complex Theta, double d)
        {
            Matrix<Complex> res = new Matrix<Complex>(2, 2);
            res[0, 0] = Complex.Cosh(G * d * Complex.Cos(Theta));
            res[0, 1] = Z * Complex.Sinh(G * d * Complex.Cos(Theta)) / Complex.Cos(Theta);
            res[1, 0] = Complex.Sinh(G * d * Complex.Cos(Theta)) * Complex.Cos(Theta) / Z;
            res[1, 1] = Complex.Cosh(G * d * Complex.Cos(Theta));
            return res;
        }
        public static Matrix<Complex> ShuntMatrix(Complex Z)
        {
            Matrix<Complex> res = new Matrix<Complex>(2, 2);
            res[0, 0] = 1;
            res[0, 1] = 0;
            res[1, 0] = 1 / Z;
            res[1, 1] = 1;
            return res;
        }
        public static Matrix<Complex> SeriesMatrix(Complex Z)
        {
            Matrix<Complex> res = new Matrix<Complex>(2, 2);
            res[0, 0] = 1;
            res[0, 1] = Z;
            res[1, 0] = 0;
            res[1, 1] = 1;
            return res;
        }

        public static Matrix<Complex> VelocityMatrix(Complex a)
        {
            Matrix<Complex> res = new Matrix<Complex>(2, 2);
            res[0, 0] = 1;
            res[0, 1] = 0;
            res[1, 0] = 0;
            res[1, 1] = a;
            return res;
        }

        public static Matrix<Complex> PressureMatrix(Complex a)
        {
            Matrix<Complex> res = new Matrix<Complex>(2, 2);
            res[0, 0] = a;
            res[0, 1] = 0;
            res[1, 0] = 0;
            res[1, 1] = 1;
            return res;
        }
    }
}
