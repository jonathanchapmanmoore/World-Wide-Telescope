namespace Renci.Wwt.DataManager.Common.BaseClasses
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.ComponentModel;
    using System.Linq.Expressions;
    using System.ComponentModel.DataAnnotations;
    using System.Windows.Input;
    using Microsoft.Practices.Prism.Commands;

    public abstract class ViewModelBase : ModelBase
    {
        #region Close Command and events

        RelayCommand _closeCommand;
        [Browsable(false)]
        public ICommand CloseCommand
        {
            get
            {
                if (_closeCommand == null)
                {
                    _closeCommand = new RelayCommand(() => this.Close(), () => { return this.CanClose(); });
                }
                return _closeCommand;
            }
        }
        public event EventHandler Closing;

        public event Action RequestClose;

        public virtual void Close()
        {
            if (this.RequestClose != null)
            {
                this.RequestClose();
            }

            if (this.Closing != null)
            {
                this.Closing(this, EventArgs.Empty);
            }
        }

        public virtual bool CanClose()
        {
            return true;
        }

        #endregion
    }
}
