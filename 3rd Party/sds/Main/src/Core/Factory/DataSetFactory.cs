// Copyright © 2010 Microsoft Corporation, All Rights Reserved.
// This code released under the terms of the Microsoft Research License Agreement (MSR-LA, http://sds.codeplex.com/License)
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;
using System.Configuration;

namespace Microsoft.Research.Science.Data.Factory
{
    /// <summary>Facilitates creating a <see cref="DataSet"/> instance from a URI.</summary>
    /// <remarks>
    /// <para>
    /// <see cref="DataSetFactory"/> is designated to enable creating a <see cref="DataSet"/> instance for a
    /// particular provider from a special URI (see <see cref="DataSetUri"/>).
    /// </para>
    /// <example>
    /// These examples demonstrate how to open datasets of different provider type
    /// from URI.
    /// <code>
    /// // If the URI is a simple path to a file, a provider can be determined by the extension: 
    /// DataSet dataSet = DataSet.Open("csv_for_autotypes.csv"); // ".csv" for CsvDataSet
    /// 
    /// // If the URI has a schema "msds", a provider name is specified after a colon:
    /// DataSet dataSet = DataSet.Open("msds:csv?file=csv_for_autotypes.csv");  // "csv" for CsvDataSet
    ///  
    /// DataSet dataSet = DataSet.Open(@"c:\data\ncfile.nc"); // ".nc" for NetCDFDataSet
    /// 
    /// DataSet dataSet = DataSet.Open("msds:nc?file=ncfile.nc");  // "nc" for NetCDFDataSet
    /// 
    /// // URI can contain parameters specific for a particular provider:
    /// DataSet dataSet = DataSet.Open("msds:as?server=(local)&amp;database=ActiveStorage&amp;integrated security=true&amp;GroupName=mm5&amp;UseNetcdfConventions=true");
    /// </code>
    /// </example>
    /// <para>
    /// A path also may contain parameters specified at the end after '?' (if the
    /// provider associated with the extension supports this feature).
    /// For example, the code:
    /// <code>
    /// DataSet ds  = DataSet.Open(@"c:\data\air0.csv?openMode=open&amp;fillUpMissingValues=true&amp;inferInt=true");
    /// </code>
    /// does the same as the code:
    /// <code>
    /// DataSet ds = DataSet.Open(@"msds:csv?file=c:\data\air0.csv&amp;openMode=open&amp;fillUpMissingValues=true&amp;inferInt=true");
    /// </code>
    /// </para>
    /// <para>
    /// The <see cref="DataSetUri"/> type enables a customization of a URI prior to creating a
    /// DataSet. See <see cref="CreateUri(string)"/> for details.
    /// </para>
    /// <para>
    /// Any DataSet provider must be registered in the factory first to enable its instances 
    /// to be created by the <see cref="Create(string)"/> method.
    /// Attribute <see cref="Microsoft.Research.Science.Data.DataSetProviderNameAttribute"/>,
    /// applied to a DataSet provider type, associate it with a provider name.
    /// Attribute <see cref="Microsoft.Research.Science.Data.DataSetProviderFileExtensionAttribute"/>
    /// associates a provider type with one or severial extensions.
    /// Attribute <see cref="Microsoft.Research.Science.Data.DataSetProviderUriTypeAttribute"/>
    /// associates a provider type with a type derived from the <see cref="DataSetUri"/>
    /// that provides a customization of its URI.
    /// </para>
    /// <para>
    /// The factory control class is a static class 
    /// <see cref="Microsoft.Research.Science.Data.Factory.DataSetFactory"/>. 
    /// The class allows registering of providers and creating new instances. 
    /// The class is used by the <see cref="Microsoft.Research.Science.Data.DataSet.Open(string)"/> 
    /// and <see cref="DataSetUri.Create(string)"/> methods.
    /// </para>
    /// <para>
    /// There are three ways to register providers. First is a DataSetFactory.Register group of methods. 
    /// These methods allow both to register provider names and associate extensions 
    /// to a particular provider. The methods intensively use mentioned attributes 
    /// <see cref="Microsoft.Research.Science.Data.DataSetProviderNameAttribute"/> and 
    /// <see cref="Microsoft.Research.Science.Data.DataSetProviderFileExtensionAttribute"/> 
    /// in the process.
    /// </para>
    /// <example>
    /// <para>For example, the <see cref="T:Microsoft.Research.Science.Data.NetCDF4.NetCDFDataSet"/> 
    /// provider can be registered in this way:</para>
    /// <code>
    /// DataSetFactory.Register(typeof(NetCDFDataSet));
    /// </code>
    /// <para>or this:</para>
    /// <code>
    /// DataSetFactory.RegisterAssembly("Microsoft.Research.Science.Data.NetCDF4.dll");
    /// </code>
    /// </example>
    /// <para>
    /// The second way to register providers is the method <see cref="SearchFolder"/> 
    /// that accepts a path and makes a search in that folder for all assemblies 
    /// containing providers and registers found providers. The method works only
    /// for code compiled for DEBUG configuration. This makes it convenient for
    /// development on a machine without Scientific DataSet runtime installation.
    /// <example>
    /// <para>In the following example all providers found in assemblies in 
    /// the current directory are registered:
    /// </para>
    /// <code>
    /// DataSetFactory.SearchFolder(Environment.CurrentDirectory);
    /// </code>
    /// </example>
    /// </para>
    /// <para>Third way to register provider is to add registration entry to configuration file. 
    /// <example>
    /// <para>Example of configuration part is given below</para>
    /// <code>
    /// &lt;?xml version="1.0" encoding="utf-8" ?&gt;
    /// &lt;configuration&gt;
    ///   &lt;!-- Declare configuration section named Microsoft.Research.Science.Data --&gt;
    ///   &lt;configSections&gt;
    ///     &lt;section name="Microsoft.Research.Science.Data" type="Microsoft.Research.Science.Data.Factory.FactoryConfigurationSection, Microsoft.Research.Science.Data" /&gt;
    ///     &lt;!-- Declarations of other config sections --&gt;
    ///   &lt;/configSections&gt;
    ///   &lt;Microsoft.Research.Science.Data&gt;
    ///        &lt;factories&gt;
    ///          &lt;!-- Register factory by name --&gt;
    ///          &lt;add name="myProvider" type="MyNamespace.DbxFactory, MyAssembly"/&gt;
    ///          &lt;!-- Register factory by extension --&gt; 
    ///          &lt;add ext=".dat" type="MyNamespace.DatProvider, MyAssembly"/&gt;
    ///        &lt;/factories&gt;
    ///     &lt;/Microsoft.Research.Science.Data&gt;
    /// &lt;/configuration>
    /// </code>
    /// </example>
    /// </para>
    /// <para>Registration by name is similar to invocation of 
    /// <see cref="DataSetFactory.Register(string,Type)"/>.
    /// Registration by extension is similar to invocation of
    /// <see cref="DataSetFactory.RegisterExtension(string,Type)"/>.
    /// </para>
    /// <para>DataSet runtime installer adds registration entries to Machine.config files for 
    /// <see cref="T:Microsoft.Research.Science.Data.NetCDF4.NetCDFDataSet"/> (with provider name <c>"nc"</c>), 
    /// <see cref="T:Microsoft.Research.Science.Data.CSV.CsvDataSet"/> (with provider name <c>"csv"</c>), 
    /// <see cref="T:Microsoft.Research.Science.Data.Memory.MemoryDataSet"/> (with provider name <c>"memory"</c>), 
    /// <see cref="T:Microsoft.Research.Science.Data.Proxy.WCF.RemoteDataSetFactory"/>
    /// (with provider name <c>"wcf"</c>) and
    /// <see cref="T:Microsoft.Research.Science.Data.Proxy.Remoting.RemoteDataSetFactory"/>
    /// (with provider name <c>"remoting"</c>). So
    /// it is not necessary to register this classes in application code and also 
    /// makes it possible to instantiate WCF and Remoting proxies 
    /// via <see cref="DataSetFactory.Create(string)"/> method.
    /// </para>
    /// <example>
    /// The following example creates a proxy connected to the service through the WCF channel
    /// (please refer documentation for 
    /// <see cref="T:Microsoft.Research.Science.Data.Proxy.ProxyDataSet">Microsoft.Research.Science.Data.Proxy.ProxyDataSet</see>):
    /// <code>
    /// // Creating a storage service for WCF:
    /// StorageService storage = new StorageService(taskQueue, dataSet);
    /// StorageServiceHost host = new StorageServiceHost(taskQueue, storage.MainPort);
    /// 
    /// // Creating proxy:
    /// DataSet proxy = DataSet.Open("msds:wcf?uri=" + host.Uri); 
    /// </code>
    /// </example>
    /// </remarks>
    /// <seealso cref="Register(Type)"/>
    /// <seealso cref="RegisterExtension(string, Type)"/>
    /// <seealso cref="RegisterAssembly(Assembly)"/>
    /// <seealso cref="SearchFolder(string)"/>
    /// <seealso cref="Create(string)"/>
    /// <seealso cref="CreateUri(string)"/>
    /// <seealso cref="T:Microsoft.Research.Science.Data.NetCDF4.NetCDFDataSet"/>
    /// <seealso cref="T:Microsoft.Research.Science.Data.Proxy.WCF.RemoteDataSetFactory"/>
    public static class DataSetFactory
    {
        /// <summary>
        /// Tables of registered providers. Key is the provider name.
        /// </summary>
        private static readonly Dictionary<string, DataSetFactoryEntry> providersByName =
            new Dictionary<string, DataSetFactoryEntry>();
        /// <summary>
        /// Tables of registered providers. Key is the acceptable file extension.
        /// </summary>
        private static readonly Dictionary<string, List<DataSetFactoryEntry>> providersByExt =
            new Dictionary<string, List<DataSetFactoryEntry>>();

