using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;

namespace CheckBoxStudy;

public sealed class MainWindowViewModel : INotifyPropertyChanged, IPropertiesChangedListener
{
    #region 基底クラスを設け、そちらに移すべきもの
    /// <summary>
    /// INotifyPropertyChanged の実装<br/>
    /// プロパティ更新リスナーであるVのリスト
    /// Vはここに自分をリスナーとして登録する。
    /// </summary>
    public event PropertyChangedEventHandler? PropertyChanged;

    /// <summary>
    /// IPropertiesChangedListener の実装
    /// Mから通知されたデータモデル更新内容を、VMのプロパティに反映し、Vに通知する。
    /// </summary>
    /// <param name="modelProperties"></param>
    public void OnPropertiesChanged(List<ChangedProperty> modelProperties)
    {
        Application.Current.Dispatcher.Invoke(() =>
        {
            // VMのプロパティを更新する
            var viewModelPropertyNames = new HashSet<string>();
            foreach (var changedModelProperty in modelProperties)
                UpdateViewModelProperties(changedModelProperty, viewModelPropertyNames);

            // 更新されたVMのプロパティ名群をVに通知する。
            NotifyPropertiesChanged(viewModelPropertyNames);
        });
    }

    /// <summary>
    /// Vに対してプロパティ群の更新を通知する
    /// </summary>
    /// <param name="propertyNames"></param>
    void NotifyPropertiesChanged(IEnumerable<string> propertyNames)
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

    public MainWindowViewModel(MyModel model)
    {
        _model = model;
        model.AddListener(this, ["FirstName", "MiddleName", "LastName", "IsChangeFirstName", "IsChangeMiddleName", "IsChangeLastName"]);

        ChangeNameRandomlyCommand = new RelayCommand(() => model.ChangeNameRandomly());
    }

    #region Vに開陳するプロパティ
    public string FirstName { get; private set; } = "";
    public string MiddleName { get; private set; } = "";
    public string LastName { get; private set; } = "";
    public string DisplayName { get; private set; } = "";
    public RelayCommand ChangeNameRandomlyCommand { get; }
    public bool IsChangeTargetSelectable
    {
        get => _isChangeTargetSelectable;
        set
        {
            if (_isChangeTargetSelectable != value)
            {
                _isChangeTargetSelectable = value;
                NotifyPropertyChanged(nameof(IsChangeTargetSelectable));
            }
        }
    }
    public bool IsChangeFirstName
    {
        get => _isChangeFirstName;
        set => _model.SetIsChangeFirstName(value);
    }
    public bool IsChangeMiddleName
    {
        get => _isChangeMiddleName;
        set => _model.SetIsChangeMiddleName(value);
    }
    public bool IsChangeMiddleNameEnabled
    {
        get => _isChangeMiddleName;
        set => _model.SetIsChangeMiddleName(value);
    }
    public bool IsChangeLastName
    {
        get => _isChangeLastName;
        set => _model.SetIsChangeLastName(value);
    }
    #endregion Vに開陳するプロパティ

    #region 基底クラスで抽象メソッドとして定義し、サブクラスでそれを実装すべきもの
    /// <summary>
    /// Mのデータモデル更新内容を、VMのプロパティに反映する。
    /// また、更新したVMのプロパティ名群をviewModelPropertyNamesに追加する。
    /// </summary>
    /// <param name="modelProperty"></param>
    /// <param name="viewModelPropertyNames"></param>
    /// <exception cref="NotImplementedException"></exception>
    void UpdateViewModelProperties(ChangedProperty modelProperty, ISet<string> viewModelPropertyNames)
    {
        switch (modelProperty.Name)
        {
            case "FirstName":
                FirstName = (string)modelProperty.Value;
                UpdateDisplayName();
                viewModelPropertyNames.Add(nameof(FirstName));
                viewModelPropertyNames.Add(nameof(DisplayName));
                break;
            case "MiddleName":
                MiddleName = (string)modelProperty.Value;
                UpdateDisplayName();
                viewModelPropertyNames.Add(nameof(MiddleName));
                viewModelPropertyNames.Add(nameof(DisplayName));
                break;
            case "LastName":
                LastName = (string)modelProperty.Value;
                UpdateDisplayName();
                viewModelPropertyNames.Add(nameof(LastName));
                viewModelPropertyNames.Add(nameof(DisplayName));
                break;
            case "IsChangeFirstName":
                _isChangeFirstName = (bool)modelProperty.Value;
                viewModelPropertyNames.Add(nameof(IsChangeFirstName));
                break;
            case "IsChangeMiddleName":
                _isChangeMiddleName = (bool)modelProperty.Value;
                viewModelPropertyNames.Add(nameof(IsChangeMiddleName));
                break;
            case "IsChangeLastName":
                _isChangeLastName = (bool)modelProperty.Value;
                viewModelPropertyNames.Add(nameof(IsChangeLastName));
                break;
            default:
                throw new NotImplementedException();
        }
    }
    #endregion 基底クラスで抽象メソッドとして定義し、サブクラスでそれを実装すべきもの

    #region privateメンバ
    readonly MyModel _model;
    bool _isChangeTargetSelectable = true;
    bool _isChangeFirstName;
    bool _isChangeMiddleName;
    bool _isChangeLastName;
    #endregion privateメンバ

    #region privateメソッド
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
    #endregion privateメソッド
}
