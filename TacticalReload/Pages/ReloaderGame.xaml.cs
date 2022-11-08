using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using TacticalReload.Game;
using WpfAnimatedGif;

namespace TacticalReload.Pages
{
    /// <summary>
    /// Interaction logic for ReloaderGame.xaml
    /// </summary>
    public partial class ReloaderGame : Page
    {
        public ReloaderGame()
        {
            InitializeComponent();
            Core.Window.SetTitle("Tactical Reload - Playing");
        }

        private void GameLoaded(object sender, RoutedEventArgs e)
        {
            ImageBehavior.AddAnimationLoadedHandler(GunAnimation, (sender, args) =>
            {
                Players[0].LoadController(ImageBehavior.GetAnimationController(GunAnimation));

                if (!IsStarted)
                    StartGame();
            });

            ImageBehavior.AddAnimationLoadedHandler(GunAnimation2, (sender, args) =>
            {
                Players[1].LoadController(ImageBehavior.GetAnimationController(GunAnimation2));

                if (!IsStarted)
                    StartGame();
            });

            Players = new[] { new Player(this, Key.R, "Player 1", false), new Player(this, Key.NumPad5, "Player 2", true) };
        }

        public Player[] Players { get; private set; }
        public bool Paused { get; private set; } = false;
        public bool IsStarted = false;

        private DispatcherTimer PauseTimer = new DispatcherTimer();

        public void Win(Player player)
        {
            PauseTimer.Stop();

            PauseScreen.Visibility = Visibility.Visible;
            Paused = true;

            Players[0].Pause();
            Players[1].Pause();

            PauseTimer = new DispatcherTimer(DispatcherPriority.Normal, Dispatcher);
            int times = 5;
            PauseDescription.Content = $"{(Players[0] == player ? Players[1].Name : Players[0].Name)} has win!";
            PauseTimer.Tick += (sender, args) =>
            {
                times--;
                if (times == 0)
                {
                    PauseTimer.Stop();

                    Core.Window.LoadPage<MainMenu>();
                }
            };
            PauseTimer.Interval = new TimeSpan(0, 0, 1);
            PauseTimer.Start();
        }
        private void StartGame()
        {
            IsStarted = true;
            PauseTimer.Stop();

            PauseScreen.Visibility = Visibility.Visible;
            Paused = true;

            PauseTimer = new DispatcherTimer(DispatcherPriority.Normal, Dispatcher);
            int times = 5;
            PauseDescription.Content = "Starting in " + times;
            PauseTimer.Tick += (sender, args) =>
            {
                times--;
                PauseDescription.Content = "Starting in " + times;
                if (times == 0)
                {
                    PauseScreen.Visibility = Visibility.Collapsed;
                    Paused = false;

                    PauseTimer.Stop();

                    Players[0].Reload();
                    Players[1].Reload();
                }
            };
            PauseTimer.Interval = new TimeSpan(0, 0, 1);
            PauseTimer.Start();
        }
        private void Pause()
        {
            Dispatcher.Invoke(() =>
            {
                if (Paused)
                {
                    PauseScreen.Visibility = Visibility.Visible;
                    PauseDescription.Content = "Game Paused";
                }
                else
                {
                    PauseTimer.Stop();

                    Paused = true;
                    PauseScreen.Visibility = Visibility.Visible;

                    PauseTimer = new DispatcherTimer(DispatcherPriority.Normal, Dispatcher);
                    int times = 5;
                    PauseDescription.Content = "Starting in " + times;
                    PauseTimer.Tick += (sender, args) =>
                    {
                        times--;
                        PauseDescription.Content = "Starting in " + times;
                        if (times == 0)
                        {
                            PauseScreen.Visibility = Visibility.Collapsed;
                            Paused = false;

                            Players[0].Pause(!Paused);
                            Players[1].Pause(!Paused);

                            PauseTimer.Stop();
                        }
                    };
                    PauseTimer.Interval = new TimeSpan(0, 0, 1);
                    PauseTimer.Start();
                }

                Players[0].Pause(!Paused);
                Players[1].Pause(!Paused);
            });
        }
        public void KeyPressed(Key key)
        {
            if (key == Key.P || key == Key.Escape)
            {
                //Pausing game
                Paused = !Paused;
                Pause();
            }

            if (key == Players[0].ActionKey)
            {
                if (Players[0].IsShooting)
                    Players[0].Shoot(1);
            }

            if (key == Players[1].ActionKey)
            {
                if (Players[1].IsShooting)
                    Players[1].Shoot(1);
            }
        }
    }
}
