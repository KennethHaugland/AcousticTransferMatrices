using AcousticTransferMatrices.Core;
using AcousticTransferMatrices.Core.Acoustics;
using AcousticTransferMatrices.Core.Acoustics.Calculations;
using AcousticTransferMatrices.Core.Acoustics.Configurations;
using AcousticTransferMatrices.Core.ServiceMessage;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace AcousticTransferMatrices.BoundaryConditions.ViewModels
{
    public class ViewAViewModel : BindableBase
    {
        private IBoundaryConfiguration BC;
        private IEventMessager EM;
        public ViewAViewModel(IBoundaryConfiguration boundaryConfiguration, IEventMessager eventMessager)
        {
            BC = boundaryConfiguration;         
            EM = eventMessager;
                       
            Items = MainCalculation.GetAwailableLayers();

            PropertyChanged += (s, e) => {
                if (e.PropertyName == "CalculationTypeIndex")
                {
                    if (CalculationTypeIndex < 2)
                        BackMaterialVisibility = Visibility.Collapsed;
                    else
                        BackMaterialVisibility = Visibility.Visible;
                }
            };
        }

        private DelegateCommand _SaveChangesToBoundaryConditions;
        public DelegateCommand SaveChangesToBoundaryConditions =>
            _SaveChangesToBoundaryConditions ?? (_SaveChangesToBoundaryConditions = new DelegateCommand(Execute_SaveChangesToBoundaryConditions));

        void Execute_SaveChangesToBoundaryConditions()
        {
            BC.FrontLayer = (LayerBaseClass)Activator.CreateInstance(FrontLayer);
            BC.BackLayer = (LayerBaseClass)Activator.CreateInstance(BackLayer);
            BC.Transmission = TransmissionType;
            EM.Publish((BoundaryConfiguration)BC);
        }

        private int pCalculationTypeIndex = 0;
        public int CalculationTypeIndex
        {
            get { return pCalculationTypeIndex; }
            set { SetProperty(ref pCalculationTypeIndex, value); }
        }

        private List<Type> pItems = new List<Type>();
        public List<Type> Items
        {
            get { return pItems; }
            set { SetProperty(ref pItems, value); }
        }

        private Visibility pBackMaterialVisibility = Visibility.Collapsed;
        public Visibility BackMaterialVisibility
        {
            get { return pBackMaterialVisibility; }
            set { SetProperty(ref pBackMaterialVisibility, value); }
        }

        private Type pFrontLayer;
        public Type FrontLayer
        {
            get { return pFrontLayer; }
            set { SetProperty(ref pFrontLayer, value); }
        }

        private int pFrontMaterialIndex = 0;
        public int FrontMaterialIndex
        {
            get { return pFrontMaterialIndex; }
            set { SetProperty(ref pFrontMaterialIndex, value); }
        }

        private Type pBackLayer;
        public Type BackLayer
        {
            get { return pBackLayer; }
            set { SetProperty(ref pBackLayer, value); }
        }

        private int pBackMaterialIndex = 0;
        public int BackMaterialIndex
        {
            get { return pBackMaterialIndex; }
            set { SetProperty(ref pBackMaterialIndex, value); }
        }

        private TransmissionTypes pTransmissionType;
        public TransmissionTypes TransmissionType
        {
            get { return pTransmissionType; }
            set { SetProperty(ref pTransmissionType, value); }
        }

        private string _message;
        public string Message
        {
            get { return _message; }
            set { SetProperty(ref _message, value); }
        }

        public ViewAViewModel()
        {
            Message = "View A from your Prism Module";
        }
    }
}
