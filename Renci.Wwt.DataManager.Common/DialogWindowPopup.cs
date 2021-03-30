using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.Prism.Interactivity.InteractionRequest;
using Renci.Wwt.DataManager.Common.BaseClasses;

namespace Renci.Wwt.DataManager.Common
{
    public class DialogWindowPopup : Notification
    {
        public bool IsModal { get; private set; }

        public DialogWindowPopup(ViewModelBase viewModel)
        {
            this.Content = viewModel;
        }

        public DialogWindowPopup(bool isModal, ViewModelBase viewModel)
            : this(viewModel)
        {
            this.IsModal = isModal;
        }
    }
}
