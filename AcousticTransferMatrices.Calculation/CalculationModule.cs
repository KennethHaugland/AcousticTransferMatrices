using AcousticTransferMatrices.Calculation.Views;
using AcousticTransferMatrices.Core.Regions;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;

namespace AcousticTransferMatrices.Calculation
{
    public class CalculationModule : IModule
    {
        private IRegionManager rm;
        public CalculationModule(IRegionManager regionManager)
        {
            rm = regionManager;
        }

        public void OnInitialized(IContainerProvider containerProvider)
        {
            rm.RegisterViewWithRegion(RegionNames.CalcualtionRegion, typeof(Calcualtion));
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            
        }
    }
}