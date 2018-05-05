using LuaFramework;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

[CanEditMultipleObjects]
[CustomEditor(typeof(LuaBehaviour))]
public class LuaBehaviourEditor : Editor
{
    SerializedProperty luaName;
    LuaBehaviour luaBehaviour;
    string fieldType = "";
    Dictionary<string, bool> allFoldout = new Dictionary<string, bool>();

    void OnEnable() {
        luaName = serializedObject.FindProperty("luaFile");
        luaBehaviour = target as LuaBehaviour;
    }

    /// <summary>
    /// 渲染显示面板
    /// </summary>
    public override void OnInspectorGUI() {
        serializedObject.Update();
        EditorGUILayout.PropertyField(luaName, new GUIContent("Lua文件名:"));
        serializedObject.ApplyModifiedProperties();
        if (luaBehaviour == null) {
            return;
        }
        if (string.IsNullOrEmpty(luaBehaviour.luaFile)) {
            return;
        }
        string luaPath = Application.dataPath + "/lua/" + luaBehaviour.luaFile;
        fieldType = "";
        if (File.Exists(luaPath)) {
            string[] lines = File.ReadAllLines(luaPath);
            bool started = false;
            for (int i = 0; i < lines.Length; i++) {    // 逐行去分析
                string ln = lines[i].Trim();
                if (!started) {
                    if (ln.StartsWith("--") && ln.TrimStart('-', ' ') == "!@#definestart")
                        started = true;
                } else {
                    if (ln.StartsWith("--") && ln.TrimStart('-', ' ') == "!@#defineend")
                        break;
                    parseField(ln, luaBehaviour);
                }
            }
        } else {
            EditorGUILayout.HelpBox("classs is not exists:" + luaPath, MessageType.Error);
        }
    }

    /// <summary>
    /// 根据字符串获取类型
    /// </summary>
    /// <param name="realType"></param>
    /// <returns></returns>
    Type getType(string realType) {
        Type type = Type.GetType(realType + ",Assembly-CSharp");
        string stype = "";
        if (type == null) {
            stype = realType.Trim();
            type = System.Reflection.Assembly.Load("UnityEngine").GetType(stype);//UnityEngine.Types.GetType(stype, "UnityEngine");
        }
        if (type == null) {
            stype = realType.Trim();
            type = System.Reflection.Assembly.Load("UnityEngine.UI").GetType(stype);//UnityEngine.Types.GetType(stype, "UnityEngine.UI");
        }
        if (type == null) {
            type = Type.GetType("sw.ui.ext." + realType + ",Assembly-CSharp");
        }
        return type;
    }

    /// <summary>
    /// 逐行分析!@#definestart到!@#defineend的详细内容
    /// </summary>
    /// <param name="ln"></param>
    /// <param name="luaBehaviour"></param>
    void parseField(string ln, LuaBehaviour luaBehaviour) {
        int pos = ln.IndexOf("--");
        if (pos == 0) {
            fieldType = ln.Substring(2);
            return;
        }
        pos = ln.IndexOf("=");
        if (pos > 0) {
            string field = ln.Substring(0, pos).Trim();
            object value = null;
            BindItem item = null;
            if (luaBehaviour.bindObjs != null) {
                for (int i = 0; i < luaBehaviour.bindObjs.Count; i++) {
                    if (luaBehaviour.bindObjs[i].name == field) {
                        item = luaBehaviour.bindObjs[i];
                        value = luaBehaviour.bindObjs[i].obj;
                        break;
                    }
                }
            }
            if (fieldType == "") { 
                fieldType = "GameObject";
            }
            Type type = getType(fieldType);
            UnityEngine.Object go = EditorGUILayout.ObjectField(field, (item == null ? null : item.obj as UnityEngine.Object), type, true);
            if (item != null)
                item.obj = go;
            else if (go != null) {
                item = new BindItem();
                item.obj = go;
                item.name = field;
                if (luaBehaviour.bindObjs == null) { 
                    luaBehaviour.bindObjs = new List<BindItem>();
                }
                luaBehaviour.bindObjs.Add(item);
            }
            fieldType = "";
        }
    }
}