using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using System.Xml;
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.CodeCompletion;
using ICSharpCode.AvalonEdit.Folding;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Highlighting.Xshd;
using IniEditor.AvalonEdit;
using IniEditor.Models;
using IniEditor.Util;
using IniWrapper.Settings;
using IniWrapper.Wrapper;
using Microsoft.Win32;

namespace IniEditor
{
    public partial class MainWindow : Window
    {
        private const string filter = "INIファイル (*.ini)|*.ini|全てのファイル (*.*)|*.*";

        private readonly FoldingManager foldingManager;
        private readonly IniFoldingStrategy foldingStrategy = new IniFoldingStrategy();
        private readonly IIniWrapper iniWrapper;

        private ViewModel vm => (ViewModel)DataContext;

        private CompletionWindow completionWindow;


        public MainWindow()
        {
            InitializeComponent();

            var exePath = Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName);
            var settingsPath = Path.Combine(exePath, "settings.ini");

            var iniSettings = new IniSettings() { IniFilePath = settingsPath, };
            iniWrapper = new IniWrapperFactory().CreateWithDefaultIniParser(iniSettings);

            if(!File.Exists(settingsPath))
            {
                File.WriteAllText(settingsPath, ";this file is IniEditor settings file.");
                iniWrapper.SaveConfiguration(new ViewModel());
            }

            DataContext = iniWrapper.LoadConfiguration<ViewModel>();

            // 強調表示
            textEditor.SyntaxHighlighting = LoadHighlightingDefinition("AvalonEdit.ini.xshd");

            // 折り畳み
            foldingManager = FoldingManager.Install(textEditor.TextArea);
            // 2秒おきに折り畳みチェック（微妙な気がするがサンプルがそうなっている）
            var foldingTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(2) };
            foldingTimer.Tick += (s, e)
                => foldingStrategy.UpdateFoldings(foldingManager, textEditor.Document);
            foldingTimer.Start();

            // コード補完
            textEditor.TextArea.TextEntering += TextArea_TextEntering;
            textEditor.TextArea.TextEntered += TextArea_TextEntered;


            textEditor.Options = new TextEditorOptions
            {
                AllowScrollBelowDocument = true,
                HighlightCurrentLine = true,
            };

            vm.FilePath = settingsPath;
            textEditor.Load(vm.FilePath);
        }

        private void TextArea_TextEntering(object sender, TextCompositionEventArgs e)
        {
            if(e.Text.Length == 0 || completionWindow == null) return;
            if(char.IsLetterOrDigit(e.Text[0])) return;

            completionWindow.CompletionList.RequestInsertion(e);
        }

        // familyかfontが含まれる行で=を打った時にインストールされているフォントをサジェスト
        private void TextArea_TextEntered(object sender, TextCompositionEventArgs e)
        {
            if(e.Text != "=") return;

            var caret = textEditor.TextArea.Caret;
            // 行頭からキャレット位置-1までのテキスト（おそらく^^;）
            var text = textEditor.Document.GetText(caret.Offset - caret.Column + 1, caret.Column - 2).ToLower();
            if(!text.Contains("family") && !text.Contains("font")) return;

            completionWindow = new CompletionWindow(textEditor.TextArea)
            {
                SizeToContent = SizeToContent.Width,
            };
            completionWindow.Closed += (_, __) => completionWindow = null;

            var data = completionWindow.CompletionList.CompletionData;
            foreach(var item in SystemFontsModel.SystemFontNames)
            {
                data.Add(new MyCompletionData(item));
            }

            completionWindow.Show();
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            // 本来最後に自身の設定ファイルを保存するが
            // settings.iniを開いていた場合、編集が反映されないので
            // 保存後に編集結果で上書きする
            iniWrapper.SaveConfiguration(DataContext);
            if(SaveCancel()) e.Cancel = true;
        }

        private void NewCmdExecuted(object target, ExecutedRoutedEventArgs e)
        {
            if(SaveCancel()) return;
            New();
        }
        private void OpenCmdExecuted(object target, ExecutedRoutedEventArgs e)
        {
            if(SaveCancel()) return;
            Open();
        }

        private void SaveCmdExecuted(object target, ExecutedRoutedEventArgs e) => Save();

        private void New()
        {
            vm.FilePath = null;
            textEditor.Clear();
            textEditor.IsModified = false;
        }

        private void Open()
        {
            var dialog = new OpenFileDialog { Title = $"開く - {Product.Name}", Filter = filter, };
            if(dialog.ShowDialog() == true)
            {
                vm.FilePath = dialog.FileName;
                textEditor.Load(vm.FilePath);
            }
        }

        private void Save()
        {
            if(vm.FilePath == null) SaveAs();
            else textEditor.Save(vm.FilePath);
        }

        private void SaveAs()
        {
            var dialog = new SaveFileDialog { Title = $"名前を付けて保存 - {Product.Name}", Filter = filter, };
            if(dialog.ShowDialog() == true)
            {
                vm.FilePath = dialog.FileName;
                textEditor.Save(vm.FilePath);
            }
        }

        private bool SaveCancel()
        {
            if(textEditor.IsModified)
            {
                var r = MessageBox.Show("変更内容を保存しますか？", Product.Name, MessageBoxButton.YesNoCancel);
                if(r == MessageBoxResult.Yes) Save();
                if(r == MessageBoxResult.Cancel) return true;
            }

            return false;
        }


        // xshdを埋め込みリソースから読む
        private static IHighlightingDefinition LoadHighlightingDefinition(string resourceName)
        {
            var type = typeof(MainWindow);
            var fullName = type.Namespace + "." + resourceName;
            using(var stream = type.Assembly.GetManifestResourceStream(fullName))
            using(var reader = new XmlTextReader(stream))
                return HighlightingLoader.Load(reader, HighlightingManager.Instance);
        }
    }
}
