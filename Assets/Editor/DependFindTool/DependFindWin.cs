using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;
using System;
using System.IO;

public class DependFindWin : EditorWindow
{
    static private DependFindWin _instance;

    public static DependFindWin Instance {
        get {
            if (_instance == null) {
                _instance = (DependFindWin)EditorWindow.GetWindow(typeof(DependFindWin));
                _instance.titleContent = new GUIContent("检查资源依赖");
                _instance.maxSize = new Vector2(500, 650);
                _instance.minSize = new Vector2(500, 650);
            }
            return DependFindWin._instance;
        }
    }

    // 页签
    private int titleSelectIndex = 0;
    private string[] titleStrs = new string[] { "资源依赖", "查找资源", "资源使用情况", "查找大图" };
    private int typeIndex = 0;
    private string[] typeStrs = new string[] { "查找该资源有哪些依赖", "查找该资源被谁依赖了" };
    private int checkDependDeep = 0;
    private string[] dependDeepStrs = new string[] { "检查完整依赖", "只检查第一层依赖" };

    // 资源依赖
    private UnityEngine.Object selectObj;
    private UnityEngine.Object checkFolder;
    private List<DependObject> dependObjs;
    private Vector2 resultPos = new Vector2();

    // 查找资源
    private string guidStr = "";
    private UnityEngine.Object findObj;
    private int bundleCount = 0;

    // 资源使用情况
    private string prefabPath = "";
    private string texPath = "";
    private UnityEngine.Object prefabFolder;
    private UnityEngine.Object texFolder;
    private List<string> prefabDepends;
    private List<string> texNoDependPaths;
    private List<Texture2D> texNoDependTexs;
    private Vector2 texPos = new Vector2();
    private int checkTexSum = 0;

    // 查找大图
    private UnityEngine.Object findLargePicObj;
    private float largeWidth = 500;
    private float largeHeight = 500;
    private List<UnityEngine.Object> largePicList;
    private Vector2 largePicPos = new Vector2();

    [MenuItem("Tools/资源依赖查找", false, 104)]
    static void ShowWin() {
        DependFindWin.Instance.Show();
    }

    void OnGUI() {
        ShowSelectTitle();
        if (titleSelectIndex == 0) {     // 资源依赖
            ShowResDepend();
        } else if (titleSelectIndex == 1) {   // 查找资源
            ShowFindRes();
        } else if (titleSelectIndex == 2) {   // 资源使用情况
            ShowResUsedChcek();
        } else if (titleSelectIndex == 3) {   // 查找大图
            ShowFindLargePic();
        }
    }

    private void ShowSelectTitle() {
        titleSelectIndex = GUILayout.SelectionGrid(titleSelectIndex, titleStrs, 4, GUILayout.Width(450), GUILayout.Height(30));
        ShowLine();
    }

