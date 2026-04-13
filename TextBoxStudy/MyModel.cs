using System.Collections.Immutable;

namespace TextBoxStudy;

public class MyModel
{
    #region 後で基底クラスを作って、そちらに移動させるべきもの
    /// <summary>
    /// 本クラスの主処理を行うスレッド
    /// </summary>
    readonly Thread? _modelThread;

    /// <summary>
    /// アプリケーション終了要求を受けたかどうかを示すフラグ
    /// </summary>
    bool _stopRequested = false;

    /// <summary>
    /// コマンドキュー
    /// VMから要求内容をため込む場所
    /// </summary>
    readonly Queue<Action> _commandQueue = new();

    /// <summary>
    /// キュー処理用の同期オブジェクト
    /// </summary>
    readonly AutoResetEvent _queueEvent = new(false);

    /// <summary>
    /// プロパティ名からリスナー群を取得するための辞書。<br>
    /// キー:プロパティ名<br>
    /// 値:当該プロパティの変更に関心のあるリスナーの集合<br>
    /// </summary>
    readonly Dictionary<string, HashSet<IPropertiesChangedListener>> _propertyNameToListeners = [];

    public MyModel()
    {
        _modelThread = new Thread(MainLoop)
        {
            IsBackground = true
        };
        _modelThread.Start();
    }

    #region publicメソッド
    /// <summary>
    /// リスナーを登録する。
    /// </summary>
    /// <param name="listener"></param>
    /// <param name="interestedPropertyNames">リスナーが関心のあるプロパティ名のセット</param>
    public void AddListener(IPropertiesChangedListener listener, params string[] interestedPropertyNames)
    {
        // プロパティ名からリスナー群を取得するための辞書に、リスナーを追加する
        foreach (var name in interestedPropertyNames)
        {
            if (!_propertyNameToListeners.TryGetValue(name, out var value))
            {
                value = [];
                _propertyNameToListeners[name] = value;
            }
            value.Add(listener);
        }

        // 登録されたリスナーに、関心のあるプロパティの現在値(リスナーにとっては初期値)を通知する。これ以降は差分が通知される。
        var changedPropertySet = CreateChangedProperties(interestedPropertyNames);
        listener.OnPropertiesChanged(changedPropertySet);
    }

    /// <summary>
    /// リスナーの登録を解除する。
    /// </summary>
    /// <param name="listener"></param>
    public void RemoveListener(IPropertiesChangedListener listener)
    {
        foreach (var propertyName in _propertyNameToListeners.Keys)
            _propertyNameToListeners[propertyName].Remove(listener);
    }

    /// <summary>
    /// 終了要求
    /// </summary>
    public void Stop()
    {
        _stopRequested = true;
        _queueEvent.Set();
        _modelThread?.Join();
    }
    #endregion publicメソッド

    /// <summary>
    /// VMからの要求をコマンドとしてキューにため込む。
    /// </summary>
    /// <param name="action"></param>
    void EnqueueCommand(Action action)
    {
        lock (_commandQueue)
        {
            _commandQueue.Enqueue(action);
        }
        _queueEvent.Set();
    }

    /// <summary>
    /// 本クラスの専用スレッドにて、主処理を行うもの。<br>
    /// VMからの要求がため込まれたキューから要求を取り出し、実行する
    /// </summary>
    void MainLoop()
    {
        while (!_stopRequested)
        {
            Action? action = null;

            lock (_commandQueue)
            {
                if (_commandQueue.Count > 0)
                    action = _commandQueue.Dequeue();
            }

            if (action != null)
            {
                try
                {
                    action();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Command execution error: {ex}");
                }
            }
            else
                _queueEvent.WaitOne();
        }
    }

    /// <summary>
    /// リスナーに、データモデルの更新内容を通知する
    /// </summary>
    /// <param name="propertyNameSet">更新されたプロパティ名の集合</param>
    void NotifyToListeners(params string[] propertyNames)
    {
        // プロパティ名とその値からなる通知プロパティセットを構築する
        var changedPropertySet = CreateChangedProperties(propertyNames);

        // リスナー別に、通知プロパティセットを構築する
        var changedPropertiesDictionary = new Dictionary<IPropertiesChangedListener, List<ChangedProperty>>();
        foreach (var changedProperty in changedPropertySet)
        {
            foreach (var listener in _propertyNameToListeners.GetValueOrDefault(changedProperty.Name, []))
            {
                if (!changedPropertiesDictionary.TryGetValue(listener, out List<ChangedProperty>? props))
                {
                    props = [];
                    changedPropertiesDictionary[listener] = props;
                }
                props.Add(changedProperty);
            }
        }

        // 各リスナーにプロパティセットを通知する
        foreach (var (listener, properties) in changedPropertiesDictionary)
            listener.OnPropertiesChanged(properties);
    }
    #endregion 後で基底クラスを作って、そちらに移動させるべきもの

    #region サブクラス側で定義すべきもの
    string _name = "JAMES";

    List<ChangedProperty> CreateChangedProperties(IEnumerable<string> propertyNameSet)
    {
        return [.. propertyNameSet.Select(name =>
        {
            return name switch
            {
                "Name" => new ChangedProperty("Name", _name),
                _ => throw new InvalidOperationException($"Unknown property name: {name}")
            };
        })];
    }

    public void SetName(string value)
    {
        EnqueueCommand(() =>
        {
            _name = value.ToUpper();
            NotifyToListeners("Name");
        });
    }
    #endregion サブクラス側で定義すべきもの
}
