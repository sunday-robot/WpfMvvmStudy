using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace IdealAppStudy;

public class MainWindow : Window
{
    public MainWindow(MainWindowViewModel viewModel)
    {
        Width = 300;
        Height = 200;
        Title = "名前表示サンプル";
        Content = CreateContent();
        DataContext = viewModel;
    }

    static StackPanel CreateContent()
    {
        var stackPanel = new StackPanel();
        stackPanel.Children.Add(CreateDisplayNameLabel());
        stackPanel.Children.Add(CreateFirstNameLabel());
        stackPanel.Children.Add(CreateMiddleNameLabel());
        stackPanel.Children.Add(CreateLastNameLabel());
        stackPanel.Children.Add(CreateChangeNameButton());
        return stackPanel;
    }

    static Label CreateDisplayNameLabel()
    {
        var label = new Label();
        label.SetBinding(Label.ContentProperty, new Binding("DisplayName") { Mode = BindingMode.OneWay });
        return label;
    }

    static Label CreateFirstNameLabel()
    {
        var label = new Label();
        label.SetBinding(Label.ContentProperty, new Binding("FirstName") { Mode = BindingMode.OneWay });
        return label;
    }

    static Label CreateMiddleNameLabel()
    {
        var label = new Label();
        label.SetBinding(Label.ContentProperty, new Binding("MiddleName") { Mode = BindingMode.OneWay });
        return label;
    }

    static Label CreateLastNameLabel()
    {
        var label = new Label();
        label.SetBinding(Label.ContentProperty, new Binding("LastName") { Mode = BindingMode.OneWay });
        return label;
    }

    static Button CreateChangeNameButton()
    {
        var button = new Button { Content = "Change Name" };
        button.Click += (s, e) => ((MainWindowViewModel)button.DataContext).ChangeNameRandomly();
        return button;
    }
}
