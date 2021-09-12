using AcousticTransferMatrices.Core.Acoustics;
using AcousticTransferMatrices.Core.Acoustics.Configurations;
using AcousticTransferMatrices.Core.Acoustics.MatrixMaterials;
using AcousticTransferMatrices.Core.ServiceMessage;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Disposables;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace AcousticTransferMatrices.LayerSetup.ViewModels
{
    public class LayerCompositionViewModel : BindableBase
    {
        CompositeDisposable ReactiveSubscribtions = new CompositeDisposable();

        private IBoundaryConfiguration BC;
        private IEventMessager EM;
        public LayerCompositionViewModel(IBoundaryConfiguration boundaryConfiguration, IEventMessager eventMessager)
        {
            EM = eventMessager;
            ReactiveSubscribtions.Add(EM.Observe<SendMaterial>().Subscribe(x => AddMaterial(x)));

            PropertyChanged += (s, e) => 
            {
                if (e.PropertyName == "MatrixType")
                {
                    MatrixTypeChanged(MatrixType);
                    BC.Transmission = MatrixType;
                }
            };

            ReactiveSubscribtions.Add(EM.Observe<BoundaryConditionsFromLayer>().Subscribe(arg => {

                if (arg.SetItem)
                {
                    BC = arg.SelectedLayer.BC;
                    FrontLayer = BC.FrontLayer;
                    BackLayer = BC.BackLayer;
                    Layers = new ObservableCollection<LayerBaseClass>(arg.SelectedLayer.Materials);
                    MatrixType = BC.Transmission;
                    MatrixTypeChanged(MatrixType);
                }
                else
                {
                    arg.SelectedLayer.BC = BC;
                    arg.SelectedLayer.Materials = Layers.ToList();
                    EM.Publish(new CalculationConditionsFromLayer() {SelectedLayer = arg.SelectedLayer, Calculate = arg.Calculate, SetItem = false });
                }
            }));

            BC = boundaryConfiguration;
            FrontLayer = BC.FrontLayer;
            BackLayer = BC.BackLayer;
            MatrixType = BC.Transmission;
            MatrixTypeChanged(MatrixType);
        }

        private void MatrixTypeChanged(TransmissionTypes transmission)
        {
            if (transmission == TransmissionTypes.AbsorptionAnechoic)
            {
                BackLayer = new Air() { Thickness = double.PositiveInfinity, Name = "Anechoic termintation", HasPropagationCoefficient = false };
            }
            else if (transmission == TransmissionTypes.AbsorptionHardWall)
            {
                BackLayer = new Air() { Thickness = double.PositiveInfinity, Name = "Hard wall", HasPropagationCoefficient = false };
            }
            else if (transmission == TransmissionTypes.TransmissionLossQuaterWaveMatrix)
            {
                BackLayer = new Air() { Thickness = double.PositiveInfinity, Name = "Pressure relese", HasPropagationCoefficient = false };
            }
            else
            {
                BackLayer = new Air() { Thickness = double.PositiveInfinity, Name = "Air", HasPropagationCoefficient = true };
            }
        }

        private void AddMaterial(SendMaterial layerBase)
        {
            Layers.Add(layerBase.item);
        }

        private bool _IsEditable = false;
        public bool IsEditable
        {
            get { return _IsEditable; }
            set { SetProperty(ref _IsEditable, value); }
        }

        private TransmissionTypes _MatrixType = TransmissionTypes.AbsorptionHardWall;
        public TransmissionTypes MatrixType
        {
            get { return _MatrixType; }
            set { SetProperty(ref _MatrixType, value); }
        }

        private ObservableCollection<LayerBaseClass> pLayers = new ObservableCollection<LayerBaseClass>();
        public ObservableCollection<LayerBaseClass> Layers
        {
            get { return pLayers; }
            set { SetProperty(ref pLayers, value); }
        }

        private LayerBaseClass _FrontLayer = new Air() { Name="Air" , Thickness=double.PositiveInfinity};
        public LayerBaseClass FrontLayer
        {
            get { return _FrontLayer; }
            set {SetProperty(ref _FrontLayer, value); }
        }

        private LayerBaseClass _BackLayer = new Air() { Name = "Air", Thickness = double.PositiveInfinity };
        public LayerBaseClass BackLayer
        {
            get { return _BackLayer; }
            set 
            { 
                SetProperty(ref _BackLayer, value);
                BackPropertyCollection.Clear();
                BackPropertyCollection.Add(value);
            }
        }

        private ObservableCollection<LayerBaseClass> _BackPropertyCollection = new ObservableCollection<LayerBaseClass>();
        public ObservableCollection<LayerBaseClass> BackPropertyCollection
        {
            get { return _BackPropertyCollection; }
            set { SetProperty(ref _BackPropertyCollection, value); }
        }

        private DelegateCommand<LayerBaseClass> _EditMaterial;
        public DelegateCommand<LayerBaseClass> EditMaterial =>
            _EditMaterial ?? (_EditMaterial = new DelegateCommand<LayerBaseClass>(ExecuteEditMaterial));

        void ExecuteEditMaterial(LayerBaseClass layerBaseClass)
        {
            EM.Publish(new EditMaterialMessage() { item = layerBaseClass });
        }

        private DelegateCommand<LayerBaseClass> _MoveMaterialUp;
        public DelegateCommand<LayerBaseClass> MoveMaterialUp =>
            _MoveMaterialUp ?? (_MoveMaterialUp = new DelegateCommand<LayerBaseClass>(ExecuteMoveMaterialUp));

        void ExecuteMoveMaterialUp(LayerBaseClass layerBaseClass)
        {
            int index = Layers.IndexOf(layerBaseClass);
            if (index != 0)
            {
                Layers.Move(index, index - 1);
            }
        }

        private DelegateCommand<LayerBaseClass> _MoveMaterialDown;
        public DelegateCommand<LayerBaseClass> MoveMaterialDown =>
            _MoveMaterialDown ?? (_MoveMaterialDown = new DelegateCommand<LayerBaseClass>(ExecuteMoveMaterialDown));

        void ExecuteMoveMaterialDown(LayerBaseClass layerBaseClass)
        {
            int index = Layers.IndexOf(layerBaseClass);
            if (index != (Layers.Count() - 1))
            {
                Layers.Move(index, index + 1);
            }
        }

        private DelegateCommand _SaveCommand;
        public DelegateCommand SaveCommand =>
            _SaveCommand ?? (_SaveCommand = new DelegateCommand(ExecuteSaveCommand));

        void ExecuteSaveCommand()
        {
            EM.Publish(Layers.ToList());
        }

        private DelegateCommand<LayerBaseClass> _RemoveMaterial;
        public DelegateCommand<LayerBaseClass> RemoveMaterial =>
            _RemoveMaterial ?? (_RemoveMaterial = new DelegateCommand<LayerBaseClass>(ExecuteRemoveMaterial));

        void ExecuteRemoveMaterial(LayerBaseClass layerBaseClass)
        {
            Layers.Remove(layerBaseClass);
        }  
    }
}
