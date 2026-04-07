using System.ComponentModel;

namespace IdealAppStudy
{
    public class MainWindowViewModel : INotifyPropertyChanged, IPropertiesChangedListener
    {
        private readonly MyModel _model;

        public void Changed(List<ChangedProperty> changedProperties)
        {
            var changedNames = new HashSet<string>();
            foreach (var changed in changedProperties)
            {
                switch (changed.Name)
                {
                    case "FirstName":
                        FirstName = (string)changed.Value;
                        UpdateDisplayName();
                        changedNames.Add(nameof(FirstName));
                        changedNames.Add(nameof(DisplayName));
                        break;
                    case "MiddleName":
                        MiddleName = (string)changed.Value;
                        UpdateDisplayName();
                        changedNames.Add(nameof(MiddleName));
                        changedNames.Add(nameof(DisplayName));
                        break;
                    case "LastName":
                        LastName = (string)changed.Value;
                        UpdateDisplayName();
                        changedNames.Add(nameof(LastName));
                        changedNames.Add(nameof(DisplayName));
                        break;
                }
            }
            foreach (var name in changedNames)
            {
                OnPropertyChanged(name);
            }
        }

        /// <summary>
        /// 表示用の名前を更新する。
        /// 
        /// ファーストネームしかない場合は、"First"のように整形する。
        /// ミドルネームがない場合は、"First Last"。
        /// ミドルネームもある場合は、"First M. Last"。
        /// </summary>

        void UpdateDisplayName()
        {
            if (LastName.Length == 0)
                DisplayName = Capitalize(FirstName);
            else if (MiddleName.Length == 0)
                DisplayName = $"{Capitalize(FirstName)} {Capitalize(LastName)}";
            else
                DisplayName = $"{Capitalize(FirstName)} {char.ToUpper(MiddleName[0])}. {Capitalize(LastName)}";
        }

        public MainWindowViewModel(MyModel model)
        {
            _model = model;
            _model.AddListener(this, ["FirstName", "MiddleName", "LastName"]);
        }

        public string FirstName { get; private set; } = "";
        public string MiddleName { get; private set; } = "";
        public string LastName { get; private set; } = "";
        public string DisplayName { get; private set; } = "";

        /// <summary>
        /// nameを先頭大文字、残り小文字に変換する
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        static string Capitalize(string name)
        {
            if (string.IsNullOrWhiteSpace(name)) return "";
            return char.ToUpper(name[0]) + name[1..].ToLower();
        }

        public void ChangeNameRandomly() => _model.ChangeNameRandomly();

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string name) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
