using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.Prism.Events;
using Renci.Wwt.DataManager.Common.Models;
using Renci.Wwt.DataManager.Common.Events;
using Microsoft.Practices.ServiceLocation;
using Renci.Wwt.DataManager.Common.Services;
using Renci.Wwt.DataManager.Common.BaseClasses;

namespace Renci.Wwt.DataManager.Services
{
    public class ApplicationService : IApplicationService
    {
        private readonly IEventAggregator _eventAggregator;

        private readonly Dictionary<Type, Func<ModelBase, ViewModelBase>> _viewModelTypeResolver;

        public ApplicationService(IEventAggregator eventAggregator)
        {
            this._eventAggregator = eventAggregator;
            this._viewModelTypeResolver = new Dictionary<Type, Func<ModelBase, ViewModelBase>>();
        }

        private WorkDocument _currentWorkDocument;
        public WorkDocument CurrentWorkDocument
        {
            get
            {
                if (this._currentWorkDocument == null)
                {
                    this._currentWorkDocument = new WorkDocument();
                    this._eventAggregator.GetEvent<WorkDocumentChangedEvent>().Publish(this._currentWorkDocument);
                }
                return this._currentWorkDocument;
            }
            set
            {
                //  TODO:   Save previous document if needed.
                if (this._currentWorkDocument != value)
                {
                    this._currentWorkDocument = value;
                    this._eventAggregator.GetEvent<WorkDocumentChangedEvent>().Publish(this._currentWorkDocument);
                }
            }
        }

        public void RegisterViewModel<T>(Func<ModelBase, ViewModelBase> resolver) where T : ModelBase
        {
            this._viewModelTypeResolver.Add(typeof(T), resolver);
        }

        public ViewModelBase ResolveViewModel(ModelBase dataItem)
        {
            var type = dataItem.GetType();

            if (this._viewModelTypeResolver.ContainsKey(type))
            {
                return this._viewModelTypeResolver[type](dataItem);
            }
            else
            {
                throw new InvalidOperationException(string.Format("ViewModel cannot be resolved for type '{0}'", type.FullName));
            }
        }
    }
}
