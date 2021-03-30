// Copyright © 2010 Microsoft Corporation, All Rights Reserved.
// This code released under the terms of the Microsoft Research License Agreement (MSR-LA, http://sds.codeplex.com/License)
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace Microsoft.Research.Science.Data.Factory
{
    /// <summary>Class that corresponds to root section for DataSet factories configuration. 
    /// Typically you don't need to create instances of this class. It is mostly used
    /// by DataSet factories infrastructure</summary>
    internal class FactoryConfigurationSection : ConfigurationSection
    {
        /// <summary>Gets collection of configured factories</summary>
        [ConfigurationProperty("factories", IsDefaultCollection = true),
         ConfigurationCollection(typeof(FactoryConfigurationElementCollection))]
        public FactoryConfigurationElementCollection Factories
        {
            get { return (FactoryConfigurationElementCollection)base["factories"]; }
        }
    }
}

