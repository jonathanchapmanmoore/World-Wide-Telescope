using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.Prism.Interactivity.InteractionRequest;
using System.Windows;

namespace Renci.Wwt.DataManager.Common
{
    public enum MessageDialogBoxTypes
    {
        None,
        Information,
        Error
    }

    public class MessageDialogBox : Notification
    {
        public MessageDialogBoxTypes Type { get; set; }

        public MessageDialogBox(string title, string message)
        {
            this.Title = title;
            this.Content = message;
        }

        public MessageDialogBox(string title, string message, MessageDialogBoxTypes type)
            : this(title, message)
        {
            this.Type = type;
        }

    }
}
