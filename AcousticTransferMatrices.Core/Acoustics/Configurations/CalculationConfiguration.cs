using AcousticTransferMatrices.Core.Acoustics.Calcualtions;
using AcousticTransferMatrices.Core.Serialization;
using AcousticTransferMatrices.Core.ServiceMessage;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AcousticTransferMatrices.Core.Acoustics.Configurations
{


    [Serializable]
    public class CalculationConfiguration : ICalculationConfiguration
    {

        public CalculationConfiguration()
        {

        }
        public double Frequency { get; set; }
        public bool IsAbsorbtion { get; set; }
        public IIntegrationConfiguration IC { get; set; }
        public IBoundaryConfiguration BC { get; set; }
        public List<LayerBaseClass> Layers { get; set; } = new List<LayerBaseClass>();

        public double Value { get; set; }
        public TransmissionTypes CalculationType { get; set; }

        public void RunCalculations(Action<int> Progress, Action<List<FreqValues>> Result, params double[] freq)
        {
            int ProgressValue = 0;
            List<FreqValues> FrequencyValues = new List<FreqValues>();
            var calc = freq.ToObservable(NewThreadScheduler.Default).
                        Select(Frequency => Clone(Frequency)).
                        SelectMany(CurrentFrequencyCalcualtion => Task.Run(() => CurrentFrequencyCalcualtion.Calculate())).
                        ObserveOnDispatcher().
                        Subscribe(args =>
                        {
                            // Single frequency completed
                            FrequencyValues.Add(args);
                            Progress(++ProgressValue);                            
                        }
                        , () =>
                        {
                            // Calucaltions completed
                            FrequencyValues.Sort();
                            Result(FrequencyValues);
                        });
        }

        public FreqValues Calculate()
        {
            IC.ConfigureSetup();
            IsAbsorbtion = BC.Transmission.ToString().Contains("Absorption");
            for (int k = 0; k < IC.Angles.Count; k++)
            {
                LayerTransition LC = BC.FrontMaterial(Frequency, IC.Angles[k], k);

                for (int i = 0; i < Layers.Count; i++)
                {
                    LC = Layers[i].GetOutputLayer(LC);
                }

                IC.AngleResult.Add(BC.GetResult(LC));
                IC.AngleIndex.Add(LC.AngleIndex);
                IC.InputAngles.Add(LC.InputThetaAngle);
            }

            return new FreqValues(Frequency, IC.IntegrationResult(IsAbsorbtion));
        }


        public ICalculationConfiguration Clone(double frequency)
        {
            CalculationConfiguration res = this.DeepClone();
            res.Frequency = frequency;
            return res;
        }
    }
}

