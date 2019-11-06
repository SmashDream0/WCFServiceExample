using Microsoft.Win32;
using ServiceHost.DTO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Sender.MVVM
{
    public class MainViewModel: ANotifier
    {
        public MainViewModel()
        { Initialize(); }

        private ClientModel _clientLogic;
        
        private string _login;
        private string _password;
        private string _guid;
        private string _request;
        private bool _allowGUILimitations;
        
        /// <summary>
        /// Имя пользователя
        /// </summary>
        public string Login
        {
            get => _login;
            set
            {
                _login = value;
                propertyChanged("Login");
                UpdateCommand();
            }
        }

        /// <summary>
        /// Пароль
        /// </summary>
        public string Password
        {
            get => _password;
            set
            {
                _password = value;
                propertyChanged("Password");
                UpdateCommand();
            }
        }

        /// <summary>
        /// Ключ сессии
        /// </summary>
        public string Guid
        {
            get => _guid;
            private set
            {
                _guid = value;
                propertyChanged("Guid");
                UpdateCommand();
            }
        }


        /// <summary>
        /// Включить/отключить ограничивающую логику экрана
        /// </summary>
        public bool AllowGUILimitations
        {
            get => _allowGUILimitations;
            set
            {
                _allowGUILimitations = value;
                propertyChanged("AllowGUILimitations");
                UpdateCommand();
            }
        }

        /// <summary>
        /// Запрос
        /// </summary>
        public string FileName
        {
            get => _request;
            set
            {
                _request = value;
                propertyChanged("FileName");
                UpdateCommand();
            }
        }

        /// <summary>
        /// Команда логина
        /// </summary>
        public IExecuteCommand LoginCommand
        { get; private set; }

        /// <summary>
        /// Команда разлогина
        /// </summary>
        public IExecuteCommand UnloginCommand
        { get; private set; }

        /// <summary>
        /// Команда отправки запроса
        /// </summary>
        public IExecuteCommand GetFileCommand
        { get; private set; }

        /// <summary>
        /// Команда обновления сессии
        /// </summary>
        public IExecuteCommand UpdateSessionCommand
        { get; private set; }

        private void Initialize()
        {
            _clientLogic = new ClientModel();

            LoginCommand = new Command(LoginAction, CanLogin);
            UnloginCommand = new Command(UnloginAction, CanUnlogin);
            GetFileCommand = new Command(GetFileAction, CanGetFile);
            
            AllowGUILimitations = true;
        }

        private void LoginAction()
        {
            SessionInfo sessionInfo;

            if (_clientLogic.Login(Login, Password, out sessionInfo))
            {
                Guid = sessionInfo.Guid;
                var timeUpdate = (int)(sessionInfo.TimeoutSec * 0.666);

                _clientLogic.StartRegularSessionUpdate(timeUpdate, sessionInfo.Guid);
            }
            else
            { MessageBox.Show(sessionInfo.Message); }

            UpdateCommand();
        }

        private bool CanLogin()
        { return !AllowGUILimitations || !String.IsNullOrEmpty(Login) && !String.IsNullOrEmpty(Password); }

        private void UnloginAction()
        {
            string error;

            _clientLogic.StopRegularSessionUpdate();
            if (_clientLogic.Unlogin(Guid, out error))
            { Guid = String.Empty; }
            else
            { MessageBox.Show(error); }

            UpdateCommand();
        }

        private bool CanUnlogin()
        { return !AllowGUILimitations || !String.IsNullOrEmpty(Guid); }

        private void GetFileAction()
        {
            string error;

            var bytes = _clientLogic.GetFile(Guid, FileName, out error);

            if (!String.IsNullOrEmpty(error))
            { MessageBox.Show(error); }
            else
            {
                var sf = new SaveFileDialog();
                sf.FileName = FileName;

                if (sf.ShowDialog() ?? false)
                {
                    using (var fs = new FileStream(sf.FileName, FileMode.OpenOrCreate, FileAccess.Write))
                    {
                        fs.SetLength(0);

                        fs.Write(bytes, 0, bytes.Length);
                    }
                }
            }

            UpdateCommand();
        }

        private bool CanGetFile()
        { return !AllowGUILimitations || !String.IsNullOrEmpty(Guid) && !String.IsNullOrEmpty(FileName); }

        private void UpdateCommand()
        {
            LoginCommand.UpdateCanExecute();
            UnloginCommand.UpdateCanExecute();
            GetFileCommand.UpdateCanExecute();
        }
    }
}
