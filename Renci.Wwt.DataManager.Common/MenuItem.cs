namespace Renci.Wwt.DataManager.Common
{
    using System.Collections.Generic;
    using System.Windows.Input;

    public class MenuItem
    {
        public string Text { get; private set; }

        public List<MenuItem> Children { get; private set; }

        public ICommand Command { get; private set; }

        public MenuItem(string item, ICommand command)
        {
            this.Text = item;
            this.Children = new List<MenuItem>();
            this.Command = command;
        }
    }
}
