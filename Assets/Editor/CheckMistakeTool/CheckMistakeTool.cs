using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine.UI;
using System.IO;

public class CheckMistakeTool : EditorWindow
{
    static CheckMistakeTool check;
    private int _titleSelectIndex = 0;
    private string[] _titleStrs = new string[] { "检查prefab缺失资源" };
    private Object selectedObj;
    private string path;
    private List<Object> lostList = new List<Object>();
    private List<string> lostInfoList = new List<string>();
    private Vector2 vec2 = new Vector2();

    [MenuItem("Tools/查找错误资源", false, 107)]
    static public void Check() {
        check = (CheckMistakeTool)EditorWindow.GetWindow(typeof(CheckMistakeTool));
        check.titleContent = new GUIContent("查找错误资源");
        check.maxSize = new Vector2(500, 650);
        check.minSize = new Vector2(500, 650);
        check.Show();
    }

    void OnGUI() {
        ShowSelectTitle();
        if (_titleSelectIndex == 0) {
            ShowPrefabMissing();
        }
    }

    private void ShowSelectTitle() {
        _titleSelectIndex = GUILayout.SelectionGrid(_titleSelectIndex, _titleStrs, 3, GUILayout.Width(450), GUILayout.Height(30));
    }

    private void ShowPrefabMissing() {
        GUILayout.Label("请把需要检查的文件夹拖到下面的框内");
        selectedObj = EditorGUILayout.ObjectField(selectedObj, typeof(Object), GUILayout.Width(200));
        if (GUILayout.Button("开始检查")) {
            lostList.Clear();
            lostInfoList.Clear();
            CheckBtnClick();
        }
        vec2 = GUILayout.BeginScrollView(vec2, GUILayout.Width(500), GUILayout.Height(450));
        for (int i = 0; i < lostList.Count; i++) {
            EditorGUILayout.ObjectField(lostList[i], typeof(Object), GUILayout.Width(200));
            GUILayout.Label(lostInfoList[i]);
        }
        GUILayout.EndScrollView();
    }

    void CheckBtnClick() {
        if (selectedObj == null) {
            return;
        }
        path = AssetDatabase.GetAssetPath(selectedObj);
        CheckMissingScripts();
    }

    void CheckMissingScripts() {
        List<string> listString = new List<string>();
        CollectFiles(path, ref listString);
        for (int i = 0; i < listString.Count; i++) {
            string Path = listString[i];
            float progressBar = (float)i / listString.Count;
            EditorUtility.DisplayProgressBar("Check Missing component", "The progress of ： " + ((int)(progressBar * 100)).ToString() + "%", progressBar);
            if (!Path.EndsWith(".prefab"))//只处理prefab文件
			{
                continue;
            }
            Path = ChangeFilePath(Path);
            Debug.Log("Path:::" + Path);
            AssetImporter tmpAssetImport = AssetImporter.GetAtPath(Path);

            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(tmpAssetImport.assetPath);
            if (prefab == null) {
                Debug.LogError("空的预设 ： " + tmpAssetImport.assetPath);
                continue;
            }
            Transform transform = prefab.GetComponent<Transform>();
            GameObject obj = transform.gameObject;
            Component[] components = obj.GetComponentsInChildren<Component>(true);
            //获取对象所有的Component组件
            //所有继承MonoBehaviour的脚本都继承Component
            for (int k = 0; k < components.Length; k++) {
                if (components[k] == null) {
                    Debug.LogError("这个预制中有空的脚本 ：" + tmpAssetImport.assetPath + " 挂在对象 : " + obj.name + " 上");
                    Object _obj = AssetDatabase.LoadAssetAtPath<Object>(tmpAssetImport.assetPath);
                    lostList.Add(_obj);
                    lostInfoList.Add("组件丢失：");
                } else {
                    //************************************************************************	particle
                    if (components[k].GetType() == typeof(ParticleSystem)) {
                        ParticleSystem p = (ParticleSystem)components[k];
                        if (p.shape.enabled == true && p.shape.shapeType == ParticleSystemShapeType.Mesh && p.shape.mesh == null) {
                            Debug.Log("粒子系统丢失mesh：" + p.name);
                            Object _obj = AssetDatabase.LoadAssetAtPath<Object>(tmpAssetImport.assetPath);
                            lostList.Add(_obj);
                            lostInfoList.Add("粒子系统丢失mesh：" + p.name);
                        }
                    }
                    //************************************************************************	ugui image
                    if (components[k].GetType() == typeof(Image)) {
                        Image img = components[k].GetComponent<Image>();
                        //if (img.overrideSprite == null) {
                        if (img.sprite == null) {
                            Object _obj = AssetDatabase.LoadAssetAtPath<Object>(tmpAssetImport.assetPath);
                            lostList.Add(_obj);
                            lostInfoList.Add("image纹理丢失：" + img.name);
                        }
                    }

                    //************************************************************************	ugui rawImage
                    if (components[k].GetType() == typeof(RawImage)) {
                        RawImage img = components[k].GetComponent<RawImage>();
                        if (img.texture == null) {
                            Object _obj = AssetDatabase.LoadAssetAtPath<Object>(tmpAssetImport.assetPath);
                            lostList.Add(_obj);
                            lostInfoList.Add("RAW纹理丢失：" + img.name);
                        }
                    }

                    //************************************************************************	text font
                    if (components[k].GetType() == typeof(Text)) {
                        Text text = components[k].GetComponent<Text>();
                        if (text.font == null) {
                            Object _obj = AssetDatabase.LoadAssetAtPath<Object>(tmpAssetImport.assetPath);
                            lostList.Add(_obj);
                            lostInfoList.Add("text字体丢失：" + text.name);
                        }
                    }

                    //************************************************************************	material
                    if (components[k] is Renderer) {
                        Material[] mats = ((Renderer)components[k]).sharedMaterials;
                        for (int x = 0; x < mats.Length; x++) {
                            if (mats[x] != null) {
                                int count = ShaderUtil.GetPropertyCount(mats[x].shader);
                                for (int a = 0; a < count; a++) {
                                    if (ShaderUtil.GetPropertyType(mats[x].shader, a) == ShaderUtil.ShaderPropertyType.TexEnv) {
                                        string propertyName = ShaderUtil.GetPropertyName(mats[x].shader, a);
                                        Texture tex = mats[x].GetTexture(propertyName);
                                        if (tex == null) {
                                            Object _obj = AssetDatabase.LoadAssetAtPath<Object>(tmpAssetImport.assetPath);
                                            lostList.Add(_obj);
                                            lostInfoList.Add("材质纹理丢失：" + mats[x].name + ":::" + mats[x].shader);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
        EditorUtility.ClearProgressBar();
    }

    //改变路径  
    //这种格式的路径 "C:/Users/XX/Desktop/aaa/New Unity Project/Assets\a.prefab" 改变成 "Assets/a.prefab"
    string ChangeFilePath(string path) {
        path = path.Replace("\\", "/");
        path = path.Replace(Application.dataPath + "/", "");
        return path;

    }

    //迭代获取文件路径;
    void CollectFiles(string dirPath, ref List<string> files) {
        string file = string.Empty;
        string[] filesPath = Directory.GetFiles(dirPath);
        foreach (string path in filesPath) {
            if (path.EndsWith(".meta"))
                continue;
            file = path.Substring(path.IndexOf("Assets/"));
            if (!string.IsNullOrEmpty(file) && !files.Contains(file))
                files.Add(file);
        }
        if (dirPath.IndexOf('.') != -1)
            return;
        foreach (string path in Directory.GetDirectories(dirPath))
            CollectFiles(path, ref files);
    }
}