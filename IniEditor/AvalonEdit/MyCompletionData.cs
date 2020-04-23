using System;
using System.Windows.Media;
using ICSharpCode.AvalonEdit.CodeCompletion;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Editing;

namespace IniEditor.AvalonEdit
{
    internal class MyCompletionData : ICompletionData
    {
        public ImageSource Image => null;

        ///<summary>エディタ挿入される文字列</summary>
        public string Text { get; private set; }

        ///<summary>リストボックスに表示される文字列やUIElement</summary>
        public object Content => Text;

        ///<summary>ツールチップに表示される文字列やUIElement</summary>
        public object Description => null;

        // 絞り込んだ時の優先度？
        public double Priority => 0;

        public MyCompletionData(string text) => Text = text;

        public void Complete(TextArea textArea, ISegment completionSegment, EventArgs insertionRequestEventArgs)
            => textArea.Document.Replace(completionSegment, Text);
    }
}
