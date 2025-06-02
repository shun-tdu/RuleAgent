using System.IO;
using UnityEngine;

public static class ModelPersistence
{
    private static string FilePath => Path.Combine(Application.dataPath, "../SavedModels/learned_model.json");
    

    /// <summary>
    /// Json形式で学習済みモデルをセーブ
    /// </summary>
    public static void Save(LeanedModel model)
    {
        var fullDir = Path.GetDirectoryName(FilePath);
        if (!Directory.Exists(fullDir))
            Directory.CreateDirectory(fullDir);
        
        model.sensorEnabled = CustomizationState.CurrentConfig.sensorEnabled;
        string json = JsonUtility.ToJson(model);
        File.WriteAllText(FilePath, json);
        Debug.Log("Model saved to " + Path.GetFullPath(FilePath));
    }

    /// <summary>
    /// Json形式で学習済みモデルをロード
    /// </summary>
    public static LeanedModel Load()
    {
        if (!File.Exists(FilePath)) return null;
        string json = File.ReadAllText(FilePath);
        return JsonUtility.FromJson<LeanedModel>(json);
    }
}