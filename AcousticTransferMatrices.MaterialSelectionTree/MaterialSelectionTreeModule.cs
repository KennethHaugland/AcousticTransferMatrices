using AcousticTransferMatrices.Core.Regions;
using AcousticTransferMatrices.MaterialSelectionTree.Views;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;

namespace AcousticTransferMatrices.MaterialSelectionTree
{
    public class MaterialSelectionTreeModule : IModule
    {
        private readonly IRegionManager RM;
        public MaterialSelectionTreeModule(IRegionManager regionManager)
        {
            RM = regionManager;
        }
        public void OnInitialized(IContainerProvider containerProvider)
        {
            RM.RegisterViewWithRegion(RegionNames.BoundaryRegion, typeof(ViewA));
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            
        }
    }
}