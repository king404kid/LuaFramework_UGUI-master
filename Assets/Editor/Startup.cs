using UnityEngine;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using LuaFramework;
using UnityEngine.UI;

[InitializeOnLoad]
public class Startup
{
    // 编辑器运行后自动调用此静态函数
    static Startup() {
        EditorApplication.playmodeStateChanged += OnplaymodeStateChanged;
        //EditorApplication.hierarchyWindowChanged += hierarchyWindowChanged;
        SceneView.onSceneGUIDelegate += OnUISceneGUI;
    }

    /// <summary>
    /// 当hierarchy视图上的物品发生改变（创建，改名，父节点变化，删除等）调用
    /// </summary>
    static void hierarchyWindowChanged() {
        string path = UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene().path;
        if (path.Contains("Assets/UINew/scene")) {
            EditorApplication.ExecuteMenuItem("Assets/查看引用图片");
        }
    }

    /// <summary>
    /// 当编辑器的播放模式改变的时候调用
    /// </summary>
    public static void OnplaymodeStateChanged() {
        Debug.Log("OnplaymodeStateChanged\t" + EditorApplication.isPlayingOrWillChangePlaymode + "\t" + Application.isPlaying + "\t" + EditorApplication.isPlaying);
        if (EditorApplication.isPlayingOrWillChangePlaymode) {
            if (EditorApplication.isPlaying) {
                GameObject obj = GameObject.Find("run_main");
                if (obj && obj.activeSelf) {
                    //Application.LoadLevel(0);        // 废弃
                    SceneManager.LoadScene(0);
                }
            }
        }
    }

    /// <summary>
    /// 绘制scene视图
    /// </summary>
    /// <param name="scene"></param>
    static void OnUISceneGUI(SceneView scene) {
        string spath = UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene().path;
        if (!spath.Contains("Assets/UINew/scene")) {
            return;
        }
        GameObject obj = GameObject.Find("Canvas/Prefab");
        if (obj == null) {
            return;
        }
        Handles.BeginGUI();
        //规定GUI显示区域
        GUILayout.BeginArea(new Rect(5, 5, 400, 100));
        GUILayout.BeginHorizontal();
        //GUI绘制一个按钮
        if (GUILayout.Button("生成UI预设")) {
            Scene2Prefab.CreatePrefab(obj, true);
            AssetDatabase.Refresh();
        }
        if (GUILayout.Button("生成UI预设并启动")) {
            Scene2Prefab.CreatePrefab(obj, true);
            AssetDatabase.Refresh();
            EditorApplication.isPlaying = true;
        }
        if (GUILayout.Button("生成普通预设")) {
            Scene2Prefab.CreatePrefab(obj, false);
            AssetDatabase.Refresh();
        }
        //显示引用图集数量
        Dictionary<string, List<string>> dic = UITools.GetAtlasRefDetailByScenes(spath);
        string filename = FileTools.GetFilenameWithRelativePath(spath, false);
        List<string> list = dic[filename];
        GUIStyle style = new GUIStyle();
        style.richText = true;
        if (list.Count > 2) {
            GUILayout.Label("<color='#ff0000'><size=20>引用图集：" + list.Count + "</size></color>", style);
        } else {
            GUILayout.Label("<color='#00ff00'><size=20>引用图集：" + list.Count + "</size></color>", style);
        }
        GUILayout.EndHorizontal();
        GUILayout.EndArea();
        Handles.EndGUI();
    }
}