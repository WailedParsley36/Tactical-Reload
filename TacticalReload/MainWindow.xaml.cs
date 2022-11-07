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
using System.Windows.Shapes;
using TacticalReload.Pages;

namespace TacticalReload
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        public void SetTitle(string title)
        {
            Dispatcher.Invoke(() =>
            {
                Title = title;
            });
        }

        public void LoadPage<T>(T instance)
        {
            Dispatcher.Invoke(() =>
            {
                LoadedPage.Content = instance;
            });
        }

        public void LoadPage<T>() where T : Page, new() =>
            LoadPage(new T());

        private void KeyPressed(object sender, KeyEventArgs e)
        {
            if(LoadedPage.Content is ReloaderGame)
                ((ReloaderGame)LoadedPage.Content)!.KeyPressed(e.Key);
        }
    }
}
