using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;
using System.Text.RegularExpressions;
using System.IO;

public class LanguageWin : EditorWindow
{
    static private LanguageWin _instance;

    // lua面板
    private static int startID;//需要导出的起始id
    private string luaLangPath = "";//语言表的路径
    private static int gameTipConfigMaxID = 0;// 游戏提示表里面的最大ID
    private Dictionary<string, string> allTextInfoDict_lua = new Dictionary<string, string>();//包含了新旧所有信息的字典（包含翻译的和未翻译的）
    private Dictionary<string, string> currentFindDict_lua = new Dictionary<string, string>();//重新遍历得到的数据（包含新和旧的）
    private string allText_lua = "";//所有字符串

    // 场景预设面板
    private string scenePrefabLangPath = "";//语言表的路径
    private Object folderScene;//需要遍历的场景文件夹
    private Object folderPrefab;//需要遍历的预制体文件夹
    private Dictionary<string, string> allTextInfoDict = new Dictionary<string, string>();//包含了新旧所有信息的字典（包含翻译的和未翻译的）
    private Dictionary<string, string> currentFindDict = new Dictionary<string, string>();//重新遍历得到的数据（包含新和旧的）
    private Dictionary<string, string> revertDict = new Dictionary<string, string>();//逆向字典，用于还原的（key翻译，value原字符串）
    private string allText = "";//所有字符串

    [MenuItem("Tools/多语言工具", false, 102)]
    static void ShowWin() {
        Instance.Show();
    }

    public static LanguageWin Instance {
        get {
            if (_instance == null) {
                _instance = (LanguageWin)EditorWindow.GetWindow(typeof(LanguageWin));
                _instance.titleContent = new GUIContent("批量转换语言");
                _instance.maxSize = new Vector2(500, 300);
                _instance.minSize = new Vector2(500, 300);
                _instance.InitData();
            }
            return LanguageWin._instance;
        }
    }

    private void InitData() {
        folderScene = AssetDatabase.LoadAssetAtPath(Constants.SCENE_PATH, typeof(Object));
        folderPrefab = AssetDatabase.LoadAssetAtPath(Constants.PREFAB_PATH, typeof(Object));
    }

    void OnGUI() {
        ShowLuaWin();
        ShowScenePrefabWin();
    }

    //-----------------------------lua面板-----------------------------
    private void ShowLuaWin() {
        ShowLine("翻译lua");
        GUILayout.BeginHorizontal();
        GUILayout.Label("国际语言版本路径, 具体到csv:", GUILayout.Width(180));
        luaLangPath = EditorGUILayout.TextField(luaLangPath, GUILayout.Width(220));
        if (GUILayout.Button("选择", GUILayout.Width(60))) {
            luaLangPath = SelectFile(luaLangPath);
        }
        GUILayout.EndHorizontal();
        if (luaLangPath != "") {
            GUILayout.BeginHorizontal();
            GUILayout.Label("请填写y_游戏提示信息表里面的最大ID:");
            gameTipConfigMaxID = int.Parse(EditorGUILayout.TextField(gameTipConfigMaxID.ToString(), GUILayout.Width(100)));
            if (GUILayout.Button("扫描并保存lua中文", GUILayout.Width(160))) {
                CheckLuaChinese();
            }
            GUILayout.EndHorizontal();
        }
        EditorGUILayout.Space();
        EditorGUILayout.Space();
        EditorGUILayout.Space();
        EditorGUILayout.Space();
    }

    /// <summary>
    /// 检查所有lua的中文
    /// </summary>
    void CheckLuaChinese() {
        LoadOldCsvData_Lua();
        string path = FileTools.GetFullPath(Constants.LUA_PATH);
        string[] subPaths = Directory.GetFiles(path, "*.lua", SearchOption.AllDirectories);
        startID = 1 + gameTipConfigMaxID;
        for (int i = 0; i < subPaths.Length; i++) {
            EditorUtility.DisplayProgressBar("正在检查lua文件", subPaths[i], (float)i / (float)subPaths.Length);
            GetOneLuaTextList(subPaths[i]);
        }
        CheckData_lua();
        EditorUtility.ClearProgressBar();
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        ShowTips("提取lua并替换完成");
    }

