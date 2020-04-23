using System.IO;
using IniEditor.Models;
using IniEditor.Util;
using IniWrapper.Attribute;

namespace IniEditor
{

    class ViewModel : Observable
    {
        public WindowModel Window { get => _Window; set => Set(ref _Window, value); }
        private WindowModel _Window = new WindowModel();

        public FontModel Font { get => _Font; set => Set(ref _Font, value); }
        private FontModel _Font = new FontModel();

        [IniIgnore]
        public string FilePath { get => _FilePath; set => Set(ref _FilePath, value); }
        private string _FilePath;

        [IniIgnore]
        public bool IsModified
        {
            get => _IsModified;
            set { if(Set(ref _IsModified, value)) OnPropertyChanged(nameof(Title)); }
        }
        private bool _IsModified;

        [IniIgnore]
        public string Title => $"{(IsModified ? "*" : "")}{fileName} - {Product.Name}";
        private string fileName => Path.GetFileNameWithoutExtension(FilePath) ?? "無題";
    }
}
