using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Windows;

namespace ListBoxAndSetStudy;

public class MainWindowViewModel : INotifyPropertyChanged, IModelPropertiesChangedListener
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
    public void OnPropertiesChanged(List<ModelPropertyDifference> modelPropertyDifferences)
    {
        Application.Current.Dispatcher.Invoke(() =>
        {
            // VMのプロパティを更新する
            var viewModelPropertyNames = new HashSet<string>();
            foreach (var modelPropertyDifference in modelPropertyDifferences)
                UpdateViewModelProperties(modelPropertyDifference, viewModelPropertyNames);

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
        model.AddListener(this, ["FriendNames"]);

        AddNameCommand = new RelayCommand(
            () => model.AddName(InputNameText),
            () => !string.IsNullOrWhiteSpace(InputNameText));

        RemoveNameCommand = new RelayCommand(
            () => model.RemoveName(SelectedFriendName ?? ""),
            () => SelectedFriendName != null);
    }

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

    #region 基底クラスで抽象メソッドとして定義し、サブクラスでそれを実装すべきもの
    /// <summary>
    /// Mのデータモデル更新内容を、VMのプロパティに反映する。
    /// また、更新したVMのプロパティ名群をviewModelPropertyNamesに追加する。
    /// </summary>
    /// <param name="modelProperty"></param>
    /// <param name="viewModelPropertyNames"></param>
    /// <exception cref="NotImplementedException"></exception>
    void UpdateViewModelProperties(ModelPropertyDifference modelPropertyDifference, HashSet<string> viewModelPropertyNames)
    {
        switch (modelPropertyDifference.Name)
        {
            case "FriendNames":
                switch (modelPropertyDifference)
                {
                    case ModelPropertyDifference.Set.Add add:
                        FriendNames.Add(Capitalize((string)add.Value));
                        break;
                    case ModelPropertyDifference.Set.Delete delete:
                        FriendNames.Remove(Capitalize((string)delete.Value));
                        break;
                    default:
                        throw new UnreachableException();
                }
                viewModelPropertyNames.Add(nameof(FriendNames));
                break;
            default:
                throw new NotImplementedException($"Unknown model property name: {modelPropertyDifference.Name}");
        }
    }
    #endregion 基底クラスで抽象メソッドとして定義し、サブクラスでそれを実装すべきもの

    #region privateメンバ
    readonly MyModel _model;
    string? _selectedFriendName;
    string _inputNameText = "";
    #endregion privateメンバ

    #region privateメソッド
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
