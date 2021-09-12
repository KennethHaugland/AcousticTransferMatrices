using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AcousticTransferMatrices.Core.ServiceMessage
{
    public class ReportCalculationProgress
    {
        public ReportCalculationProgress(int val , int max)
        { 
            value = val;
            maxvalue = max;
        }
        public int value { get; set; }
        public int maxvalue { get; set; }
    }
}
