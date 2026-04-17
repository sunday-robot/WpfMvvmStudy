using System.Windows;

namespace CheckBoxStudy;

class Program
{
    [STAThread]
    public static void Main()
    {
        var app = new Application();

        var model = new MyModel();

        var viewModel = new MainWindowViewModel(model);
        var window = new MainWindow(viewModel);
        window.Show();

        app.Run();
    }
}