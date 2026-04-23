using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;

namespace TabStudy;

public sealed class MainWindowViewModel : INotifyPropertyChanged, IModelPropertiesChangedListener
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
        model.AddListener(this, ["TabItems"]);

        AddTabCommand = new RelayCommand(() => { _model.AddTabContentRandomly(); });
        DeleteTabCommand = new RelayCommand(() =>
        {
            if (SelectedTab != null)
            {
                var id = _tabItemViewModel2Id[SelectedTab];
                _model.RemoveTab(id);
            }
        });
    }

    #region Vに開陳するプロパティ
    public ObservableCollection<TabItemViewModel> Tabs { get; } = [];
    public TabItemViewModel? SelectedTab
    {
        get => _selectedTab;
        set => _selectedTab = value;
    }
    public RelayCommand AddTabCommand { get; }
    public RelayCommand DeleteTabCommand { get; }
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
            case "TabItems":
                {
                    var mapDifference = (ModelPropertyDifference.Map)modelPropertyDifference;
                    foreach (var key in mapDifference.DeleteKeys)
                    {
                        var id = (int)key;
                        var tabItemViewModel = _id2TabItemViewModel[id];
                        Tabs.Remove(tabItemViewModel);
                        _id2TabItemViewModel.Remove(id);
                        _tabItemViewModel2Id.Remove(tabItemViewModel);
                    }
                    foreach (var (Key, Value) in mapDifference.AddEntries)
                    {
                        var id = (int)Key;
                        var tabItem = (TabItem)Value;
                        var tabItemViewModel = new TabItemViewModel(tabItem.Name, tabItem.Content);
                        Tabs.Add(tabItemViewModel);
                        _id2TabItemViewModel[id] = tabItemViewModel;
                        _tabItemViewModel2Id[tabItemViewModel] = id;
                    }
                    if (mapDifference.AddEntries.Any())
                    {
                        var id = (int)mapDifference.AddEntries.First().Key;
                        SelectedTab = _id2TabItemViewModel[id];
                        viewModelPropertyNames.Add(nameof(SelectedTab));
                    }
                }
                viewModelPropertyNames.Add(nameof(Tabs));
                break;
            default:
                throw new NotImplementedException($"Unknown model property name: {modelPropertyDifference.Name}");
        }
    }
    #endregion 基底クラスで抽象メソッドとして定義し、サブクラスでそれを実装すべきもの

    #region privateメンバ
    readonly MyModel _model;
    TabItemViewModel? _selectedTab;
    readonly Dictionary<int, TabItemViewModel> _id2TabItemViewModel = [];
    readonly Dictionary<TabItemViewModel, int> _tabItemViewModel2Id = [];
    #endregion privateメンバ

    #region privateメソッド
    #endregion privateメソッド
}
