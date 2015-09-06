using LPD.Compiler.Helpers;
using LPD.Compiler.Lexical;
using MahApps.Metro.Controls;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace LPD.Compiler
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        private const string OpenFileDialogFilter = "Todos os arquivos|*.*";

        private string _selectedFile;

        private bool isOpenFile = false;

        public MainWindow()
        {
            InitializeComponent();
        }

        private async void ReadAndShowFile()
        {
            CodeTextBox.Text = await FileHelper.GetFileContentAsStringAsync(_selectedFile);
            isOpenFile = true;
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

        private void OnErrorsListViewSelectedChanged(object sender, SelectionChangedEventArgs e)
        {
            var position = ErrorsListView.SelectedItem as ErrorViewModel;
            var line = position.Position.Line - 1;
            int charIndex;

            CodeTextBox.Focus();
            CodeTextBox.ScrollToLine(line);
            charIndex = CodeTextBox.GetCharacterIndexFromLineIndex(line);
            CodeTextBox.CaretIndex = charIndex + position.Position.Column - 1;
        }

        private void OnStartButtonClick(object sender, RoutedEventArgs e)
        {
            if (isOpenFile)
            {
                try
                {
                    Tokens.ItemsSource = new LexicalAnalizer(_selectedFile).GetTokens();
                }
                catch (InvalidTokenException ex)
                {
                    ErrorsListView.ItemsSource = new List<ErrorViewModel>()
                    {
                        new ErrorViewModel() { Position = ex.Position, Message = ex.Message }
                    };
                }
            }
        }

        private void OnRefreshButtonClick(object sender, RoutedEventArgs e)
        {
            //TODO - Implementar um refresh? se necessário!
        }
    }



    class ErrorViewModel
    {
        public Lexical.Position Position { get; set; }

        public string Message { get; set; }
    }
}
