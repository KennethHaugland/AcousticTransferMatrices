using AcousticTransferMatrices.Views;
using Prism.Ioc;
using Prism.Modularity;
using System.Windows;
using AcousticTransferMatrices.FrequencyRange;
using AcousticTransferMatrices.Integration;
using AcousticTransferMatrices.Core.ServiceMessage;
using System;
using AcousticTransferMatrices.Core.Acoustics.Configurations;
using AcousticTransferMatrices.Core.Acoustics.MatrixMaterials;
using AcousticTransferMatrices.Core.Acoustics;
using AcousticTransferMatrices.Calculation;
using AcousticTransferMatrices.LayerSetup;
using AcousticTransferMatrices.MaterialSelectionTree;
using AcousticTransferMatrices.MaterialProperties;
using Mathematics;
using Mathematics.Acoustics;
using ExcelOpenXMLInterface;
using System.Data;

namespace AcousticTransferMatrices
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        protected override Window CreateShell()
        {
            return Container.Resolve<MainWindow>();
        }


        protected override void ConfigureModuleCatalog(IModuleCatalog moduleCatalog)
        {

            //Vector<double> R_1_1_i = new double[] { 50, 57, 60, 60, 60 };
            //Vector<double> R_1_3_i = new double[] { 20.4, 16.3, 17.7, 22.6, 22.4, 22.7, 24.8, 26.6, 28, 30.5, 31.8, 32.5, 33.4, 33, 31, 25.5 };

            //var x = R_1_3_i.Rw_13<double>();
            //var y = R_1_1_i.Rw_11<double>();

            DataTable table1 = new DataTable("patients");
            table1.Columns.Add("name", typeof(string));
            table1.Columns.Add("id", typeof(int));
            table1.Rows.Add("sam", 1);
            table1.Rows.Add("mark", 2);

            DataTable table2 = new DataTable("medications");
            table2.Columns.Add("id",typeof(int));
            table2.Columns.Add("medication",typeof(string));
            table2.Rows.Add(1, "atenolol");
            table2.Rows.Add(2, "amoxicillin");

            // Create a DataSet and put both tables in it.
            DataSet set = new DataSet("office");
            set.Tables.Add(table1);
            set.Tables.Add(table2);

            ExcelFunctions.GenerateExcelRapport(set);
            ExcelFunctions.GenerateExcelRapport(table1, "test2");

            moduleCatalog.AddModule<FrequencyRangeModule>();          
            moduleCatalog.AddModule<MaterialSelectionTreeModule>();
            moduleCatalog.AddModule<MaterialPropertiesModule>();

            moduleCatalog.AddModule<IntegrationModule>();
            moduleCatalog.AddModule<CalculationModule>();
            moduleCatalog.AddModule<LayerSetupModule>();
        }
        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            // Services
            _ = containerRegistry.RegisterSingleton<IEventMessager, EventMessager>();

            // Retun a default instance of this method, which implements clone for saving.
            _ = containerRegistry.Register<IIntegrationConfiguration, IntegrationConfiguration>();
            //{
            //    Integration = IntegrationConfiguration.IntegrationType.SimpleSum,
            //    IntegrationPointResolution = 200,
            //    Angles = { 0, 90 * Math.PI / 180 }
            //});

            _ = containerRegistry.RegisterInstance<IBoundaryConfiguration>(new BoundaryConfiguration()
            {
                FrontLayer = new Air(),
                BackLayer = new Air(),
                Transmission = TransmissionTypes.TransmissionLossMatrix
            });

            _ = containerRegistry.RegisterInstance(Current.Resources);

            // Register normal classes

            //Contain all Calculations configurations
            _ = containerRegistry.Register<ICalculationConfiguration, CalculationConfiguration>();

            //Complete layermodel with results and configuration
            _ = containerRegistry.Register<ILayerModel, LayerModel>();

        }
    }
}