    /// <summary>
    /// 检查单个文件
    /// </summary>
    /// <param name="path"></param>
    void GetOneLuaTextList(string path) {
        string filename = FileTools.GetFilename(path, false);
        string[] lines = FileTools.ReadAllLines(path);
        for (int j = 0; j < lines.Length; j++) {
            string content = lines[j];
            if (string.IsNullOrEmpty(content) == true) {
                continue;
            }
            if (StringTools.isContainCN(content) == false) {
                continue;
            }
            if (content.Contains("error") || content.Contains("print") || content.Contains("--")) {
                continue;
            }
            int index = content.IndexOf('"');
            if (index == -1) {
                continue;
            }
            string[] arr = content.Split('"');
            int len = arr.Length;
            List<string> lineList = new List<string>();
            for (int m = 0; m < len; m++) {
                lineList.Add(arr[m]);
            }
            lines[j] = ReplaceSingleLine(lines[j], lineList, filename);
        }
        FileTools.SaveAllLines(path, lines);
    }

    //加载旧语言配置表
    public void LoadOldCsvData_Lua() {
        if (FileTools.IsFileExist(luaLangPath) == false) {
            ShowTips(luaLangPath + "不存在，请先选择路径");
            return;
        }
        string str = FileTools.ReadFileText(luaLangPath);
        if (string.IsNullOrEmpty(str) == true) {
            return;
        }
        if (str == "\r\n") {
            str = "";
            allText_lua = "";
            return;
        }
        allText_lua = str;
        string[] lines = str.Split('\n');
        foreach (string i in lines) {
            string lineStr = i.ToString();
            if (string.IsNullOrEmpty(lineStr)) {
                continue;
            }
            string[] subStrs = lineStr.Split(',');
            if (subStrs == null || subStrs.Length < 2) {
                continue;
            }
            string key = subStrs[0];
            allTextInfoDict_lua[key] = lineStr;
        }
    }

    private string ReplaceSingleLine(string line, List<string> arr, string filename) {
        int len = arr.Count;
        if (len == 0) {
            return line;
        }
        string s = arr[0];
        arr.RemoveAt(0);
        if (StringTools.isContainCN(s) == false) {
            return ReplaceSingleLine(line, arr, filename);
        }
        if (allTextInfoDict_lua.ContainsKey(s)) {  // 已经存在则直接替换
            string str = allTextInfoDict_lua[s];
            int id = int.Parse(str.Split(',')[1]);
            line = line.Replace('"' + s + '"', "GameTipsMgr:GetGameTips(" + id + ")");
            return ReplaceSingleLine(line, arr, filename);
        }
        if (currentFindDict_lua.ContainsKey(s)) {
            string old_text = currentFindDict_lua[s];
            if (currentFindDict_lua[s].Contains(filename) == false) {
                old_text = old_text.Replace("\n", "");
                currentFindDict_lua[s] = old_text + "DaDouHao" + filename + '\n';
            }
            int id = int.Parse(old_text.Split(',')[1]);
            line = line.Replace('"' + s + '"', "GameTipsMgr:GetGameTips(" + id + ")");
        } else {
            s = ChangeStr(s, true);
            string str = s + "," + startID + "," + filename + "\n";
            currentFindDict_lua[s] = str;
            line = line.Replace('"' + s + '"', "GameTipsMgr:GetGameTips(" + startID + ")");
            startID++;
        }
        return ReplaceSingleLine(line, arr, filename);
    }

    //检查旧表是否有新表的数据
    private void CheckData_lua() {
        foreach (string key in currentFindDict_lua.Keys) {
            if (allTextInfoDict_lua.ContainsKey(key) == false) {
                allTextInfoDict_lua[key] = currentFindDict_lua[key];
                allText_lua += currentFindDict_lua[key];
            } else {
                string[] subStrs = currentFindDict_lua[key].Split(',');
                string translate_str = GetTranslateStr_lua(key);
                string replace_text = subStrs[0] + "," + translate_str + "," + subStrs[2];
                allText_lua = allText_lua.Replace(allTextInfoDict_lua[key] + '\n', replace_text);
                allTextInfoDict_lua[key] = replace_text;
            }

        }
        AddDataToCsv_lua();
    }

    private void AddDataToCsv_lua() {
        FileTools.SaveFile(luaLangPath, allText_lua);
        allText_lua = "";
    }

    //获取翻译语句
    private string GetTranslateStr_lua(string key) {
        if (allTextInfoDict_lua.ContainsKey(key) == false) {
            return "";
        }
        string str = allTextInfoDict_lua[key];
        if (str == null || str.Length == 0) {
            return "";
        }
        string[] lines = str.Split(',');
        return lines[1];
    }

