using UnityEditor;
using UnityEngine;

public class StageBuilder : MonoBehaviour
{
    [SerializeField] private GameObject[] tilePrefabs;
    [SerializeField] private GameObject borderWallPrefab;

    [SerializeField] private LevelData levelData;


    private void Start()
    {
        BuildStage();
        BuildWallBorder();
    }

    void BuildStage()
    {
        foreach (Transform c in transform) DestroyImmediate(c.gameObject);

        //LevelDataを読み出してステージの要素をインスタンス化
        for (int y = 0; y < levelData.height; y++)
        {
            for (int x = 0; x < levelData.width; x++)
            {
                int code = levelData.GetTile(x, y);
                var tileType = (LevelData.TileType)code;
                Transform parent = this.transform;

                switch (tileType)
                {
                    case LevelData.TileType.Floor:
                        var floorPrefab = tilePrefabs[code];
                        Vector3 floorPos = new Vector3(x, -0.5f, y);
                        InstantiateStageElement(floorPos, floorPrefab, tileType.ToString(), parent);
                        break;
                    case LevelData.TileType.Wall:
                        var wallPrefab = tilePrefabs[code];
                        Vector3 wallPos = new Vector3(x, 0.5f, y);
                        InstantiateStageElement(wallPos, wallPrefab, tileType.ToString(), parent);
                        break;
                    case LevelData.TileType.OneWayUp:
                        var oneWayUpPrefab = tilePrefabs[code];
                        Vector3 oneWayUpPos = new Vector3(x, -1.0f, y);
                        InstantiateStageElement(oneWayUpPos, oneWayUpPrefab, tileType.ToString(), parent);
                        break;
                    case LevelData.TileType.OneWayDown:
                        var oneWayDownPrefab = tilePrefabs[code];
                        Vector3 oneWayDownPos = new Vector3(x, -1.0f, y);
                        InstantiateStageElement(oneWayDownPos, oneWayDownPrefab, tileType.ToString(), parent);
                        break;
                    case LevelData.TileType.OneWayLeft:
                        var oneWayLeftPrefab = tilePrefabs[code];
                        Vector3 oneWayLeftPos = new Vector3(x, -1.0f, y);
                        InstantiateStageElement(oneWayLeftPos, oneWayLeftPrefab, tileType.ToString(), parent);
                        break;
                    case LevelData.TileType.OneWayRight:
                        var oneWayRightPrefab = tilePrefabs[code];
                        Vector3 oneWayRightPos = new Vector3(x, -1.0f, y);
                        InstantiateStageElement(oneWayRightPos, oneWayRightPrefab, tileType.ToString(), parent);
                        break;
                    // case LevelData.TileType.TeleportPad:
                    //     var teleporterPadPrefab = tilePrefabs[code];
                    //     Vector3 teleporterPadtPos = new Vector3(x, -0.5f, y);
                    //     InstantiateStageElement(teleporterPadtPos, teleporterPadPrefab, tileType.ToString(), parent);
                    //     break;
                    default:
                        var defoultPrefab = tilePrefabs[code];
                        Vector3 defoultPos = new Vector3(x, -0.5f, y);
                        InstantiateStageElement(defoultPos, defoultPrefab, tileType.ToString(), parent);
                        break;
                }
            }
        }
    }

    void BuildWallBorder()
    {
        if (borderWallPrefab == null)
        {
            Debug.LogError("wallPrefab がアサインされていません！");
            return;
        }

        Transform parent = this.transform;

        // Z = -1 と Z = height（外側）に並べる
        for (int x = -1; x <= levelData.width; x++)
        {
            InstantiateWall(x, -1, parent);
            InstantiateWall(x, levelData.height, parent);
        }

        // X = -1 と X = width に並べる（Z=0..height-1）
        for (int z = -1; z <= levelData.height; z++)
        {
            InstantiateWall(-1, z, parent);
            InstantiateWall(levelData.width, z, parent);
        }
    }

    void InstantiateStageElement(Vector3 pos, GameObject obj, string elementName, Transform parent)
    {
        GameObject element = Instantiate(obj, pos, obj.transform.rotation, parent);
        element.name = $"{elementName}_{pos.x}_{pos.z}";
    }

    void InstantiateWall(int gx, int gz, Transform parent)
    {
        // 壁のワールド座標：セル角基準ならセル座標×cellSize、その後 Y を上げる
        Vector3 pos = new Vector3(
            gx * levelData.cellSize,
            0.5f, // 壁の高さを2ユニットとした場合、Y=1で床から1ユニット上がった位置（中心）
            gz * levelData.cellSize
        );

        // プレハブを生成して、親を BorderBuilder に
        GameObject wall = Instantiate(
            borderWallPrefab,
            pos,
            Quaternion.identity,
            parent
        );
        wall.name = $"Wall_{gx}_{gz}";
    }
}