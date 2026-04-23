using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace TabStudy;

public sealed class MainWindow : Window
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
        stackPanel.Children.Add(CreateTabControl());
        stackPanel.Children.Add(CreateButton("Add Tab", nameof(MainWindowViewModel.AddTabCommand)));
        stackPanel.Children.Add(CreateButton("Delete Tab", nameof(MainWindowViewModel.DeleteTabCommand)));
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

    static TabControl CreateTabControl()
    {
        var tabControl = new TabControl
        {
            Height = 100,
        };

        tabControl.SetBinding(
            TabControl.ItemsSourceProperty,
            new Binding("Tabs")
        );

        // SelectedItem = Binding("SelectedTab")
        tabControl.SetBinding(
            TabControl.SelectedItemProperty,
            new Binding("SelectedTab")
        );

        // TabItem の生成方法を指定（Header と Content をバインド）
        tabControl.ItemTemplate = CreateHeaderTemplate();
        tabControl.ContentTemplate = CreateContentTemplate();

        return tabControl;
    }


    static DataTemplate CreateHeaderTemplate()
    {
        var text = new FrameworkElementFactory(typeof(TextBlock));
        text.SetBinding(TextBlock.TextProperty, new Binding("Header"));

        var template = new DataTemplate
        {
            VisualTree = text
        };
        return template;
    }

    static DataTemplate CreateContentTemplate()
    {
        var text = new FrameworkElementFactory(typeof(TextBlock));
        text.SetBinding(TextBlock.TextProperty, new Binding("Content"));

        var template = new DataTemplate
        {
            VisualTree = text
        };
        return template;
    }
}
