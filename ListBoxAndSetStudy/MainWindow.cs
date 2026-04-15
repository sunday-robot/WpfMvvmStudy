using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace ListBoxAndSetStudy;

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
        stackPanel.Children.Add(CreateSimpleListBox(nameof(MainWindowViewModel.FriendNames), nameof(MainWindowViewModel.SelectedFriendName)));
        stackPanel.Children.Add(CreateTextBox(nameof(MainWindowViewModel.InputNameText)));
        stackPanel.Children.Add(CreateButton("Add", nameof(MainWindowViewModel.AddNameCommand)));
        stackPanel.Children.Add(CreateButton("Remove", nameof(MainWindowViewModel.RemoveNameCommand)));
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

    /// <summary>
    /// VMのプロパティがstringなどの単純なもののコレクションの場合は、このようなシンプルなものでよい。
    /// </summary>
    /// <param name="itemsPropertyName"></param>
    /// <param name="selectedItemPropertyName"></param>
    /// <returns></returns>
    static ListBox CreateSimpleListBox(string itemsPropertyName, string selectedItemPropertyName)
    {
        var listBox = new ListBox
        {
            SelectionMode = SelectionMode.Single,
            Height = 100,
        };
        listBox.SetBinding(ListBox.ItemsSourceProperty,
            new Binding(itemsPropertyName));
        listBox.SetBinding(ListBox.SelectedItemProperty,
            new Binding(selectedItemPropertyName));
        ScrollViewer.SetCanContentScroll(listBox, false);   // この指定をしないと、行単位のスクロールとなってしまい、末尾に空白部分が表示されてしまうという問題がある。
        ScrollViewer.SetVerticalScrollBarVisibility(listBox, ScrollBarVisibility.Auto);
        ScrollViewer.SetHorizontalScrollBarVisibility(listBox, ScrollBarVisibility.Disabled);
        return listBox;
    }
}
