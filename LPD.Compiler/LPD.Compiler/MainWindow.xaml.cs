using System.Windows;

namespace LPD.Compiler
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            TokensListView.ItemsSource = new Lexical.LexicalAnalizer(@"C:\Users\andre\Desktop\Compiladores\Testes\Lexema1.lpd").GetTokens();
        }
    }
}
