using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace ListBoxAndMapStudy;

public class MainWindow : Window
{
    public MainWindow(MainWindowViewModel viewModel)
    {
        Width = 300;
        Height = 300;
        Title = "名前表示サンプル";
        Content = CreateContent();
        DataContext = viewModel;
    }

    static StackPanel CreateContent()
    {
        var stackPanel = new StackPanel();
        var listBox = CreateSimpleListBox(nameof(MainWindowViewModel.FriendNames), nameof(MainWindowViewModel.SelectedFriendName));
        listBox.Height = 100;
        stackPanel.Children.Add(listBox);
        var textBox = CreateReadOnlyTextBox(nameof(MainWindowViewModel.SelectedFriendAddress));
        textBox.Height = 30;
        stackPanel.Children.Add(textBox);
        var label = CreateLabel(nameof(MainWindowViewModel.SelectedFriendDescription));
        label.Height = 30;
        stackPanel.Children.Add(label);
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
        textBox.SetBinding(TextBox.TextProperty, new Binding(propertyName)
        {
            UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
        });
        return textBox;
    }

    /// <summary>
    /// 複数行にわたるテキストを表示するためのTextBox
    /// TextBlockではスクロールバーが表示されないため、スクロールが必要な場合は、このようにTextBoxを使う。
    /// </summary>
    /// <param name="propertyName"></param>
    /// <returns></returns>
    static TextBox CreateReadOnlyTextBox(string propertyName)
    {
        var textBox = new TextBox
        {
            TextWrapping = TextWrapping.Wrap,                 // 長文を自然に折り返して表示するため
            AcceptsReturn = true,                             // 改行を含むテキストを正しく表示するため
            VerticalScrollBarVisibility = ScrollBarVisibility.Auto,   // 長文の上下スクロールを可能にするため
            HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled, // 横スクロールを禁止し、読みやすさを保つため
            IsReadOnly = true,                                // 入力不可にし、表示専用として扱うため
            IsTabStop = false,                                // Tab移動の対象にしない（入力欄ではないため）
            BorderThickness = new Thickness(0),               // 入力欄っぽさを消し、表示専用の見た目に寄せるため
            Background = Brushes.Transparent                  // 背景を透明にし、TextBlock に近い見た目にするため
        };

        textBox.SetBinding(TextBox.TextProperty, new Binding(propertyName)
        {
            Mode = BindingMode.OneWay   // IsReadOnly=trueで、入力はできないのだが、この設定がないと、VMのプロパティを書き込み可能なものにしなければならない
        });

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
