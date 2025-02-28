using System;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Styx.Botting
{
    public class AutoLogin
    {
        private readonly string _server;
        private AEC _aec;
        private Steps _currentStep;
        private LoginManager _loginManager;
        private LoginState _loginState;

        public AutoLogin(string serverName)
        {
            _server = serverName;
            _currentStep = Steps.Idle;
        }

        public static bool IsLoggedIn => Session.MyPlayerData != null;

        public static bool IsLoaded
        {
            get
            {
                try
                {
                    var ec = Entities.GetInstance()?.me?.entitycontroller?.enabled;
                    var mc = Entities.GetInstance()?.me?.moveController?.enabled;
                    return IsLoggedIn && Session.MyPlayerData?.allItems.Count > 0 && Game.Instance?.uigame?.ActionBar != null && ec == true && mc == true;
                }
                catch (Exception)
                {
                    System.Windows.Forms.MessageBox.Show(@"Something went wrong, this is probably due to a game update. Please notify us in our Discord server.", "Styx Error: Loaded");
                    return false;
                }
            }
        }

        private void LoginInternal()
        {
            var mi = _loginManager.GetType().GetMethod("LoginAQ3D");
            mi.Invoke(_loginManager,
                new object[] { PlayerPrefs.GetString("USERNAME"), PlayerPrefs.GetString("PASSWORD") });
        }

        public bool Login()
        {
            switch (_currentStep)
            {
                case Steps.Idle:
                    _aec = AEC.getInstance();
                    if ((_loginState = Object.FindObjectOfType<LoginState>()) == null)
                        return false;
                    if ((_loginManager = Object.FindObjectOfType<LoginManager>()) == null)
                        return false;

                    LoginInternal();
                    _currentStep = Steps.WaitingForLogin;
                    return false;
                case Steps.WaitingForLogin:
                    if (Object.FindObjectOfType<CharSelect>() != null &&
                        !string.IsNullOrEmpty(Object.FindObjectOfType<CharSelect>().serverSelectLabel.text))
                    {
                        var targetServer = ServerInfo.Servers.FirstOrDefault(
                            s => s.Name.Equals(_server, StringComparison.OrdinalIgnoreCase));

                        _aec.onConnect += ConnectionEstablished;
                        _aec.connect(targetServer.HostName, targetServer.Port, targetServer.ID);
                        _currentStep = Steps.WaitingForGame;
                    }

                    return false;
                case Steps.WaitingForGame:
                    if (IsLoggedIn && IsLoaded)
                    {
                        _currentStep = Steps.Idle;
                        Bot.Instance.IsRunning = true;
                        return true;
                    }

                    break;
            }

            return false;
        }

        private void ConnectionEstablished(string message)
        {
            _aec.onConnect -= ConnectionEstablished;
            BusyDialog.Close();
            StateManager.LoadState("scene.game");
        }

        private enum Steps // Used to keep track of where we currently are in the login process. (Update())
        {
            Idle,
            WaitingForLogin,
            WaitingForGame
        }
    }
}