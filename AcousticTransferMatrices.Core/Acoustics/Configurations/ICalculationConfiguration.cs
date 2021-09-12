using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace AcousticTransferMatrices.Core.Acoustics.Configurations
{
    public interface ICalculationConfiguration
    {
        TransmissionTypes CalculationType { get; set; }
        IBoundaryConfiguration BC { get; set; }
        double Frequency { get; set; }
        IIntegrationConfiguration IC { get; set; }
        bool IsAbsorbtion { get; set; }
        List<LayerBaseClass> Layers { get; set; }
        double Value { get; set; }

        void RunCalculations(Action<int> Progress, Action<List<FreqValues>> Result, params double[] freq);

        FreqValues Calculate();
        ICalculationConfiguration Clone(double frequency);
    }
}