using AcousticTransferMatrices.Integration.Views;
using AcousticTransferMatrices.Core.Regions;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;

namespace AcousticTransferMatrices.Integration
{
    public class IntegrationModule : IModule
    {
        private readonly IRegionManager RM;
        public IntegrationModule(IRegionManager regionManager)
        {
            RM = regionManager;
        }
        public void OnInitialized(IContainerProvider containerProvider)
        {
            RM.RegisterViewWithRegion(RegionNames.IntegrationRegion, typeof(ViewA));
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            
        }
    }
}