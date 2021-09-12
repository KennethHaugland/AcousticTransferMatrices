using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AcousticTransferMatrices.MaterialSelectionTree.ViewModels
{
    public class Group:BindableBase
    {

        private string _Name;
        public string Name
        {
            get { return _Name; }
            set { SetProperty(ref _Name, value); }
        }

        private bool _IsMaterial;
        public bool IsMaterial
        {
            get { return _IsMaterial; }
            set { SetProperty(ref _IsMaterial, value); }
        }

        private Type _MaterialType;
        public Type MaterialType
        {
            get { return _MaterialType; }
            set { SetProperty(ref _MaterialType, value); }
        }

        private ObservableCollection<Group> _Children = new ObservableCollection<Group>();
        public ObservableCollection<Group> Children
        {
            get { return _Children; }
            set { SetProperty(ref _Children, value); }
        }
    }
}
