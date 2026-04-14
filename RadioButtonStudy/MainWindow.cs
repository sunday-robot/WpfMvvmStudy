using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace RadioButtonStudy;

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
        stackPanel.Children.Add(CreateLabel(nameof(MainWindowViewModel.DisplayName)));
        stackPanel.Children.Add(CreateLabel(nameof(MainWindowViewModel.FirstName)));
        stackPanel.Children.Add(CreateLabel(nameof(MainWindowViewModel.MiddleName)));
        stackPanel.Children.Add(CreateLabel(nameof(MainWindowViewModel.LastName)));
#if false
        // ↓以下は意味もなく、二つのスタックパネルにラジオボタンを分けて配置している。
        // WPFのラジオボタンは、groupnameというプロパティでグループ化することができるのだが、
        // この機構がMVVMの場合は邪魔になることがあるように思われたので、このようにしてみたのだが、実際には何の問題もなかった。
        // 複数のラジオボタングループがなければgroupnameの機構は邪魔にならないのかもしれない。
        {
            var subStackPanel = new StackPanel();
            subStackPanel.Children.Add(CreateRadio("All", nameof(MainWindowViewModel.ChangeTarget), ChangeTarget.All));
            subStackPanel.Children.Add(CreateRadio("First Name", nameof(MainWindowViewModel.ChangeTarget), ChangeTarget.FirstName));
            stackPanel.Children.Add(subStackPanel);
        }
        {
            var subStackPanel = new StackPanel();
            subStackPanel.Children.Add(CreateRadio("Middle Name", nameof(MainWindowViewModel.ChangeTarget), ChangeTarget.MiddleName));
            subStackPanel.Children.Add(CreateRadio("Last Name", nameof(MainWindowViewModel.ChangeTarget), ChangeTarget.LastName));
            stackPanel.Children.Add(subStackPanel);
        }
#else
        stackPanel.Children.Add(CreateRadio("All", nameof(MainWindowViewModel.ChangeTarget), ChangeTarget.All));
        stackPanel.Children.Add(CreateRadio("First Name", nameof(MainWindowViewModel.ChangeTarget), ChangeTarget.FirstName));
        stackPanel.Children.Add(CreateRadio("Middle Name", nameof(MainWindowViewModel.ChangeTarget), ChangeTarget.MiddleName));
        stackPanel.Children.Add(CreateRadio("Last Name", nameof(MainWindowViewModel.ChangeTarget), ChangeTarget.LastName));

        // 以下のように、同じスタックパネルに複数のラジオボタングループを混在させる場合は、groupnameをきちんと指定しておかないと、グループが混ざってしまう。
        stackPanel.Children.Add(CreateRadio("Red", nameof(MainWindowViewModel.RgbColor), RgbColor.Red));
        stackPanel.Children.Add(CreateRadio("Green", nameof(MainWindowViewModel.RgbColor), RgbColor.Green));
        stackPanel.Children.Add(CreateRadio("Blue", nameof(MainWindowViewModel.RgbColor), RgbColor.Blue));
#endif

        stackPanel.Children.Add(CreateButton("Change Name", nameof(MainWindowViewModel.ChangeNameRandomlyCommand)));
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

    static RadioButton CreateRadio(string caption, string propertyName, object value)
    {
        var rb = new RadioButton { Content = caption, GroupName = propertyName };
        rb.SetBinding(RadioButton.IsCheckedProperty, new Binding(propertyName)
        {
            Mode = BindingMode.TwoWay,
            Converter = new EnumEqualsConverter(),
            ConverterParameter = value
        });
        return rb;
    }
}
