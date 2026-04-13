using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ButtonStudy;

public class MainWindowViewModel : INotifyPropertyChanged
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
    void NotifyPropertyChanged([CallerMemberName] string? propertyName = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    #endregion 基底クラスを設け、そちらに移すべきもの

    #region メンバ変数
    readonly MyModel _model;

    /// <summary>
    /// 主役ボタンのenable/disable状態を保持するフィールド。
    /// </summary>
    bool _canChangeNameRandomlyCommandExecute = true;
    #endregion メンバ変数

    #region Vに開陳するプロパティ
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
    /// このサンプルでの主役のボタン用のICommand。
    /// </summary>
    public RelayCommand ChangeNameRandomlyCommand { get; }
    /// <summary>
    /// 上のボタンのenabled/disabledを切り替えるためのボタン用のICommand。
    /// </summary>
    public RelayCommand DisableEnableChangeNameRandomlyCommand { get; }
    #endregion Vに開陳するプロパティ

    public MainWindowViewModel(MyModel model)
    {
        _model = model;
        model.PropertyChanged += (s, e) =>
        {
            switch (e.PropertyName)
            {
                case nameof(model.FirstName):
                    NotifyPropertyChanged(nameof(FirstName));
                    NotifyPropertyChanged(nameof(DisplayName));
                    break;
                case nameof(model.MiddleName):
                    NotifyPropertyChanged(nameof(MiddleName));
                    NotifyPropertyChanged(nameof(DisplayName));
                    break;
                case nameof(model.LastName):
                    NotifyPropertyChanged(nameof(LastName));
                    NotifyPropertyChanged(nameof(DisplayName));
                    break;
            }
        };

        // このサンプルでの主役のボタン用のICommandを生成
        ChangeNameRandomlyCommand = new RelayCommand(() => _model.ChangeNameRandomly(), () => _canChangeNameRandomlyCommandExecute);

        // 上のボタンのenabled/disabledを切り替えるためのボタン用のICommandを生成
        DisableEnableChangeNameRandomlyCommand = new RelayCommand(() =>
        {
            // 主役のボタンの実行可能状態を切り替える(ただメンバ変数を変更するだけでは当然ダメなので、下のRaiseCanExecuteChangedも必要)
            _canChangeNameRandomlyCommandExecute = !_canChangeNameRandomlyCommandExecute;
            ChangeNameRandomlyCommand.RaiseCanExecuteChanged();
        });
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
