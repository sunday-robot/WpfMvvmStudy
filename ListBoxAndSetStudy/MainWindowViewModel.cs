using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using static ListBoxAndSetStudy.CollectionDifference;

namespace ListBoxAndSetStudy;

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
    /// IPropertiesChangedListener の実装
    /// Mからの通知を受け、VMがVに対して開陳するプロパティを更新し、Vに通知する。
    /// </summary>
    /// <param name="modelProperties"></param>
    public void OnPropertiesChanged(List<ChangedProperty> modelProperties)
    {
        Application.Current.Dispatcher.Invoke(() =>
        {
            var viewModelPropertyNames = new List<string>();

            // VMがVに対して開陳するプロパティを更新する。
            foreach (var changedModelProperty in modelProperties)
                UpdateViewModelProperties(changedModelProperty, viewModelPropertyNames);

            // Vに対してVMのプロパティ群の更新を通知する。
            NotifyPropertiesChanged(viewModelPropertyNames);
        });
    }

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
    private string? _selectedFriendName;
    string _inputNameText = "";
    #endregion メンバ変数

    #region Vに開陳するプロパティ
    public ObservableCollection<string> FriendNames { get; } = [];
    public string? SelectedFriendName
    {
        get => _selectedFriendName;
        set
        {
            if (_selectedFriendName != value)
            {
                _selectedFriendName = value;
                RemoveNameCommand.RaiseCanExecuteChanged();
            }
        }
    }
    public string InputNameText
    {
        get => _inputNameText;
        set
        {
            _inputNameText = value;
            AddNameCommand.RaiseCanExecuteChanged();
        }
    }
    public RelayCommand AddNameCommand { get; }
    public RelayCommand RemoveNameCommand { get; }
    #endregion Vに開陳するプロパティ

    public MainWindowViewModel(MyModel model)
    {
        _model = model;
        model.AddListener(this, ["FriendNames"]);

        AddNameCommand = new RelayCommand(
            () => model.AddName(InputNameText),
            () => !string.IsNullOrWhiteSpace(InputNameText));

        RemoveNameCommand = new RelayCommand(
            () => model.RemoveName(SelectedFriendName ?? ""),
            () => SelectedFriendName != null);
    }

    #region 基底クラスで中小メソッドして定義し、サブクラスでそれを実装すべきもの
    void UpdateViewModelProperties(ChangedProperty modelProperty, ICollection<string> viewModelPropertyNames)
    {
        switch (modelProperty.Name)
        {
            case "FriendNames":
                foreach (var diff in (IEnumerable<CollectionDifference>)modelProperty.Value)
                {
                    switch (diff)
                    {
                        case Add add:
                            FriendNames.Add(Capitalize((string)add.Value));
                            break;
                        case Update update:
                            var index = FriendNames.IndexOf(Capitalize((string)update.OldValue));
                            FriendNames[index] = Capitalize((string)update.NewValue);
                            break;
                        case Delete delete:
                            FriendNames.Remove(Capitalize((string)delete.Value));
                            break;
                    }
                }
                viewModelPropertyNames.Add(nameof(FriendNames));
                break;
            default:
                throw new NotImplementedException($"Unknown model property name: {modelProperty.Name}");
        }
    }
    #endregion 基底クラスで中小メソッドして定義し、サブクラスでそれを実装すべきもの

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