    //-----------------------------场景预设面板-----------------------------
    private void ShowScenePrefabWin() {
        ShowLine("翻译unity场景和预设");
        GUILayout.BeginHorizontal();
        GUILayout.Label("国际语言版本路径, 具体到csv:", GUILayout.Width(180));
        scenePrefabLangPath = EditorGUILayout.TextField(scenePrefabLangPath, GUILayout.Width(220));
        if (GUILayout.Button("选择", GUILayout.Width(60))) {
            scenePrefabLangPath = SelectFile(scenePrefabLangPath);
        }
        GUILayout.EndHorizontal();
        if (scenePrefabLangPath != "") {
            GUILayout.BeginHorizontal();
            GUILayout.Label("1. 翻译前会先导入旧的csv（含翻译内容）和导出新加的文本：");
            if (GUILayout.Button("第一步", GUILayout.Width(100))) {
                ImportExportText();
            }
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            GUILayout.Label("2. 根据导入内容进行翻译：");
            if (GUILayout.Button("第二步", GUILayout.Width(100))) {
                TranslateFontFun(false);
            }
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            GUILayout.Label("3. 根据导入内容进行逆向翻译（即还原中文）：");
            if (GUILayout.Button("还原（可选）", GUILayout.Width(100))) {
                TranslateFontFun(true);
            }
            GUILayout.EndHorizontal();
        }
    }

    /// <summary>
    /// 先导入旧的csv（含翻译内容）和导出新加的文本
    /// </summary>
    private void ImportExportText() {
        LoadOldCsvData();
        string assetPath = AssetDatabase.GetAssetPath(folderScene);
        string rootPath = Application.dataPath.Replace("Assets", "");
        rootPath = rootPath + assetPath;
        string[] paths = Directory.GetFiles(rootPath, "*.unity", SearchOption.AllDirectories);
        //获取到所有场景Text数据
        for (int i = 0; i < paths.Length; i++) {
            EditorUtility.DisplayProgressBar("正在提取场景中文", paths[i], (float)i / (float)paths.Length);
            if (paths[i] != "") {
                GetOneSceneTextList(paths[i]);
            }
        }
        EditorUtility.ClearProgressBar();
        CheckData();
        ShowTips("提取完成");
    }

    //获取到单个场景所有text数据
    private void GetOneSceneTextList(string path) {
        string realPath = "Assets" + path.Replace(Application.dataPath, "");
        EditorSceneManager.OpenScene(realPath);
        GameObject go = GameObject.Find("Canvas/Prefab");
        if (go == null) {
            return;
        }
        string loadedName = SceneManager.GetActiveScene().name;
        //获取所有的text
        Text[] texts = go.GetComponentsInChildren<Text>(true);
        if (texts == null || texts.Length == 0) {
            return;
        }
        if (texts != null) {
            for (int i = 0; i < texts.Length; i++) {
                if (texts[i].text == "") {
                    continue;
                }
                if (StringTools.isContainCN(texts[i].text) == false) {
                    continue;
                }
                bool is_translate = IsHadTranslate(texts[i].text);
                if (is_translate) {
                    continue;
                }
                if (currentFindDict.ContainsKey(texts[i].text)) {
                    if (currentFindDict[texts[i].text].Contains(loadedName) == false) {
                        string old_text = currentFindDict[texts[i].text];
                        old_text = old_text.Replace("\n", "");
                        currentFindDict[texts[i].text] = old_text + "DaDouHao" + loadedName + '\n';
                    }
                } else {
                    string text = (texts[i].text);
                    text = ChangeStr(text, true);
                    string str = text + "," + "," + loadedName + "\n";
                    currentFindDict[text] = str;
                }
            }
        }
    }

    //加载旧语言配置表
    public void LoadOldCsvData() {
        if (FileTools.IsFileExist(scenePrefabLangPath) == false) {
            ShowTips(scenePrefabLangPath + "不存在，请先选择路径");
            return;
        }
        string str = FileTools.ReadFileText(scenePrefabLangPath);
        if (string.IsNullOrEmpty(str) == true) {
            return;
        }
        if (str == "\r\n") {
            str = "";
            allText = "";
            return;
        }
        allText = str;
        string[] lines = str.Split('\n');
        foreach (string i in lines) {
            string lineStr = i.ToString();
            if (string.IsNullOrEmpty(lineStr)) {
                continue;
            }
            string[] subStrs = lineStr.Split(',');
            if (subStrs == null || subStrs.Length < 2) {
                continue;
            }
            string key = subStrs[0];
            allTextInfoDict[key] = lineStr;
            if (subStrs[1] != "") {
                revertDict[subStrs[1]] = key;
            }
        }
    }

