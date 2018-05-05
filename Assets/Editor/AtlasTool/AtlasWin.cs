using UnityEngine;
using System.Collections;
using UnityEditor;
using LuaFramework;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;
using System.IO;

public class AtlasWin : EditorWindow
{
    static private AtlasWin _instance;
    private Vector2 outPutPos = new Vector2();
    private string outPutStr = "";
    private Object folderObj;
    private string inPutStr = "";

    public static AtlasWin Instance {
        get {
            if (_instance == null) {
                _instance = EditorWindow.CreateInstance<AtlasWin>();
                _instance.titleContent = new GUIContent("检查图集");
                _instance.maxSize = new Vector2(400, 500);
                _instance.minSize = new Vector2(400, 500);
            }
            return AtlasWin._instance;
        }
    }

    [MenuItem("Tools/图集查询面板", false, 103)]
    static void ShowWin() {
        Instance.Show();
    }

    void OnGUI() {
        ShowUIContent1();
        ShowUIContent2();
    }

    void ShowUIContent1() {
        ShowLine("工具1");
        if (GUILayout.Button("查询当前场景图集引用数量")) {
            Scene scene = EditorSceneManager.GetActiveScene();
            if (scene == null) {
                ShowTips("当前没有激活的场景");
                return;
            }
            PrintResult(scene.path);
        }
    }

    /// <summary>
    /// 显示"查询多个场景图集"的ui内容
    /// </summary>
    void ShowUIContent2() {
        EditorGUILayout.Space();
        ShowLine("工具2");
        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("拖拉scene文件夹", GUILayout.Width(100));
        folderObj = EditorGUILayout.ObjectField(folderObj, typeof(Object), false, GUILayout.Width(100));
        if (GUILayout.Button("打印详情")) {
            if (folderObj == null) {
                ShowTips("请选择scene文件或文件夹");
                return;
            }
            string folderPath = AssetDatabase.GetAssetPath(folderObj);
            PrintResult(folderPath);
        }
        if (GUILayout.Button("清空")) {
            outPutStr = "";
            GUI.FocusControl("InputField");        // 不写这行代码不能清空，不知为何一定要把焦点移开才能清空
        }
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.LabelField("输出结果：");
        outPutPos = GUILayout.BeginScrollView(outPutPos, GUILayout.Width(390), GUILayout.Height(390));
        GUI.SetNextControlName("OutputField");
        outPutStr = EditorGUILayout.TextArea(outPutStr, GUILayout.Width(370), GUILayout.Height(490));
        GUILayout.EndScrollView();
        GUI.SetNextControlName("InputField");
        inPutStr = EditorGUILayout.TextArea(inPutStr, GUILayout.Width(370), GUILayout.Height(30));
    }

    void PrintResult(string folderPath) {
        Dictionary<string, List<string>> dic = UITools.GetAtlasRefDetailByScenes(folderPath);
        if (dic == null) {
            outPutStr = "当前场景引用了：0个图集";
            return;
        }
        outPutStr = "";
        foreach (KeyValuePair<string, List<string>> item in dic) {
            outPutStr += "场景：" + item.Key + "， 引用了：" + item.Value.Count + "个图集\n";
            for (int i = 0; i < item.Value.Count; i++) {
                outPutStr += "        " + item.Value[i] + "\n";
            }
	    }
    }

    #region 其他显示

    /// <summary>
    /// 显示提示
    /// </summary>
    /// <param name="content"></param>
    private void ShowTips(string content) {
        EditorUtility.DisplayDialog("提示", content, "确定");
    }

    /// <summary>
    /// 显示横条分割
    /// </summary>
    /// <param name="w"></param>
    /// <param name="h"></param>
    private void ShowLine(string t = "", float w = -1, float h = -1) {
        string content = "";
        float ww;
        float hh;
        if (!string.IsNullOrEmpty(t)) {
            content = t;
        }
        if (string.IsNullOrEmpty(content)) {
            if (w < 0) {
                ww = this.maxSize.x;
            } else {
                ww = w;
            }

            if (h < 0) {
                hh = 5;
            } else {
                hh = h;
            }
        } else {
            if (w < 0) {
                ww = this.maxSize.x;
            } else {
                ww = w;
            }

            if (h < 0) {
                hh = 20;
            } else {
                hh = h;
            }
        }
        GUILayout.Box(content, GUILayout.Width(ww), GUILayout.Height(hh));
    }

    #endregion
}