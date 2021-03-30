namespace Renci.Wwt.DataManager.Common.BaseClasses
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Reflection;
    using System.ComponentModel.DataAnnotations;
    using System.Linq.Expressions;
    using System.Windows.Threading;
    using System.Dynamic;

    [Serializable]
    public abstract class ModelBase : IDataErrorInfo, INotifyPropertyChanged
    {
        private readonly Dictionary<string, object> _values = new Dictionary<string, object>();

        [Browsable(false)]
        public virtual bool IsValid
        {
            get { return string.IsNullOrEmpty(this.Error); }
        }

        protected void ExecuteOnUIThread(Action action)
        {
            var dispatcher = Dispatcher.CurrentDispatcher;

            if (dispatcher.CheckAccess())
                action();
            else
                dispatcher.Invoke(action);
        }

        protected T Get<T>(string name)
        {
            return Get(name, default(T));
        }

        protected T Get<T>(string name, T defaultValue)
        {
            if (_values.ContainsKey(name))
            {
                return (T)_values[name];
            }

            return defaultValue;
        }

        protected T Get<T>(string name, Func<T> initialValue)
        {
            if (_values.ContainsKey(name))
            {
                return (T)_values[name];
            }

            Set(name, initialValue());
            return Get<T>(name);
        }

        protected T Get<T>(Expression<Func<T>> expression)
        {
            return Get<T>(PropertyName(expression));
        }

        protected T Get<T>(Expression<Func<T>> expression, T defaultValue)
        {
            return Get(PropertyName(expression), defaultValue);
        }

        protected T Get<T>(Expression<Func<T>> expression, Func<T> initialValue)
        {
            return Get(PropertyName(expression), initialValue);
        }

        protected void Set<T>(string name, T value)
        {
            if (_values.ContainsKey(name))
            {
                if (_values[name] == null && value == null)
                    return;

                if (_values[name] != null && _values[name].Equals(value))
                    return;

                _values[name] = value;
            }
            else
            {
                _values.Add(name, value);
            }

            NotifyOfPropertyChange(name);
        }

        protected void Set<T>(Expression<Func<T>> expression, T value)
        {
            Set(PropertyName(expression), value);
        }

        protected void NotifyOfPropertyChange<TProperty>(Expression<Func<TProperty>> property)
        {
            var propertyName = PropertyName<TProperty>(property);

            NotifyOfPropertyChange(propertyName);
        }

        protected void NotifyOfPropertyChange(string propertyName)
        {
            this.OnPropertyChanged(propertyName);
            this.ExecuteOnUIThread(() => PropertyChanged(this, new PropertyChangedEventArgs(propertyName)));
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
        }

        #region INotifyPropertyChanged Members

        [field: NonSerialized]
        public event PropertyChangedEventHandler PropertyChanged = delegate { };    //  Empty event handler to avoid checking for null

        #endregion

        #region IDataErrorInfo Members

        [Browsable(false)]
        public string Error
        {
            get
            {

                var context = new ValidationContext(this, null, null);
                var results = new List<ValidationResult>();

                return !Validator.TryValidateObject(this, context, results)
                    ? string.Join(Environment.NewLine, results.Select(x => x.ErrorMessage))
                    : null;
            }
        }

        [Browsable(false)]
        public string this[string propertyName]
        {
            get
            {
                var context = new ValidationContext(this, null, null)
                {
                    MemberName = propertyName
                };

                var results = new List<ValidationResult>();
                var value = this.GetType().GetProperty(propertyName).GetValue(this, null);

                return !Validator.TryValidateProperty(value, context, results)
                    ? string.Join(Environment.NewLine, results.Select(x => x.ErrorMessage))
                    : null;
            }
        }

        #endregion

        private static string PropertyName<TProperty>(Expression<Func<TProperty>> property)
        {
            var lambda = (LambdaExpression)property;

            MemberExpression memberExpression;
            if (lambda.Body is UnaryExpression)
            {
                var unaryExpression = (UnaryExpression)lambda.Body;
                memberExpression = (MemberExpression)unaryExpression.Operand;
            }
            else memberExpression = (MemberExpression)lambda.Body;

            return memberExpression.Member.Name;
        }
    }
}
