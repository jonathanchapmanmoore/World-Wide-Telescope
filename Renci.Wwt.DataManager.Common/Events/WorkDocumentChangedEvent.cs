using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.Prism.Events;
using Renci.Wwt.DataManager.Common.Models;

namespace Renci.Wwt.DataManager.Common.Events
{
    public class WorkDocumentChangedEvent : CompositePresentationEvent<WorkDocument>
    {
    }    
}
