using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace TextBoxStudy;

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
    /// Vに対してプロパティの更新を通知する
    /// </summary>
    /// <param name="propertyName"></param>
    private void NotifyPropertyChanged([CallerMemberName] string? propertyName = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    #endregion 基底クラスを設け、そちらに移すべきもの

    #region メンバ変数
    readonly MyModel _model;    // TODO Mへの参照だが、必要ないのかもしれない。少なくともこのサンプルアプリでは不要である。
    string _inputNameText = "";
    #endregion メンバ変数

    #region Vに開陳するプロパティ
    public string DisplayName { get; private set; } = "";
    public string RawName { get; private set; } = "";
    public string InputNameText
    {
        get => _inputNameText;
        set
        {
            _inputNameText = value;
            SetNameCommand.RaiseCanExecuteChanged();
        }
    }
    public RelayCommand SetNameCommand { get; }
    public RelayCommand ClearInputNameTextCommand { get; }
    #endregion Vに開陳するプロパティ

    public MainWindowViewModel(MyModel model)
    {
        _model = model;
        model.AddListener(this, ["Name"]);

        SetNameCommand = new RelayCommand(() => model.SetName(InputNameText), () => !String.IsNullOrWhiteSpace(InputNameText));
        ClearInputNameTextCommand = new RelayCommand(() =>
        {
            InputNameText = "";
            NotifyPropertyChanged(nameof(InputNameText));
        });
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
                case "Name":
                    RawName = (string)changed.Value;
                    DisplayName = Capitalize((string)changed.Value);
                    changedNames.Add(nameof(RawName));
                    changedNames.Add(nameof(DisplayName));
                    break;
            }
        }
        foreach (var name in changedNames)
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
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
