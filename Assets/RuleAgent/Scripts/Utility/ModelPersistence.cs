using System.IO;
using UnityEngine;

public static class ModelPersistence
{
    private static string Path => PathCombine();

    private static string PathCombine()
    {
        return System.IO.Path.Combine(Application.persistentDataPath, "learned_model.json");
    }
    
    /// <summary>
    /// Json形式で学習済みモデルをセーブ
    /// </summary>
    public static void Save(LeanedModel model)
    {
        model.sensorEnabled = CustomizationState.CurrentConfig.sensorEnabled;
        string json = JsonUtility.ToJson(model);
        File.WriteAllText(Path, json);
        Debug.Log("Model saved to " + Path);
    }

    /// <summary>
    /// Json形式で学習済みモデルをロード
    /// </summary>
    public static LeanedModel Load()
    {
        if (!File.Exists(Path)) return null;
        string json = File.ReadAllText(Path);
        return JsonUtility.FromJson<LeanedModel>(json);
    }
}