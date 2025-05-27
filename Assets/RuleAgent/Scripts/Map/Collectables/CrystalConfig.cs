using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Collectables/CrystalConfig")]
public class CrystalConfig : ScriptableObject
{
    [Serializable]
    public struct CrystalData
    {
        public CrystalType type;
        public GameObject prefab;
        public int pointValue;
    }

    public CrystalData[] crystals;
}