    //检查旧表是否有新表的数据
    private void CheckData() {
        foreach (string key in currentFindDict.Keys) {
            if (allTextInfoDict.ContainsKey(key) == false) {
                allTextInfoDict[key] = currentFindDict[key];
                string[] subStrs = currentFindDict[key].Split(',');
                revertDict[subStrs[1]] = key;
                allText += currentFindDict[key];
            } else {
                string[] subStrs = currentFindDict[key].Split(',');
                string translate_str = GetTranslateStr(key);
                string replace_text = subStrs[0] + "," + translate_str + "," + subStrs[2];
                allText = allText.Replace(allTextInfoDict[key] + '\n', replace_text);
                allTextInfoDict[key] = replace_text;
            }

        }
        AddDataToCsv();
    }

    private void AddDataToCsv() {
        FileTools.SaveFile(scenePrefabLangPath, allText);
        allText = "";
    }

    private void TranslateFontFun(bool is_recover) {
        ChangeFontSceneFun(is_recover);
        ChangeFontPrefabFun(is_recover);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        ShowTips("翻译完成");
    }

    private void ChangeFontSceneFun(bool is_recover) {
        if (folderScene == null) {
            return;
        }
        string assetPath = AssetDatabase.GetAssetPath(folderScene);
        string rootPath = Application.dataPath.Replace("Assets", "");
        rootPath = rootPath + assetPath;
        string[] paths = Directory.GetFiles(rootPath, "*.unity", SearchOption.AllDirectories);
        string lang = "中文"; 
        if (is_recover) {
            lang = "外文";
        }
        for (int i = 0; i < paths.Length; i++) {
            EditorUtility.DisplayProgressBar("正在替换场景的" + lang, paths[i], (float)i / (float)paths.Length);
            ChangeOneFileScene(paths[i], is_recover);
        }
        EditorUtility.ClearProgressBar();
    }

    private void ChangeFontPrefabFun(bool is_recover) {
        if (folderPrefab == null) {
            return;
        }
        string assetPath = AssetDatabase.GetAssetPath(folderPrefab);
        string rootPath = Application.dataPath.Replace("Assets", "");
        rootPath = rootPath + assetPath;
        string[] paths = Directory.GetFiles(rootPath, "*.prefab", SearchOption.AllDirectories);
        string lang = "中文";
        if (is_recover) {
            lang = "外文";
        }
        for (int i = 0; i < paths.Length; i++) {
            EditorUtility.DisplayProgressBar("正在替换预设的" + lang, paths[i], (float)i / (float)paths.Length);
            ChangeOneFilePrefab(paths[i], is_recover);
        }
        EditorUtility.ClearProgressBar();
    }

