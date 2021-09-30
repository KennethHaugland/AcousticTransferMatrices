using AcousticTransferMatrices.Core.Acoustics;
using AcousticTransferMatrices.Core.Acoustics.Calculations;
using AcousticTransferMatrices.Core.ServiceMessage;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace AcousticTransferMatrices.MaterialSelectionTree.ViewModels
{
    public class ViewAViewModel : BindableBase
    {
        private string _message;
        public string Message
        {
            get { return _message; }
            set { SetProperty(ref _message, value); }
        }

        private ObservableCollection<Group> _MaterialComponents = new ObservableCollection<Group>();
        public ObservableCollection<Group> MaterialComponents
        {
            get { return _MaterialComponents; }
            set { SetProperty(ref _MaterialComponents, value); }
        }

        private IEventMessager EM;

        public ViewAViewModel(IEventMessager eventMessanger)
        {
            EM = eventMessanger;


            Message = "View A from your Prism Module";

            Group RootElement = new Group() { Name="Materials"};
            List<Type> MaterialTypes = MainCalculation.GetAwailableLayers();
            foreach (Type item in MaterialTypes)
            {
                LayerBaseClass CurrentMaterial = (LayerBaseClass)Activator.CreateInstance(item);
                Group CurrentMaterialGroup = new Group() { Name = CurrentMaterial.Group };

                if (!RootElement.Children.Contains(CurrentMaterialGroup))
                    RootElement.Children.Add(CurrentMaterialGroup);

                Group CurrentMaterialItem = RootElement.Children[RootElement.Children.IndexOf(CurrentMaterialGroup)]; 
                CurrentMaterialItem.Children.Add(new Group() { Name = CurrentMaterial.Name, IsMaterial = true, MaterialType = item });
            }
            MaterialComponents.Add(RootElement);
        }

        private Group _SelectedItem;
        public Group SelectedItem
        {
            get { return _SelectedItem; }
            set { 
                SetProperty(ref _SelectedItem, value); 
            }
        }

        private DelegateCommand _AddItemCommand;
        public DelegateCommand AddItemCommand =>
            _AddItemCommand ?? (_AddItemCommand = new DelegateCommand(ExecuteAddItemCommand));

        void ExecuteAddItemCommand()
        {
            if (SelectedItem == null)
                return;

            if (SelectedItem.IsMaterial)
            {
                LayerBaseClass item = (LayerBaseClass)Activator.CreateInstance(SelectedItem.MaterialType);
                EM.Publish(new SendMaterial() { item = item });
            }
        }
    }
}
