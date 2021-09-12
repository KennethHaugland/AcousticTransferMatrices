using AcousticTransferMatrices.Core.Regions;
using AcousticTransferMatrices.LayerSetup.Views;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;

namespace AcousticTransferMatrices.LayerSetup
{
    public class LayerSetupModule : IModule
    {
        private IRegionManager rm;
        public LayerSetupModule(IRegionManager regionManager)
        {
            rm = regionManager;
        }

        public void OnInitialized(IContainerProvider containerProvider)
        {
            rm.RegisterViewWithRegion(RegionNames.LayerSetupRegion, typeof(LayerComposition));
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {

        }
    }
}