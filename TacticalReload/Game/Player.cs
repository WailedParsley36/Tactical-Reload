using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using System.Xml.Linq;
using TacticalReload.Pages;
using WpfAnimatedGif;

namespace TacticalReload.Game
{
    public class Player
    {
        private readonly ReloaderGame _Game;

        private ImageAnimationController _GunController;
        private Image _Gun;
        private ProgressBar _HealthBar;
        private Label _PlayerName;

        private Label _BarDescription;

        private ProgressBar _CooldownBar;
        private Border _CooldownElement;

        private Border _ReloadElement;
        private DockPanel _ReloadMarker;
        private DockPanel _ReloadRanger;

        public Player(ReloaderGame game, Key actionKey, string name, bool second)
        {
            ActionKey = actionKey;
            _Game = game;

            if (second)
            {
                _Gun = _Game.GunAnimation2;
                _PlayerName = _Game.PlayerName2;
                _HealthBar = _Game.PlayerHealth2;

                _BarDescription = _Game.BarDescription2;
                _CooldownBar = _Game.CooldownBar2;
                _CooldownElement = _Game.CooldownElement2;

                _ReloadElement = _Game.ReloadElement2;
                _ReloadMarker = _Game.ReloadMarker2;
                _ReloadRanger = _Game.ReloadRange2;
            }
            else
            {
                _Gun = _Game.GunAnimation;
                _PlayerName = _Game.PlayerName;
                _HealthBar = _Game.PlayerHealth;

                _BarDescription = _Game.BarDescription;
                _CooldownBar = _Game.CooldownBar;
                _CooldownElement = _Game.CooldownElement;

                _ReloadElement = _Game.ReloadElement;
                _ReloadMarker = _Game.ReloadMarker;
                _ReloadRanger = _Game.ReloadRange;
            }

            Name = name;
            Health = 100;
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
        public int ReloadCombo { get; private set; } = 100;
        public bool IsShooting { get; private set; } = false;

        private bool _Paused;

        public void Pause(bool unpause = false)
        {
            _Paused = !unpause;
            _Game.Dispatcher.Invoke(() =>
            {
                if (unpause && IsShooting)
                    _GunController.Play();
                else
                    _GunController.Pause();
            });
        }

        private (int, int) _ReloadRange;
        private int _ReloadValue;
        private DispatcherTimer? _ReloadTimer;
        public void Reload()
        {
            _ReloadTimer?.Stop();
            _ReloadTimer = new DispatcherTimer(DispatcherPriority.Normal, _Game.Dispatcher);

            _ReloadValue = 0;
            _ReloadRange.Item1 = RandomNumberGenerator.GetInt32(96);
            _ReloadRange.Item2 = Math.Clamp((_ReloadRange.Item1 + RandomNumberGenerator.GetInt32(50)), 5, 100);
            IsShooting = true;

            _ReloadTimer.Tick += (sender, args) =>
            {
                if (_Paused)
                    return;

                _ReloadValue += /*(int)((ReloadCombo / 100) * 1) */1;
                MoveReloadMarkTo(_ReloadValue);

                if (_GunController.CurrentFrame == 34)
                {
                    _GunController.Pause();
                    _GunController.GotoFrame(9);
                    ImageBehavior.SetAnimationDuration(_Gun, new System.Windows.Duration(new TimeSpan(0, 0, 5)));
                    _GunController.Play();
                }

                if (_ReloadValue == 100)
                    Cooldown(RandomNumberGenerator.GetInt32(11) / 10);
                else if (_ReloadValue >= _ReloadRange.Item1 && _ReloadValue <= _ReloadRange.Item2)
                {
                    //TODO: Highlight marker
                    _BarDescription.Content = "Shoot already!";
                }
                else
                {
                    _BarDescription.Content = $"Reloading...{_ReloadRange.Item1}-{_ReloadRange.Item2}";
                }
            };
            _ReloadTimer.Interval = new TimeSpan(0, 0, 0, 0, Math.Clamp(RandomNumberGenerator.GetInt32(101), 10, 100));

            _Game.Dispatcher.Invoke(() =>
            {
                _CooldownElement.Visibility = System.Windows.Visibility.Collapsed;
                _ReloadElement.Visibility = System.Windows.Visibility.Visible;
                _ReloadMarker.Visibility = System.Windows.Visibility.Visible;
                _ReloadRanger.Visibility = System.Windows.Visibility.Visible;

                _BarDescription.Content = $"Reloading...{_ReloadRange.Item1}-{_ReloadRange.Item2}";

                _GunController.Pause();
                _GunController.GotoFrame(9);
                ImageBehavior.SetAnimationDuration(_Gun, new System.Windows.Duration(new TimeSpan(0, 0, 5)));
                _GunController.Play();

                SetReloadRangeUI();
                MoveReloadMarkTo(0);

                _ReloadTimer.Start();
            });
        }
        private void SetReloadRangeUI()
        {
            double left = 80 + 475 * ((double)_ReloadRange.Item1 / 100);
            double right = 80 + 475 * ((100 - (double)_ReloadRange.Item2) / 100);
            _ReloadRanger.Margin = new System.Windows.Thickness(left, -100, right, _ReloadMarker.Margin.Bottom);
        }
        private void MoveReloadMarkTo(int percent)
        {
            _ReloadMarker.Margin = new System.Windows.Thickness(930 * ((double)percent / 100) - 465, _ReloadMarker.Margin.Top, _ReloadMarker.Margin.Right, _ReloadMarker.Margin.Bottom);
        }
        public void Cooldown(double ammount = 1, bool shot = false)
        {
            _ReloadTimer?.Stop();
            _ReloadTimer = new DispatcherTimer(DispatcherPriority.Normal, _Game.Dispatcher);
            ammount = (double)RandomNumberGenerator.GetInt32(15, 101) / 100;
            _ReloadTimer.Tick += (sender, args) =>
            {
                if (_Paused)
                    return;

                _CooldownBar.Value -= ammount;

                if (_GunController.CurrentFrame == 10)
                {
                    _GunController.Pause();
                    _GunController.GotoFrame(5);
                    ImageBehavior.SetAnimationDuration(_Gun, new System.Windows.Duration(new TimeSpan(0, 0, 12)));
                    _GunController.Play();
                }

                if (_CooldownBar.Value <= 0)
                {
                    _CooldownElement.Visibility = System.Windows.Visibility.Collapsed;
                    Reload();
                }
            };
            _ReloadTimer.Interval = new TimeSpan(0, 0, 0, 0, 10);

            ReloadCombo = 100;
            _ReloadValue = 0;
            _ReloadRange = (0, 0);
            IsShooting = false;

            _Game.Dispatcher.Invoke(() =>
            {
                //After shooting
                _GunController.Pause();
                _GunController.GotoFrame(shot ? 0 : 5);
                ImageBehavior.SetAnimationDuration(_Gun, new System.Windows.Duration(new TimeSpan(0, 0, 12)));
                _GunController.Play();

                _CooldownElement.Visibility = System.Windows.Visibility.Visible;
                _ReloadElement.Visibility = System.Windows.Visibility.Collapsed;
                _ReloadMarker.Visibility = System.Windows.Visibility.Collapsed;
                _ReloadRanger.Visibility = System.Windows.Visibility.Collapsed;

                _BarDescription.Content = "Cool it down!";

                _CooldownBar.Value = 100;
                _ReloadTimer.Start();
            });
        }

        public void Shoot(double damageMultiplier)
        {
            if (!IsShooting)
                return;

            if (_ReloadValue >= _ReloadRange.Item1 && _ReloadValue <= _ReloadRange.Item2)
            {
                _GunController.GotoFrame(1);
                ImageBehavior.SetAnimationDuration(_Gun, new System.Windows.Duration(new TimeSpan(0, 0, 12)));
                _GunController.Play();
                IsShooting = false;
                ReloadCombo++;
                if (_Game.PlayerName2 == _PlayerName)
                    _Game.Players[0].TakeDamage(RandomNumberGenerator.GetInt32((int)(damageMultiplier * 10) + (_ReloadValue / 10)), RandomNumberGenerator.GetInt32(0, 2) == 0 ? false : true);
                else
                    _Game.Players[1].TakeDamage(RandomNumberGenerator.GetInt32((int)(damageMultiplier * 10) + (_ReloadValue / 10)), RandomNumberGenerator.GetInt32(0, 2) == 0 ? false : true);
            }

            Cooldown(RandomNumberGenerator.GetInt32(11) / 10);
        }
        public void TakeDamage(int ammount, bool crit = false)
        {
            if (crit)
                ammount = (int)(ammount * 1.25);

            Health -= ammount;

            if (Health == 0)
                _Game.Win(this);
        }
    }
}
