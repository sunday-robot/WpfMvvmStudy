using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace ButtonStudy;

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
        stackPanel.Children.Add(CreateLabel(nameof(MainWindowViewModel.DisplayName)));
        stackPanel.Children.Add(CreateLabel(nameof(MainWindowViewModel.FirstName)));
        stackPanel.Children.Add(CreateLabel(nameof(MainWindowViewModel.MiddleName)));
        stackPanel.Children.Add(CreateLabel(nameof(MainWindowViewModel.LastName)));
        stackPanel.Children.Add(CreateButton("Change Name", nameof(MainWindowViewModel.ChangeNameRandomlyCommand)));
        stackPanel.Children.Add(CreateButton("Disable/Enable Change Name", nameof(MainWindowViewModel.DisableEnableChangeNameRandomlyCommand)));
        return stackPanel;
    }

    static Label CreateLabel(string propertyName)
    {
        var label = new Label();
        label.SetBinding(Label.ContentProperty, propertyName);
        return label;
    }

    static Button CreateButton(string caption, string commandName)
    {
        var button = new Button { Content = caption };
        button.SetBinding(Button.CommandProperty, commandName);
        return button;
    }
}
