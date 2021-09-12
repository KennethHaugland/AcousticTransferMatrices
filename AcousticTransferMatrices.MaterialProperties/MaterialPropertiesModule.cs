using AcousticTransferMatrices.Core.Regions;
using AcousticTransferMatrices.MaterialProperties.Views;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;

namespace AcousticTransferMatrices.MaterialProperties
{
    public class MaterialPropertiesModule : IModule
    {
        private readonly IRegionManager RM;
        public MaterialPropertiesModule(IRegionManager rm)
        {
            RM = rm;

        }
        public void OnInitialized(IContainerProvider containerProvider)
        {
            RM.RegisterViewWithRegion(RegionNames.PropertyRegion, typeof(ViewA));
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            
        }
    }
}