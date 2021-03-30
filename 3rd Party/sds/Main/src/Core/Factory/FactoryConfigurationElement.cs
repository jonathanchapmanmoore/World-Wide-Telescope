// Copyright © 2010 Microsoft Corporation, All Rights Reserved.
// This code released under the terms of the Microsoft Research License Agreement (MSR-LA, http://sds.codeplex.com/License)
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Reflection;
using System.Diagnostics;

namespace Microsoft.Research.Science.Data.Factory
{
    internal class FactoryConfigurationElement : ConfigurationElement
    {
        [ConfigurationProperty("ext",IsRequired = false)]
        public string Extension
        {
            get { return (string)this["ext"]; }
            set { this["ext"] = value; }
        }

        [ConfigurationProperty("name", IsRequired = false)]
        public string Name
        {
            get { return (string)this["name"]; }
            set { this["name"] = value; }
        }

        [ConfigurationProperty("type",IsRequired = true)]
        public string Type
        {
            get { return (string)this["type"]; }
            set { this["type"] = value; }
        }

        public override string ToString()
        {
            return String.Format("name={0}; ext={1}; type={2}", Name, Extension, Type);
        }

        public void RegisterFactory()
        {
            Type provider = null;
            try
            {
                provider = System.Type.GetType(Type);
            }
            catch (Exception exc)
            {

				Trace.WriteLineIf(DataSet.TraceDataSet.TraceError, "DataSet factory: unable to load factory type " + Type + ": " + exc.Message);
                return;
            }
            if (provider == null)
            {
				Trace.WriteLineIf(DataSet.TraceDataSet.TraceError, "DataSet factory: type " + Type + " is not found.");
                return;
            }
            try
            {
                if (!String.IsNullOrEmpty(Name))
                    DataSetFactory.Register(Name, provider);
                else if (!String.IsNullOrEmpty(Extension))
                    DataSetFactory.RegisterExtension(Extension, provider);
                else
                    DataSetFactory.Register(provider);
            }
            catch (Exception exc)
            {
				Trace.WriteLineIf(DataSet.TraceDataSet.TraceError, "DataSet factory " + this.ToString() + ": unable register factory: " + exc.Message);
            }
        }
    }
}

