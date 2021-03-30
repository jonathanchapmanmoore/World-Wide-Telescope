using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using Microsoft.Practices.Prism.Interactivity.InteractionRequest;
using System.Windows.Interactivity;
using System.Windows.Controls;
using Renci.Wwt.DataManager.Common.Framework;
using Renci.Wwt.DataManager.Common.BaseClasses;

namespace Renci.Wwt.DataManager.Common
{
    public class DialogWindowAction : TriggerAction<FrameworkElement>
    {
        public Type WindowDataType
        {
            get { return (Type)GetValue(WindowDataTypeProperty); }
            set { SetValue(WindowDataTypeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for WindowDataType.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty WindowDataTypeProperty =
            DependencyProperty.Register("WindowDataType", typeof(Type), typeof(DialogWindowAction));

        /// <summary>
        /// Displays the child window and collects results for <see cref="IInteractionRequest"/>.
        /// </summary>
        /// <param name="parameter">The parameter to the action. If the action does not require a parameter, the parameter may be set to a null reference.</param>
        protected override void Invoke(object parameter)
        {
            var args = parameter as InteractionRequestedEventArgs;
            if (args == null)
            {
                return;
            }

            //  Find window owner
            var parentWindow = this.AssociatedObject as Window;
            while (parentWindow == null && this.AssociatedObject.Parent != null)
            {
                parentWindow = this.AssociatedObject.Parent as Window;
            }

            if (args.Context is DialogWindowPopup)
            {
                var dialogWindowRequest = args.Context as DialogWindowPopup;

                var window = Activator.CreateInstance(this.WindowDataType) as Window;

                //  Set window owner
                window.Owner = parentWindow as Window;

                ViewModelBinder.Bind(dialogWindowRequest.Content, window);

                var viewModel = dialogWindowRequest.Content as ViewModelBase;
                if (viewModel != null)
                {
                    EventHandler h = null;
                    h = delegate(object sender, EventArgs e)
                    {
                        viewModel.Closing -= h;
                        window.Close();
                    };

                    viewModel.Closing += h;
                }

                var callback = args.Callback;
                EventHandler handler = null;
                handler =
                    (o, e) =>
                    {
                        window.Closed -= handler;
                        callback();
                    };
                window.Closed += handler;

                if (dialogWindowRequest.IsModal)
                {
                    window.Show();

                }
                else
                {
                    window.Show();
                }
            }
            else if (args.Context is MessageDialogBox)
            {
                var message = args.Context as MessageDialogBox;

                var icon = MessageBoxImage.None;
                switch (message.Type)
                {
                    case MessageDialogBoxTypes.None:
                        icon = MessageBoxImage.None;
                        break;
                    case MessageDialogBoxTypes.Information:
                        icon = MessageBoxImage.Information;
                        break;
                    case MessageDialogBoxTypes.Error:
                        icon = MessageBoxImage.Error;
                        break;
                    default:
                        break;
                }

                MessageBox.Show(parentWindow, message.Content.ToString(), message.Title, MessageBoxButton.OK, icon);
            }
            else if (args.Context is Confirmation)
            {
                var confirmation = args.Context as Confirmation;

                confirmation.Confirmed = MessageBox.Show(parentWindow, confirmation.Content.ToString(), confirmation.Title, MessageBoxButton.OKCancel) == MessageBoxResult.OK;
            }
            else if (args.Context is Notification)
            {
                var notification = args.Context as Notification;

                MessageBox.Show(parentWindow, notification.Content.ToString(), notification.Title);
            }
            else
            {
                throw new NotSupportedException();
            }
        }
    }
}
