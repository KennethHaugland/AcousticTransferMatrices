using AcousticTransferMatrices.BoundaryConditions.Views;
using AcousticTransferMatrices.Core.Regions;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;

namespace AcousticTransferMatrices.BoundaryConditions
{
    public class BoundaryConditionsModule : IModule
    {

        private readonly IRegionManager _regionManager;

        public BoundaryConditionsModule(IRegionManager regionManager)
        {
            _regionManager = regionManager;
        }

        public void OnInitialized(IContainerProvider containerProvider)
        {
          //  _regionManager.RegisterViewWithRegion(RegionNames.BoundaryRegion, typeof(ViewA));
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            
        }
    }
}