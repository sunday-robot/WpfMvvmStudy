using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace TextBoxStudy;

public class MainWindow : Window
{
    public MainWindow(MainWindowViewModel viewModel)
    {
        Width = 300;
        Height = 250;
        Title = "名前表示サンプル";
        Content = CreateContent();
        DataContext = viewModel;
    }

    static StackPanel CreateContent()
    {
        var stackPanel = new StackPanel();
        stackPanel.Children.Add(CreateLabel(nameof(MainWindowViewModel.DisplayName)));
        stackPanel.Children.Add(CreateLabel(nameof(MainWindowViewModel.RawName)));
        stackPanel.Children.Add(CreateTextBox(nameof(MainWindowViewModel.InputNameText)));
        stackPanel.Children.Add(CreateButton("Set Name", nameof(MainWindowViewModel.SetNameCommand)));
        stackPanel.Children.Add(CreateButton("Clear Name TextBox", nameof(MainWindowViewModel.ClearInputNameTextCommand)));
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

    static TextBox CreateTextBox(string propertyName)
    {
        var textBox = new TextBox();
        textBox.SetBinding(TextBox.TextProperty, new Binding(propertyName) { UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged });
        return textBox;
    }
}
