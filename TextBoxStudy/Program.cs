using System.Windows.Threading;

namespace TextBoxStudy;

class Program
{
    [STAThread]
    public static void Main()
    {
        var model = new MyModel();

        var viewModel = new MainWindowViewModel(model);
        var window = new MainWindow(viewModel);
        window.Closed += (s, e) => Dispatcher.ExitAllFrames();
        window.Show();

        Dispatcher.Run(); // メッセージループ開始
    }
}