        static DataSetFactory()
        {
            // Check behaviour of AddrOfPinnedObject
            double[,] testArr = new double[2, 2] { { 11, 12 }, { 13, 14 } };
            var gcHandle = System.Runtime.InteropServices.GCHandle.Alloc(testArr, System.Runtime.InteropServices.GCHandleType.Pinned);
            try
            {
                IntPtr ptrPinned = gcHandle.AddrOfPinnedObject();
                unsafe
                {
                    double* pDbl = (double*)ptrPinned;
                    if (*pDbl != 11)
                    {
                        Trace.WriteLine("Current platform is not supported for GCHandle.AddrOfPinnedObject behaves unexpectedly");
                        throw new NotSupportedException("Current platform is not supported for GCHandle.AddrOfPinnedObject behaves unexpectedly");
                    }
                }
            }
            finally
            {
                gcHandle.Free();
            }

            LoadConfiguration();
        }

        /// <summary>
        /// Gets an array of providers names registered in the factory.
        /// </summary>
        public static string[] GetRegisteredProviders()
        {
            if (providersByName.Count == 0)
                return new string[0];

            string[] names = new string[providersByName.Count];
            int i = 0;
            foreach (var item in providersByName)
                names[i++] = DataSetUri.DataSetUriScheme + ":" + item.Key;
            return names;
        }

        /// <summary>
        /// Gets an array of extensions (".ext") for the given provider name.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static string[] GetExtensionsForProvider(string name)
        {
            if (name == null)
                throw new ArgumentNullException("name");
            if (providersByName.Count == 0)
                return new string[0];
            if (DataSetUri.IsDataSetUri(name))
                name = new DataSetUri(name).ProviderName;
            if (providersByName.ContainsKey(name))
            {
                var entry = providersByName[name];

                List<string> extensions = new List<string>();
                Type dst = entry.DataSetType;
                foreach (var v in providersByExt)
                {
                    if (v.Value.Exists(p => p.DataSetType == dst))
                    {
                        extensions.Add(v.Key);
                    }
                }
                return extensions.ToArray();
            }
            return new string[0];
        }

