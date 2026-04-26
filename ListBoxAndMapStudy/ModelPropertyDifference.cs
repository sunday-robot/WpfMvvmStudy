namespace ListBoxAndMapStudy;

/// <summary>
/// モデルのプロパティの更新内容を保持するクラス群の抽象基底クラス
/// </summary>
/// <param name="Name"></param>
public abstract record ModelPropertyDifference(string Name)
{
    /// <summary>
    /// スカラープロパティ(コレクションではないプロパティ)の更新内容を保持するもの
    /// </summary>
    /// <param name="Name"></param>
    /// <param name="Value"></param>
    public sealed record Scalar(string Name, object? Value) : ModelPropertyDifference(Name);

    public abstract record Set(string Name, IEnumerable<object> Values) : ModelPropertyDifference(Name)
    {
    /// <summary>
        /// Set(集合)プロパティの現在の値(VMにとっては初期値)を保持するもの
    /// </summary>
        /// <param name="Values">値の集合</param>
        public sealed record Initialize(string Name, IEnumerable<object> Values) : Set(Name, Values);

        /// <summary>
        /// Set(集合)プロパティに追加された値を保持するもの
        /// </summary>
        /// <param name="Values">追加された値の集合</param>
        public sealed record Add(string Name, IEnumerable<object> Values) : Set(Name, Values);

        /// <summary>
        /// Set(集合)プロパティから削除された値を保持するもの
        /// </summary>
        /// <param name="Values">削除された値の集合</param>
        public sealed record Remove(string Name, IEnumerable<object> Values) : Set(Name, Values);
    }

    /// <summary>
    /// Map(写像、辞書)プロパティの更新内容の差分を保持するもの
    /// </summary>
    /// <param name="AddEntries">追加されたエントリー(キーと値のペア)のリスト</param>
    /// <param name="DeleteKeys">削除されたエントリーのキーのリスト</param>
    public sealed record Map(string Name, IEnumerable<(object Key, object Value)> AddEntries, IEnumerable<object> DeleteKeys) : ModelPropertyDifference(Name);
}
