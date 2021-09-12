using System;
using System.ComponentModel;

namespace AcousticTransferMatrices.Core.Acoustics.Configurations
{
    [Serializable]
    public enum TransmissionTypes
    {
        //"Absorbtion - Hard wall", "Absorbtion - Anechoic", "Absorbtion - Impedance", "Transmissionloss", "Intersectionloss" };
        [Description("Absorbtion - Hard wall")]
        AbsorptionHardWall,
        [Description("Absorbtion - Anechoic")]
        AbsorptionAnechoic,
        [Description("Absorbtion - Impedance")]
        AbsorbtionCustom,
        [Description("Transmissionloss")]
        TransmissionLossMatrix,
        [Description("Intersectionloss")]
        IntersectionLossMatrix,
        [Description("Quater wave loss")]
        TransmissionLossQuaterWaveMatrix


    }

}
