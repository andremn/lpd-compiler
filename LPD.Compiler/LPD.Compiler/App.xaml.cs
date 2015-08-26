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
        public App()
        {
            new Lexical.LexicalAnalizer(@"C:\Users\andre\Desktop\Compiladores\Testes\Lexema1.lpd").DoAnalisys();
        }
    }
}
