using System.ComponentModel;

namespace LabelStudy;

public class MainWindowViewModel : INotifyPropertyChanged
{
    private readonly MyModel _model;

    public MainWindowViewModel(MyModel model)
    {
        _model = model;
        _model.PropertyChanged += (s, e) =>
        {
            if (e.PropertyName == nameof(_model.FirstName) ||
                e.PropertyName == nameof(_model.MiddleName) ||
                e.PropertyName == nameof(_model.LastName))
            {
                OnPropertyChanged(nameof(DisplayName));
            }
        };
    }

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
            if (string.IsNullOrWhiteSpace(_model.FirstName))
                throw new InvalidOperationException("First name is required.");

            if (string.IsNullOrWhiteSpace(_model.LastName))
            {
                if (!string.IsNullOrWhiteSpace(_model.MiddleName))
                    throw new InvalidOperationException("Middle name cannot exist without last name.");

                return Capitalize(_model.FirstName);
            }

            if (string.IsNullOrWhiteSpace(_model.MiddleName))
                return $"{Capitalize(_model.FirstName)} {Capitalize(_model.LastName)}";

            return $"{Capitalize(_model.FirstName)} {Capitalize(_model.MiddleName)[0]}. {Capitalize(_model.LastName)}";
        }
    }

    static string Capitalize(string name)
    {
        if (string.IsNullOrWhiteSpace(name)) return "";
        return char.ToUpper(name[0]) + name[1..].ToLower();
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged(string name) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}