        /// <summary>Tries to load configuration section that is handled by current
        /// version of DataSet core libraries</summary>
        /// <returns>Loaded section or null of no section is found</returns>
        private static FactoryConfigurationSection GetFactoryConfigurationSection()
        {
            string sectionTypeName = typeof(FactoryConfigurationSection).AssemblyQualifiedName;
            Configuration config =
                ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.PerUserRoamingAndLocal);
            for (int i = 0; i < config.Sections.Count; i++)
                try
                {
                    if (config.Sections[i].SectionInformation.Type == sectionTypeName)
                    {
                        Trace.WriteLineIf(DataSet.TraceDataSet.TraceInfo,
                            "Found configuration section " + config.Sections[i].SectionInformation.Name);
                        return (FactoryConfigurationSection)config.Sections[i];
                    }
                }
                catch (Exception exc)
                {
                    Trace.WriteLineIf(DataSet.TraceDataSet.TraceError,
                        "Error loading cofiguration section: " + exc.Message);
                }
            Trace.WriteLineIf(DataSet.TraceDataSet.TraceInfo,
                "No configuration section for Scientific DataSet is found");
            return null;
        }

        /// <summary>Registers all providers from configuration file. Information is taken both 
        /// from local configuration file (if exists) and Machine.config</summary> 
        public static void LoadConfiguration()
        {
            try
            {
                FactoryConfigurationSection dsfc = GetFactoryConfigurationSection();
                if (dsfc != null)
                    foreach (FactoryConfigurationElement e in dsfc.Factories)
                        e.RegisterFactory();
            }
            catch (Exception failure)
            {
                Trace.WriteLineIf(DataSet.TraceDataSet.TraceError, "Error loading application config section: " + failure.Message);
            }
        }

        /// <summary>
        /// Registers the file extension acceptable by the provider <paramref name="dataSetProvider"/>.
        /// </summary>
        /// <param name="fileExtension">Acceptable extension (including ".", e.g. ".dat").</param>
        /// <param name="dataSetProvider">The provider type to associate with the extension. 
        /// This type should be
        /// either derived from <see cref="Microsoft.Research.Science.Data.DataSet"/> and has
        /// public constructor accepting string or implement 
        /// <see cref="Microsoft.Research.Science.Data.Factory.IDataSetFactory"/></param>
        /// <returns>Returns true if the extension is registered.</returns>
        /// <remarks>		
        /// </remarks>
        /// <exception cref="ArgumentException" >Type <paramref name="dataSetProvider"/> is not a subclass of the <see cref="DataSet"/> class. -- or --
        /// Type <paramref name="dataSetProvider"/> has no public constructor accepting the URI. -- and --
        /// Type <paramref name="dataSetProvider"/> does not implement <see cref="Microsoft.Research.Science.Data.Factory.IDataSetFactory"/>
        /// interface.</exception>
        /// <seealso cref="RegisterExtension(Type)"/>
        /// <seealso cref="Register(Type)"/>
        /// <seealso cref="RegisterAssembly(Assembly)"/>
        /// <seealso cref="SearchFolder(string)"/>
        /// <seealso cref="Microsoft.Research.Science.Data.DataSetUri"/>
        public static bool RegisterExtension(string fileExtension, Type dataSetProvider)
        {
            if (fileExtension == null)
                throw new ArgumentNullException("fileExtension");

            var e = new DataSetFactoryEntry(fileExtension, dataSetProvider);

            List<DataSetFactoryEntry> list;
            if (!providersByExt.TryGetValue(fileExtension, out list))
            {
                list = new List<DataSetFactoryEntry>();
                providersByExt.Add(fileExtension, list);
            }
            else
            {
                if (list.Exists(p => p.DataSetType == dataSetProvider && p.Name == fileExtension))
                {
                    Debug.WriteLineIf(DataSet.TraceDataSet.TraceVerbose, "DataSetFactory: file extension " + fileExtension +
                        " for the provider " + dataSetProvider.Name + " is already registered.");
                    return true;
                }
            }

            list.Add(e);
            Trace.WriteLineIf(DataSet.TraceDataSet.TraceInfo, "DataSetFactory: provider for the extension " + fileExtension + " is registered");
            return true;
        }

        /// <summary>
        /// Registers file extensions acceptable by the provider <paramref name="dataSetProvider"/>.
        /// </summary>
        /// <param name="dataSetProvider">The provider type to associate with the extension.</param>
        /// <returns>Returns true if the extensions are registered.</returns>
        /// <exception cref="ArgumentException" >
        /// Type <paramref name="dataSetProvider"/> is not a subclass of the <see cref="DataSet"/> class. -- or --
        /// Type <paramref name="dataSetProvider"/> has no public constructor accepting the URI.
        /// </exception>
        /// <seealso cref="RegisterExtension(Type)"/>
        /// <seealso cref="Register(Type)"/>
        /// <seealso cref="Microsoft.Research.Science.Data.DataSetUri"/>
        private static bool RegisterExtension(Type dataSetProvider)
        {
            var attrs = dataSetProvider.GetCustomAttributes(typeof(DataSetProviderFileExtensionAttribute), false);
            if (attrs.Length == 0)
                return false;

            bool res = true;
            foreach (var a in attrs)
            {
                DataSetProviderFileExtensionAttribute nameAttr = (DataSetProviderFileExtensionAttribute)a;
                res = res && RegisterExtension(nameAttr.FileExtension, dataSetProvider);
            }

            return res;
        }

