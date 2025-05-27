using System;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

[CustomEditor(typeof(LevelData))]
public class LevelDataEditor : Editor
{
    private static readonly Color[] tileColors =
    {
        new Color(0.8f, 0.8f, 0.8f), //Floor
        Color.red, //Wall
        Color.yellow, //OneWayUp
        Color.yellow * 1.0f, //OneWayDown
        Color.yellow * 1.0f, //OneWayLeft
        Color.yellow * 1.0f, //OneWayRight
        Color.magenta, //Teleporter
    };

    private SerializedProperty _tileProp;
    private SerializedProperty _teleportsProp;
    private SerializedProperty _trapProp;
    private SerializedProperty _crystalProp;

    private void OnEnable()
    {
        _tileProp = serializedObject.FindProperty("rows");
        _teleportsProp = serializedObject.FindProperty("teleportList");
        _trapProp = serializedObject.FindProperty("trapList");
        _crystalProp = serializedObject.FindProperty("crystalList");
    }


    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        //タイルマップ編集処理
        DrawDefaultInspector();
        var data = (LevelData)target;
        if (data.rows != null)
        {
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("■ Tile Map Editor ■", EditorStyles.boldLabel);
            DrawTileMapGUI(data);
        }

        //テレポーターの編集処理
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("■ Teleporter Settings ■", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(
            _teleportsProp.FindPropertyRelative("Array.size"),
            new GUIContent("Count")
        );

        EditorGUI.indentLevel++;
        for (int i = 0; i < _teleportsProp.arraySize; i++)
        {
            var elem = _teleportsProp.GetArrayElementAtIndex(i);
            var src = elem.FindPropertyRelative("source");
            var dst = elem.FindPropertyRelative("destination");

            EditorGUILayout.BeginVertical("box");
            EditorGUILayout.PropertyField(src, new GUIContent("Source Cell"));
            EditorGUILayout.PropertyField(dst, new GUIContent("Destination Cell"));

            if (GUILayout.Button("Remove"))
                _teleportsProp.DeleteArrayElementAtIndex(i);
            EditorGUILayout.EndVertical();
        }

        EditorGUI.indentLevel--;

        if (GUILayout.Button("Add Teleporter"))
        {
            _teleportsProp.InsertArrayElementAtIndex(_teleportsProp.arraySize);
            var newElem = _teleportsProp.GetArrayElementAtIndex(_teleportsProp.arraySize - 1);
            newElem.FindPropertyRelative("source").vector2IntValue = Vector2Int.zero;
            newElem.FindPropertyRelative("destination").vector2IntValue = Vector2Int.zero;
        }

        //トラップの編集処理
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("■ Trap Settings ■", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(
            _trapProp.FindPropertyRelative("Array.size"),
            new GUIContent("Count")
        );

        EditorGUI.indentLevel++;
        for (int i = 0; i < _trapProp.arraySize; i++)
        {
            var elem = _trapProp.GetArrayElementAtIndex(i);
            var pos = elem.FindPropertyRelative("position");
            var trapPrefab = elem.FindPropertyRelative("trapPrefab");

            EditorGUILayout.BeginVertical("box");
            EditorGUILayout.PropertyField(pos, new GUIContent("Trap Position"));
            EditorGUILayout.PropertyField(trapPrefab, new GUIContent("Trap Prefab"));

            if (GUILayout.Button("Remove"))
                _trapProp.DeleteArrayElementAtIndex(i);
            EditorGUILayout.EndVertical();
        }

        EditorGUI.indentLevel--;

        if (GUILayout.Button("Add Trap"))
        {
            _trapProp.InsertArrayElementAtIndex(_trapProp.arraySize);
            var newElem = _trapProp.GetArrayElementAtIndex(_trapProp.arraySize - 1);
            newElem.FindPropertyRelative("position").vector2IntValue = Vector2Int.zero;
            newElem.FindPropertyRelative("trapPrefab").objectReferenceValue = null;
        }

        //クリスタルの編集処理
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("■ Crystal Settings ■", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(
            _crystalProp.FindPropertyRelative("Array.size"),
            new GUIContent("Count")
        );

        EditorGUI.indentLevel++;
        for (int i = 0; i < _crystalProp.arraySize; i++)
        {
            var elem = _crystalProp.GetArrayElementAtIndex(i);
            var pos = elem.FindPropertyRelative("position");
            var crystalType = elem.FindPropertyRelative("type");

            EditorGUILayout.BeginVertical("box");
            EditorGUILayout.PropertyField(pos, new GUIContent("Crystal Position"));
            EditorGUILayout.PropertyField(crystalType, new GUIContent("CrystalType"));

            if (GUILayout.Button("Remove"))
                _crystalProp.DeleteArrayElementAtIndex(i);
            EditorGUILayout.EndVertical();
        }

        EditorGUI.indentLevel--;

        if (GUILayout.Button("Add Crystal"))
        {
            _crystalProp.InsertArrayElementAtIndex(_crystalProp.arraySize);
            var newElem = _crystalProp.GetArrayElementAtIndex(_crystalProp.arraySize - 1);
            newElem.FindPropertyRelative("position").vector2IntValue = Vector2Int.zero;
            newElem.FindPropertyRelative("type").enumValueIndex = 0;
        }

        serializedObject.ApplyModifiedProperties();
    }

    private void DrawTileMapGUI(LevelData data)
    {
        int w = data.width, h = data.height;

        Event e = Event.current;

        for (int y = h - 1; y >= 0; y--)
        {
            EditorGUILayout.BeginHorizontal();
            for (int x = 0; x < w; x++)
            {
                //今のタイル
                var t = data.GetTileType(x, y);
                //背景色
                var prev = GUI.backgroundColor;
                GUI.backgroundColor = tileColors[(int)t];

                //クリック領域の設定
                Rect rect = GUILayoutUtility.GetRect(20, 20);
                GUI.Box(rect, GUIContent.none);

                if (e.type == EventType.MouseDown && rect.Contains(e.mousePosition))
                {
                    int index = (int)t;
                    if (e.button == 0)
                    {
                        index = (index + 1) % tileColors.Length;
                    }
                    else if (e.button == 1)
                    {
                        index = (index - 1 + tileColors.Length) % tileColors.Length;
                    }

                    data.SetTileType(x, y, (LevelData.TileType)index);
                    EditorUtility.SetDirty(data);
                    e.Use();
                }

                GUI.backgroundColor = prev;
            }

            EditorGUILayout.EndHorizontal();
        }
    }
}