using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using TacticalReload.Pages;
using WpfAnimatedGif;

namespace TacticalReload.Game
{
    public class Player
    {
        private readonly ReloaderGame _Game;

        private ImageAnimationController _GunController;
        private ProgressBar _HealthBar;
        private Label _PlayerName;

        private Label _BarDescription;

        private ProgressBar _CooldownBar;
        private Border _CooldownElement;

        public Player(ReloaderGame game, Key actionKey, string name, bool second)
        {
            ActionKey = actionKey;
            _Game = game;

            if (second)
            {
                _PlayerName = _Game.BarDescription;
            }
            else
            {
                _PlayerName = _Game.PlayerName;
                _HealthBar = _Game.PlayerHealth;

                _BarDescription = _Game.BarDescription;
                _CooldownBar = _Game.CooldownBar;
                _CooldownElement = _Game.CooldownElement;
            }

            Name = name;
        }
        public void LoadController(ImageAnimationController gun)
        {
            _GunController = gun;
        }

        public string Name
        {
            get =>
                (string)_PlayerName.Content;
            private set
            {
                _Game.Dispatcher.Invoke(() =>
                {
                    _PlayerName.Content = value;
                });
            }
        }
        public int Health
        {
            get =>
                (int)_HealthBar.Value;
            private set
            {
                _Game.Dispatcher.Invoke(() =>
                {
                    _HealthBar.Value = value;
                });
            }
        }
        public Key ActionKey { get; private set; } = Key.R;


        public void Pause(bool unpause = false)
        {
            _Game.Dispatcher.Invoke(() =>
            {
                if (unpause)
                    _GunController.Play();
                else
                    _GunController.Pause();
            });
        }

        public void Shoot(double speedMultipler)
        {
            var timer = new DispatcherTimer(DispatcherPriority.Normal, _Game.Dispatcher);
            timer.Tick += (sender, args) =>
            {
                
            };
            timer.Interval = new TimeSpan(0, 0, 0, 0, 10);

            _Game.Dispatcher.Invoke(() =>
            {

                timer.Start();
            });
            //TODO: Shoot here
        }

        public void Cooldown(double ammount)
        {
            var timer = new DispatcherTimer(DispatcherPriority.Normal, _Game.Dispatcher);
            timer.Tick += (sender, args) =>
            {
                _CooldownBar.Value -= ammount;
            };
            timer.Interval = new TimeSpan(0, 0, 0, 0, 10);

            _Game.Dispatcher.Invoke(() =>
            {
                //After shooting
                _GunController.GotoFrame(3);

                _CooldownElement.Visibility = System.Windows.Visibility.Visible;
                //TODO: Gun visibility

                _BarDescription.Content = "Cool it down!";

                _CooldownBar.Value = 100;
                timer.Start();
            });
        }

        public void TakeDamage(int ammount, bool crit = false)
        {
            Health -= ammount;

            if (crit)
                Console.WriteLine("Critical Hit!");
        }

    }
}
