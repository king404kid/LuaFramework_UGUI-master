using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using LuaFramework;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using LuaEditor;

public class Scene2Prefab
{
    static private Scene2Prefab _instance;

    [MenuItem("Tools/场景文件夹---->预设", false, 200)]
    static void SceneToPrefab() {
        Object obj = Selection.activeObject;
        //UnityEngine.Object[] arr = Selection.GetFiltered(typeof(UnityEngine.Object), SelectionMode.TopLevel);
        //Object obj = arr[0];
        if (obj == null) {
            ShowTips("请在Project视图选中您需要转换的场景文件夹");
            return;
        }
        string path = AssetDatabase.GetAssetPath(obj);
        if (path.Contains("Assets/UINew/scene")) {
            path = path.Replace("Assets/UINew/scene", "/UINew/scene");
        } else {
            ShowTips("请在Assets/UINew/scene路径下选中您需要转换的场景文件夹");
            return;
        }
        string folderPath = Application.dataPath + path;
        CreateUIPrefab(folderPath);
    }

    /// <summary>
    /// 创建文件夹下的go为prefab
    /// </summary>
    /// <param name="folderPath"></param>
    private static void CreateUIPrefab(string folderPath) {
        string[] prefabs = System.IO.Directory.GetFiles(folderPath, "*.unity", System.IO.SearchOption.AllDirectories);
        int count = prefabs.Length;
        for (int i = 0; i < count; ++i) {
            string scene = prefabs[i];
            string sceneName = System.IO.Path.GetFileName(scene);
            EditorUtility.DisplayProgressBar("正在转化场景", "场景名称：" + sceneName + "(" + i + "/" + count + ")", (float)i / count);
            EditorSceneManager.OpenScene(scene);
            GameObject prefabobj = GameObject.Find("Canvas/Prefab");
            if (prefabobj != null) {
                CreatePrefab(prefabobj, false);
            }
        }
        EditorUtility.ClearProgressBar();
        AssetDatabase.Refresh();
    }

    /// <summary>
    /// 创建一个prefab
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="createLuafile"></param>
    /// <param name="alert"></param>
    public static void CreatePrefab(GameObject obj, bool createLuafile, bool alert = true) {
        // prefab部分
        UITools.SetGameObjectLayer(obj.transform, LayerMask.NameToLayer("UI"));
        Transform parent = obj.transform.parent;
        Dictionary<Transform, Transform> dic = new Dictionary<Transform, Transform>();
        Dictionary<Transform, int> dicindex = new Dictionary<Transform, int>();
        Transform[] tfs = obj.transform.GetComponentsInChildren<Transform>(true);
        foreach (Transform tf in tfs) {     // 去除#np的节点
            if (tf.name.IndexOf(Constants.LUA_SUFFIX) >= 0) {
                dic.Add(tf, tf.parent);
                dicindex.Add(tf, tf.GetSiblingIndex());
                tf.SetParent(parent, true);
            }
        }
        Scene scene = UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene();
        string path = scene.path;
        if (!path.Contains(Application.dataPath)) {
            path = path.Replace("Assets/UINew/", "/UINew/");
            path = Application.dataPath + path;
        }
        string ext = System.IO.Path.GetExtension(path);
        string filename = System.IO.Path.GetFileNameWithoutExtension(path);
        path = path.Replace(ext, ".prefab").Replace("Assets/UINew/scene", "Assets/UINew/prefab");
        string folder = System.IO.Path.GetDirectoryName(path);
        string assetpath = UnityEditor.FileUtil.GetProjectRelativePath(path);
        string respath = assetpath.Replace(".prefab", "").Replace("Assets/UINew/prefab/", "");
        if (!System.IO.Directory.Exists(folder)) {
            System.IO.Directory.CreateDirectory(folder);
        }
        // lua部分
        LuaBehaviour lb = obj.GetComponent<LuaBehaviour>();
        if (lb != null) {
            createLuafile = true;
        }
        if (createLuafile) {
            if (lb == null || string.IsNullOrEmpty(lb.luaFile)) {
                if (!UILuaEditor.CreateUIFile(obj, filename, respath)) {
                    return;
                }
            } else {
                if (filename == System.IO.Path.GetFileNameWithoutExtension(lb.luaFile)) {
                    UILuaEditor.ModificationLuaFile(obj, filename, respath, alert);
                } else {
                    UnityEditor.EditorUtility.DisplayDialog("生成失败", "lua 文件名不匹配！", "OK");
                    return;
                }
            }
        }
        // 生成prefab
        if (System.IO.File.Exists(path)) {
            GameObject tar = AssetDatabase.LoadAssetAtPath<GameObject>(assetpath);
            PrefabUtility.ReplacePrefab(obj, tar, ReplacePrefabOptions.ReplaceNameBased);
        } else {
            PrefabUtility.CreatePrefab(assetpath, obj);
        }
        string bundlename = assetpath.Replace(".prefab", ".unity3d").Replace("Assets/UINew/prefab", "prefab");
        CreateAssetBundle.SetAssetName(assetpath, bundlename, false);
        foreach (KeyValuePair<Transform, Transform> kv in dic) {     // 还原#np的节点
            kv.Key.SetParent(kv.Value, true);
            kv.Key.SetSiblingIndex(dicindex[kv.Key]);
        }
        dic.Clear();
        dic = null;
        dicindex.Clear();
        dicindex = null;
        EditorSceneManager.SaveScene(SceneManager.GetActiveScene());
    }

