namespace IdealAppStudy;

/// <summary>
/// Modelのプロパティが変更されたことを通知するリスナーのインターフェース
/// VMがこのインターフェースを実装し、Modelのプロパティ変更通知を受け取る。
/// </summary>
public interface IModelPropertiesChangedListener
{
    void OnPropertiesChanged(List<ModelPropertyDifference> propertyDifferences);
}
