// Copyright © 2010 Microsoft Corporation, All Rights Reserved.
// This code released under the terms of the Microsoft Research License Agreement (MSR-LA, http://sds.codeplex.com/License)
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace Microsoft.Research.Science.Data.Factory
{
    /// <summary>Collection of DataSet factory configuration records. It is mostly used by DataSet factories
    /// infrastructure.</summary>
    internal class FactoryConfigurationElementCollection : ConfigurationElementCollection
    {
        public override ConfigurationElementCollectionType CollectionType
        {
            get
            {
                return ConfigurationElementCollectionType.AddRemoveClearMap;
            }
        }

        public FactoryConfigurationElement this[int index]
        {
            get
            {
                return (FactoryConfigurationElement)BaseGet(index);
            }
            set
            {
                if (BaseGet(index) != null)
                    BaseRemoveAt(index);
                BaseAdd(index, value);
            }
        }

        public void Add(FactoryConfigurationElement element)
        {
            BaseAdd(element);
        }

        public void Clear()
        {
            BaseClear();
        }

        protected override ConfigurationElement CreateNewElement()
        {
            return new FactoryConfigurationElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return element.ToString();
        }

        public void Remove(FactoryConfigurationElement element)
        {
            BaseRemove(element.ToString());
        }

        public void RemoveAt(int index)
        {
            BaseRemoveAt(index);
        }
    }
}

