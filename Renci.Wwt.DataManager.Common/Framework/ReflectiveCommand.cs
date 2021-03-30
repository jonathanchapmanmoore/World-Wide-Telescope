namespace Renci.Wwt.DataManager.Common.Framework
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Reflection;
    using System.Windows.Input;

    public class ReflectiveCommand : ICommand
    {
        private readonly PropertyInfo _canExecute;
        private readonly MethodInfo _execute;
        private readonly object _model;

        public ReflectiveCommand(object model, MethodInfo execute, PropertyInfo canExecute)
        {
            this._model = model;
            this._execute = execute;
            this._canExecute = canExecute;

            var notifier = this._model as INotifyPropertyChanged;
            if (notifier != null && this._canExecute != null)
            {
                notifier.PropertyChanged += (s, e) =>
                {
                    if (e.PropertyName == this._canExecute.Name)
                        CanExecuteChanged(this, EventArgs.Empty);
                };
            }
        }

        public event EventHandler CanExecuteChanged = delegate { };

        public bool CanExecute(object parameter)
        {
            if (this._canExecute != null)
                return (bool)this._canExecute.GetValue(this._model, null);
            return true;
        }

        public void Execute(object parameter)
        {
            var returnValue = _execute.Invoke(this._model, null);
            if (returnValue != null)
                HandleReturnValue(returnValue);
        }

        private static void HandleReturnValue(object returnValue)
        {
            if (returnValue is IResult)
                returnValue = new[]
                {
                    returnValue as IResult
                };

            if (returnValue is IEnumerable<IResult>)
                new ResultEnumerator(returnValue as IEnumerable<IResult>).Enumerate();
        }
    }
}
