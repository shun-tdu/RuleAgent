using UnityEngine;


public class VoxelStageBuilder : MonoBehaviour
{
    [Header("ステージサイズ(X + Z + Y)")] [SerializeField]
    private int width = 16; // X方向セル数

    private int length = 16; // Z方向セル数
    private int maxHeight = 4; // Y方向の最大積み上げ高さ

    [Header("ブロック設定")] [SerializeField] private GameObject blockPrefab;
    private float cubeSize = 1f;

    private int[,] heightMap;

    private void Start()
    {
        //ランダムな高さマップを生成
        heightMap = new int[width, length];
        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < length; z++)
            {
                heightMap[x, z] = Random.Range(1, maxHeight + 1);
            }
        }

        BuildStage();
    }

    public void BuildStage()
    {
        Transform parent = this.transform;
        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < length; z++)
            {
                int h = heightMap[x, z];
                for (int y = 0; y < h; y++)
                {
                    Vector3 pos = new Vector3(x * cubeSize, y * cubeSize, z * cubeSize);

                    GameObject block = Instantiate(
                        blockPrefab,
                        pos,
                        Quaternion.identity,
                        parent);
                    block.transform.localScale = Vector3.one * cubeSize;
                }
            }
        }
    }
}