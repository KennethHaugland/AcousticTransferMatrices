using Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace AcousticTransferMatrices.Core.Acoustics.Calcualtions
{
    public class LayerTransition
    {
        public double Freq { get; set; }

        public double InputThetaAngle { get; set; }

        public int AngleIndex { get; set; }
        public Complex ThetaAngle { get; set; }
        public Complex PsiAngle { get; set; }
        public Complex PropagationCoefficient { get; set; }
        public Complex CharacteristicImpedance { get; set; }

        [XmlIgnore]
        public Matrix<Complex> Matrix { get; set; }
        public Complex GetAngle(Complex G2, Complex InnboundAngle)
        {
            Complex G1 = PropagationCoefficient;
           // return Complex.Acos(Complex.Sqrt(1 - Complex.Pow(G1 / G2 * Complex.Sin(InnboundAngle), 2)));
            return Complex.Asin(G1 / G2 * Complex.Sin(InnboundAngle));
        }


        public Matrix<Complex> GetMatrix(Matrix<Complex> CurrentMatrix)
        {
            return Matrix * CurrentMatrix;
        }
    }
}
