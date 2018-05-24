using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class TestWin : EditorWindow
{
    static private TestWin _instance;

    public static TestWin Instance {
        get {
            if (_instance == null) {
                _instance = EditorWindow.CreateInstance<TestWin>();
                _instance.titleContent = new GUIContent("测试面板");
                _instance.maxSize = new Vector2(400, 500);
                _instance.minSize = new Vector2(400, 500);
            }
            return TestWin._instance;
        }
    }

    [MenuItem("Tools/测试面板", false, 1)]
    static void ShowWin() {
        Instance.Show();
    }

    void OnGUI() {
        if (GUILayout.Button("测试ReplacePrefab方法")) {
            CreatePrefab();
        }
    }

    /// <summary>
    /// 如果预设创建了，则替换，没有则创建
    /// </summary>
    static void CreatePrefab() {
        GameObject[] objs = Selection.gameObjects;
        foreach (GameObject go in objs) {
            string localPath = "Assets/UINew/prefab/" + go.name + ".prefab";
            if (AssetDatabase.LoadAssetAtPath(localPath, typeof(GameObject))) {
                if (EditorUtility.DisplayDialog("Are you sure?",
                    "The prefab already exists. Do you want to overwrite it?",
                    "Yes",
                    "No"))
                CreateNew(go, localPath);
            } else {
                CreateNew(go, localPath);
            }
        }
    }

    static void CreateNew(GameObject obj, String localPath) {
        UnityEngine.Object prefab = PrefabUtility.CreateEmptyPrefab(localPath);
        PrefabUtility.ReplacePrefab(obj, prefab, ReplacePrefabOptions.ConnectToPrefab);
        //PrefabUtility.ReplacePrefab(obj, prefab, ReplacePrefabOptions.ReplaceNameBased);
    }
}