    [MenuItem("File/新建界面 无", false, 6)]
    static public void NewComSecondBackdropUI4() {
        NewComUIScene(-1);
        AssetDatabase.Refresh();
    }

    /// <summary>
    /// 创建空的UI场景
    /// </summary>
    /// <param name="type"></param>
    /// <param name="path"></param>
    static public void NewComUIScene(int type, string path = null) {
        if (string.IsNullOrEmpty(path)) {
            path = EditorUtility.SaveFilePanel("选择", Application.dataPath + "/UINew/scene", "Noname", "unity");
        }
        if (string.IsNullOrEmpty(path)) {
            return;
        }
        string folder = FileTools.GetDirectoryName(path);
        FileTools.CreateDirectoryIfNotExist(folder);
        Scene scene = EditorSceneManager.OpenScene("Assets/UINew/scene/template/template.unity");
        ModifySceneBackground(type, scene);
        EditorSceneManager.SaveScene(scene, path);
        AssetDatabase.Refresh();
        EditorGUIUtility.PingObject(AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(FileTools.GetRelativePath(path)));
    }

    /// <summary>
    /// 修改背景框
    /// </summary>
    /// <param name="type"></param>
    /// <param name="scene"></param>
    public static void ModifySceneBackground(int type, Scene scene) {
        Transform background = GameObject.Find("Canvas/background").transform;
        while (background.childCount > 0) {
            Transform child = background.GetChild(0);
            UnityEngine.Object.DestroyImmediate(child.gameObject);
        }
        GameObject prefab = null;
        if (type == 0) {
            prefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/UINew/prefab/common/ComBackdropUI.prefab");
        } else if (type > 0) {
            prefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/UINew/prefab/common/ComSecondBackdropUI.prefab");
        }
        if (prefab != null) {
            GameObject bg = PrefabUtility.InstantiatePrefab(prefab, scene) as GameObject;
            if (type > 0) {
                for (int i = 1; i < 4; i++) {
                    bg.transform.Find("node" + i + "#go").gameObject.SetActive(type == i);
                }
            }
            bg.transform.SetParent(background, false);
        }
    }

    #region 其他显示

    /// <summary>
    /// 显示提示
    /// </summary>
    /// <param name="content"></param>
    private static void ShowTips(string content) {
        EditorUtility.DisplayDialog("提示", content, "确定");
    }

    #endregion
}