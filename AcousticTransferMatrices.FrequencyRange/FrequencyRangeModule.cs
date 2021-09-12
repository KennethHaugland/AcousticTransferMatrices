using AcousticTransferMatrices.Core.Regions;
using AcousticTransferMatrices.FrequencyRange.Views;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;

namespace AcousticTransferMatrices.FrequencyRange
{
    public class FrequencyRangeModule : IModule
    {

        private readonly IRegionManager _regionManager;

        public FrequencyRangeModule(IRegionManager regionManager)
        {
            _regionManager = regionManager;
        }
        public void OnInitialized(IContainerProvider containerProvider)
        {
            _regionManager.RegisterViewWithRegion(RegionNames.FrequencyRegion, typeof(ViewFrequencyRange));
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {

        }
    }
}