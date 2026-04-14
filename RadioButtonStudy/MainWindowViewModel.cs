using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace RadioButtonStudy;

public class MainWindowViewModel : INotifyPropertyChanged, IPropertiesChangedListener
{
    #region 基底クラスを設け、そちらに移すべきもの
    /// <summary>
    /// INotifyPropertyChanged の実装<br/>
    /// プロパティ更新リスナーであるVのリスト
    /// Vはここに自分をリスナーとして登録する。
    /// </summary>
    public event PropertyChangedEventHandler? PropertyChanged;

    /// <summary>
    /// Vに対してプロパティ群の更新を通知する
    /// </summary>
    /// <param name="propertyNames"></param>
    protected void NotifyPropertiesChanged(IEnumerable<string> propertyNames)
    {
        foreach (var name in propertyNames)
            NotifyPropertyChanged(name);
    }

    /// <summary>
    /// Vに対してプロパティの更新を通知する
    /// </summary>
    /// <param name="propertyName"></param>
    void NotifyPropertyChanged([CallerMemberName] string? propertyName = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    #endregion 基底クラスを設け、そちらに移すべきもの

    #region メンバ変数
    readonly MyModel _model;
    ChangeTarget _changeTarget = ChangeTarget.All;
    RgbColor _rgbColor = RgbColor.Red;
    #endregion メンバ変数

    #region Vに開陳するプロパティ
    public string FirstName { get; private set; } = "";
    public string MiddleName { get; private set; } = "";
    public string LastName { get; private set; } = "";
    public string DisplayName { get; private set; } = "";
    public RelayCommand ChangeNameRandomlyCommand { get; }
    public ChangeTarget ChangeTarget
    {
        get => _changeTarget;
        set => _model.SetChangeTarget(value);
    }
    public RgbColor RgbColor
    {
        get => _rgbColor;
        set => _model.SetRgbColor(value);
    }
    #endregion Vに開陳するプロパティ

    public MainWindowViewModel(MyModel model)
    {
        _model = model;
        model.AddListener(this, ["FirstName", "MiddleName", "LastName", "ChangeTarget", "RgbColor"]);

        ChangeNameRandomlyCommand = new RelayCommand(() => model.ChangeNameRandomly());
    }

    /// <summary>
    /// IPropertiesChangedListener の実装
    /// Mからの通知を受け、このクラスが開陳するプロパティを更新し、Vに通知する。
    /// </summary>
    /// <param name="changedProperties"></param>
    public void OnPropertiesChanged(List<ChangedProperty> changedProperties)
    {
        var changedNames = new HashSet<string>();
        foreach (var changed in changedProperties)
        {
            switch (changed.Name)
            {
                case "FirstName":
                    FirstName = (string)changed.Value;
                    UpdateDisplayName();
                    changedNames.Add(nameof(FirstName));
                    changedNames.Add(nameof(DisplayName));
                    break;
                case "MiddleName":
                    MiddleName = (string)changed.Value;
                    UpdateDisplayName();
                    changedNames.Add(nameof(MiddleName));
                    changedNames.Add(nameof(DisplayName));
                    break;
                case "LastName":
                    LastName = (string)changed.Value;
                    UpdateDisplayName();
                    changedNames.Add(nameof(LastName));
                    changedNames.Add(nameof(DisplayName));
                    break;
                case "ChangeTarget":
                    _changeTarget = (ChangeTarget)changed.Value;
                    changedNames.Add(nameof(ChangeTarget));
                    break;
                case "RgbColor":
                    _rgbColor = (RgbColor)changed.Value;
                    changedNames.Add(nameof(RgbColor));
                    break;
                default:
                    throw new NotImplementedException();
            }
        }
        NotifyPropertiesChanged(changedNames);
    }

    /// <summary>
    /// 表示用の名前を更新する。
    /// 
    /// ファーストネームしかない場合は、"First"のように整形する。
    /// ミドルネームがない場合は、"First Last"。
    /// ミドルネームもある場合は、"First M. Last"。
    /// </summary>
    void UpdateDisplayName()
    {
        if (LastName.Length == 0)
            DisplayName = Capitalize(FirstName);
        else if (MiddleName.Length == 0)
            DisplayName = $"{Capitalize(FirstName)} {Capitalize(LastName)}";
        else
            DisplayName = $"{Capitalize(FirstName)} {char.ToUpper(MiddleName[0])}. {Capitalize(LastName)}";
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
}
