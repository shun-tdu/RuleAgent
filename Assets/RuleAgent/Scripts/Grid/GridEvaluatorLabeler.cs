using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.VisualScripting;

public class GridEvaluatorLabeler : MonoBehaviour
{
    public GridManager grid;
    private AgentControllerEpsilonGreedy agent;
    public GameObject labelPrefab;

    private List<TextMeshPro> labels = new List<TextMeshPro>();

    private void Start()
    {
        for (int x = 0; x < grid.Width; x++)
        {
            for (int y = 0; y < grid.Height; y++)
            {
                Vector3 pos = grid.CellToWorld(x, y) + Vector3.up * 0.1f;
                var go = Instantiate(labelPrefab, pos, Quaternion.identity, transform);
                labels.Add(go.GetComponent<TextMeshPro>());
            }
        }
    }

    private void Update()
    {
        if (agent == null) return;

        int idx = 0;
        var sensors = agent.sensors;
        var evaluators = agent.evaluators;
        for (int x = 0; x < grid.Width; x++)
        {
            for (int y = 0; y < grid.Height; y++)
            {
                var cell = new Vector2Int(x, y);
                float score = 0f;
                if (grid.IsWalkable(cell))
                {
                    for (int i = 0; i < sensors.Length; i++)
                    {
                        float sv = sensors[i].Sense(cell, grid);
                        score += evaluators[i].Evaluate(cell, sv);
                    }

                    labels[idx].text = score.ToString("F1");
                }
                else
                {
                    labels[idx].text = "";
                }
            }
        }
    }
}