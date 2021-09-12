using AcousticTransferMatrices.Core.Acoustics.Configurations;
using System.Collections.Generic;

namespace AcousticTransferMatrices.Core.Acoustics
{
    public interface ILayerModel
    {
        IBoundaryConfiguration BC { get; set; }
        TransmissionTypes CalculationType { get; set; }
        IIntegrationConfiguration IC { get; set; }
        List<LayerBaseClass> Materials { get; set; }
        List<FreqValues> Results { get; set; }
    }
}