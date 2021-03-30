namespace Renci.Wwt.DataManager.Common.Framework
{
    using System;
    using System.Collections.Generic;

    public class ResultEnumerator
    {
        private readonly IEnumerator<IResult> _enumerator;

        public ResultEnumerator(IEnumerable<IResult> children)
        {
            this._enumerator = children.GetEnumerator();
        }

        public void Enumerate()
        {
            this.ChildCompleted(null, EventArgs.Empty);
        }

        private void ChildCompleted(object sender, EventArgs args)
        {
            var previous = sender as IResult;

            if (previous != null)
                previous.Completed -= ChildCompleted;

            if (!this._enumerator.MoveNext())
                return;

            var next = this._enumerator.Current;
            next.Completed += ChildCompleted;
            next.Execute();
        }
    }
}
