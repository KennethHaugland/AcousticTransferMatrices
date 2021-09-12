using AcousticTransferMatrices.Core.Acoustics.Calcualtions;
using AcousticTransferMatrices.Core.Attribute;
using Mathematics;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace AcousticTransferMatrices.Core.Acoustics
{
    [Serializable]
    public abstract class LayerBaseClass
    {
        public Complex PsiAngle { get; set; }
        public Complex ThetaAngle { get; set; }

        [ShowPropertyGrid(), DisplayName("Name"), Category("1"), PropertyOrder(1)]
        public string Name { get; set; }
        public string Group { get; set; }
        [ShowPropertyGrid(), DisplayName("Thickness"), Category("1"), PropertyOrder(2)]
        public double Thickness { get; set; }
        public bool HasPropagationCoefficient { get; set; } = true;
        public LayerTransition GetOutputLayer(LayerTransition inputLayer)
        {

            if (HasPropagationCoefficient)
            {
                ThetaAngle = inputLayer.GetAngle(G(inputLayer.Freq, inputLayer.ThetaAngle), inputLayer.ThetaAngle);
                inputLayer.ThetaAngle = ThetaAngle;
                inputLayer.CharacteristicImpedance = Z(inputLayer.Freq, ThetaAngle);
                inputLayer.PropagationCoefficient = G(inputLayer.Freq, ThetaAngle);
            }
            else
            {
                ThetaAngle = inputLayer.ThetaAngle;
            }

            inputLayer.Matrix *= TM(inputLayer.Freq, ThetaAngle);
            return inputLayer;
        }

        public abstract Matrix<Complex> TM(double freq, Complex theta, Complex psi = default);

        public abstract Complex Z(double freq, Complex theta, Complex psi = default);

        public abstract Complex G(double freq, Complex theta, Complex psi = default);
    }
}

