namespace Renci.Wwt.DataManager.Common.Framework
{
    using System;

    public interface IResult
    {
        void Execute();
        event EventHandler Completed;
    }
}
