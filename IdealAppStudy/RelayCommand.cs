using System.Windows.Input;

namespace IdealAppStudy;

public class RelayCommand(Action execute, Func<bool>? canExecute = null) : ICommand
{
    /// <summary>
    /// コマンドが実行可能か(ボタンをenbleにするか、disableにするか)を返す。
    /// </summary>
    public bool CanExecute(object? parameter) => canExecute?.Invoke() ?? true;

    public void Execute(object? parameter) => execute();

    public event EventHandler? CanExecuteChanged;

    /// <summary>
    /// CanExecute()の返り値が変わった場合、本メソッドを呼び、Vに通知し、画面更新を行わせる。
    /// </summary>
    public void RaiseCanExecuteChanged()
        => CanExecuteChanged?.Invoke(this, EventArgs.Empty);
}
