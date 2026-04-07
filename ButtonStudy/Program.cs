using System.Windows.Threading;

namespace IdealAppStudy
{
    class Program
    {
        [STAThread]
        public static void Main()
        {
            var model = new MyModel
            {
                FirstName = "ARTHUR",
                MiddleName = "CHARLES",
                LastName = "CLARKE"
            };

            var viewModel = new MainWindowViewModel(model);
            var window = new MainWindow(viewModel);
            window.Closed += (s, e) => Dispatcher.ExitAllFrames();
            window.Show();

            Dispatcher.Run(); // メッセージループ開始
        }
    }
}