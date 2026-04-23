using System.ComponentModel;

namespace TabStudy;

public class TabItemViewModel(string header, string content) : INotifyPropertyChanged
{
    public string Header { get; set; } = header;
    public string Content { get; set; } = content;

    public event PropertyChangedEventHandler? PropertyChanged;
}
