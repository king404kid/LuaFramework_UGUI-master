using System.Collections;
using System.Collections.Generic;
using Tools;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class AtlasCheckTool : EditorWindow
{
    private static AtlasCheckTool _instance;
    private Vector2 _texPos = new Vector2();
    private int _totalAtlasNum;
    private int _totalImgNum;
    private Dictionary<string, AtlasCheckObject> _atlasDic;
    private GameObject _checkObject;

    public static AtlasCheckTool Instance {
        get {
            if (_instance == null) {
                _instance = (AtlasCheckTool)EditorWindow.GetWindow(typeof(AtlasCheckTool));
                _instance.titleContent = new GUIContent("图集检查");
                _instance.maxSize = new Vector2(500, 500);
                _instance.minSize = new Vector2(500, 500);
            }
            return AtlasCheckTool._instance;
        }
    }

    [MenuItem("Tools/查找运行时节点图集", false, 108)]
    static void ShowWin() {
        AtlasCheckTool.Instance.Show();
    }

    void OnGUI() {
        if (Application.isPlaying == false) {
            GUILayout.Label("只检查运行时节点");
        } else {
            ShowCtrl();
            ShowResult();
        }
    }

    private int checkTexSum = 0;
    private void ShowCtrl() {
        ShowLine("操作");
        GUILayout.Label("请拖放要检查的节点");
        GUILayout.BeginHorizontal();
        _checkObject = (GameObject)EditorGUILayout.ObjectField(_checkObject, typeof(GameObject), true, GUILayout.Width(200));
        if (GUILayout.Button("检查", GUILayout.Width(100))) {
            CheckFun();
            return;
        }
        GUILayout.EndHorizontal();
    }

    private void CheckFun() {
        if (_checkObject == null) {
            ShowTips("请拖放要检查的节点");
            return;
        }
        Image[] imageList = _checkObject.GetComponentsInChildren<Image>(true);
        if (imageList.Length == 0) {
            ShowTips("该节点下不包含image组件");
            return;
        }
        _totalAtlasNum = 0;
        _totalImgNum = 0;
        _atlasDic = new Dictionary<string, AtlasCheckObject>();
        for (int i = 0; i < imageList.Length; i++) {
            Image img = imageList[i];
            if (img != null && img.sprite != null) {
                string path = AssetDatabase.GetAssetPath(img.sprite);
                if (string.IsNullOrEmpty(path) == false && path.IndexOf("Assets/UINew/atlas") > -1) {
                    string atlasPath = path.Substring(0, path.LastIndexOf("/"));
                    AtlasCheckObject obj = null;
                    if (_atlasDic.TryGetValue(atlasPath, out obj) == false) {
                        AtlasCheckObject newObj = new AtlasCheckObject();
                        newObj.atlasName = atlasPath;
                        _atlasDic.Add(atlasPath, newObj);
                        _totalAtlasNum++;
                        obj = newObj;
                    }
                    obj.imgObjectList.Add(AssetDatabase.LoadAssetAtPath<Object>(path));
                    obj.gameObjectList.Add(img.gameObject);
                    _totalImgNum++;
                }
            }
        }
        Debug.Log("");
    }

    private void ShowResult() {
        ShowLine("检查结果");
        if (_atlasDic == null) {
            return;
        }
        GUILayout.BeginHorizontal();
        GUILayout.Label("文件夹内共引用了：" + _totalAtlasNum + "个图集,含有" + _totalImgNum + "图片");
        if (GUILayout.Button("复制到节点图集", GUILayout.Width(100))) {
            CopyAtlas();
        }
        GUILayout.EndHorizontal();
        _texPos = GUILayout.BeginScrollView(_texPos, GUILayout.Width(500), GUILayout.Height(350));
        foreach (KeyValuePair<string, AtlasCheckObject> item in _atlasDic) {
            GUILayout.Label("-------------------" + item.Value.atlasName + "-------共" + item.Value.imgObjectList.Count + "个-------------------");
            for (int i = 0; i < item.Value.imgObjectList.Count; i++) {
                Object img = item.Value.imgObjectList[i];
                Object go = item.Value.gameObjectList[i];
                GUILayout.BeginHorizontal();
                EditorGUILayout.ObjectField(go, typeof(GameObject), false, GUILayout.Width(200));
                EditorGUILayout.ObjectField(img, typeof(Texture2D), false, GUILayout.Width(200));
                GUILayout.EndHorizontal();
            }
        }
        GUILayout.EndScrollView();
    }

    private void CopyAtlas() {
        string atlasName = _checkObject.name;
        string newPath = "Assets/UINew/atlas/" + atlasName + "/";
        string newFullPath = Application.dataPath + "/UINew/atlas/" + atlasName + "/";
        FileManager.DelFolder(newPath);
        FileManager.CheckDirection(newPath);
        int index = 0;
        foreach (KeyValuePair<string, AtlasCheckObject> item in _atlasDic) {
            for (int i = 0; i < item.Value.imgObjectList.Count; i++) {
                Object img = item.Value.imgObjectList[i];
                string oldPath = AssetDatabase.GetAssetPath(img);
                string imgName = FilePathHelper.GetFileName(oldPath);
                EditorUtility.DisplayProgressBar("复制中请稍后", "复制" + imgName + "到" + newPath, (float)index / _totalImgNum);
                index++;
                if (FileManager.IsFileExists(newFullPath + imgName) == false) {
                    AssetDatabase.CopyAsset(oldPath, newPath + imgName);
                    AssetDatabase.Refresh();
                }
                GameObject go = item.Value.gameObjectList[i] as GameObject;
                Image sprite = go.GetComponent<Image>();
                if (sprite != null) {
                    Sprite newSp = AssetDatabase.LoadAssetAtPath<Sprite>(newPath + imgName);
                    sprite.sprite = newSp;
                }
            }
        }
        EditorUtility.ClearProgressBar();
        ShowTips("复制成功：" + index + "个");
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

public class AtlasCheckObject {
    public string atlasName;
    public List<Object> gameObjectList = new List<Object>();
    public List<Object> imgObjectList = new List<Object>();
}