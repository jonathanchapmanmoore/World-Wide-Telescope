using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Controls.Primitives;
using Microsoft.Practices.Prism.Regions.Behaviors;
using Microsoft.Practices.Prism.Regions;

namespace Renci.Wwt.DataManager.Common
{
    public static class RegionExtensions
    {
        public static void Hide(this IRegion region, string viewName)
        {
            Hide(region, region.GetView(viewName));
        }

        public static void Show(this IRegion region, string viewName)
        {
            Show(region, region.GetView(viewName));
        }


        public static void Show(this IRegion region, object view)
        {
            ChangeViewVisibility(region, view, Visibility.Visible);
        }

        public static void Hide(this IRegion region, object view)
        {
            ChangeViewVisibility(region, view, Visibility.Collapsed);
        }

        private static void ChangeViewVisibility(IRegion region, object view, Visibility visibility)
        {
            // Validate that the region is attached to an ItemsControl
            ItemsControl attachedControl = GetAttachedControl(region) as ItemsControl;
            if (attachedControl == null)
                throw new ArgumentException("The region must be attached to an ItemsControl");

            // Validate that the view is inside the region
            if (!region.Views.Contains(view))
                throw new ArgumentException("The view is not in the region");

            // Get the ItemContainer 
            UIElement viewContainer = attachedControl.ItemContainerGenerator.ContainerFromItem(view) as UIElement;
            //UIElement viewContainer = view as UIElement;
            if (viewContainer != null)
            {
                // show / hide the ItemContainer
                viewContainer.Visibility = visibility;

                // show/ hide the view
                UIElement viewElement = view as UIElement;
                viewElement.Visibility = visibility;

                // activate other view if necessary 
                Selector selector = attachedControl as Selector;

                var selectedItem = selector.SelectedItem;
                var selectedValue = selector.SelectedValue;

                if (selector != null)
                {
                    //  Find fist visible item if any
                    var r = (from item in selector.Items.OfType<TabItem>()
                            where item.Visibility == Visibility.Visible
                            select item).FirstOrDefault();
                    selector.SelectedItem = r;
                }

                if (selector != null && visibility == Visibility.Collapsed || visibility == Visibility.Hidden)
                {
                    // if there is another view that can be shown
                    var viewToActivate = NextViewToActivate(region, view);

                    if (viewToActivate != null)
                    {
                        region.Activate(viewToActivate);
                    }
                    else
                    {
                        selector.SelectedIndex = -1;
                    }
                }

                // Uncomment the following lines to activate a view when you show it again
                /*else if(selector != null)
                {
                    Activate the view
                    region.Activate(view);   // It is not always needed activate the view when unhiding
                }*/
            }
        }

        private static object NextViewToActivate(IRegion region, object view)
        {
            var currentActiveView = region.ActiveViews.FirstOrDefault();
            // if the view to hide is the active view and there are other visible views
            if ((currentActiveView == null || view == currentActiveView) && region.Views.Any(v => v != view && (v as UIElement).Visibility == Visibility.Visible))
            {
                return region.Views.First(v => v != view && (v as UIElement).Visibility == Visibility.Visible);
            }
            return null;
        }

        public static DependencyObject GetAttachedControl(this IRegion region)
        {
            RegionManagerRegistrationBehavior behavior = (RegionManagerRegistrationBehavior)region.Behaviors.First(b => b.Key == RegionManagerRegistrationBehavior.BehaviorKey).Value;
            return behavior.HostControl;
        }
    }
}
