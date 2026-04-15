using System.Collections.Immutable;
using System.Diagnostics;

namespace ListBoxAndSetStudy;

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
    /// VMからの要求をため込む場所
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
        var currentProperties = new List<ChangedProperty>();
        foreach (var name in interestedPropertyNames)
            currentProperties.Add(CreateCurrentProperty(name));
        listener.OnPropertiesChanged(currentProperties);
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
    /// <param name="changedProperties">更新されたプロパティ名と、更新の内容</param>
    void NotifyToListeners(IEnumerable<ChangedProperty> changedProperties)
    {
        // リスナー別に、通知プロパティセットを構築する
        var changedPropertiesDictionary = new Dictionary<IPropertiesChangedListener, List<ChangedProperty>>();
        foreach (var changedProperty in changedProperties)
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

    void NotifyToListeners(ChangedProperty changedProperty) => NotifyToListeners([changedProperty]);
    #endregion 後で基底クラスを作って、そちらに移動させるべきもの

    #region 基底クラスで抽象メソッドとして宣言し、サブクラスでそれを実装すべきもの
    /// <summary>
    /// VMが本クラスにリスナー登録してきた際に、現在のプロパティの内容を初期値として通知するためのもの
    /// </summary>
    /// <param name="propertyName"></param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    ChangedProperty CreateCurrentProperty(string propertyName)
    {
        switch (propertyName)
        {
            case "FriendNames":
                {
                    var diffs = new List<CollectionDifference>();
                    foreach (var name in _friendNames)
                        diffs.Add(new CollectionDifference.Add(name));
                    return new ChangedProperty("FriendNames", diffs);
                }
            default:
                throw new NotImplementedException($"Unknown property name: {propertyName}");
        }
    }
    #endregion 基底クラスで抽象メソッドとして宣言し、サブクラスでそれを実装すべきもの

    #region サブクラス側で定義すべきもの
    readonly HashSet<string> _friendNames = [
        "ALICE",
        "BOB",
        "CHARLIE"
    ];

    public void AddName(string value)
    {
        EnqueueCommand(() =>
        {
            if (!_friendNames.Add(value.ToUpper()))
            {
                // TODO エラー通知の実装
                Debug.WriteLine($"Name already exists: {value}");
                return;
            }
            NotifyToListeners(new ChangedProperty("FriendNames", new[] { new CollectionDifference.Add(value) }));
        });
    }

    public void RemoveName(string value)
    {
        EnqueueCommand(() =>
        {
            if (!_friendNames.Remove(value.ToUpper()))
            {
                // TODO エラー通知の実装
                Debug.WriteLine($"Name does not exists: {value}");
                return;
            }
            NotifyToListeners(new ChangedProperty("FriendNames", new[] { new CollectionDifference.Delete(value) }));
        });
    }
    #endregion サブクラス側で定義すべきもの
}
