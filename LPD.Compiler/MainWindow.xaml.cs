using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Highlighting.Xshd;
using LPD.Compiler.Helpers;
using LPD.Compiler.Lexical;
using LPD.Compiler.Shared;
using LPD.Compiler.Syntactic;
using LPD.Compiler.ViewModel;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace LPD.Compiler
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        #region Constants

        private const string WindowTitleFormat = "LPD COMPILER - {0}";
        private const string FileDialogFilter = "Arquivo LPD,TXT|*.lpd;*.txt| Todos os arquivos|*.*";
        private const string VirtualMachineName = "LPD Virtual Machine";
        private const string VirtualMachineExeName = "LPD.VirtualMachine.exe";

        private const string SaveButtonEnabledSource = "Images/Save.png";
        private const string SaveButtonDisabledSource = "Images/Save_Disabled.png";
        private const string SaveAsButtonEnabledSource = "Images/SaveAs.png";
        private const string SaveAsButtonDisabledSource = "Images/SaveAs_Disabled.png";
        private const string CompileButtonDisabledSource = "Images/Compile_Disabled.png";
        private const string CompileButtonEnabledSource = "Images/Compile.png";
        private const string ExecuteButtonEnabledSource = "Images/Execute.png";
        private const string ExecuteButtonDisabledSource = "Images/Execute_Disabled.png";

        #endregion

        private string _selectedFile;
        private string _outputFile;
        private string _vmInstallationPath = null;
        private bool _hasTextChanged = true;
        private ushort _modificationsCount = 0;

        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindow"/> class.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            Loaded += OnLoaded;
            Editor.SyntaxHighlighting = HighlightingLoader.Load(FileHelper.GetSyntaxHighlighting(), HighlightingManager.Instance);
        }

        private async void OnLoaded(object sender, RoutedEventArgs e)
        {
            if (App.ArgumentFilePath != null)
            {
                _selectedFile = App.ArgumentFilePath;
                await ReadAndShowFileAsync();
            }

            UpdateSaveButtons();
            _vmInstallationPath = RegistryHelper.GetProgramInstallationPath(VirtualMachineName);
            UpdateExecuteButton(_vmInstallationPath != null);
            UpdateCompileExecuteButtons();
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
            TokensList.ItemsSource = null;
            ErrorListView.ItemsSource = null;
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
            Title = string.Format(WindowTitleFormat, Path.GetFileNameWithoutExtension(_selectedFile));
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
            Title = string.Format(WindowTitleFormat, Path.GetFileNameWithoutExtension(_selectedFile));
        }

        /// <summary>
        /// Updates ExecuteButton.
        /// </summary>
        private void UpdateExecuteButton(bool isEnabled)
        {
            Image executeButtonContent = ExecuteButton.Content as Image;

            if (isEnabled)
            {
                executeButtonContent.Source = new BitmapImage(new Uri(ExecuteButtonEnabledSource, UriKind.Relative));
                ExecuteButton.IsEnabled = isEnabled;
            }
            else
            {
                executeButtonContent.Source = new BitmapImage(new Uri(ExecuteButtonDisabledSource, UriKind.Relative));
                ExecuteButton.IsEnabled = false;
            }
        }

        /// <summary>
        /// Updates CompileButton.
        /// </summary>
        private void UpdateCompileExecuteButtons()
        {
            Image compileButtonContent = CompileButton.Content as Image;

            if (Editor.Text.Length > 0)
            {
                if (_vmInstallationPath != null)
                {
                    UpdateExecuteButton(true);
                }

                compileButtonContent.Source = new BitmapImage(new Uri(CompileButtonEnabledSource, UriKind.Relative));
                CompileButton.IsEnabled = true;
            }

            else
            {
                if (_vmInstallationPath != null)
                {
                    UpdateExecuteButton(false);
                }

                compileButtonContent.Source = new BitmapImage(new Uri(CompileButtonDisabledSource, UriKind.Relative));
                CompileButton.IsEnabled = false;
            }
        }

        /// <summary>
        /// Updates SaveButton and SaveAsButton.
        /// </summary>
        private void UpdateSaveButtons()
        {
            Image saveButtonContent = SaveButton.Content as Image;
            Image saveAsButtonContent = SaveAsButton.Content as Image;

            if (_modificationsCount > 0)
            {
                saveButtonContent.Source = new BitmapImage(new Uri(SaveButtonEnabledSource, UriKind.Relative));
                saveAsButtonContent.Source = new BitmapImage(new Uri(SaveAsButtonEnabledSource, UriKind.Relative));
                SaveButton.IsEnabled = SaveAsButton.IsEnabled = true;
            }
            else
            {
                saveButtonContent.Source = new BitmapImage(new Uri(SaveButtonDisabledSource, UriKind.Relative));
                saveAsButtonContent.Source = new BitmapImage(new Uri(SaveAsButtonDisabledSource, UriKind.Relative));
                SaveButton.IsEnabled = SaveAsButton.IsEnabled = false;
            }
        }

        /// <summary>
        /// Compiles the source code.
        /// </summary>
        /// <returns><see cref="Task"/></returns>
        private async Task<bool> CompileAsync()
        {
            if (string.IsNullOrEmpty(Editor.Text))
            {
                return false;
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

            TokensList.ItemsSource = null;
            ErrorListView.ItemsSource = null;

            var success = false;

            using (LexicalAnalyzer lexical = new LexicalAnalyzer(_selectedFile))
            {
                SyntacticAnalyzer syntactic = new SyntacticAnalyzer(lexical);
                CompilationResult compilationResult = await syntactic.DoAnalysisAsync();

                if (compilationResult.Error != null)
                {
                    ErrorListView.ItemsSource = new List<ErrorViewModel>
                    {
                        new ErrorViewModel()
                        {
                            Message = compilationResult.Error.Message,
                            Position = compilationResult.Error.Position
                        }
                    };
                }
                else
                {
                    _outputFile = compilationResult.AssemblyFilePath;
                    success = true;
                }

                TokensList.ItemsSource = lexical.ReadTokens;
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

            return success;
        }

        /// <summary>
        /// Opens the virtual machine with the current file, if the VM is installed.
        /// </summary>
        /// <returns><see cref="Task"/>.</returns>
        private async Task LaunchVirtualMachineAsync()
        {
            if (_vmInstallationPath != null)
            {
                var success = await CompileAsync();

                if (success)
                {
                    var exePath = Path.Combine(_vmInstallationPath, VirtualMachineExeName);

                    ProcessHelper.StartProcess(exePath, _outputFile);
                }
            }
        }

        /// <summary>
        /// Saves the modified file or asks the user to create a new one.
        /// </summary>
        /// <returns><see cref="Task"/>.</returns>
        private async Task SaveAsync()
        {
            if (string.IsNullOrEmpty(_selectedFile))
            {
                await SaveAsFileAsync();
                return;
            }

            await SaveFileAsync();
            Title = string.Format(WindowTitleFormat, Path.GetFileNameWithoutExtension(_selectedFile));
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
            await SaveAsync();
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

            var offset = Editor.Document.GetOffset(model.Position.Line, model.Position.Column);

            Editor.Select(offset, 0);
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
                await SaveAsync();
            }
            else if (e.Key == Key.F6)
            {
                await CompileAsync();
            }
            else if (e.Key == Key.F5)
            {
                await LaunchVirtualMachineAsync();
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
                UpdateCompileExecuteButtons();
                _hasTextChanged = false;
            }
            else if (e.Key == Key.Y && Keyboard.Modifiers == ModifierKeys.Control)
            {
                _modificationsCount++;
                UpdateSaveButtons();
                UpdateCompileExecuteButtons();
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
            try
            {
                await CompileAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Erro!");
            }
        }

        /// <summary>
        /// Occurs when ExecuteButton is pressed.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The data of the event.</param>
        private async void OnExecuteButtonClick(object sender, RoutedEventArgs e)
        {
            await LaunchVirtualMachineAsync();
        }

        /// <summary>
        /// Occurs then the text in the editor changes.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The data of the event.</param>
        private void OnEditorTextChanged(object sender, EventArgs e)
        {
            if (!_hasTextChanged)
            {
                _hasTextChanged = true;
                return;
            }

            _modificationsCount++;
            UpdateSaveButtons();
            UpdateCompileExecuteButtons();
        }

        private async void OnEditorDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string file = ((string[])e.Data.GetData(DataFormats.FileDrop))[0];

                _selectedFile = file;
                Title = string.Format(WindowTitleFormat, Path.GetFileNameWithoutExtension(_selectedFile));
                await ReadAndShowFileAsync();
            }
        }
    }
}