using AcousticTransferMatrices.Core.Attribute;
using Mathematics;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Numerics;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace AcousticTransferMatrices.Core.Acoustics.MatrixMaterials
{
    [Serializable]
    [KnownType(typeof(DelanyBazely))]
    public class DelanyBazely : LayerBaseClass
    {
        public DelanyBazely(double Sigma, double thickness)
        {
            sigma = Sigma;
            Thickness = thickness;
            Name = "Delaney-Bazely";
            Group = "Porous";
        }
        public DelanyBazely()
        {
            Name = "Delaney-Bazely";
            Group = "Porous";
        }



        [ShowPropertyGrid(), DisplayName("Speed of sound (m/s)"), Category("1"), PropertyOrder(4)]
        public double C0 { get; set; } = 343;

        [ShowPropertyGrid(), DisplayName("Weight of medium (kg/m^3)"), Category("1"), PropertyOrder(5)]
        public double rho0 { get; set; } = 1.21;

        [ShowPropertyGrid(), DisplayName("rayls - Specific )"), Category("1"), PropertyOrder(3)]
        public double sigma { get; set; }

        public override Complex Z(double Frequency, Complex Theta, Complex psi)
        {
            double E = rho0 * Frequency / sigma;
            return rho0 / Complex.Cos(Theta) * C0 * new Complex(1 + 0.0571 * Math.Pow(E, -0.754), -0.087 * Math.Pow(E, -0.732));
        }
        public override Complex G(double Frequency, Complex Theta, Complex psi)
        {
            double E = rho0 * Frequency / sigma;
            return new Complex(0, 2 * Math.PI * Frequency / C0) * new Complex(1 + 0.0978 * Math.Pow(E, -0.7), -0.189 * Math.Pow(E, -0.595));
        }
        public override Matrix<Complex> TM(double Frequency, Complex Theta, Complex psi)
        {
            return AcosuticMatrixHelpers.PorousMatrix(G(Frequency, Theta, psi), Z(Frequency, Theta, psi), Theta, Thickness);
        }
    }
}
