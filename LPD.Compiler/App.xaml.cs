using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace LPD.Compiler
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static string ArgumentFilePath { get; set; }

        public App()
        {

        }

        protected override void OnStartup(StartupEventArgs e)
        {
            ArgumentFilePath = e.Args.Length > 0 ? e.Args[0] : null;
            base.OnStartup(e);
        }
    }
}
