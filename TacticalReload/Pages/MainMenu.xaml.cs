using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Media;
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
using TacticalReload.Resources;

namespace TacticalReload.Pages
{
    /// <summary>
    /// Interaction logic for MainMenu.xaml
    /// </summary>
    public partial class MainMenu : Page
    {
        private SoundPlayer _SoundPlayer;

        public MainMenu()
        {
            InitializeComponent();
            _SoundPlayer = new SoundPlayer();
            _SoundPlayer.Stream = Res.MainMenuTheme;
            _SoundPlayer.PlayLooping();
        }

        private void PlayReloader(object sender, RoutedEventArgs e)
        {
            _SoundPlayer.Stop();
            _SoundPlayer.Dispose();

            Core.Window.LoadPage<ReloaderGame>();
        }

        private void ExitGame(object sender, RoutedEventArgs e)
        {
            Environment.Exit(0x0);
        }
    }
}
