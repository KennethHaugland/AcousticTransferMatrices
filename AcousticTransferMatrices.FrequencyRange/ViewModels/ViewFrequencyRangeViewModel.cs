using AcousticTransferMatrices.Core.ServiceMessage;
using Mathematics.Acoustics;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AcousticTransferMatrices.FrequencyRange.ViewModels
{
    public class ViewFrequencyRangeViewModel : BindableBase
    {
        private readonly IEventMessager EM;
        public ViewFrequencyRangeViewModel(IEventMessager eventMessanger)
        {
            EM = eventMessanger;
            ExecuteGenerateFrequencyCommand();
            EM.Publish(new FrequencyInformation() { Frequencies = Frequencies.ToArray() });
            EM.Observe<GetFrequencies>().Subscribe(x => 
                    ExecuteGenerateFrequencyCommand()
            );

            EM.Observe<FrequencyFromLayer>().Subscribe(arg =>
            {
                if (arg.SetItem)
                {
                    Frequencies = new ObservableCollection<double>(arg.SelectedLayer.Frequencies);
                    StartFrequency = arg.SelectedLayer.StartFrequency;
                    EndFrequency = arg.SelectedLayer.EndFrequency;
                    SelectedIndex = arg.SelectedLayer.OktaveSpacing?0:1;
                    OctavebandType = arg.SelectedLayer.OktavebandType;
                    EM.Publish(new IntigationyConditionsFromLayer () { SelectedLayer = arg.SelectedLayer, Calculate = arg.Calculate, SetItem = arg.SetItem });
                }
                else
                {
                    arg.SelectedLayer.Frequencies = Frequencies.ToArray();
                    arg.SelectedLayer.StartFrequency = StartFrequency;
                    arg.SelectedLayer.EndFrequency = EndFrequency;
                    arg.SelectedLayer.OktaveSpacing = SelectedIndex == 0 ? true : false;
                    arg.SelectedLayer.OktavebandType = OctavebandType;
                    EM.Publish(new IntigationyConditionsFromLayer() { SelectedLayer = arg.SelectedLayer, Calculate = arg.Calculate, SetItem = arg.SetItem });
                }
            });
        }

        private DelegateCommand _GenerateFrequencyCommand;
        public DelegateCommand GenerateFrequencyCommand =>
            _GenerateFrequencyCommand ?? (_GenerateFrequencyCommand = new DelegateCommand(ExecuteGenerateFrequencyCommand));

        public void ExecuteGenerateFrequencyCommand()
        {
            Frequencies.Clear();
            OctaveBand test = new OctaveBand(StartFrequency, EndFrequency, OctavebandType);

            if (SelectedIndex == 0)
            {
                double[] testing = test.GetOctavebandFrequencies();
                foreach (double item in testing)
                {
                    Frequencies.Add(item);
                }

                // Dont want Octavband explicitly, we want start and end frequency also (mostly for plot)
                if (!Frequencies.Contains(StartFrequency))
                    Frequencies.Insert(0, StartFrequency);
                if (!Frequencies.Contains(EndFrequency))
                    Frequencies.Add(EndFrequency);
            }
            else
            {
                double[] testing = test.Logspace(StartFrequency, EndFrequency, OctavebandType);
                foreach (double item in testing)
                {
                    Frequencies.Add(item);
                }
            }

            EM.Publish(new FrequencyInformation() { Frequencies = Frequencies.ToArray() });

        }

        private double pStartFrequency = 50;
        public double StartFrequency
        {
            get { return pStartFrequency; }
            set { SetProperty(ref pStartFrequency, value); }
        }

        private double pEndFrequency = 5000;
        public double EndFrequency
        {
            get { return pEndFrequency; }
            set { SetProperty(ref pEndFrequency, value); }
        }

        private int pSelectedIndex = 0;
        public int SelectedIndex
        {
            get { return pSelectedIndex; }
            set { SetProperty(ref pSelectedIndex, value); }
        }

        private ObservableCollection<double> pFrequencies = new ObservableCollection<double>();
        public ObservableCollection<double> Frequencies
        {
            get { return pFrequencies; }
            set { SetProperty(ref pFrequencies, value); }
        }


        private ObservableCollection<string> pIntevalSpacing = new ObservableCollection<string>() { "Oktavband type", "Number of items (logspaced)" };
        public ObservableCollection<string> IntevalSpacing
        {
            get { return pIntevalSpacing; }
            set { SetProperty(ref pIntevalSpacing, value); }
        }

        private int pOctavebandType = 12;
        public int OctavebandType
        {
            get { return pOctavebandType; }
            set { SetProperty(ref pOctavebandType, value); }
        }

    }
}