        /// <summary>
        /// Registers the <paramref name="providerName"/> and associates it with the provider <paramref name="dataSetProvider"/>.
        /// </summary>
        /// <param name="providerName">Provider name.</param>
        /// <param name="dataSetProvider">The provider type to associate with the provider name. </param>
        /// <returns>Returns true if the provider is registered.</returns>
        /// <remarks>
        /// If the name <paramref name="providerName"/> is already registered in the factory
        /// the registration for the provider will be silently skipped and the method will return false.
        /// </remarks>
        /// <exception cref="ArgumentException" />
        /// <exception cref="ArgumentNullException">
        /// Type <paramref name="dataSetProvider"/> is not a subclass of the <see cref="DataSet"/> class. -- or --
        /// Type <paramref name="dataSetProvider"/> has no public constructor accepting the URI. -- and --
        /// Type <paramref name="dataSetProvider"/> does not implement <see cref="Microsoft.Research.Science.Data.Factory.IDataSetFactory"/>
        /// interface.</exception>
        /// <seealso cref="Register(Type)"/>
        /// <seealso cref="Register(string,string)"/>
        /// <seealso cref="RegisterExtension(Type)"/>
        /// <seealso cref="Microsoft.Research.Science.Data.DataSetUri"/>
        public static bool Register(string providerName, Type dataSetProvider)
        {
            if (dataSetProvider == null)
                throw new ArgumentNullException("dataSetProvider");

            if (providersByName.ContainsKey(providerName))
            {
                Debug.WriteLineIf(DataSet.TraceDataSet.TraceVerbose, "DataSetFactory: provider name " + providerName + " is already registered.");
                return true;
            }

            providersByName.Add(providerName, new DataSetFactoryEntry(providerName, dataSetProvider));
            Trace.WriteLineIf(DataSet.TraceDataSet.TraceInfo, "DataSetFactory: provider " + providerName + " is registered");
            return true;
        }

        /// <summary>
        /// Registers the <paramref name="providerName"/> and associates it with the provider <paramref name="dataSet"/>.
        /// </summary>
        /// <param name="providerName">Provider name.</param>
        /// <param name="providerTypeName">The provider type name to associate with the provider name. This type should be
        /// either derived from <see cref="Microsoft.Research.Science.Data.DataSet"/> and has
        /// public constructor accepting string or implement 
        /// <see cref="Microsoft.Research.Science.Data.Factory.IDataSetFactory"/></param>
        /// <returns>Returns true if the provider is registered.</returns>
        /// <remarks>
        /// If the name <paramref name="providerName"/> is already registered in the factory
        /// the registration for the provider will be silently skipped and the method will return false.
        /// </remarks>
        /// <exception cref="ArgumentException" />
        /// <exception cref="ArgumentNullException">
        /// Type <paramref name="providerTypeName"/> is not a subclass of the <see cref="DataSet"/> class. -- or --
        /// Type <paramref name="providerTypeName"/> has no public constructor accepting the URI. -- and --
        /// Type <paramref name="providerTypeName"/> does not implement <see cref="Microsoft.Research.Science.Data.Factory.IDataSetFactory"/>
        /// interface.</exception>
        /// <seealso cref="Register(Type)"/>
        /// <seealso cref="Register(string,Type)"/>
        /// <seealso cref="RegisterExtension(Type)"/>
        /// <seealso cref="Microsoft.Research.Science.Data.DataSetUri"/>
        public static bool Register(string providerName, string providerTypeName)
        {
            if (providersByName.ContainsKey(providerName))
            {
                Debug.WriteLineIf(DataSet.TraceDataSet.TraceVerbose, "DataSetFactory: provider name " + providerName + " is already registered.");
                return true;
            }
            Type provider = null;
            try
            {
                provider = System.Type.GetType(providerTypeName);
            }
            catch (Exception exc)
            {
                throw new ArgumentException("Unable to create type with name " + providerTypeName, exc);
            }

            return Register(providerName, provider);
        }

        /// <summary>
        /// Registers the <paramref name="providerName"/> and associates it with the provider constructor <paramref name="ctorFunc"/>.
        /// </summary>
        /// <param name="providerName">Provider name.</param>
        /// <param name="ctorFunc">Function to create DataSet from uri string</param>
        /// <returns>Returns true if the provider is registered.</returns>
        /// <remarks>
        /// If the name <paramref name="providerName"/> is already registered in the factory
        /// the registration for the provider will be silently skipped and the method will return false.
        /// </remarks>
        /// <seealso cref="Register(Type)"/>
        /// <seealso cref="RegisterExtension(Type)"/>
        /// <seealso cref="Microsoft.Research.Science.Data.DataSetUri"/>
        public static bool Register(string providerName, Func<string, DataSet> ctorFunc)
        {
            if (providersByName.ContainsKey(providerName))
            {
                Debug.WriteLineIf(DataSet.TraceDataSet.TraceVerbose, "DataSetFactory: provider name " + providerName + " is already registered.");
                return true;
            }

            providersByName.Add(providerName, new DataSetFactoryEntry(providerName, ctorFunc));
            Trace.WriteLineIf(DataSet.TraceDataSet.TraceInfo, "DataSetFactory: provider " + providerName + " is registered");
            return true;
        }


        /// <summary>
        /// Registers the provider <paramref name="dataSetProvider"/> in the factory.
        /// </summary>
        /// <param name="dataSetProvider">The provider type to register.</param>
        /// <returns>Returns true if the provider is registered.</returns>
        /// <remarks>
        /// <para>
        /// Registers the name of the provider and its acceptable extensions in the factory.
        /// </para>
        /// <para>
        /// The name of the provider is taken from its attribute <see cref="DataSetProviderNameAttribute"/>.
        /// If the provider type contains several attributes all the names specified will be pended for the provider to be
        /// registered with. If the type has no attribute of this type no exception will be thrown.
        /// If the name <paramref name="dataSetProvider"/> is already registered in the factory
        /// the registration for the provider will be silently skipped and the method will return false.
        /// </para>		
        /// <para>
        /// About extensions registering see remarks for the method <see cref="RegisterExtension(Type)"/>.
        /// </para>
        /// </remarks>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException">
        /// Type <paramref name="dataSetProvider"/> is not a subclass of the <see cref="DataSet"/> class. -- or --
        /// Type <paramref name="dataSetProvider"/> has no public constructor accepting the URI.</exception>
        /// <seealso cref="Register(string, Type)"/>
        /// <seealso cref="RegisterExtension(Type)"/>
        /// <seealso cref="RegisterAssembly(Assembly)"/>
        /// <seealso cref="SearchFolder(string)"/>
        /// <seealso cref="Microsoft.Research.Science.Data.DataSetUri"/>
        public static bool Register(Type dataSetProvider)
        {
            if (dataSetProvider == null)
                throw new ArgumentNullException("dataSetProvider");

            var attrs = dataSetProvider.GetCustomAttributes(typeof(DataSetProviderNameAttribute), false);
            if (attrs.Length == 0)
                return false;

            bool res = true;
            foreach (var a in attrs)
            {
                DataSetProviderNameAttribute nameAttr = (DataSetProviderNameAttribute)a;
                res = res && Register(nameAttr.Name, dataSetProvider);
            }

            res = res && RegisterExtension(dataSetProvider);
            return res;
        }

