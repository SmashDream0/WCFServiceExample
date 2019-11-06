using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sender
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            //Тут я проверял как это работает, до того как сделал экран
            /*var loginClient = new LoginContractClient();

            var guid = loginClient.Login("one", "123");

            var exchangeClient = new ExchangeContractClient();

            exchangeClient.AddQuestionAnswer(guid, "test", "blo");
            var answers = exchangeClient.GetAnswers(guid, "test");*/

            //Далее пример паттерна MVVM
            //View->ViewModel->Model
            var view = new MVVM.MainView();

            //Все настройки касаемые подключение к сервису в файле config
            //Этот файл читает сборка ServiceModel, которая входит в состав .Net
            view.DataContext = new MVVM.MainViewModel();

            view.ShowDialog();
        }
    }
}
