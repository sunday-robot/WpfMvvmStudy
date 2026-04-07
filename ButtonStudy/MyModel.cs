// このモデルは、VMに対し、OnPropertyChangedイベントで更新通知を行うようにしているが、これはおそらく、不適切で、OnPropertyChangedは、VMからVへの通知だけにすべきだと思う。

using System.ComponentModel;

namespace IdealAppStudy;

//  <summary>
/// FirstName、MiddleName、LastNameの各プロパティを持ち、プロパティ値の変更をクライアントに通知するモデルを表します。
/// 一応以下の制約を設けることとしている。
/// FirstNameは必須。
/// LastNameは省略可能だが、省略した場合はMiddleNameも省略しなければならない。
/// MiddleNameは省略可能。
/// </summary>
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

    public void ChangeNameRandomly()
    {
        var rand = new Random();

        Thread.Sleep(rand.Next(3000)); // 最大3秒の遅延(重い処理のシミュレーション)

        string[] firsts = { "JAMES", "MARY", "MICHAEL" };
        string[] middles = { "", "MARIE", "ROSE", "LEE" };
        string[] lasts = { "", "SMITH", "JOHNSON", "WILLIAMS" };

        switch (rand.Next(3))
        {
            case 0:
                FirstName = firsts[rand.Next(firsts.Length)];
                break;
            case 1:
                MiddleName = middles[rand.Next(middles.Length)];
                break;
            case 2:
                LastName = lasts[rand.Next(lasts.Length)];
                break;
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged(string name) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}
