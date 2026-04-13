using System.Windows.Threading;

namespace LabelStudy;

class Program
{
    [STAThread]
    public static void Main()
    {
        var model = new MyModel
        {
            FirstName = "arthur",
            MiddleName = "charles",
            LastName = "clarke"
        };

        var viewModel = new MainWindowViewModel(model);
        var window = new MainWindow(viewModel);
        window.Closed += (s, e) => Dispatcher.ExitAllFrames();
        window.Show();

        Dispatcher.Run(); // メッセージループ開始
    }
}