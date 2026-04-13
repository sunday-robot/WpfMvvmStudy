using System.ComponentModel;

namespace LabelStudy;

public class MyModel : INotifyPropertyChanged
{
    private string _firstName = "";
    private string _middleName = "";
    private string _lastName = "";
    public string FirstName
    {
        get => _firstName;
        set
        {
            _firstName = value.ToUpper();
            OnPropertyChanged(nameof(FirstName));
        }
    }

    public string MiddleName
    {
        get => _middleName;
        set
        {
            _middleName = value.ToUpper();
            OnPropertyChanged(nameof(MiddleName));
        }
    }

    public string LastName
    {
        get => _lastName;
        set
        {
            _lastName = value.ToUpper();
            OnPropertyChanged(nameof(LastName));
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged(string name) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}
