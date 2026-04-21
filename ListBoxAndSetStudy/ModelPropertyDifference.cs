namespace ListBoxAndSetStudy;

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
    /// <param name="OldValue"></param>
    /// <param name="NewValue"></param>
    public abstract record Scalar(string Name, object OldValue, object NewValue) : ModelPropertyDifference(Name);

    /// <summary>
    /// Set(集合)プロパティの更新内容の差分を保持するクラス群の抽象基底クラス
    /// </summary>
    public abstract record Set(string Name) : ModelPropertyDifference(Name)
    {
        /// <summary>
        /// Set(集合)プロパティへの要素追加内容を保持するもの
        /// </summary>
        /// <param name="Name"></param>
        /// <param name="Value"></param>
        public sealed record Add(string Name, object Value) : Set(Name);

        /// <summary>
        /// Set(集合)プロパティからの要素削除内容を保持するもの
        /// </summary>
        /// <param name="Name"></param>
        /// <param name="Value"></param>
        public sealed record Delete(string Name, object Value) : Set(Name);
    }

    /// <summary>
    /// Map(写像、辞書)プロパティの更新内容の差分を保持するクラス群の抽象基底クラス
    /// </summary>
    /// </summary>
    public abstract record Map(string Name) : ModelPropertyDifference(Name)
    {
        /// <summary>
        /// Map(写像、辞書)プロパティへの要素追加内容を保持するもの
        /// </summary>
        /// <param name="Name"></param>
        /// <param name="Key"></param>
        /// <param name="Value"></param>
        public sealed record Add(string Name, object Key, object Value) : Map(Name);

        /// <summary>
        /// TODO これはいらないかも、結局スカラーの更新として扱うことでよいかもしれない。
        /// Map(写像、辞書)プロパティの要素の更新内容を保持するもの
        /// </summary>
        /// <param name="Name"></param>
        /// <param name="Key"></param>
        /// <param name="OldValue"></param>
        /// <param name="NewValue"></param>
        public sealed record Update(string Name, object Key, object OldValue, object NewValue) : Map(Name);

        /// <summary>
        /// Map(写像、辞書)プロパティからの要素削除内容を保持するもの
        /// </summary>
        /// <param name="Name"></param>
        /// <param name="Key"></param>
        public sealed record Delete(string Name, object Key) : Map(Name);
    }
}
