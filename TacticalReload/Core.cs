using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace TacticalReload
{
    internal static class Core
    {
        public static MainWindow Window { get; private set; }

        public static void Initialize()
        {
            Window = new MainWindow();
        }
    }
}
