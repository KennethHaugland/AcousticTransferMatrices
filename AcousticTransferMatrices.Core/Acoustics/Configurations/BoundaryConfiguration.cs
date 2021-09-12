using AcousticTransferMatrices.Core.Acoustics.Calcualtions;
using AcousticTransferMatrices.Core.Acoustics.MatrixMaterials;
using AcousticTransferMatrices.Core.Extensions;
using Mathematics;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Numerics;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace AcousticTransferMatrices.Core.Acoustics.Configurations
{
    [Serializable]
    [KnownType(typeof(Air))]
    [KnownType(typeof(DelanyBazely))]
    public class BoundaryConfiguration : IBoundaryConfiguration
    {
        public BoundaryConfiguration()
        {
            HardWall = new Matrix<Complex>(2, 1);
            HardWall[0, 0] = 1;
            HardWall[1, 0] = 0;

            Anecohic = new Matrix<Complex>(2, 1);
            Anecohic[0, 0] = 0;
            Anecohic[1, 0] = 1;
        }

        public LayerTransition FrontMaterial(double freq, Complex Theta, int index)
        {
            LayerTransition res = new LayerTransition();
            this.Theta_in = Theta;
            res.InputThetaAngle = Theta.Real;
            res.AngleIndex = index;
            res.Freq = freq;
            res.Matrix = new Matrix<Complex>(2, 2);
            res.Matrix[0, 0] = 1;
            res.Matrix[1, 1] = 1;
            res.PropagationCoefficient = G_in(freq, Theta);
            res.ThetaAngle = Theta;
            res.PsiAngle = Psi_in;
            res.CharacteristicImpedance = Z_in(freq, Theta);
            return res;
        }

        public LayerBaseClass FrontLayer { get; set; }
        public LayerBaseClass BackLayer { get; set; }


        private Complex Z_in(double freq, Complex Theta)
        {
            return FrontLayer.Z(freq, Theta);
        }

        private Complex G_in(double freq, Complex Theta)
        {
            return FrontLayer.G(freq, Theta);
        }

        public Complex Theta_in { get; set; }
        private Complex Psi_in { get; set; }
        private Complex Z_out(double freq, Complex Theta)
        {
            return BackLayer.Z(freq, Theta_Out);
        }

        private Complex G_out(double freq, Complex Theta)
        {
            return BackLayer.G(freq, Theta_Out);
        }

        private Complex Theta_Out { get; set; }


        public TransmissionTypes Transmission { get; set; }

        public Complex GetResult(LayerTransition LC)
        {
            Theta_Out = LC.GetAngle(G_out(LC.Freq, LC.ThetaAngle), LC.ThetaAngle);
            return MatrixResult(LC.Matrix, LC.Freq);
        }

        private Complex MatrixResult(Matrix<Complex> T, double freq)
        {
            Complex result = new Complex();

            if (!((T.Columns == 2) && (T.Rows == 2)))
                throw new Exception("Matrix must be size 2x2.");

            if (Transmission == TransmissionTypes.TransmissionLossMatrix)
            {
                result = Z_in(freq, Theta_in)  /Z_out(freq, Theta_Out) * Complex.Pow(Complex.Abs(T[0, 0] + Complex.Cos(Theta_in) / Z_in(freq, Theta_in) * T[0,1] + Z_out(freq, Theta_Out)  * T[1,0] + Z_out(freq, Theta_Out)  / Z_in(freq, Theta_in)  * T[1,1]) / 2, 2);
            }
            else if (Transmission == TransmissionTypes.IntersectionLossMatrix)
            {
                result = Complex.Pow(Z_in(freq, Theta_in) * Complex.Cos(Theta_Out) / (Z_out(freq, Theta_Out) * Complex.Cos(Theta_in)) * (Complex.Abs(T[0, 0] + Complex.Cos(Theta_in) / Z_in(freq, Theta_in) * T[0,1] + Z_out(freq, Theta_Out) / Complex.Cos(Theta_Out) * T[1,0] + Z_out(freq, Theta_Out) * Complex.Cos(Theta_in) / (Z_in(freq, Theta_in) * Complex.Cos(Theta_Out)) * T[1,1]) / 2), 2);
            }
            else if (Transmission == TransmissionTypes.TransmissionLossQuaterWaveMatrix)
            {
                Matrix<Complex> res = T * Custom(freq);
                result = 1 / res[0, 0];
            }
            else if (Transmission == TransmissionTypes.AbsorptionHardWall)
            {
                Matrix<Complex> res = T * HardWall;
                Complex Z = res[0, 0] / res[1, 0];
                result = 1 - Math.Pow(Complex.Abs((Z - Z_in(freq, Theta_in)) / (Z  + Z_in(freq, Theta_in))), 2);
            }
            else if (Transmission == TransmissionTypes.AbsorptionAnechoic)
            {
                Matrix<Complex> res = T * Anecohic;
                Complex Z = res[0,0] / res[1,0];

                result = 1 - Math.Pow(Complex.Abs((Z / Z_in(freq, Theta_in) - 1) / (Z  / Z_in(freq, Theta_in) + 1)), 2);
            }
            else if (Transmission == TransmissionTypes.AbsorbtionCustom)
            {
                Matrix<Complex> res = T * Custom(freq);
                Complex Z = res[0,0] / res[1,0];

                result = 1 - Math.Pow(Complex.Abs((Z  / Z_in(freq, Theta_in) - 1) / (Z / Z_in(freq, Theta_in) + 1)), 2);
            }

            return result;
        }

        private Matrix<Complex> HardWall { get; set; }

        private Matrix<Complex> Anecohic { get; set; }

        private Matrix<Complex> Custom(double freq)
        {
            Matrix<Complex> custom = new Matrix<Complex>(2, 1);
            custom[0, 0] = Z_out(freq, Theta_Out) / Complex.Cos(Theta_Out);
            custom[1, 0] = 1;
            return custom;
        }

    }
}