    //-----------------------------资源依赖-----------------------------
    private void ShowResDepend() {
        ShowLine("操作");
        GUILayout.Label("操作类型：");
        typeIndex = GUILayout.SelectionGrid(typeIndex, typeStrs, 2, GUILayout.Width(400), GUILayout.Height(30));
        if (typeIndex == 0) {
            checkDependDeep = GUILayout.SelectionGrid(checkDependDeep, dependDeepStrs, 2, GUILayout.Width(250), GUILayout.Height(30));
        }
        if (typeIndex == 1) {
            GUILayout.Label("请把需要查找的文件夹拖到下面的框中");
            checkFolder = EditorGUILayout.ObjectField(checkFolder, typeof(UnityEngine.Object), GUILayout.Width(200));
        }
        GUILayout.Label("请把需要查找的资源拖到下面的框中");
        selectObj = EditorGUILayout.ObjectField(selectObj, typeof(UnityEngine.Object), GUILayout.Width(200));
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("检查", GUILayout.Width(120), GUILayout.Height(50))) {
            Check();
        }
        GUILayout.EndHorizontal();
        ShowResult();
    }

    private void ShowResult() {
        ShowLine("依赖结果");
        if (dependObjs == null || dependObjs.Count == 0) {
            return;
        }
        GUILayout.BeginHorizontal();
        GUILayout.Label("依赖结果：" + dependObjs.Count + "个", GUILayout.Width(200));
        if (typeIndex == 0) {
            GUILayout.Label("其中有BundleName的依赖(不包括自己)：" + bundleCount + "个");
        }
        GUILayout.EndHorizontal();
        resultPos = GUILayout.BeginScrollView(resultPos, GUILayout.Width(450), GUILayout.Height(350));
        for (int i = 0; i < dependObjs.Count; i++) {
            GUILayout.BeginHorizontal();
            EditorGUILayout.ObjectField(dependObjs[i].obj, typeof(UnityEngine.Object), GUILayout.Width(300));
            if (dependObjs[i].isBundle == true) {
                GUILayout.Label("assetBundle", GUILayout.Width(100));
            }
            GUILayout.EndHorizontal();
        }
        GUILayout.EndScrollView();
    }

    private void Check() {
        if (typeIndex == 0) {
            CheckDepend();
        } else if (typeIndex == 1) {
            CheckBeDepended();
        }
    }

    /// <summary>
    /// 检查依赖
    /// </summary>
    private void CheckDepend() {
        if (selectObj == null) {
            ShowTips("请把需要检查的文件放入");
            return;
        }
        string objPath = AssetDatabase.GetAssetPath(selectObj);
        if (FileTools.IsFileExist(objPath) == false) {
            ShowTips("请把需要检查的文件放入，而不是文件夹");
            return;
        }
        dependObjs = new List<DependObject>();
        bundleCount = 0;
        bool isCheckAll = true;
        if (checkDependDeep != 0) {
            isCheckAll = false;
        }
        string[] paths = AssetDatabase.GetDependencies(objPath, isCheckAll);
        if (paths != null && paths.Length > 0) {
            Array.Sort(paths);
            for (int i = 0; i < paths.Length; i++) {
                UnityEngine.Object obj = AssetDatabase.LoadAssetAtPath(paths[i], typeof(UnityEngine.Object));
                if (obj != selectObj) {
                    AssetImporter ai = AssetImporter.GetAtPath(paths[i]);
                    DependObject dObj = new DependObject();
                    dObj.obj = obj;
                    if (ai != null && string.IsNullOrEmpty(ai.assetBundleName) == false) {
                        dObj.isBundle = true;
                        bundleCount++;
                    }
                    dependObjs.Add(dObj);
                }
            }
        }
        Debug.Log("共检查出：" + dependObjs.Count + "个文件");
        ShowTips("检查完成");
    }

    /// <summary>
    /// 检查被依赖
    /// </summary>
    private void CheckBeDepended() {
        if (selectObj == null) {
            ShowTips("2处：请把需要检查的文件放入");
            return;
        }
        string objPath = AssetDatabase.GetAssetPath(selectObj);
        if (FileTools.IsFileExist(objPath) == false) {
            ShowTips("2处：请把需要检查的文件放入，而不是文件夹");
            return;
        }
        if (checkFolder == null) {
            ShowTips("1处：请把需要检查的文件夹放入");
            return;
        }
        string folderPath = AssetDatabase.GetAssetPath(checkFolder);
        if (FileTools.IsDirectoryExist(folderPath) == false) {
            ShowTips("1处：请把需要检查的文件夹放入，而不是文件");
            return;
        }
        dependObjs = new List<DependObject>();
        string path = FileTools.GetFullPath(folderPath);
        Debug.Log("检查的文件夹:" + path);
        List<string> subPaths = FileTools.GetAllFilesExceptList(path);
        if (subPaths == null || subPaths.Count == 0) {
            ShowTips("该文件夹没有符合要求的文件！");
            return;
        }
        for (int i = 0; i < subPaths.Count; i++) {
            EditorUtility.DisplayProgressBar("正在收集项目依赖", i + "/" + subPaths.Count, (float)i / subPaths.Count);
            string tempStr = subPaths[i].Replace(Application.dataPath, "").Replace("\\", "/");
            tempStr = "Assets" + tempStr;
            string[] subDepends = AssetDatabase.GetDependencies(tempStr, true);
            if (subDepends != null && subDepends.Length > 0) {
                for (int j = 0; j < subDepends.Length; j++) {
                    if (subDepends[j] == objPath) {
                        DependObject dependObj = new DependObject();
                        UnityEngine.Object obj = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(tempStr);
                        dependObj.obj = obj;
                        dependObj.isBundle = false;
                        dependObjs.Add(dependObj);
                        break;
                    }
                }
            }
        }
        EditorUtility.ClearProgressBar();
        Debug.Log("共检查出：" + dependObjs.Count + "个文件");
        ShowTips("检查完成");
    }

    //-----------------------------查找资源-----------------------------
    private void ShowFindRes() {
        ShowLine("查找资源");
        GUILayout.BeginHorizontal();
        GUILayout.Label("请输入GUID：", GUILayout.Width(60));
        guidStr = EditorGUILayout.TextField(guidStr, GUILayout.Width(250));
        if (GUILayout.Button("查找", GUILayout.Width(80))) {
            FindRes();
        }
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        GUILayout.Label("资源：", GUILayout.Width(60));
        findObj = EditorGUILayout.ObjectField(findObj, typeof(UnityEngine.Object), GUILayout.Width(150));
        if (GUILayout.Button("查找", GUILayout.Width(80))) {
            FindGUID();
        }
        GUILayout.EndHorizontal();
    }

    private void FindGUID() {
        guidStr = "";
        if (findObj == null) {
            ShowTips("请选择需要查找的物体");
            return;
        }
        string path = AssetDatabase.GetAssetPath(findObj);
        if (string.IsNullOrEmpty(path)) {
            ShowTips("选择的物体找不到预设路径");
            return;
        }
        guidStr = AssetDatabase.AssetPathToGUID(path);
    }

    private void FindRes() {
        if (string.IsNullOrEmpty(guidStr)) {
            ShowTips("请输入guid");
            return;
        }
        string path = AssetDatabase.GUIDToAssetPath(guidStr);
        if (string.IsNullOrEmpty(path)) {
            ShowTips("找不到对应的资源");
            return;
        }
        UnityEngine.Object obj = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(path);
        if (obj == null) {
            ShowTips("找到对应的路径，查找资源出错：" + path);
            return;
        }
        findObj = obj;
        ShowTips("找到资源：" + path);
    }

    //-----------------------------资源使用情况-----------------------------
    private void ShowResUsedChcek() {
        ShowLine("资源使用情况");
        GUILayout.Label("请把需要检查的预设文件夹拖到框内，注意是prefab对应的文件夹");
        GUILayout.BeginHorizontal();
        prefabFolder = EditorGUILayout.ObjectField(prefabFolder, typeof(UnityEngine.Object), GUILayout.Width(200));
        GUILayout.EndHorizontal();
        GUILayout.Label("请把需要检查的图片文件夹拖到框内");
        GUILayout.BeginHorizontal();
        texFolder = EditorGUILayout.ObjectField(texFolder, typeof(UnityEngine.Object), GUILayout.Width(200));
        GUILayout.EndHorizontal();
        if (GUILayout.Button("检查", GUILayout.Width(100))) {
            CheckFun();
        }
        ShowResult1();
    }

    private void CheckFun() {
        if (prefabFolder == null) {
            ShowTips("请指定需要检查的图片文件夹");
            return;
        }
        if (texFolder == null) {
            ShowTips("请指定需要检查的图片文件夹");
            return;
        }
        checkTexSum = 0;
        prefabPath = AssetDatabase.GetAssetPath(prefabFolder);
        texPath = AssetDatabase.GetAssetPath(texFolder);
        GetPrefabDepends();
        CheckTexDepends();
    }

    private void GetPrefabDepends() {
        string projectPath = Application.dataPath.Replace("Assets", "");
        string realPath = projectPath + prefabPath;
        prefabDepends = new List<string>();
        string[] subPrefabPaths = Directory.GetFiles(realPath, "*.prefab", SearchOption.AllDirectories);
        if (subPrefabPaths == null || subPrefabPaths.Length == 0) {
            return;
        }
        for (int i = 0; i < subPrefabPaths.Length; i++) {
            EditorUtility.DisplayProgressBar("正在检查预设依赖", subPrefabPaths[i], (float)i / (float)subPrefabPaths.Length);
            CheckOnePrefabDepend(subPrefabPaths[i].Replace(projectPath, ""));
            EditorUtility.ClearProgressBar();
        }
    }

    private void CheckOnePrefabDepend(string path) {
        string[] depends = AssetDatabase.GetDependencies(path, true);
        if (depends == null || depends.Length == 0) {
            return;
        }
        for (int i = 0; i < depends.Length; i++) {
            if (prefabDepends.IndexOf(depends[i]) < 0) {
                prefabDepends.Add(depends[i]);
            }
        }
    }

    private void CheckTexDepends() {
        texNoDependPaths = new List<string>();
        texNoDependTexs = new List<Texture2D>();
        if (prefabDepends == null || prefabDepends.Count == 0) {
            Debug.Log("没有预设的依赖信息");
            return;
        }
        string projectPath = Application.dataPath.Replace("Assets", "");
        string realPath = projectPath + texPath;
        string[] subTexPaths = FileTools.GetAllImageFiles(realPath);
        if (subTexPaths == null || subTexPaths.Length == 0) {
            return;
        }
        checkTexSum = subTexPaths.Length;
        for (int i = 0; i < subTexPaths.Length; i++) {
            string assetPath = subTexPaths[i].Replace(projectPath, "").Replace("\\", "/");
            EditorUtility.DisplayProgressBar("正在检查图片依赖", assetPath, (float)i / (float)subTexPaths.Length);
            if (prefabDepends.IndexOf(assetPath) < 0) {
                if (texNoDependPaths.IndexOf(assetPath) < 0) {
                    texNoDependPaths.Add(assetPath);
                }
            }
            EditorUtility.ClearProgressBar();
        }
        if (texNoDependPaths.Count > 0) {
            for (int i = 0; i < texNoDependPaths.Count; i++) {
                Texture2D tex = (Texture2D)AssetDatabase.LoadAssetAtPath(texNoDependPaths[i], typeof(Texture2D));
                if (tex != null) {
                    texNoDependTexs.Add(tex);
                }
            }
        }
    }

    private void ShowResult1() {
        ShowLine("检查结果，以下为没有引用的图片");
        if (texNoDependTexs == null) {
            GUILayout.Label("请先查找");
            return;
        }
        if (texNoDependTexs.Count == 0) {
            GUILayout.Label("没有找到结果");
            return;
        }
        GUILayout.Label("文件夹内共有：" + checkTexSum + "张图片，其中没有被预设用到的有：" + texNoDependTexs.Count + "张");//"找到结果：" + texNoDependTexs.Count);
        texPos = GUILayout.BeginScrollView(texPos, GUILayout.Width(450), GUILayout.Height(350));
        for (int i = 0; i < texNoDependTexs.Count; i++) {
            GUILayout.BeginHorizontal();
            EditorGUILayout.ObjectField(texNoDependTexs[i], typeof(Texture2D), false, GUILayout.Width(200));
            if (GUILayout.Button("删除", GUILayout.Width(70), GUILayout.Height(20))) {
                string path = AssetDatabase.GetAssetPath(texNoDependTexs[i]);
                AssetDatabase.MoveAssetToTrash(path);
                texNoDependTexs.RemoveAt(i);
            }
            GUILayout.EndHorizontal();
        }
        GUILayout.EndScrollView();
    }

    //-----------------------------查找大图-----------------------------
    private void ShowFindLargePic() {
        ShowLine("查找大尺寸图片");
        GUILayout.Label("请把需要查找的文件夹拖到下面");
        findLargePicObj = EditorGUILayout.ObjectField(findLargePicObj, typeof(UnityEngine.Object), GUILayout.Width(150));
        GUILayout.Label("设置最小的宽高");
        GUILayout.BeginHorizontal();
        GUILayout.Label("宽：", GUILayout.Width(20));
        largeWidth = EditorGUILayout.FloatField(largeWidth, GUILayout.Width(50));
        GUILayout.Label("X", GUILayout.Width(20));
        GUILayout.Label("高：", GUILayout.Width(20));
        largeHeight = EditorGUILayout.FloatField(largeHeight, GUILayout.Width(50));
        GUILayout.EndHorizontal();
        if (GUILayout.Button("检查", GUILayout.Width(100), GUILayout.Height(50))) {
            CheckLargePicFun();
        }
        ShowFindLargePicResult();
    }

    private void CheckLargePicFun() {
        largePicList = new List<UnityEngine.Object>();
        if (findLargePicObj == null) {
            ShowTips("请先指定文件夹");
            return;
        }
        string path = AssetDatabase.GetAssetPath(findLargePicObj);
        if (FileTools.IsDirectoryExist(path) == false) {
            ShowTips("需要选择文件夹");
            return;
        }
        path = FileTools.GetFullPath(path);
        if (string.IsNullOrEmpty(path)) {
            ShowTips("选择的文件夹无效，请重新选择");
            return;
        }
        List<string> subPath = FileTools.GetAllFilesExceptMeta(path);
        if (subPath == null || subPath.Count == 0) {
            ShowTips("选择的文件夹内没有可以检查的文件");
            return;
        }
        for (int i = 0; i < subPath.Count; i++) {
            EditorUtility.DisplayProgressBar("正在检查图片", i + "/" + subPath.Count, (float)i / subPath.Count);
            string assetPath = FileTools.GetRelativePath(subPath[i]);
            Texture2D obj = (Texture2D)AssetDatabase.LoadAssetAtPath(assetPath, typeof(Texture2D));
            if (obj == null) {
                continue;
            }
            if (obj.width >= largeWidth || obj.height >= largeHeight) {
                largePicList.Add(obj);
            }
        }
        EditorUtility.ClearProgressBar();
    }

    private void ShowFindLargePicResult() {
        ShowLine("查找结果");
        if (largePicList != null && largePicList.Count > 0) {
            largePicPos = GUILayout.BeginScrollView(largePicPos, GUILayout.Width(450), GUILayout.Height(350));
            for (int i = 0; i < largePicList.Count; i++) {
                EditorGUILayout.ObjectField(largePicList[i], typeof(UnityEngine.Object), GUILayout.Width(150));
            }
            GUILayout.EndScrollView();
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

    public class DependObject
    {
        public bool isBundle = false;
        public UnityEngine.Object obj;
    }
}