using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Renci.Wwt.DataManager.Common.Models;
using Microsoft.Practices.Prism.Events;
using Renci.Wwt.DataManager.Common.Events;
using System.IO;
using System.Xml.Linq;
using Microsoft.Practices.ServiceLocation;
using System.Windows.Data;
using Microsoft.Practices.Unity;
using System.ComponentModel;
using Renci.Wwt.DataManager.Common.BaseClasses;
using System.Collections.ObjectModel;

namespace Renci.Wwt.DataManager.Common.Models
{
    public class WorkDocument : ModelBase
    {
        public ObservableCollection<DataSourceInfo> DataSources { get; private set; }

        public ObservableCollection<FilterInfo> DataFilters { get; private set; }

        public WorkDocument()
        {
            this.DataSources = new ObservableCollection<DataSourceInfo>();
            this.DataFilters = new ObservableCollection<FilterInfo>();
        }

        public WorkDocument(XElement document)
            : this()
        {
            if (!document.Name.LocalName.Equals("workDocument", StringComparison.InvariantCulture))
                throw new InvalidDataException("'workDocument' is expected.");
            
            foreach (var element in document.Descendants("dataFilter"))
            {
                var type = Type.GetType(element.Attribute("type").Value);

                var dataSourceFilter = Activator.CreateInstance(type) as FilterInfo;
                dataSourceFilter.Load(element);
                this.DataFilters.Add(dataSourceFilter);
            }

            foreach (var element in document.Descendants("dataSource"))
            {
                var type = Type.GetType(element.Attribute("type").Value);

                var dataSourceInfo = Activator.CreateInstance(type) as DataSourceInfo;
                dataSourceInfo.Load(this, element);
                this.DataSources.Add(dataSourceInfo);
            }
        }

        public void AddDataSourceInfo(DataSourceInfo dataSourceInfo)
        {
            if (dataSourceInfo.Index < 1)
            {
                dataSourceInfo.Index = (from item in this.DataSources.OfType<DataSourceInfo>() select item.Index).DefaultIfEmpty().Max() + 1;
            }
            this.DataSources.Add(dataSourceInfo);
        }

        public void RemoveDataSourceInfo(DataSourceInfo dataSourceInfo)
        {
            this.DataSources.Remove(dataSourceInfo);
        }

        public void AddDataSourceFilter(FilterInfo dataSourceFilter)
        {
            if (dataSourceFilter.Index < 1)
            {
                dataSourceFilter.Index = (from item in this.DataFilters.OfType<FilterInfo>() select item.Index).DefaultIfEmpty().Max() + 1;
            }

            this.DataFilters.Add(dataSourceFilter);
        }

        public void RemoveDataSourceFilter(FilterInfo dataSourceFilter)
        {
            this.DataFilters.Remove(dataSourceFilter);
        }

        public XElement Save()
        {
            return new XElement("workDocument",
                from ds in this.DataSources.OfType<DataSourceInfo>()
                select ds.Save(),
                from ds in this.DataFilters.OfType<FilterInfo>()
                select ds.Save()
                );
        }
    }
}
