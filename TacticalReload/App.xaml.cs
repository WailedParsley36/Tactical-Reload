using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using TacticalReload.Pages;

namespace TacticalReload
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void EntryPoint(object sender, StartupEventArgs e)
        {
            Core.Initialize();

            Core.Window.Title = "Tactical Reloader - Main Menu";

            Core.Window.LoadPage<MainMenu>();
            Core.Window.Show();

        }
    }
}
