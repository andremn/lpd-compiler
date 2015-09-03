using LPD.Compiler.Helpers;
using LPD.Compiler.Lexical;
using MahApps.Metro.Controls;
using Microsoft.Win32;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace LPD.Compiler
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        private const string OpenFileDialogFilter = "Arquivo LPD|*.lpd|Todos os arquivos|*.*";

        private string _selectedFile;

        public MainWindow()
        {
            InitializeComponent();
        }

        private async void ReadAndShowFile()
        {
            CodeTextBox.Text = await FileHelper.GetFileContentAsStringAsync(_selectedFile);
        }

        private void OnNewFileButtonClick(object sender, RoutedEventArgs e)
        {

        }

        private void OnOpenFileButtonClick(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog()
            {
                Filter = OpenFileDialogFilter
            };

            bool? result = openFileDialog.ShowDialog();

            if (!result.HasValue || !result.Value)
            {
                return;
            }

            _selectedFile = openFileDialog.FileName;
            ReadAndShowFile();
        }
    }
}
