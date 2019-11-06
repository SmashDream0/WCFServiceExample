using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Sender.MVVM
{
    public interface IExecuteCommand: ICommand
    {
        /// <summary>
        /// Могу ли произвести запуск действий по команде
        /// </summary>
        bool CanExecuteLast { get; set; }

        /// <summary>
        /// Событие до запуска команды, способное предотсратить запуск
        /// </summary>
        event Action<object, CanExecuteEventArg> OnCanExecute;

        /// <summary>
        /// Обновить экран
        /// </summary>
        void UpdateCanExecute();
    }

    public class TypedEventArg<T>
    {
        public TypedEventArg(object sender, T value)
            : this()
        {
            Value = value;
            Sender = sender;
        }
        public TypedEventArg()
        { }

        /// <summary>
        /// Целевое значение
        /// </summary>
        public T Value { get; set; }

        /// <summary>
        /// Источник
        /// </summary>
        public object Sender { get; set; }
    }

    public class CanExecuteEventArg
        : TypedEventArg<bool>
    { }

    public class Command : ANotifier, IExecuteCommand
    {
        public Command(Action action) : this(action, null)
        { }
        public Command(Action action, Func<bool> canExcute)
        {
            this._action = action;
            this._canExecute = canExcute;
        }

        /// <summary>
        /// Разрешение на изменение изменено
        /// </summary>
        public event EventHandler CanExecuteChanged;

        /// <summary>
        /// Событие до запуска команды, способное предотсратить запуск
        /// </summary>
        public event Action<object, CanExecuteEventArg> OnCanExecute;

        private bool _canExecuteLast;
        public bool CanExecuteLast
        {
            get { return _canExecuteLast; }
            set
            {
                _canExecuteLast = value;
                propertyChanged("CanExecuteLast");
            }
        }

        protected Action _action;
        private Func<bool> _canExecute;

        /// <summary>
        /// Могу ли произвести запуск действий по команде
        /// </summary>
        public bool CanExecute(object parameter)
        {
            if (_canExecute == null)
            { return true; }
            else
            { return _canExecute(); }
        }

        /// <summary>
        /// Выполнить действие по команде
        /// </summary>
        /// <param name="parameter">Параметр</param>
        public void Execute(object parameter)
        {
            _action();
        }

        /// <summary>
        /// Обновить экран
        /// </summary>
        public void UpdateCanExecute()
        {
            CanExecute(null);
            if (CanExecuteChanged != null)
            { CanExecuteChanged(this, EventArgs.Empty); }
        }
    }
}
