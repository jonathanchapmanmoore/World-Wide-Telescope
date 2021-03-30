using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.Prism.Events;
using Microsoft.Research.ScientificWorkflow.NetCDF.CSharpAPI;

namespace Renci.Wwt.DataManager.NetCDF.Events
{
    public class NetCDFFileSelectedEvent : CompositePresentationEvent<NetCDFReader>
    {
    }
}
