using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Numerics;

namespace AcousticTransferMatrices.Core.Acoustics.Configurations
{
    public interface IIntegrationConfiguration
    {
        List<int> AngleIndex { get; set; }
        List<Complex> AngleResult { get; set; }
        ObservableCollection<double> Angles { get; set; }
        List<double> InputAngles { get; set; }
        IntegrationConfiguration.IntegrationType Integration { get; set; }
        int IntegrationPointResolution { get; set; }

        void AnglesInDegrees(IEnumerable<double> angles);
        object Clone();
        void ConfigureSetup();
        List<double> IntegrationResult(bool Absorbtion);
        void SetIntegrationLimits(double high, double low = 0, bool inDegrees = true);
    }
}