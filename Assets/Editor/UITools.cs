using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using System.IO;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;
using UnityEngine.UI;
using UnityEditor;

public class UITools
{
    /// <summary>
    /// 设置gameObject的layer
    /// </summary>
    /// <param name="t"></param>
    /// <param name="layer"></param>
    public static void SetGameObjectLayer(Transform t, int layer) {
        int count = t.childCount;
        t.gameObject.layer = layer;
        for (int i = 0; i < count; i++) {
            SetGameObjectLayer(t.GetChild(i), layer);
        }
    }

    /// <summary>
    /// 递归获取所有指定组件
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="root"></param>
    /// <param name="list"></param>
    /// <param name="predicate"></param>
    public static void GetComponentsExclude<T>(Transform root, List<T> list, System.Predicate<GameObject> predicate) {
        int count = root.childCount;
        for (int i = 0; i < count; i++) {
            Transform child = root.GetChild(i);
            T t = child.GetComponent<T>();
            if (predicate(child.gameObject)) {
                if (t != null) {
                    list.Add(t);
                }
                if (child.childCount > 0) {
                    GetComponentsExclude<T>(child, list, predicate);
                }
            }
        }
    }

    /// <summary>
    /// 根据名字获取图集的引用相对路径
    /// </summary>
    /// <param name="path">相对路径，Assets/UINew/scene</param>
    /// <returns></returns>
    public static Dictionary<string, List<string>> GetAtlasRefDetailByScenes(string relativePath) {
        if (relativePath.StartsWith(Constants.SCENE_PATH) == false) {
            return null;
        }
        Dictionary<string, List<string>> totalList = new Dictionary<string, List<string>>();
        //string fullPath = Application.dataPath + relativePath.Replace("Assets/UINew/scene", "/UINew/scene");
        string fullPath = FileTools.GetFullPath(relativePath);
        string[] allPathList;
        if (File.Exists(fullPath)) {
            allPathList = new string[] { fullPath };
        } else {
            allPathList = Directory.GetFiles(fullPath, "*.unity", SearchOption.AllDirectories);
        }
        foreach (string path in allPathList) {
            //string temp = path.Replace(Application.dataPath, "Assets");
            string temp = FileTools.GetRelativePath(path);
            if (relativePath != temp) {
                Scene scene = EditorSceneManager.OpenScene(temp);
            }
            GameObject obj = GameObject.Find("Canvas/Prefab");
            if (obj != null) {
                List<Image> imageList = new List<Image>();
                UITools.GetComponentsExclude<Image>(obj.transform, imageList, go => go.name.Contains(Constants.LUA_SUFFIX) == false);
                List<string> list = new List<string>();
                foreach (Image img in imageList) {
                    string imagePath = AssetDatabase.GetAssetPath(img.sprite);
                    if (string.IsNullOrEmpty(imagePath)) {
                        continue;
                    }
                    string atlasPath = FileTools.GetAtlasDir(imagePath);
                    if (atlasPath == null) {
                        continue;
                    }
                    if (list.Contains(atlasPath)) {
                        continue;
                    }
                    list.Add(atlasPath);
                }
                string filename = Path.GetFileNameWithoutExtension(path);
                totalList.Add(filename, list);
            }
        }
        return totalList;
    }
}