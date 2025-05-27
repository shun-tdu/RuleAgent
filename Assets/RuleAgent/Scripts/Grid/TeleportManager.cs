using UnityEngine;

public class TeleportManager : MonoBehaviour
{
    [SerializeField] private Teleporter teleporterPrefab;
    [SerializeField] private LevelData levelData;
    [SerializeField] private GridManager grid;

    private void Awake()
    {
        foreach (var info in levelData.teleportList)
        {
            var srcWorld = grid.CellToWorld(info.source.x, info.source.y) + Vector3.up * 0.5f;
            var tp = Instantiate(teleporterPrefab, srcWorld, Quaternion.identity, transform);

            var dstTransform = tp.transform.Find("DestinationPoint");
            var dstPos = grid.CellToWorld(info.destination.x, info.destination.y) + Vector3.up* 0.5f;
            dstTransform.position = dstPos;
        }
    }
}