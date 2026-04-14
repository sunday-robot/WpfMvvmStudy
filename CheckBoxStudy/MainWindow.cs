using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace CheckBoxStudy;

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
        stackPanel.Children.Add(CreateLabel(nameof(MainWindowViewModel.FirstName)));
        stackPanel.Children.Add(CreateLabel(nameof(MainWindowViewModel.MiddleName)));
        stackPanel.Children.Add(CreateLabel(nameof(MainWindowViewModel.LastName)));
        stackPanel.Children.Add(CreateCheckBox("Change target selectable", nameof(MainWindowViewModel.IsChangeTargetSelectable)));
        stackPanel.Children.Add(CreateCheckBox("Change First Name", nameof(MainWindowViewModel.IsChangeFirstName), nameof(MainWindowViewModel.IsChangeTargetSelectable)));
        stackPanel.Children.Add(CreateCheckBox("Change Middle Name", nameof(MainWindowViewModel.IsChangeMiddleName), nameof(MainWindowViewModel.IsChangeTargetSelectable)));
        stackPanel.Children.Add(CreateCheckBox("Change Last Name", nameof(MainWindowViewModel.IsChangeLastName), nameof(MainWindowViewModel.IsChangeTargetSelectable)));
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

    static CheckBox CreateCheckBox(string caption, string propertyName, string? enabledPropertyName = null)
    {
        var checkBox = new CheckBox { Content = caption };
        checkBox.SetBinding(CheckBox.IsCheckedProperty, new Binding(propertyName) { Mode = BindingMode.TwoWay, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged });
        if (enabledPropertyName != null)
            checkBox.SetBinding(CheckBox.IsEnabledProperty, enabledPropertyName);
        return checkBox;
    }
}
