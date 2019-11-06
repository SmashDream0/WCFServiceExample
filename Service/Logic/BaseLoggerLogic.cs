using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceHost.Logic
{
    public abstract class BaseLoggerLogic
    {
        protected void Log(Func<String> getText)
        { InnerLog(getText()); }

        protected void Log(String text)
        { InnerLog(text); }

        private void InnerLog(String text)
        {
            Console.WriteLine(text);
        }
    }
}