        /// <summary>
        /// Creates the <see cref="DataSet"/> instance for the specified <paramref name="uri"/>.
        /// </summary>
        /// <param name="uri">URI describing the DataSet to create.</param>
        /// <returns>New instance of the <see cref="DataSet"/>.</returns>
        /// <remarks>
        /// <para>The method creates a <see cref="DataSet"/> instance based on the <paramref name="uri"/>.
        /// If the uri contains schema definition (i.e. starts with <c>"msds:"</c>) the factory
        /// tries to find a corresponded provider type from the registered providers table
        /// (see method <see cref="Register(Type)"/>). If the provider is not registered,
        /// an exception will be thrown.</para>
        /// <para>If the uri has no schema definition, the string is considered as a path and
        /// the factory looks for provider by the extension of the given path
        /// (see method <see cref="RegisterExtension(Type)"/>). If there are more than one
        /// providers associated with the extension, they will be constructed one by one until
        /// the data set is created successfully.
        /// </para>
        /// <para>
        /// The path also may contain parameters specified at the end after '?' (if the
        /// provider associated with the extensions supports this feature).
        /// For example, the code:
        /// <code>
        /// DataSet ds  = DataSet.Open(@"c:\data\air0.csv?openMode=open&amp;fillUpMissingValues=true&amp;inferInt=true");
        /// </code>
        /// does the same as the code:
        /// <code>
        /// DataSet ds = DataSet.Open(@"msds:csv?file=c:\data\air0.csv&amp;openMode=open&amp;fillUpMissingValues=true&amp;inferInt=true");
        /// </code>
        /// </para>
        /// <para>
        /// If the <paramref name="uri"/> is null or an empty string, an instance of the 
        /// <see cref="T:Microsoft.Research.Science.Data.Memory.MemoryDataSet"/> is created.
        /// </para>
        /// <para>
        /// See also remarks for the <see cref="DataSetFactory"/> class.
        /// </para>
        /// </remarks>
        /// <seealso cref="Register(string, Type)"/>
        /// <seealso cref="Register(Type)"/>
        /// <seealso cref="RegisterExtension(Type)"/>
        /// <seealso cref="RegisterExtension(string, Type)"/>
        /// <seealso cref="RegisterAssembly(Assembly)"/>
        /// <seealso cref="SearchFolder(string)"/>
        /// <seealso cref="Microsoft.Research.Science.Data.DataSetUri"/>
        /// <exception cref="ProviderNotRegisteredException">
        /// DataSet is not registered in the factory.</exception>
        /// <exception cref="DataSetCreateException">
        /// Particular DataSet provider has thrown an exception at construction.</exception>
        /// <exception cref="ArgumentException" />
        /// <exception cref="InvalidOperationException" />
        public static DataSet Create(string uri)
        {
            if (String.IsNullOrEmpty(uri))
                uri = "msds:memory";

            if (uri.StartsWith(DataSetUri.DataSetUriScheme + ":"))
            {
                DataSetUri dsUri = new DataSetUri(uri);
                return Create(dsUri);
            }

            Trace.WriteLineIf(DataSet.TraceDataSet.TraceInfo, "DataSetFactory: creating " + uri + " . . . ");

            // Creating data set by extension
            string ext = GetExtension(uri);
            if (string.IsNullOrEmpty(ext))
                throw BuildDataSetCreateException(uri, "Extension is empty or null");
            if (!providersByExt.ContainsKey(ext))
                throw BuildProviderNotRegisteredException(uri, "Extension " + ext + " is not registered in the factory");
            var list = providersByExt[ext];
            if (list.Count == 0)
                throw BuildProviderNotRegisteredException(uri, "No providers associated with the extension " + ext);

            Exception refuseEx = null;
            foreach (var e in list)
            {
                try
                {
                    return (DataSet)e.Constructor.Invoke(new object[] { uri });
                }
                catch (TargetInvocationException invEx)
                {
                    refuseEx = invEx.InnerException == null ? invEx : invEx.InnerException;
                    Trace.WriteLineIf(DataSet.TraceDataSet.TraceWarning, "Provider " + e.DataSetType.Name + " refused the uri.");
                }
                catch (Exception ex)
                {
                    refuseEx = ex;
                    Trace.WriteLineIf(DataSet.TraceDataSet.TraceWarning, "Provider " + e.DataSetType.Name + " refused the uri.");
                }
            }
            throw BuildDataSetCreateException(uri, refuseEx);
        }

