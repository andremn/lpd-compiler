using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Highlighting.Xshd;
using LPD.Compiler.Helpers;
using LPD.Compiler.Lexical;
using LPD.Compiler.ViewModel;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using System.ComponentModel;
using System.Linq;

namespace LPD.Compiler
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        private const string WindowTitleFormat = "LPD COMPILER - {0}";
        private const string FileDialogFilter = "Arquivo LPD,TXT|*.lpd;*.txt| Todos os arquivos|*.*";

        private const string SaveButtonEnabledSource = "Images/Save.png";
        private const string SaveButtonDisabledSource = "Images/Save_Disabled.png";
        private const string SaveAsButtonEnabledSource = "Images/SaveAs.png";
        private const string SaveAsButtonDisabledSource = "Images/SaveAs_Disabled.png";
        private const string RefreshButtonDisabledSource = "Images/refresh_disabled.png";
        private const string RefreshButtonEnabledSource = "Images/refresh.png";

        private string _selectedFile;
        private bool _hasTextChanged = true;
        private ushort _modificationsCount = 0;

        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindow"/> class.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            UpdateSaveButtons();
            Editor.SyntaxHighlighting = HighlightingLoader.Load(FileHelper.GetSyntaxHighlighting(), HighlightingManager.Instance);

            #region Needs to be removed
            /*System.Diagnostics.Stopwatch watch = new System.Diagnostics.Stopwatch();
            List<Token> tokens = new List<Token>(1000);
            int counter = 0;

            watch.Start();

            for (int i = 0; i < 1000; i++)
            {
                for (int j = 0; j < 1000; j++)
                {
                    tokens.Add(new Token() { Lexeme = "Test", Symbol = Symbols.None });
                }

                for (int k = 0; k < 1000; k++)
                {
                    if (tokens[k].Lexeme == "Test")
                    {
                        if (tokens[k].Symbol == Symbols.None)
                        {
                            counter++;
                        }
                    }
                }

                if (counter < 20)
                {
                    MessageBox.Show("Here!", "Oh noo!");
                }

                counter = 0;
                tokens.Clear();
            }
            
            watch.Stop();
            MessageBox.Show(watch.Elapsed.TotalMilliseconds + " ms", "Time");*/
            #endregion
        }
        /// <summary>
        /// Refresh the file 
        /// </summary>
        /// <returns><see cref="Task"/></returns>
        private async Task RefreshFileAsync(string file)
        {
            Editor.Text = await FileHelper.GetFileContentAsStringAsync(file);
        }


        /// <summary>
        /// Reads all the contents of the selected file and shows it to the user.
        /// </summary>
        /// <returns><see cref="Task"/></returns>
        private async Task ReadAndShowFileAsync()
        {
            Editor.Text = await FileHelper.GetFileContentAsStringAsync(_selectedFile);
            _modificationsCount = 0;
            UpdateSaveButtons();
        }

        /// <summary>
        /// Saves all the contents of the current file.
        /// </summary>
        /// <returns><see cref="Task"/></returns>
        private async Task SaveFileAsync()
        {
            await FileHelper.SaveFileStringContentsAsync(_selectedFile, Editor.Text);
            _modificationsCount = 0;
            UpdateSaveButtons();
        }

        /// <summary>
        /// Saves all the contents of the selected file.
        /// </summary>
        /// <returns><see cref="Task"/></returns>
        private async Task SaveAsFileAsync()
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog()
            {
                Filter = FileDialogFilter,
                FileName = Path.GetFileName(_selectedFile)
            };

            bool? result = saveFileDialog.ShowDialog();

            if (result == null || !result.Value)
            {
                return;
            }

            _selectedFile = saveFileDialog.FileName;
            await SaveFileAsync();
        }

        /// <summary>
        /// Updates SaveButton and SaveAsButton.
        /// </summary>
        private void UpdateSaveButtons()
        {
            Image saveButtonContent = SaveButton.Content as Image;
            Image saveAsButtonContent = SaveAsButton.Content as Image;
            Image refreshButtonContent = Refresh.Content as Image;

            if  (_modificationsCount > 0)
            {
                saveButtonContent.Source = new BitmapImage(new Uri(SaveButtonEnabledSource, UriKind.Relative));
                saveAsButtonContent.Source = new BitmapImage(new Uri(SaveAsButtonEnabledSource, UriKind.Relative));
                refreshButtonContent.Source = new BitmapImage(new Uri(RefreshButtonEnabledSource,UriKind.Relative));
                SaveButton.IsEnabled = SaveAsButton.IsEnabled = Refresh.IsEnabled= true;
            }
            else
            {
                saveButtonContent.Source = new BitmapImage(new Uri(SaveButtonDisabledSource, UriKind.Relative));
                saveAsButtonContent.Source = new BitmapImage(new Uri(SaveAsButtonDisabledSource, UriKind.Relative));
                refreshButtonContent.Source = new BitmapImage(new Uri(RefreshButtonDisabledSource, UriKind.Relative));
                SaveButton.IsEnabled = SaveAsButton.IsEnabled = Refresh.IsEnabled = false;
            }
        }

        /// <summary>
        /// Compiles the source code.
        /// </summary>
        /// <returns><see cref="Task"/></returns>
        private async Task CompileAsync()
        {
            if (string.IsNullOrEmpty(Editor.Text))
            {
                return;
            }

            if (_modificationsCount > 0)
            {
                if (string.IsNullOrEmpty(_selectedFile))
                {
                    await SaveAsFileAsync();
                }
                else
                {
                    await SaveFileAsync();
                }
            }

            TokensList.Items.Clear();
            ErrorTextBlock.Text = string.Empty;
            
            using (LexicalAnalizer lexical = new LexicalAnalizer(_selectedFile))
            {
                LexicalItem item;

                while (lexical.Next(out item))
                {
                    if (item.Error != null)
                    {
                        CodePosition position = item.Error.Position;

                        ErrorTextBlock.Text = string.Format("Linha {0}, coluna {1}: {2}", position.Line, position.Column, item.Error.Message);
                        break;
                    }

                    TokensList.Items.Add(item.Token);
                }
            }

            int tokensCount = TokensList.Items.Count;

            if (tokensCount > 0)
            {
                TokenGroupBox.Header = tokensCount > 1 ? tokensCount + " tokens" : "1 token";
            }
            else
            {
                TokenGroupBox.Header = "Nenhum token";
            }
        }

        /// <summary>
        /// Called when this window is about to close.
        /// </summary>
        /// <param name="e">A <see cref="CancelEventArgs"/> that contains the event data.</param>
        protected async override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);

            if (_modificationsCount > 0)
            {
                MessageDialogResult result;
                MetroDialogSettings settings = new MetroDialogSettings()
                {
                    AffirmativeButtonText = "Salvar",
                    NegativeButtonText = "Não salvar"
                };

                e.Cancel = true;
                 result = await this.ShowMessageAsync("Salvar mudanças", "O arquivo foi modificado. Deseja salvar as mudanças?", 
                     MessageDialogStyle.AffirmativeAndNegative, settings);

                if (result == MessageDialogResult.Affirmative)
                {
                    await SaveFileAsync();
                }
                else
                {
                    _modificationsCount = 0;
                }

                Close();
            }
        }

        /// <summary>
        /// Occurs when NewFileButton is clicked.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The data of the event.</param>
        private void OnNewFileButtonClick(object sender, RoutedEventArgs e)
        {

        }

        /// <summary>
        /// Occurs when OpenFileButton is clicked.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The data of the event.</param>
        private async void OnOpenFileButtonClick(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog()
            {
                Filter = FileDialogFilter
            };

            bool? result = openFileDialog.ShowDialog();

            if (!result.HasValue || !result.Value)
            {
                return;
            }

            _selectedFile = openFileDialog.FileName;
            Title = string.Format(WindowTitleFormat, Path.GetFileNameWithoutExtension(_selectedFile));
            await ReadAndShowFileAsync();
        }

        /// <summary>
        /// Occurs when SaveFileButton is clicked.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The data of the event.</param>
        private async void OnSaveFileButtonClick(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(_selectedFile))
            {
                await SaveAsFileAsync();
                return;
            }

            await SaveFileAsync();
        }

        /// <summary>
        /// Occurs when SaveAsFileButton is clicked.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The data of the event.</param>
        private async void OnSaveAsFileButtonClick(object sender, RoutedEventArgs e)
        {
            await SaveAsFileAsync();
        }

        /// <summary>
        /// Occurs when a item within the ErrorListView is double-clicked.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The data of the event.</param>
        private void OnErrorListViewItemPreviewMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var model = (sender as ListViewItem).Content as ErrorViewModel;

            if (model == null)
            {
                return;
            }
            
            Editor.ScrollToLine(model.Position.Line - 1);
            Editor.Select(model.Position.Index, 0);
            //Forces the focus!
            Dispatcher.BeginInvoke(DispatcherPriority.Input, new Action(delegate ()
            {
                Editor.Focus();
                Keyboard.Focus(Editor);
            }));
        }

        /// <summary>
        /// Occurs when a key is pressed.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The data of the event.</param>
        private async void OnMainWindowPreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.S && Keyboard.Modifiers == ModifierKeys.Control)
            {
                await SaveFileAsync();
            }

            if (e.Key == Key.F6)
            {
                await CompileAsync();
            }
        }

        /// <summary>
        /// Occurs when a key is pressed inside Editor.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The data of the event.</param>
        private void OnEditorPreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Z && Keyboard.Modifiers == ModifierKeys.Control)
            {
                _modificationsCount--;
                UpdateSaveButtons();
                _hasTextChanged = false;
            }
            else if (e.Key == Key.Y && Keyboard.Modifiers == ModifierKeys.Control)
            {
                _modificationsCount++;
                UpdateSaveButtons();
                _hasTextChanged = false;
            }
        }

        /// <summary>
        /// Occurs when CompileButton is pressed.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The data of the event.</param>
        private async void OnCompileButtonClick(object sender, RoutedEventArgs e)
        {
            await CompileAsync();
        }

        private async void Refresh_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(_selectedFile))
            {
                await RefreshFileAsync(_selectedFile);

                return;
            }
        }

        private void OnEditorTextChanged(object sender, EventArgs e)
        {
            if (!_hasTextChanged)
            {
                _hasTextChanged = true;
                return;
            }

            _modificationsCount++;
            UpdateSaveButtons();
        }
    }
}
