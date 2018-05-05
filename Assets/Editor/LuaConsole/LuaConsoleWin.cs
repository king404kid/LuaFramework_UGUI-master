using UnityEngine;
using System.Collections;
using UnityEditor;
using LuaFramework;
using System;

public class LuaConsoleWin : EditorWindow
{

    static private LuaConsoleWin _instance;

    public static LuaConsoleWin Instance {
        get {
            if (_instance == null) {
                _instance = EditorWindow.CreateInstance<LuaConsoleWin>();
                _instance.maxSize = new Vector2(600, 600);
                _instance.minSize = new Vector2(600, 600);
                _instance.titleContent = new GUIContent("lua控制台");
                //_instance.InitPath();
            }
            return LuaConsoleWin._instance;
        }
    }

    private string luaCmdStr = "";

    private string luaOutPutStr = "";

    private Vector2 outPutPos = new Vector2();

    public LuaManager luaMgr {
        get { return AppFacade.Instance.GetManager<LuaManager>(ManagerName.Lua); }
    }

    [MenuItem("Tools/lua控制台", false, 101)]
    static void ShowWin() {
        Instance.Show();
    }

    void OnGUI() {
        if (Application.isPlaying == false) {
            ShowNotPlaying();
        } else {
            ShowCtrl();
        }
    }

    private void ShowNotPlaying() {
        GUILayout.Label("只能在运行中使用");
    }

    private void ShowCtrl() {
        ShowOutPut();
        ShowInput();
    }

    private void ShowOutPut() {
        GUILayout.BeginArea(new Rect(0, 0, 600, 440));
        ShowLine("输出");
        outPutPos = GUILayout.BeginScrollView(outPutPos, GUILayout.Width(590), GUILayout.Height(390));
        GUI.SetNextControlName("OutputField");
        luaOutPutStr = EditorGUILayout.TextArea(luaOutPutStr, GUILayout.Width(570));
        GUILayout.EndScrollView();
        if (GUILayout.Button("清除输出")) {
            luaOutPutStr = "";
            GUI.FocusControl("InputField");
        }
        GUILayout.EndArea();
    }

    private void ShowInput() {
        GUILayout.BeginArea(new Rect(0, 450, 600, 150));
        ShowLine("输入");
        GUI.SetNextControlName("InputField");
        luaCmdStr = EditorGUILayout.TextArea(luaCmdStr, GUILayout.Width(580), GUILayout.Height(90));
        if (GUILayout.Button("执行", GUILayout.Height(30))) {
            DoString(luaCmdStr);
        }

        GUILayout.EndArea();
    }

    private void DoString(string cmdStr) {
        string tempStr = cmdStr;
        Add2OutPut("-----------start cmd------------");
        Add2OutPut(cmdStr);
        luaCmdStr = "";
        try {
            luaMgr.luaState.DoString(tempStr);
            //object[] objs = luaMgr.luaState.DoString(tempStr);
            //if (objs != null && objs.Length > 0) {
            //    for (int i = 0; i < objs.Length; i++) {
            //        Add2OutPut(objs[i].ToString());
            //    }
            //}
        } catch (Exception ex) {
            Add2OutPut(ex.Message);
        }
        Add2OutPut("-----------end cmd------------");
        GUI.FocusControl("OutputField");
    }

    private void Add2OutPut(string str) {
        luaOutPutStr += str;
        luaOutPutStr += "\n";
    }

    private void ShowLine(string str = "", int w = 600, int h = 20) {
        if (string.IsNullOrEmpty(str)) {
            h = 5;
        }
        GUILayout.Box(str, GUILayout.Width(w), GUILayout.Height(h));
    }
}
