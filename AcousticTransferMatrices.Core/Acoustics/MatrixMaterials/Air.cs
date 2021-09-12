using Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using AcousticTransferMatrices.Core;
using AcousticTransferMatrices.Core.Attribute;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace AcousticTransferMatrices.Core.Acoustics.MatrixMaterials
{
    [Serializable]
    [KnownType(typeof(Air))]
    public class Air : LayerBaseClass
    {
        public Air()
        {
            Name = "Air";
            Group = "Gas";
        }

        public Air(double thickness)
        {
            Thickness = thickness;
            Name = "Air";
            Group = "Gas";
        }

        [ShowPropertyGrid(), DisplayName("Speed of sound (m/s)"), Category("1"), PropertyOrder(1)]
        public double C0 { get; set; } = 343;

        [ShowPropertyGrid(), DisplayName("Weight of medium (kg/m^3)"), Category("1"), PropertyOrder(2)]
        public double rho0 { get; set; } = 1.21;


        double damping { get; set; } = 0;
        public Complex Theta { get; set; }

        public override Complex G(double Frequency, Complex Theta, Complex psi) { return new Complex(0, 2 * Frequency * Math.PI / C0) * Complex.Sqrt(new Complex(1, -damping / (rho0 * 2 * Math.PI * Frequency))); }
        public override Complex Z(double Frequency, Complex Theta, Complex psi) { return rho0 / Complex.Cos(Theta)* C0 * Complex.Sqrt(new Complex(1, -damping / (rho0 * 2 * Math.PI * Frequency))); }

        public override Matrix<Complex> TM(double Frequency, Complex Theta, Complex psi)
        {
            return AcosuticMatrixHelpers.PorousMatrix(G(Frequency, Theta, psi), Z(Frequency, Theta, psi), Theta, Thickness);
        }
    }
}
