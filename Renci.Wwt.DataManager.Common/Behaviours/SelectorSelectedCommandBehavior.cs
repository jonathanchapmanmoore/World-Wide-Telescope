using Microsoft.Practices.Prism.Commands;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace Renci.Wwt.DataManager.Common.Behaviours
{
    public class SelectorSelectedCommandBehavior : CommandBehaviorBase<Selector>
    {
        public SelectorSelectedCommandBehavior(Selector selectableObject)
            : base(selectableObject)
        {
            selectableObject.SelectionChanged += OnSelectionChanged;
        }

        private void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.CommandParameter = TargetObject.SelectedItem;
            this.ExecuteCommand();
        }
    }
}
