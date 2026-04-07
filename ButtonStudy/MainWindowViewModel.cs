using System.ComponentModel;

namespace IdealAppStudy;

public class MainWindowViewModel : INotifyPropertyChanged
{
    private readonly MyModel _model;

    public MainWindowViewModel(MyModel model)
    {
        _model = model;
        _model.PropertyChanged += (s, e) =>
        {
            switch (e.PropertyName)
            {
                case nameof(_model.FirstName):
                    OnPropertyChanged(nameof(FirstName));
                    OnPropertyChanged(nameof(DisplayName));
                    break;
                case nameof(_model.MiddleName):
                    OnPropertyChanged(nameof(MiddleName));
                    OnPropertyChanged(nameof(DisplayName));
                    break;
                case nameof(_model.LastName):
                    OnPropertyChanged(nameof(LastName));
                    OnPropertyChanged(nameof(DisplayName));
                    break;
            }
        };
    }

    public string FirstName => _model.FirstName;
    public string MiddleName => _model.MiddleName.Length == 0 ? "(empty)" : _model.MiddleName;
    public string LastName => _model.LastName.Length == 0 ? "(empty)" : _model.LastName;

    /// <summary>
    /// 表示用の名前を返す。（設定はできない。）
    /// 
    /// ファーストネームしかない場合は、"First"のように整形する。
    /// ミドルネームがない場合は、"First Last"。
    /// ミドルネームもある場合は、"First M. Last"。
    /// </summary>
    public string DisplayName
    {
        get
        {
            if (_model.LastName.Length == 0)
                return Capitalize(_model.FirstName);
            if (_model.MiddleName.Length == 0)
                return $"{Capitalize(_model.FirstName)} {Capitalize(_model.LastName)}";
            return $"{Capitalize(_model.FirstName)} {char.ToUpper(_model.MiddleName[0])}. {Capitalize(_model.LastName)}";
        }
    }

    /// <summary>
    /// nameを先頭大文字、残り小文字に変換する
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    static string Capitalize(string name)
    {
        if (string.IsNullOrWhiteSpace(name)) return "";
        return char.ToUpper(name[0]) + name[1..].ToLower();
    }

    public void ChangeNameRandomly() => _model.ChangeNameRandomly();

    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged(string name) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}
