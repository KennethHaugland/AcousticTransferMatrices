using AcousticTransferMatrices.Core.Acoustics;
using AcousticTransferMatrices.Core.ServiceMessage;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AcousticTransferMatrices.MaterialProperties.ViewModels
{
    public class ViewAViewModel : BindableBase
    {


        IEventMessager EM;
        public ViewAViewModel(IEventMessager eventMessager)
        {
            EM = eventMessager;
            EM.Observe<EditMaterialMessage>().Subscribe(arg => {
                MaterialEdit = null;
                MaterialEdit = arg.item;  
            });
        }

        private DelegateCommand _SaveEdit;
        public DelegateCommand SaveEdit =>
            _SaveEdit ?? (_SaveEdit = new DelegateCommand(ExecuteSaveEdit));

        void ExecuteSaveEdit()
        {
            //EM.Publish(new EditMaterialFinisedMessage() { item = MaterialEdit });
        }

        private DelegateCommand _SaveCommand;
        public DelegateCommand SaveCommand =>
            _SaveCommand ?? (_SaveCommand = new DelegateCommand(ExecuteSaveCommand));

        void ExecuteSaveCommand()
        {

        }

        private LayerBaseClass  _MaterialEdit;
        public LayerBaseClass MaterialEdit
        {
            get { return _MaterialEdit; }
            set { SetProperty(ref _MaterialEdit, value); }
        }
    }
}
