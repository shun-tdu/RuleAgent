using UnityEngine;

/// <summary>
/// テレポート可能かつ、移動するオブジェクトが実装する
/// </summary>
public interface ITeleportable
{
    /// <summary>
    /// テレポート時に呼び出される。ワープ先のワールド座標を受け取る。
    /// </summary>
    void TeleportTo(Vector3 worldPos);

    /// <summary>
    /// 移動中なら強制停止させる
    /// </summary>
    void ForceStopMovement();
}