    //改变场景中的字体
    private void ChangeOneFileScene(string path, bool is_recover) {
        string realPath = "Assets" + path.Replace(Application.dataPath, "");
        EditorSceneManager.OpenScene(realPath);
        GameObject go = GameObject.Find("Canvas/Prefab");
        if (go == null) {
            return;
        }
        Text[] texts = go.GetComponentsInChildren<Text>(true);
        if (texts == null || texts.Length == 0) {
            return;
        }
        int count = 0;
        for (int i = 0; i < texts.Length; i++) {
            if (texts[i].text == "") {
                continue;
            }
            if (StringTools.isContainCN(texts[i].text) == false) {
                continue;
            }
            if (is_recover == false) {
                string key = texts[i].text;
                key = ChangeStr(key, true);
                //没有该文本或已翻译过
                if (allTextInfoDict.ContainsKey(key) == false) {
                    continue;
                }
                string text = GetTranslateStr(key);
                text = ChangeStr(text, false);
                if (text == "") {
                    continue;
                }
                //翻译
                texts[i].text = text;
                EditorUtility.SetDirty(texts[i]);
                count++;
            } else {
                string key = texts[i].text;
                key = ChangeStr(key, true);
                string text = GetOriginalStr(key);
                if (text == "") {
                    continue;
                }
                text = ChangeStr(text, false);
                texts[i].text = text;
                EditorUtility.SetDirty(texts[i]);
                count++;
            }
        }
        if (count > 0) {
            EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene());
        }
    }

    //改变预设中的字体
    private void ChangeOneFilePrefab(string path, bool is_recover) {
        string realPath = "Assets" + path.Replace(Application.dataPath, "");
        Object assetObj = AssetDatabase.LoadAssetAtPath(realPath, typeof(Object));
        if (assetObj is GameObject) {
            GameObject go = (GameObject)assetObj;
            if (go == null) {
                return;
            }
            Text[] texts = go.GetComponentsInChildren<Text>(true);
            if (texts == null || texts.Length == 0) {
                return;
            }
            int count = 0;
            for (int i = 0; i < texts.Length; i++) {
                if (texts[i].text == "") {
                    continue;
                }
                if (StringTools.isContainCN(texts[i].text) == false) {
                    continue;
                }
                //翻译
                if (is_recover == false) {
                    string key = texts[i].text;
                    key = ChangeStr(key, true);
                    //没有该文本或已翻译过
                    if (allTextInfoDict.ContainsKey(key) == false) {
                        continue;
                    }
                    string text = GetTranslateStr(key);
                    text = ChangeStr(text, false);
                    texts[i].text = text;
                    count++;
                    EditorUtility.SetDirty(texts[i]);
                } else {
                    string key = texts[i].text;
                    key = ChangeStr(key, true);
                    string text = GetOriginalStr(key);
                    if (text == "") {
                        continue;
                    }
                    text = ChangeStr(text, false);
                    texts[i].text = text;
                    count++;
                    EditorUtility.SetDirty(texts[i]);
                }
            }
        }
    }

    //获取翻译语句
    private string GetTranslateStr(string key) {
        if (allTextInfoDict.ContainsKey(key) == false) {
            return "";
        }
        string str = allTextInfoDict[key];
        if (str == null || str.Length == 0) {
            return "";
        }
        string[] lines = str.Split(',');
        return lines[1];
    }

    //获取翻译语句
    private string GetOriginalStr(string key) {
        if (revertDict.ContainsKey(key) == false) {
            return "";
        }
        return revertDict[key];
    }

    //获取unity上的字体是否被翻译过
    private bool IsHadTranslate(string txt) {
        //如果旧的配置表有这个数据
        if (allTextInfoDict.ContainsKey(txt) == true) {
            return false;
        }
        //如果翻译表中没有这个数据
        if (revertDict.ContainsKey(txt) == false) {
            return false;
        }
        return true;
    }

    /// <summary>
    /// 转换\n符号，以免写入读入文件出问题
    /// </summary>
    /// <param name="txt"></param>
    /// <param name="is_change_line_str"></param>
    /// <returns></returns>
    private string ChangeLineFeedStr(string txt, bool is_change_line_str) {
        if (is_change_line_str) {
            return txt.Replace("\n", "DaHuanHang");
        } else {
            return txt.Replace("DaHuanHang", "\n");
        }
    }

    /// <summary>
    /// 转换,符号，以免写入读入文件出问题
    /// </summary>
    /// <param name="txt"></param>
    /// <param name="is_change_line_str"></param>
    /// <returns></returns>
    private string ChangeDouHaoStr(string txt, bool is_change_line) {
        if (is_change_line) {
            return txt.Replace(",", "DaDouHao");
        } else {
            return txt.Replace("DaDouHao", ",");
        }
    }

    /// <summary>
    /// 转换\r符号，以免写入读入文件出问题
    /// </summary>
    /// <param name="txt"></param>
    /// <param name="is_change_line_str"></param>
    /// <returns></returns>
    private string ChangeRStr(string txt, bool is_change_line) {
        if (is_change_line) {
            return txt.Replace("\r", "DaGangR");
        } else {
            return txt.Replace("\r", ",");
        }
    }

    /// <summary>
    /// 转换特殊符号，以免写入读入文件出问题
    /// </summary>
    /// <param name="txt"></param>
    /// <param name="is_change_line_str"></param>
    /// <returns></returns>
    private string ChangeStr(string txt, bool is_change) {
        string text = txt;
        text = ChangeLineFeedStr(text, is_change);
        text = ChangeDouHaoStr(text, is_change);
        text = ChangeRStr(text, is_change);
        return text;
    }

    /// <summary>
    /// 打开选择文件
    /// </summary>
    /// <param name="str"></param>
    /// <param name="ex"></param>
    /// <returns></returns>
    private string SelectFile(string str, string ex = "") {
        return EditorUtility.OpenFilePanel("选择文件", str, ex);
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