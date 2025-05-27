
using System.Linq;
using UnityEngine;


public class CrystalManager : MonoBehaviour
{
    [SerializeField] private LevelData lebelData;
    [SerializeField] private GridManager grid;
    [SerializeField] private CrystalConfig crystalConfig;
    [SerializeField] private Transform parent;

    private void Awake()
    {
        foreach (var info in lebelData.crystalList)
        {
            Vector3 worldPos = grid.CellToWorld(info.position.x, info.position.y) + Vector3.up * 0.5f;
            var data = crystalConfig.crystals.First(c => c.type == info.type);
            var go = Instantiate(data.prefab, worldPos, Quaternion.identity, parent);
            var pickup = go.AddComponent<CrystalPickUp>();
            pickup.pointValue = data.pointValue;
        }
    }
}