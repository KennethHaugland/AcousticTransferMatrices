using AcousticTransferMatrices.Core.Acoustics.Configurations;
using AcousticTransferMatrices.Core.ServiceMessage;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static AcousticTransferMatrices.Core.Acoustics.Configurations.IntegrationConfiguration;

namespace AcousticTransferMatrices.Integration.ViewModels
{
    public class ViewAViewModel : BindableBase
    {

        private IEventMessager EM;
        public ViewAViewModel( IEventMessager eventMessenger)
        {
            EM = eventMessenger;

            EM.Observe<IntigationyConditionsFromLayer>().Subscribe(arg =>
            {
                if (arg.SetItem)
                {
                    SetAllProerties(arg.SelectedLayer.IC);
                    EM.Publish(new BoundaryConditionsFromLayer() { SetItem = arg.SetItem, Calculate = arg.Calculate, SelectedLayer = arg.SelectedLayer });
                }
                else
                {
                    GetAllProerties(arg.SelectedLayer.IC);                    
                    EM.Publish(new BoundaryConditionsFromLayer() { SetItem = arg.SetItem, Calculate = arg.Calculate, SelectedLayer = arg.SelectedLayer });
                }
            });
        }

        private void SetAllProerties(IIntegrationConfiguration ic)
        {
            if (ic.Integration == IntegrationConfiguration.IntegrationType.SingleAngles)
            {
                Angles = "";
                Angles = string.Join(", ", ic.Angles.Select(x => x * 180 / Math.PI));
            }
            else 
            {
                if (ic.InputAngles.Count > 1)
                    StartAngle = ic.InputAngles[0] * 180 / Math.PI;

                if (ic.InputAngles.Count > 0)
                    EndAngle = ic.InputAngles[ic.InputAngles.Count - 1] * 180 / Math.PI;
            }

            SelectionIndex = (int)ic.Integration;
            IntegrationStep = ic.IntegrationPointResolution;
        }

        private void GetAllProerties(IIntegrationConfiguration ic)
        {
            ic.Integration = (IntegrationType)SelectionIndex;
            ic.IntegrationPointResolution = IntegrationStep;

            if (ic.Integration == IntegrationConfiguration.IntegrationType.SingleAngles)
            {
                List<double> d = new List<double>();
                if (Angles != null)
                {
                    d = Angles
                         .Split(", ".ToArray())
                         .Where(x => !string.IsNullOrWhiteSpace(x))
                         .Select(x => Convert.ToDouble(x) * Math.PI / 180)
                         .ToList();
                    ic.Angles = new ObservableCollection<double>(d);
                }
            }
            else if (ic.Integration == IntegrationConfiguration.IntegrationType.SimpleSum)
            {
                ic.Angles = new ObservableCollection<double>();
                ic.SetIntegrationLimits(EndAngle, StartAngle, true);

            }
            else if (ic.Integration == IntegrationConfiguration.IntegrationType.GaussLagrangeQuadrature)
            {
                if (ic.IntegrationPointResolution > 64)
                {
                    ic.IntegrationPointResolution = 64;
                }
                else if (ic.IntegrationPointResolution < 2)
                {
                    ic.IntegrationPointResolution = 2;
                }
                ic.Angles = new ObservableCollection<double>();
                ic.SetIntegrationLimits(EndAngle, StartAngle, true);

            }
         }

        private double pStartAngle = 0;
        public double StartAngle
        {
            get { return pStartAngle; }
            set { SetProperty(ref pStartAngle, value); }
        }

        private double pEndAngle = 90;
        public double EndAngle
        {
            get { return pEndAngle; }
            set { SetProperty(ref pEndAngle, value); }
        }

        private int pSelectionIndex;

        public int SelectionIndex
        {
            get { return pSelectionIndex; }
            set { SetProperty(ref pSelectionIndex, value); }
        }

        private ObservableCollection<string> pIntegrationType = new ObservableCollection<string>() { "Selected angle(s)", "Simple sum", "Gauss-Lagrange" };
        public ObservableCollection<string> IntegrationType
        {
            get { return pIntegrationType; }
            set { SetProperty(ref pIntegrationType, value); }
        }

        private int pIntegrationStep;
        public int IntegrationStep
        {
            get { return pIntegrationStep; }
            set { SetProperty(ref pIntegrationStep, value); }
        }

        private string pAngles;
        public string Angles
        {
            get { return pAngles; }
            set { SetProperty(ref pAngles, value); }
        }
    }
}
