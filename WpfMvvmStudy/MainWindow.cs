using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace LabelStudy;

public class MainWindow : Window
{
    public MainWindow(MainWindowViewModel viewModel)
    {
        Width = 300;
        Height = 150;
        Title = "名前表示サンプル";
        Content = CreateContent();
        DataContext = viewModel;
    }

    static StackPanel CreateContent()
    {
        var stackPanel = new StackPanel();
        stackPanel.Children.Add(CreateDisplayNameLabel());
        return stackPanel;
    }

    static Label CreateDisplayNameLabel()
    {
        var label = new Label();
        label.SetBinding(Label.ContentProperty, new Binding("DisplayName") { Mode = BindingMode.OneWay });
        return label;
    }
}
