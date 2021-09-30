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
    [KnownType(typeof(ThinWall))]
    public class ThinWall : LayerBaseClass
    {
        public ThinWall()
        {
            SetUp();
        }

        public ThinWall(double thickness)
        {
            Thickness = thickness;
            SetUp();
        }


        private void SetUp()
        {
            Name = "ThinWall";
            Group = "Solid";
            HasPropagationCoefficient = false;
        }

        [ShowPropertyGrid(), DisplayName("Weight per m2"), Category("1"), PropertyOrder(1)]
        public double mass { get; set; } = 10;

        [ShowPropertyGrid(), DisplayName("Frequency"), Category("1"), PropertyOrder(2)]
        public double fg { get; set; } = 1000;

        [ShowPropertyGrid(), DisplayName("Loss"), Category("1"), PropertyOrder(3)]
        double damping { get; set; } = 0.1;
        public Complex Theta { get; set; }

        public override Complex G(double Frequency, Complex Theta, Complex psi) { return new Complex(0, 0); }
        public override Complex Z(double Frequency, Complex Theta, Complex psi) { return new Complex(0, 1) * 2 * Math.PI * Frequency * mass * (1 - (Complex.Pow((Frequency / fg), 2) * new Complex(1, damping) * Complex.Pow(Complex.Sin(Theta), 4))); }

        public override Matrix<Complex> TM(double Frequency, Complex Theta, Complex psi)
        {
            return AcosuticMatrixHelpers.SeriesMatrix(Z(Frequency, Theta, psi));
        }
    }
}
