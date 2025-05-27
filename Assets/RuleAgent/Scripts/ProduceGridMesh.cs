using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
[ExecuteAlways]
public class ProduceGridMesh : MonoBehaviour
{
    [Header("Grid Settings")] [Tooltip("X方向のセル数")]
    public int width = 10;

    [Tooltip("Z方向のセル数")] public int height = 10;
    [Tooltip("セル1辺の長さ")] public float cellSize = 1f;

    [Header("Elevation Settings")] [Tooltip("高さマップの最大値")]
    public float maxHeight = 2f;

    [Tooltip("グリッド上の(x/width, z/height)に対する高さの分布を制御")]
    public AnimationCurve heightCurve = AnimationCurve.Linear(0, 0, 1, 1);

    private Mesh mesh;

    private void OnEnable()
    {
        BuildMesh();
    }

    private void OnValidate()
    {
        BuildMesh();
    }

    public void BuildMesh()
    {
        Vector3[] vertices = new Vector3[(width + 1) * (height + 1)];
        Vector2[] uv = new Vector2[vertices.Length];
        int[] triangles = new int[width * height * 6];

        float[,] heights = new float[width + 1, height + 1];
        for (int z = 0; z <= height; z++)
        {
            for (int x = 0; x <= width; x++)
            {
                //正規化座標 u,v[0,1]
                float u = (float)x / width;
                float v = (float)z / height;
                //曲線でベース高さを得て
                float h = heightCurve.Evaluate((u + v) * 0.5f) * maxHeight;
            }
        }
    }
}