        /// <summary>
        /// Creates the <see cref="DataSet"/> instance for the specified <paramref name="uri"/>.
        /// </summary>
        /// <param name="uri">URI containing provider name and parameters.</param>
        /// <returns>New instance of the <see cref="DataSet"/>.</returns>
        /// <remarks>
        /// See remarks for <see cref="DataSet.Open(DataSetUri)"/>.</remarks>
        /// <seealso cref="Create(string)"/>
        /// <seealso cref="CreateUri(string)"/>
        public static DataSet Create(DataSetUri uri)
        {
            if (uri == null)
                throw new ArgumentNullException("uri");

            Trace.WriteLineIf(DataSet.TraceDataSet.TraceInfo, "DataSetFactory: creating " + uri + " . . . ");

            string provider = uri.ProviderName;
            if (String.IsNullOrEmpty(provider))
                throw BuildDataSetCreateException(uri.ToString(), "Provider name is empty or null");
            if (!providersByName.ContainsKey(provider))
                throw BuildProviderNotRegisteredException(uri.ToString(), "Provider " + provider + " is not registered");

            DataSetFactoryEntry entry = providersByName[provider];
            DataSet result = entry.CreateDataSet(uri.ToString());

            List<string> includedUris = uri.GetParameterValues("include");
            try
            {
                if (includedUris.Count > 0)
                {
                    foreach (string includedUri in includedUris)
                        result.IncludeDataSet(includedUri);
                    result.Commit();
                }
            }
            catch (DataSetCreateException)
            {
                throw;
            }
            catch (System.Reflection.TargetInvocationException invEx)
            {
                throw BuildDataSetCreateException(uri.ToString(), invEx.InnerException == null ? invEx : invEx.InnerException);
            }
            catch (Exception ex)
            {
                throw BuildDataSetCreateException(uri.ToString(), ex);
            }
            return result;
        }

