using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using AcousticTransferMatrices.Core.Acoustics.Configurations;
using AcousticTransferMatrices.Core.ReflectionHelpers;
using AcousticTransferMatrices.Core.ServiceMessage;
using System.Reactive.Linq;
using System.Reactive.Concurrency;
using System.Xml.Serialization;
//using System.Reactive.Bindings.Extensions;

namespace AcousticTransferMatrices.Core.Acoustics.Calculations
{
    public static class MainCalculation
    {
        public static List<Type> GetAwailableLayers()
        {
            return ReflectionExtencions
                 .GetTypesInNamespace(Assembly.GetExecutingAssembly(), "MatrixMaterials")
                 .Where(t => t.BaseType.Name == "LayerBaseClass")
                 .ToList();
        }
    }
}

