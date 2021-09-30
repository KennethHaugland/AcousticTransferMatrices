using AcousticTransferMatrices.Core.Serialization;
using Mathematics.Integration;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace AcousticTransferMatrices.Core.Acoustics.Configurations
{
    [Serializable]
    public class IntegrationConfiguration : ICloneable, IIntegrationConfiguration
    {
        public IntegrationConfiguration()
        {

        }

        public void SetIntegrationLimits(double high, double low = 0, bool inDegrees = true)
        {
            if (inDegrees)
            {
                Angles.Add(low * Math.PI / 180);
                Angles.Add(high * Math.PI / 180);
            }
            else
            {
                Angles.Add(low);
                Angles.Add(high);
            }
        }

        public IntegrationType Integration { get; set; } = IntegrationType.GaussLagrangeQuadrature;
        public int IntegrationPointResolution { get; set; } = 30;

        public ObservableCollection<double> Angles { get;  set; } = new ObservableCollection<double>();

        public List<Complex> AngleResult { get; set; } = new List<Complex>();
        public List<int> AngleIndex { get; set; } = new List<int>();
        public List<double> InputAngles { get; set; } = new List<double>();
        public void AnglesInDegrees(IEnumerable<double> angles)
        {
            foreach (double item in angles)
            {
                Angles.Add(item / 180.0 * Math.PI);
            }
        }

        private double dx = 0;

        public List<double> IntegrationResult(bool Absorbtion)
        {
            List<double> res = new List<double>();
            if (Absorbtion)
            {
                if (Integration == IntegrationType.SingleAngles)
                {
                    foreach (var item in AngleResult)
                    {
                        res.Add(Complex.Abs(item));
                    }
                }
                else if (Integration == IntegrationType.GaussLagrangeQuadrature)
                {
                    double sum = 0;
                    for (int i = 0; i < AngleResult.Count; i++)
                    {
                        sum += Math.Sin(2 * Angles[i]) * Complex.Abs(AngleResult[i]) * QuaderatureItemsAndWeights[i].Weight;
                    }
                    var a = 2 * Angles.First();
                    var b = 2 * Angles.Last();
                    var a1 = 0.5 *(Math.Cos(a) - Math.Cos(b));
                    var b1 = -2  * Math.Sin(a - b) * Math.Sin(a + b);
                    res.Add(sum / (0.5 * (Math.Cos(2 * Angles.First()) - Math.Cos(2 * Angles.Last()))));
                }
                else if (Integration == IntegrationType.SimpleSum)
                {
                    double sum = 0;
                    for (int i = 0; i < AngleResult.Count; i++)
                    {
                        sum += Math.Sin(2 * Angles[i]) * Complex.Abs(AngleResult[i]);
                    }

                    var a = 2 * Angles.First();
                    var b = 2 * Angles.Last();
                    var a1 = 0.5 * (Math.Cos(a) - Math.Cos(b));
                    var b1 = -2 * Math.Sin(a - b) * Math.Sin(a + b);

                    res.Add(sum * dx / (0.5 * (Math.Cos(2 * Angles.First()) - Math.Cos(2 * Angles.Last()))));
                }
            }
            else
            {
                if (Integration == IntegrationType.SingleAngles)
                {
                    for (int i = 0; i < AngleResult.Count; i++)
                    {
                        res.Add(10 * Math.Log10(AngleResult[i].Real));
                    }
                }
                else if (Integration == IntegrationType.GaussLagrangeQuadrature)
                {
                    double sum = 0;
                    for (int i = 0; i < AngleResult.Count; i++)
                    {
                        sum += Math.Sin(2 * Angles[i]) / Complex.Abs(AngleResult[i]) * QuaderatureItemsAndWeights[i].Weight;
                    }
                    res.Add(-10 * Math.Log10(sum  / (0.5 * (Math.Cos(2 * Angles.First()) - Math.Cos(2 * Angles.Last())))));
                }
                else if (Integration == IntegrationType.SimpleSum)
                {
                    double sum = 0;
                    for (int i = 0; i < AngleResult.Count; i++)
                    {
                        sum += Math.Sin(2 * Angles[i]) *dx / Complex.Abs(AngleResult[i]);
                    }
                    res.Add(-10 * Math.Log10(sum * dx  / (0.5 * (Math.Cos(2 * Angles.First()) - Math.Cos(2 * Angles.Last())))));
                }
            }
            return res;

        }

        public enum IntegrationType
        {
            SingleAngles = 0,
            SimpleSum = 1,
            GaussLagrangeQuadrature = 2,
            GaussChebychev = 3
        }
        List<QuaderatureItem> QuaderatureItemsAndWeights;

        public void ConfigureSetup()
        {
            if (Integration == IntegrationType.SingleAngles)
            {

            }
            else if (Integration == IntegrationType.SimpleSum)
            {
                dx = (Angles.Last() - Angles.First()) / IntegrationPointResolution;
                ObservableCollection<double> pAngles = new ObservableCollection<double> { Angles.First() };
                for (int i = 1; i < IntegrationPointResolution; i++)
                {
                    pAngles.Add(dx * i + Angles.First());
                }
                pAngles.Add(Angles.Last());
                Angles = pAngles;
            }
            else if (Integration == IntegrationType.GaussLagrangeQuadrature)
            {
                Quadrature quadrature = new Quadrature();
                QuaderatureItemsAndWeights = quadrature.GaussLegendrePoints(Angles.First(), Angles.Last(), IntegrationPointResolution);
                ObservableCollection<double> QuadAngles = new ObservableCollection<double>();
                foreach (QuaderatureItem item in QuaderatureItemsAndWeights)
                {
                    QuadAngles.Add(item.Abscissa);
                }
                Angles = QuadAngles;

            }
            else if (Integration == IntegrationType.GaussChebychev)
            {

            }
            else
            {

            }

        }

        public object Clone()
        {
            return this.DeepClone();
        }
    }
}