        /// <summary>
        /// Gets the provider name that is associated with the extension.
        /// </summary>
        /// <param name="extension"></param>
        /// <returns>The provider name.</returns>
        public static string GetProviderNameByExtention(string extension)
        {
            if (String.IsNullOrEmpty(extension))
                throw new ArgumentNullException("extension");
            string[] cmp = extension.Split('.');
            if (cmp.Length > 0)
            {
                string ex = cmp[cmp.Length - 1];
                if (providersByExt.ContainsKey("." + ex))
                {
                    var entry = providersByExt["." + ex];
                    if ((entry != null) && (entry.Count > 0))
                    {
                        Type dst = entry[0].DataSetType;
                        foreach (var v in providersByName)
                        {
                            if (v.Value.DataSetType == dst)
                                return v.Key;
                        }
                        return null;
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// Gets the provider name that is associated with the type.
        /// </summary>
        /// <param name="providerType">Provider type.</param>
        /// <returns>The provider name, if the type found; or null, otherwise.</returns>
        public static string GetProviderNameByType(Type providerType)
        {
            if (providerType == null)
                throw new ArgumentNullException("providerType");
            foreach (var item in providersByName)
                if (item.Value.DataSetType == providerType)
                    return item.Key;
            return null;
        }

        /// <summary>
        /// Gets all provider names associated with the type.
        /// </summary>
        /// <param name="providerType">Provider type.</param>
        /// <returns>List of provider names, if the type found; or empty list, otherwise.</returns>
        public static List<string> GetProviderNamesByType(Type providerType)
        {
            if (providerType == null)
                throw new ArgumentNullException("providerType");
            List<string> names = new List<string>();
            foreach (var item in providersByName)
                if (item.Value.DataSetType == providerType)
                    names.Add(item.Key);
            return names;
        }

        /// <summary>
        /// Creates the <see cref="DataSetUri"/> instance for the <paramref name="providerType"/>.
        /// </summary>
        /// <param name="providerType">Type of the provider whose URI is to be created.</param>
        /// <returns><see cref="DataSetUri"/> instance.</returns>
        /// <remarks>See remarks for <see cref="DataSetUri.Create(string)"/>.</remarks>
        /// <seealso cref="CreateUri(string)"/>
        /// <seealso cref="DataSetUri.Create(Type)"/>
        /// <seealso cref="DataSet.Open(DataSetUri)"/>
        public static DataSetUri CreateUri(Type providerType)
        {
            if (providerType == null)
                throw new ArgumentNullException("providerType");
            var attrs = (DataSetProviderUriTypeAttribute[])providerType.GetCustomAttributes(typeof(DataSetProviderUriTypeAttribute), false);

            if ((attrs != null) && (attrs.Length > 0))
            {
                var attr2 = (DataSetProviderNameAttribute[])providerType.GetCustomAttributes(typeof(DataSetProviderNameAttribute), false);
                if (attr2 != null && attr2.Length > 0)
                {
                    string uri = DataSetUri.DataSetUriScheme + ":" + attr2[0].Name;
                    return (DataSetUri)attrs[0].UriType.GetConstructor(new Type[] { typeof(string) }).Invoke(new object[] { uri });
                }
            }

            throw new ArgumentException("DataSetUri can't be built from " + providerType + " type for it has no required attributes.");
        }

        /// <summary>
        /// Creates the <see cref="DataSetUri"/> instance for the specified <paramref name="uri"></paramref>.
        /// </summary>
        /// <param name="uri">Uri containing provider name and probably parameters.</param>
        /// <returns><see cref="DataSetUri"/> instance.</returns>
        /// <remarks>
        /// Creates a class inhereted from DataSetUri. Method looks in registered providers for
        /// attribute <see cref="DataSetProviderUriTypeAttribute"/> and returns an appropriate class instance. 
        /// If such class is not found, throws an exception.
        /// <para>
        /// See remarks for <see cref="DataSetUri.Create(string)"/> for details.</para>
        /// </remarks>
        /// <seealso cref="CreateUri(Type)"/>
        /// <seealso cref="DataSetUri.Create(string)"/>
        /// <seealso cref="DataSet.Open(DataSetUri)"/>
        public static DataSetUri CreateUri(string uri)
        {
            Debug.WriteLine("DataSetFactory: creating " + uri + " . . . ");

            if (String.IsNullOrEmpty(uri))
                uri = "msds:memory";

            if (uri.StartsWith(DataSetUri.DataSetUriScheme + ":"))
            {
                DataSetUri dsUri = new DataSetUri(uri);
                string provider = dsUri.ProviderName;

                if (!providersByName.ContainsKey(provider))
                    throw BuildProviderNotRegisteredException(uri, "Provider " + provider + " is not registered");

                DataSetFactoryEntry entry = providersByName[provider];
                DataSetProviderUriTypeAttribute[] v = (DataSetProviderUriTypeAttribute[])entry.DataSetType.GetCustomAttributes(typeof(DataSetProviderUriTypeAttribute), false);

                if ((v != null) && (v.Length > 0))
                {
                    return (DataSetUri)v[0].UriType.GetConstructor(new Type[] { typeof(string) }).Invoke(new object[] { uri });
                }
                throw new ArgumentException("DataSetUri can't be built from " + uri + " uri for related provider type has no proper attributes");
            }

            // Creating data set by extension or provider name
            string providerName;
            string tmp; List<string> param = new List<string>();
            if (uri.Contains("?"))
            {
                param.AddRange(uri.Split('?'));
                tmp = param[0];
                param.RemoveAt(0);
            }
            else tmp = uri;
            if (uri.Contains(".") || uri.Contains("?"))
            {
                providerName = GetProviderNameByExtention(tmp);
                if (String.IsNullOrEmpty(providerName))
                    throw new ArgumentException("DataSetUri can't be built from path " + uri + ": extension is not registered");
                string newUri = DataSetUri.DataSetUriScheme + ":" + providerName;
                if ((uri.Contains(".") || uri.Contains("?")) && (!uri.StartsWith(".")))
                    newUri = newUri + "?file=" + tmp;
                if (param.Count > 0)
                    newUri = newUri + "&" + param[0];

                if (!providersByName.ContainsKey(providerName))
                    throw BuildProviderNotRegisteredException(uri, "Provider " + providerName + " is not registered");


                DataSetFactoryEntry entry = providersByName[providerName];
                DataSetProviderUriTypeAttribute[] v = (DataSetProviderUriTypeAttribute[])entry.DataSetType.GetCustomAttributes(typeof(DataSetProviderUriTypeAttribute), false);

                if ((v != null) && (v.Length > 0))
                {
                    return (DataSetUri)
                    v[0].UriType.GetConstructor(new Type[] { typeof(string) }).Invoke(new object[] { newUri });
                }

                throw new ArgumentException("DataSetUri can't be built from " + uri + " uri for provider type has no proper attributes");
            }
            throw new ArgumentException("DataSetUri can't be build from " + uri + " uri for it is incorrect");
        }

        /// <summary>
        /// Gets an extension (".ext") from the path with support of appended
        /// parameters through "?".
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private static string GetExtension(string path)
        {
            int i = path.IndexOf('?');
            if (i < 0)
                return Path.GetExtension(path);
            return Path.GetExtension(path.Substring(0, i));
        }

        /// <summary>
        /// Searches the given file or folder for assemblies containg data set providers 
        /// and registers found providers.
        /// </summary>
        /// <param name="path">Path to the folder or an assembly file to search in.</param>
        /// <remarks>
        /// <para>If path specifies a folder, all files with extension ".dll" will be considered.</para>
        /// <para>The method compiles with condition <c>"DEBUG"</c> only.</para>
        /// </remarks>
        /// <seealso cref="Register(Type)"/>
        /// <seealso cref="RegisterAssembly(Assembly)"/>
        /// <exception cref="ArgumentNullException"/>
        /// <exception cref="ArgumentException"/>
        [Conditional("DEBUG")]
        public static void SearchFolder(string path)
        {
            if (path == null)
                throw new ArgumentNullException("path");

            int n = 0;
            if (File.Exists(path))
            {
                n = RegisterAssembly(path);
            }
            else if (Directory.Exists(path))
            {
                foreach (var file in Directory.GetFiles(path, "*.dll"))
                {
                    n += RegisterAssembly(file);
                }
            }
            else
            {
                throw new ArgumentException("Invalid path: file or directory doesn't exist");
            }
        }


        /// <summary>
        /// Registers all providers found in the given assembly.
        /// </summary>
        /// <param name="fileName">The assembly to look for providers in.</param>
        /// <remarks>The method throws any exception only on loading of the assembly,
        /// but not after that. Even if there is
        /// a problem during the registration it silently returns total number
        /// of successfully registered providers.</remarks>
        /// <returns>Number of successfully registered providers.</returns>
        /// <seealso cref="Register(Type)"/>
        /// <seealso cref="RegisterAssembly(Assembly)"/>
        /// <seealso cref="SearchFolder(string)"/>
        public static int RegisterAssembly(string fileName)
        {
            if (String.Compare(Path.GetExtension(fileName), ".dll", true) != 0)
                return 0;

            int n = 0;
            try
            {
                Assembly a = Assembly.LoadFrom(fileName);
                n += RegisterAssembly(a);
            }
            catch (Exception ex)
            {
                Trace.WriteLineIf(DataSet.TraceDataSet.TraceError, "Registering assembly failed: " + ex.Message);
            }
            return n;
        }

        /// <summary>
        /// Registers all providers found in the given assembly.
        /// </summary>
        /// <param name="assembly">The assembly to look for providers in.</param>
        /// <remarks>The method doesn't throw any exception even if there is
        /// a problem during the registration and silently returns total number
        /// of successfully registered providers.</remarks>
        /// <returns>Number of sucessfully registered providers.</returns>
        /// <seealso cref="RegisterAssembly(string)"/>
        /// <seealso cref="Register(Type)"/>
        /// <seealso cref="SearchFolder(string)"/>
        public static int RegisterAssembly(Assembly assembly)
        {
            int n = 0;
            Type typeDataSet = typeof(DataSet);
            Type typeDataSetFactory = typeof(IDataSetFactory);
            try
            {
                foreach (Type type in assembly.GetTypes())
                {
                    if (type.IsClass && !type.IsAbstract &&
                        (type.IsSubclassOf(typeDataSet) || typeDataSetFactory.IsAssignableFrom(type)))
                    {
                        if (Register(type))
                            n++;
                    }
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLineIf(DataSet.TraceDataSet.TraceError, "Registering assembly failed: " + ex.Message);
            }
            return n;
        }

        /// <summary>
        /// Represents the table of registered providers as a string.
        /// </summary>
        /// <returns>String with providers.</returns>
        public static string RegisteredToString()
        {
            StringBuilder sb = new StringBuilder();
            foreach (var p in providersByName.Keys)
            {
                Type dst = providersByName[p].DataSetType;
                sb.AppendFormat("Provider {0}: class {1} [{2}]", p, dst.Name,
                    dst.Assembly.GetName().Version.ToString(4));
                sb.AppendLine();
            }
            sb.AppendLine();
            foreach (var e in providersByExt.Keys)
            {
                sb.AppendFormat("Extension {0}: ", e);
                int i = 0;
                foreach (var p in providersByExt[e])
                {
                    if (i++ > 0) sb.Append(", ");
                    sb.Append(p.DataSetType.Name);
                }
                sb.AppendLine();
            }

            return sb.ToString();
        }

        /// <summary>
        /// Formats exception message based on uri, traces it and returns the exception.
        /// </summary>
        /// <param name="uri"></param>
        /// <param name="innerException"></param>
        internal static ProviderNotRegisteredException BuildProviderNotRegisteredException(string uri, string message)
        {
            ProviderNotRegisteredException exc = new ProviderNotRegisteredException(uri, message);
            Trace.WriteLineIf(DataSet.TraceDataSet.TraceError, exc.Message);
            return exc;
        }

        /// <summary>
        /// Formats exception message based on uri and innerException, traces it and returns the exception.
        /// </summary>
        /// <param name="uri"></param>
        /// <param name="innerException"></param>
        internal static DataSetCreateException BuildDataSetCreateException(string uri, Exception innerException)
        {
            string auxMessage = innerException == null ? null : innerException.Message;
            DataSetCreateException exc = new DataSetCreateException(uri, auxMessage, innerException);
            Trace.WriteLineIf(DataSet.TraceDataSet.TraceError, exc.Message);
            return exc;
        }

        /// <summary>
        /// Formats exception message based on uri and message, traces it and returns the exception.
        /// </summary>
        /// <param name="uri"></param>
        /// <param name="message"></param>
        internal static DataSetCreateException BuildDataSetCreateException(string uri, string message)
        {
            DataSetCreateException exc = new DataSetCreateException(uri, message);
            Trace.WriteLineIf(DataSet.TraceDataSet.TraceError, exc.Message);
            return exc;
        }
    }

    internal class DataSetFactoryEntry
    {
        private string name;
        private Type type;
        private ConstructorInfo ctor;
        private Func<string, DataSet> func;

        public DataSetFactoryEntry(string name, Func<string, DataSet> func)
        {
            this.name = name;
            this.func = func;
        }

        public DataSetFactoryEntry(string name, Type dataSetType)
        {
            this.name = name;
            this.type = dataSetType;

            if (typeof(IDataSetFactory).IsAssignableFrom(dataSetType))
            {
                func = uri =>
                {
                    IDataSetFactory factory = (IDataSetFactory)Activator.CreateInstance(dataSetType);
                    return factory.Create(uri);
                };
            }
            else if (dataSetType.IsSubclassOf(typeof(DataSet)))
            {
                // Checking whether the provider has the required ctor indeed.
                ctor = dataSetType.GetConstructor(new Type[] { typeof(string) });
                if (ctor == null)
                    throw new ArgumentException("The provider has no public constructor accepting a URI.");
            }
            else
                throw new ArgumentException("The provider is not a subclass of the DataSet nor implements IDataSetFactory.");
        }

        /// <summary>
        /// Gets the constructor of the provider accepting a URI.
        /// </summary>
        public ConstructorInfo Constructor { get { return ctor; } }

        public Func<string, DataSet> ConstructorFunc { get { return func; } }

        /// <summary>
        /// Gets the type of the provider associated with the name.
        /// </summary>
        public Type DataSetType { get { return type; } }

        /// <summary>
        /// Gets the name of the provider.
        /// </summary>
        public string Name { get { return name; } }

        /// <summary>
        /// Creates an istance of the stored DataSet provider with the given uri.
        /// </summary>
        /// <param name="uri"></param>
        /// <returns></returns>
        /// <exception cref="Microsoft.Research.Science.Data.DataSetCreateException">
        /// Particular DataSet provider has thrown an exception at construction.</exception>
        /// <exception cref="InvalidOperationException" />
        public DataSet CreateDataSet(string uri)
        {
            if (ctor != null)
                try
                {
                    return (DataSet)ctor.Invoke(new object[] { uri });
                }
                catch (System.Reflection.TargetInvocationException invEx)
                {
                    throw DataSetFactory.BuildDataSetCreateException(uri, invEx.InnerException == null ? invEx : invEx.InnerException);
                }
                catch (Exception ex)
                {
                    throw DataSetFactory.BuildDataSetCreateException(uri, ex);
                }
            else if (func != null)
                try
                {
                    return func(uri);
                }
                catch (System.Reflection.TargetInvocationException invEx)
                {
                    throw DataSetFactory.BuildDataSetCreateException(uri, invEx.InnerException == null ? invEx : invEx.InnerException);
                }
                catch (Exception ex)
                {
                    throw DataSetFactory.BuildDataSetCreateException(uri, ex);
                }

            throw DataSetFactory.BuildDataSetCreateException(uri, "Factory has no possible ways to create DataSet instance");
        }
    }


    /// <summary>Provides a method to create a <see cref="DataSet"/> from a URI.</summary>
    public interface IDataSetFactory
    {
        /// <summary>Creates a <see cref="DataSet"/> from the specified URI.</summary>
        /// <param name="uri">A URI in a format supported by this factory.</param>
        /// <returns>New DataSet object.</returns>
        DataSet Create(string uri);
    }
}

