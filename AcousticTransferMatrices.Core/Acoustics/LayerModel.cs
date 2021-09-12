using AcousticTransferMatrices.Core.Acoustics.Configurations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace AcousticTransferMatrices.Core.Acoustics
{
    [Serializable]
    public class LayerModel : ILayerModel
    {
        public string Name { get; set; } = "Testname";

        public bool IsDirty { get; set; } = true;
        public List<LayerBaseClass> Materials { get; set; } = new List<LayerBaseClass>();

        public IBoundaryConfiguration BC { get; set; }
        public IIntegrationConfiguration IC { get; set; }

        public TransmissionTypes CalculationType { get; set; }

        public List<FreqValues> Results { get; set; }

        public double [] Frequencies { get; set; }

        public double StartFrequency { get; set; } = 50;
        public double EndFrequency { get; set; } = 5000;

        public int OktavebandType { get; set; } = 12;

        public bool OktaveSpacing { get; set; } = true;

    }
}
