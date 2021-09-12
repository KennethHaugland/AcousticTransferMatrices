using AcousticTransferMatrices.Core.Acoustics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AcousticTransferMatrices.Core.ServiceMessage
{
    public class FrequencyFromLayer
    {
        public bool SetItem { get; set; } = false;
        public bool Calculate { get; set; } = false;
        public LayerModel SelectedLayer { get; set; }
    }

    public class BoundaryConditionsFromLayer
    {
        public bool SetItem { get; set; } = false;
        public bool Calculate { get; set; } = false;
        public LayerModel SelectedLayer { get; set; }
    }

    public class IntigationyConditionsFromLayer
    {
        public bool SetItem { get; set; } = false;
        public bool Calculate { get; set; } = false;
        public LayerModel SelectedLayer { get; set; }
    }

    public class LayerSetupFromLayer
    {
        public bool SetItem { get; set; } = false;
        public bool Calculate { get; set; } = false;
        public LayerModel SelectedLayer { get; set; }
    }

    public class CalculationConditionsFromLayer
    {
        public bool SetItem { get; set; } = false;
        public bool Calculate { get; set; } = false;
        public LayerModel SelectedLayer { get; set; }
    }
}
