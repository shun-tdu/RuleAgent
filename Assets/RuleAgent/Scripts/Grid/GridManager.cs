using UnityEngine;

[ExecuteAlways]
public class GridManager : MonoBehaviour
{
    [SerializeField] private LevelData levelData;
    public int Width => levelData.width;
    public int Height => levelData.height;
    public float CellSize => levelData.cellSize;


    /// <summary>
    /// セル座標をワールド座標に変換
    /// </summary>
    public Vector3 CellToWorld(int x, int y)
    {
        float worldX = x * CellSize;
        float worldZ = y * CellSize;
        return new Vector3(worldX, 0, worldZ) + transform.position;
    }

    /// <summary>
    /// ワールド座標をセル座標に変換
    /// </summary>
    public Vector2Int WorldToCell(Vector3 worldPos)
    {
        int x = Mathf.RoundToInt(worldPos.x / CellSize);
        int y = Mathf.RoundToInt(worldPos.z / CellSize);
        return new Vector2Int(x, y);
    }

    /// <summary>
    /// ステージ領域内かを判定
    /// </summary>
    public bool InBounds(Vector2Int g)
    {
        return g.x >= 0 && g.x < Width && g.y >= 0 && g.y < Height;
    }

    /// <summary>
    /// 歩行可能かを判定
    /// </summary>
    public bool IsWalkable(Vector2Int g)
    {
        if (!InBounds(g)) return false;
        return levelData.IsWalkable(g.x, g.y);
    }

    /// <summary>
    /// from → toの移動が一方通行制約を守っているか
    /// </summary>
    public bool IsOneWayAllowed(Vector2Int from, Vector2Int to)
    {
        LevelData.TileType t = levelData.GetTileType(from.x, from.y);
        Vector2Int dir = to - from;
        switch (t)
        {
            case LevelData.TileType.OneWayUp: return dir == Vector2Int.up;
            case LevelData.TileType.OneWayDown: return dir == Vector2Int.down;
            case LevelData.TileType.OneWayLeft: return dir == Vector2Int.left;
            case LevelData.TileType.OneWayRight: return dir == Vector2Int.right;
            default: return true;
        }
    }

    /// <summary>
    /// デバッグ用 レベルデータをSceneビューに表示
    /// </summary>
    private void OnDrawGizmos()
    {
        if (levelData == null) return;
        for (int x = 0; x < Width; x++)
        {
            for (int y = 0; y < Height; y++)
            {
                Vector3 center = CellToWorld(x, y);

                LevelData.TileType t = levelData.GetTileType(x, y);

                switch (t)
                {
                    case LevelData.TileType.Floor:
                        Gizmos.color = Color.green; // 通常床は緑のワイヤー
                        Gizmos.DrawWireCube(center, new Vector3(CellSize, 0, CellSize));
                        break;

                    case LevelData.TileType.Wall:
                        Gizmos.color = Color.red; // 壁は赤い実線
                        Gizmos.DrawCube(center, new Vector3(CellSize, 0.1f, CellSize));
                        break;

                    case LevelData.TileType.OneWayUp:
                        Gizmos.color = Color.yellow; // 一方通行↑は黄色
                        Gizmos.DrawCube(center, new Vector3(CellSize, 0.1f, CellSize));
                        Gizmos.color = Color.black; // 矢印を黒で
                        Gizmos.DrawLine(
                            center + new Vector3(0, 0.1f, -CellSize * 0.25f),
                            center + new Vector3(0, 0.1f, CellSize * 0.25f));
                        Gizmos.DrawLine(
                            center + new Vector3(-CellSize * 0.1f, 0.1f, CellSize * 0.1f),
                            center + new Vector3(0, 0.1f, CellSize * 0.25f));
                        Gizmos.DrawLine(
                            center + new Vector3(CellSize * 0.1f, 0.1f, CellSize * 0.1f),
                            center + new Vector3(0, 0.1f, CellSize * 0.25f));
                        break;

                    case LevelData.TileType.OneWayDown:
                        Gizmos.color = Color.yellow;
                        Gizmos.DrawCube(center, new Vector3(CellSize, 0.1f, CellSize));
                        Gizmos.color = Color.black;
                        Gizmos.DrawLine(
                            center + new Vector3(0, 0.1f, CellSize * 0.25f),
                            center + new Vector3(0, 0.1f, -CellSize * 0.25f));
                        Gizmos.DrawLine(
                            center + new Vector3(-CellSize * 0.1f, 0.1f, -CellSize * 0.1f),
                            center + new Vector3(0, 0.1f, -CellSize * 0.25f));
                        Gizmos.DrawLine(
                            center + new Vector3(CellSize * 0.1f, 0.1f, -CellSize * 0.1f),
                            center + new Vector3(0, 0.1f, -CellSize * 0.25f));
                        break;

                    case LevelData.TileType.OneWayLeft:
                        Gizmos.color = Color.yellow;
                        Gizmos.DrawCube(center, new Vector3(CellSize, 0.1f, CellSize));
                        Gizmos.color = Color.black;
                        Gizmos.DrawLine(
                            center + new Vector3(-CellSize * 0.25f, 0.1f, 0),
                            center + new Vector3(CellSize * 0.25f, 0.1f, 0));
                        Gizmos.DrawLine(
                            center + new Vector3(-CellSize * 0.1f, 0.1f, CellSize * 0.1f),
                            center + new Vector3(-CellSize * 0.25f, 0.1f, 0));
                        Gizmos.DrawLine(
                            center + new Vector3(-CellSize * 0.1f, 0.1f, -CellSize * 0.1f),
                            center + new Vector3(-CellSize * 0.25f, 0.1f, 0));
                        break;

                    case LevelData.TileType.OneWayRight:
                        Gizmos.color = Color.yellow;
                        Gizmos.DrawCube(center, new Vector3(CellSize, 0.1f, CellSize));
                        Gizmos.color = Color.black;
                        Gizmos.DrawLine(
                            center + new Vector3(CellSize * 0.25f, 0.1f, 0),
                            center + new Vector3(-CellSize * 0.25f, 0.1f, 0));
                        Gizmos.DrawLine(
                            center + new Vector3(CellSize * 0.1f, 0.1f, CellSize * 0.1f),
                            center + new Vector3(CellSize * 0.25f, 0.1f, 0));
                        Gizmos.DrawLine(
                            center + new Vector3(CellSize * 0.1f, 0.1f, -CellSize * 0.1f),
                            center + new Vector3(CellSize * 0.25f, 0.1f, 0));
                        break;
                    case LevelData.TileType.TeleportPad:
                        Gizmos.color = Color.magenta; 
                        Gizmos.DrawCube(center, new Vector3(CellSize, 0.1f, CellSize));
                        break;
                }
            }
        }
    }
}