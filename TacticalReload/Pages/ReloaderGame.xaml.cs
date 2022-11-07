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
            });

            Players = new[] { new Player(this, Key.R, "Player 1", false), new Player(this, Key.NumPad5, "Player 2", true) };
        }

        public Player[] Players { get; private set; }
        public bool Paused { get; private set; } = false;

        private DispatcherTimer PauseTimer = new DispatcherTimer();

        public void KeyPressed(Key key)
        {
            if (key == Key.P || key == Key.Escape)
            {
                //Pausing game
                Paused = !Paused;

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
                                //Players[1].Pause(rPause);

                                PauseTimer.Stop();
                            }
                        };
                        PauseTimer.Interval = new TimeSpan(0, 0, 1);
                        PauseTimer.Start();
                    }

                    Players[0].Pause(!Paused);
                    //Players[1].Pause(rPause);
                });
            }

            if (key == Players[0].ActionKey)
                Players[0].Shoot(1.25);
        }

        private void DealDamage(bool second)
        {
            Dispatcher.Invoke(() =>
            {

            });
        }
    }
}
