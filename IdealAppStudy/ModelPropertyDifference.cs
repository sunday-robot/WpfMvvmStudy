namespace IdealAppStudy;

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
    public sealed record Scalar(string Name, object Value) : ModelPropertyDifference(Name);

    /// <summary>
    /// Set(集合)プロパティの更新内容の差分を保持するもの
    /// </summary>
    /// <param name="AddElements">追加された要素のリスト</param>
    /// <param name="DeleteElements">削除された要素のリスト</param>
    public sealed record Set(string Name, IEnumerable<object> AddElements, IEnumerable<object> DeleteElements) : ModelPropertyDifference(Name);

        /// <summary>
    /// Map(写像、辞書)プロパティの更新内容の差分を保持するもの
        /// </summary>
    /// <param name="AddEntries">追加されたエントリー(キーと値のペア)のリスト</param>
    /// <param name="DeleteKeys">削除されたエントリーのキーのリスト</param>
    public sealed record Map(string Name, IEnumerable<(object Key, object Value)> AddEntries, IEnumerable<object> DeleteKeys) : ModelPropertyDifference(Name);
}
