using System.Collections.Generic;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Folding;

namespace IniEditor.AvalonEdit
{
    ///<summary>INI折り畳み</summary>
    public class IniFoldingStrategy
    {
        public void UpdateFoldings(FoldingManager manager, TextDocument document)
        {
            var foldings = CreateNewFoldings(document, out var firstErrorOffset);
            manager.UpdateFoldings(foldings, firstErrorOffset);
        }

        private IEnumerable<NewFolding> CreateNewFoldings(TextDocument document, out int firstErrorOffset)
        {
            firstErrorOffset = -1;
            return CreateNewFoldings(document);
        }

        private IEnumerable<NewFolding> CreateNewFoldings(TextDocument document)
        {
            var newFoldings = new List<NewFolding>();
            var startOffset = 0;
            var isFolding = false;

            // 行単位でチェック
            foreach(var line in document.Lines)
            {
                var text = document.GetText(line.Offset, line.Length).Trim();

                // セクション
                // NewFolding endを調整で折り畳み時の見えてる範囲を制御可能
                if(text.StartsWith("["))
                {
                    if(isFolding)
                    {
                        newFoldings.Add(new NewFolding(startOffset, line.Offset - 2)); // -2 = \r\n
                    }

                    startOffset = line.Offset + line.Length;
                    isFolding = true;
                }

                // コメント
                // 連続するコメント行の折り畳みはしない
                // セクション行の上にあるコメント行は
                // セクションのコメントとみなし自身を含めずに折り畳み終了
                // パラメータ行の中にある場合は特に何もしない
                if(text.StartsWith(";"))
                {
                    if(isFolding)
                    {
                        var next = line.NextLine;
                        var nextText = next == null ? "" : document.GetText(next.Offset, next.Length).Trim();
                        if(nextText.StartsWith("["))
                        {
                            newFoldings.Add(new NewFolding(startOffset, line.Offset - 2)); // -2 = \r\n
                            startOffset = line.Offset + line.Length;
                        }
                    }
                }
            }

            if(isFolding)
            {
                newFoldings.Add(new NewFolding(startOffset, document.TextLength));
            }

            return newFoldings;
        }
    }
}
