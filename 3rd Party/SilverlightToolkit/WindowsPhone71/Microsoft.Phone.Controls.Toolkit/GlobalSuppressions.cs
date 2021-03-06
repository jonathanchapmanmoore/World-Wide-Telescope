// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.Diagnostics.CodeAnalysis;

[assembly: SuppressMessage("Microsoft.Design", "CA2210:AssembliesShouldHaveValidStrongNames", Justification = "Satisfied for Release, not for Debug.")]
[assembly: SuppressMessage("Microsoft.Design", "CA1020:AvoidNamespacesWithFewTypes", Scope = "namespace", Target = "System.Windows", Justification = "Matching Silverlight namespaces for existing classes.")]
[assembly: SuppressMessage("Microsoft.Design", "CA1020:AvoidNamespacesWithFewTypes", Scope = "namespace", Target = "System.Windows.Controls", Justification = "Matching Silverlight namespaces for existing classes.")]
[assembly: SuppressMessage("Microsoft.Design", "CA1020:AvoidNamespacesWithFewTypes", Scope = "namespace", Target = "System.Windows.Controls.Primitives", Justification = "Matching Silverlight namespaces for existing classes.")]
[assembly: SuppressMessage("Microsoft.Design", "CA1020:AvoidNamespacesWithFewTypes", Scope = "namespace", Target = "Microsoft.Phone.Controls", Justification = "Using corresponding Toolkit namespace for existing classes.")]
[assembly: SuppressMessage("Microsoft.Design", "CA1020:AvoidNamespacesWithFewTypes", Scope = "namespace", Target = "Microsoft.Phone.Controls.Primitives", Justification = "Using corresponding Toolkit namespace for existing classes.")]
[assembly: SuppressMessage("Microsoft.Naming", "CA1703:ResourceStringsShouldBeSpelledCorrectly", MessageId = "png", Scope = "resource", Target = "Microsoft.Phone.Controls.Properties.Resources.resources", Justification = "Standard file extension for PNG images.")]
[assembly: SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields", Scope = "member", Target = "Microsoft.Phone.Controls.DatePickerPage.#VisibilityStates", Justification = "VSM group name.")]
[assembly: SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields", Scope = "member", Target = "Microsoft.Phone.Controls.DatePickerPage.#Open", Justification = "VSM state name.")]
[assembly: SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields", Scope = "member", Target = "Microsoft.Phone.Controls.DatePickerPage.#Closed", Justification = "VSM state name.")]
[assembly: SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields", Scope = "member", Target = "Microsoft.Phone.Controls.DatePickerPage.#PlaneProjection", Justification = "Referenced by a VSM state.")]
[assembly: SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields", Scope = "member", Target = "Microsoft.Phone.Controls.TimePickerPage.#VisibilityStates", Justification = "VSM group name.")]
[assembly: SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields", Scope = "member", Target = "Microsoft.Phone.Controls.TimePickerPage.#Open", Justification = "VSM state name.")]
[assembly: SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields", Scope = "member", Target = "Microsoft.Phone.Controls.TimePickerPage.#Closed", Justification = "VSM state name.")]
[assembly: SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields", Scope = "member", Target = "Microsoft.Phone.Controls.TimePickerPage.#PlaneProjection", Justification = "Referenced by a VSM state.")]
[assembly: SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters", Scope = "member", Target = "Microsoft.Phone.Controls.TransitionService.#GetNavigationInTransition(System.Windows.UIElement)", Justification = "Transitions can only be applied to UIElements.")]
[assembly: SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters", Scope = "member", Target = "Microsoft.Phone.Controls.TransitionService.#SetNavigationInTransition(System.Windows.UIElement,Microsoft.Phone.Controls.NavigationInTransition)", Justification = "Transitions can only be applied to UIElements.")]
[assembly: SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Scope = "member", Target = "Microsoft.Phone.Controls.ITransition.#GetCurrentState()", Justification = "Must match the Storyboard interface.")]
[assembly: SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Scope = "member", Target = "Microsoft.Phone.Controls.ITransition.#GetCurrentTime()", Justification = "Must match the Storyboard interface.")]
[assembly: SuppressMessage("Microsoft.Naming", "CA1716:IdentifiersShouldNotMatchKeywords", MessageId = "Resume", Scope = "member", Target = "Microsoft.Phone.Controls.ITransition.#Resume()", Justification = "Must match the Storyboard interface.")]
[assembly: SuppressMessage("Microsoft.Naming", "CA1716:IdentifiersShouldNotMatchKeywords", MessageId = "Stop", Scope = "member", Target = "Microsoft.Phone.Controls.ITransition.#Stop()", Justification = "Must match the Storyboard interface.")]
[assembly: SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters", Scope = "member", Target = "Microsoft.Phone.Controls.TransitionService.#GetNavigationOutTransition(System.Windows.UIElement)", Justification = "Transitions can only be applied to UIElements.")]
[assembly: SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters", Scope = "member", Target = "Microsoft.Phone.Controls.TransitionService.#SetNavigationOutTransition(System.Windows.UIElement,Microsoft.Phone.Controls.NavigationOutTransition)", Justification = "Transitions can only be applied to UIElements.")]
[assembly: SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", MessageId = "FadeOut", Scope = "member", Target = "Microsoft.Phone.Controls.SlideTransitionMode.#SlideDownFadeOut", Justification = "Must match the metro motion names.")]
[assembly: SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", MessageId = "FadeOut", Scope = "member", Target = "Microsoft.Phone.Controls.SlideTransitionMode.#SlideLeftFadeOut", Justification = "Must match the metro motion names.")]
[assembly: SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", MessageId = "FadeOut", Scope = "member", Target = "Microsoft.Phone.Controls.SlideTransitionMode.#SlideRightFadeOut", Justification = "Must match the metro motion names.")]
[assembly: SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", MessageId = "FadeOut", Scope = "member", Target = "Microsoft.Phone.Controls.SlideTransitionMode.#SlideUpFadeOut", Justification = "Must match the FadeIn counterpart casing.")]
