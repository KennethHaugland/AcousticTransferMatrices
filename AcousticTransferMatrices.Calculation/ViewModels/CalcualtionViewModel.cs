using AcousticTransferMatrices.Core.Acoustics;
using AcousticTransferMatrices.Core.Acoustics.Configurations;
using AcousticTransferMatrices.Core.Serialization;
using AcousticTransferMatrices.Core.ServiceMessage;
using Mathematics.Acoustics;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using Prism.Commands;
using Prism.Ioc;
using Prism.Mvvm;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Forms;
using System.Xml.Linq;

namespace AcousticTransferMatrices.Calculation.ViewModels
{
    public class CalcualtionViewModel : BindableBase
    {
        private IEventMessager EM { get; set; }
        private IContainerProvider ContainerRegister;
        private ICalculationConfiguration CalculationConfiguration { get; set; }
        public CalcualtionViewModel(IEventMessager em,
                                    IContainerProvider containerRegister,
                                    ICalculationConfiguration calculationConfiguration)
        {
            ContainerRegister = containerRegister;
            EM = em;
            CalculationConfiguration = calculationConfiguration;

            EM.Observe<ReportCalculationProgress>().Subscribe(x => { ProgressValue = x.value;  });
            EM.Observe<CalculationConditionsFromLayer>().Subscribe(arg => {DoCalculations(arg.SelectedLayer, arg.Calculate);});

            PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == "SelectedCalcualtion")
                {
                    if (SelectedCalcualtion != null)
                    {
                        RebindPlot(SelectedCalcualtion);
                        EM.Publish(new FrequencyFromLayer() { SetItem = true, Calculate = false, SelectedLayer = SelectedCalcualtion });
                    }
                }
            };
        }
        
        private LayerModel pSelectedCalcualtion;
        public LayerModel SelectedCalcualtion
        {
            get { return pSelectedCalcualtion; }
            set { SetProperty(ref pSelectedCalcualtion, value); }
        }

        private ObservableCollection<LayerModel> pCalculations = new ObservableCollection<LayerModel>();
        public ObservableCollection<LayerModel> Calculations
        {
            get { return pCalculations; }
            set { SetProperty(ref pCalculations, value); }
        }

        private int _ProgressValue = 0;
        public int ProgressValue
        {
            get { return _ProgressValue; }
            set { SetProperty(ref _ProgressValue, value); }
        }

        private int pMaximumProgressValue = 100;
        public int MaximumProgressValue
        {
            get { return pMaximumProgressValue; }
            set { SetProperty(ref pMaximumProgressValue, value); }
        }

        private DelegateCommand _SaveMaterialCommand;
        public DelegateCommand SaveMaterialCommand =>
            _SaveMaterialCommand ?? (_SaveMaterialCommand = new DelegateCommand(ExecuteSaveMaterialCommand));

        void ExecuteSaveMaterialCommand()
        {
            if (SelectedCalcualtion != null)
            {
                SaveFileDialog dlg = new SaveFileDialog();                
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    XElement fil = SerializationHelpers.Serialize(SelectedCalcualtion);
                    fil.Save(dlg.FileName);
                }
            }
        }

        private DelegateCommand _LoadMaterialCommand;
        public DelegateCommand LoadMaterialCommand =>
            _LoadMaterialCommand ?? (_LoadMaterialCommand = new DelegateCommand(ExecuteLoadMaterialCommand));

        void ExecuteLoadMaterialCommand()
        {
            OpenFileDialog dlg = new OpenFileDialog();
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                XElement test = XElement.Load(dlg.FileName);
                LayerModel fil = SerializationHelpers.Deserialize<LayerModel>(test, new LayerModel() { CalculationType = TransmissionTypes.AbsorptionHardWall, BC = ContainerRegister.Resolve<IBoundaryConfiguration>(), IsDirty = true, Frequencies = new OctaveBand(50, 5000, 12).GetOctavebandFrequencies(), IC = ContainerRegister.Resolve<IIntegrationConfiguration>() });
                Calculations.Add(fil);
                SelectedCalcualtion = fil;
            }            
        }

        private void DoCalculations(LayerModel LM, bool runCalcualtions)
        {
            SelectedCalcualtion = LM;
            if (runCalcualtions)
                CalcualteAcosuticProperty();
        }

        public void RebindPlot(LayerModel UpdatedLayerModel)
        {
            PlotModel NewPlotModel = new PlotModel { LegendPosition = LegendPosition.LeftTop };
            LinearAxis linearAxis;
            if (UpdatedLayerModel.BC.Transmission.ToString().ToLower().Contains("absorp"))
            {
                linearAxis = new LinearAxis
                {
                    Maximum = 1.2,
                    Minimum = 0,
                    Title = "Absorbtion factor",
                    MajorGridlineStyle = LineStyle.Solid,
                    MinorGridlineStyle = LineStyle.Dot
                };

                NewPlotModel.Title = "Absorption coefficient";
                NewPlotModel.Subtitle = "generated from custom layers";
            }
            else
            {
                linearAxis = new LinearAxis
                {
                    Title = "Transmission - R",
                    MajorGridlineStyle = LineStyle.Solid,
                    MinorGridlineStyle = LineStyle.Dot
                };
                NewPlotModel.Title = "Sound transmission";
                NewPlotModel.Subtitle = "generated from custom layers";
            }

            NewPlotModel.Axes.Add(linearAxis);
            LogarithmicAxis logarithmicAxis = new LogarithmicAxis
            {
                Maximum = UpdatedLayerModel.Frequencies.ToList().Last(),
                Minimum = UpdatedLayerModel.Frequencies.ToList().First(),
                MajorGridlineStyle = LineStyle.Solid,
                MinorGridlineStyle = LineStyle.Dot,
                Position = AxisPosition.Bottom,
                Title = "Frequency [Hz]"
            };

            NewPlotModel.Axes.Add(logarithmicAxis);

            if (UpdatedLayerModel.Results.Count != 0)
            {
                for (int i = 0; i < UpdatedLayerModel.Results[0].Value.Count; i++)
                {
                    var CurrentSerie = new LineSeries { MarkerType = MarkerType.None };
                    if (UpdatedLayerModel.IC.Integration == IntegrationConfiguration.IntegrationType.SingleAngles)
                        CurrentSerie.Title = "Angle: " + (UpdatedLayerModel.IC.Angles[i] * 180 / Math.PI).ToString();
                    else
                        CurrentSerie.Title = "Diffuse integral: " + (UpdatedLayerModel.IC.Angles.First() * 180 / Math.PI).ToString() + " - " + (UpdatedLayerModel.IC.Angles.Last() * 180 / Math.PI).ToString();

                    foreach (FreqValues item in UpdatedLayerModel.Results)
                        CurrentSerie.Points.Add(new DataPoint(item.Freq, item.Value[i]));

                    NewPlotModel.Series.Add(CurrentSerie);
                }
            }
            SelectedPlotModel = NewPlotModel;
        }

        private DelegateCommand _AddMaterialCommand;
        public DelegateCommand AddMaterialCommand =>
            _AddMaterialCommand ?? (_AddMaterialCommand = new DelegateCommand(ExecuteAddMaterialCommand));

        void ExecuteAddMaterialCommand()
        {
            LayerModel layer = new LayerModel() { CalculationType = TransmissionTypes.AbsorptionHardWall, BC = ContainerRegister.Resolve<IBoundaryConfiguration>(), IsDirty = true, Frequencies = new OctaveBand(50, 5000, 12).GetOctavebandFrequencies(), IC = ContainerRegister.Resolve<IIntegrationConfiguration>() };
            Calculations.Add(layer);
            EM.Publish(new FrequencyFromLayer() { SetItem = false, Calculate = false, SelectedLayer = layer });
        }

        private DelegateCommand _RemoveMaterialCommand;
        public DelegateCommand RemoveMaterialCommand =>
            _RemoveMaterialCommand ?? (_RemoveMaterialCommand = new DelegateCommand(ExecuteRemoveMaterialCommand));

        void ExecuteRemoveMaterialCommand()
        {
            Calculations.Remove(SelectedCalcualtion);
        }

        private DelegateCommand _CalculateMaterialCommand;
        public DelegateCommand CalculateMaterialCommand =>
            _CalculateMaterialCommand ?? (_CalculateMaterialCommand = new DelegateCommand(ExecuteCalculateMaterialCommandAsync));

        private void CalcualteAcosuticProperty()
        {
            MaximumProgressValue = SelectedCalcualtion.Frequencies.Length;
            if (SelectedCalcualtion != null)
            {
                CalculationConfiguration.BC = SelectedCalcualtion.BC;
                CalculationConfiguration.IC = SelectedCalcualtion.IC;
                CalculationConfiguration.CalculationType = SelectedCalcualtion.CalculationType;
                CalculationConfiguration.Layers = SelectedCalcualtion.Materials;
                CalculationConfiguration.RunCalculations(
                    progress => ProgressValue = progress,
                    FreqValuesResult =>
                    {
                        SelectedCalcualtion.Results = FreqValuesResult;
                        RebindPlot(SelectedCalcualtion);
                    },
                    SelectedCalcualtion.Frequencies);
            }
        }
        public void ExecuteCalculateMaterialCommandAsync()
        {
            if (SelectedCalcualtion != null)
            {
                EM.Publish(new FrequencyFromLayer() { SetItem = false, Calculate = true, SelectedLayer = SelectedCalcualtion });
            }
        }

        private PlotModel pSelectedPlotModel;
        public PlotModel SelectedPlotModel
        {
            get { return pSelectedPlotModel; }
            set { SetProperty(ref pSelectedPlotModel, value); }
        }

    }
}
