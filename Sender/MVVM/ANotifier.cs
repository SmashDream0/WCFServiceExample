using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sender.MVVM
{
    public abstract class ANotifier : INotifyPropertyChanged
    {
        /// <summary>
        /// Событие изменения свойства
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Свойство изменено
        /// </summary>
        /// <param name="propertyName">Имя свойства</param>
        internal void propertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                if (!notUpdateProperties.Contains(propertyName))
                { PropertyChanged(this, new PropertyChangedEventArgs(propertyName)); }
            }
        }

        protected List<string> notUpdateProperties = new List<string>();
    }
}
