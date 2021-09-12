using AcousticTransferMatrices.Core.Acoustics.Calcualtions;
using System.Numerics;

namespace AcousticTransferMatrices.Core.Acoustics.Configurations
{
    public interface IBoundaryConfiguration
    {
        LayerBaseClass BackLayer { get; set; }
        LayerBaseClass FrontLayer { get; set; }
        Complex Theta_in { get; set; }
        TransmissionTypes Transmission { get; set; }

        LayerTransition FrontMaterial(double freq, Complex Theta, int index);
        Complex GetResult(LayerTransition LC);
    }
}