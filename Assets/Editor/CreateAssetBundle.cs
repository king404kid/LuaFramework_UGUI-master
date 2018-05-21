using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class CreateAssetBundle
{
    [MenuItem("Tools/更新AssetBundle名", false, 201)]
    /// <summary>
    /// 更新UGUI的资源assetbundle名
    /// </summary>
    public static void UpdateUGUIAsset() {
        string path = "";
        // texture
        path = "Assets/UINew/texture/";
        string texture_path = Application.dataPath + "/UINew/texture/";
        List<string> AllTexturePath = FileTools.GetSubFolders(texture_path);
        foreach (string folderPath in AllTexturePath) {
            string textureName = folderPath.Replace(texture_path, "texture/");
            string folder_path = path + folderPath.Replace(texture_path, "");
            UpdateAssetBundleName(folder_path, "*.jpg", textureName + "/{0}.unity3d");
            UpdateAssetBundleName(folder_path, "*.png", textureName + "/{0}.unity3d");
        }
        // atlas，比较特殊是目录名做ab名
        path = "Assets/UINew/atlas/";
        string atlas_path = Application.dataPath + "/UINew/atlas/";
        List<string> AllAtlasPath = FileTools.GetSubFolders(atlas_path);
        foreach (string folderPath in AllAtlasPath) {
            string atlasName = folderPath.Replace(atlas_path, "atlas/");
            string folder_path = path + folderPath.Replace(atlas_path, "");
            UpdateAssetBundleName(folder_path, "*.jpg", atlasName + ".unity3d");
            UpdateAssetBundleName(folder_path, "*.png", atlasName + ".unity3d");
        }
        // prefab
        path = "Assets/UINew/prefab/";
        string prefab_path = Application.dataPath + "/UINew/prefab/";
        List<string> AllPrefabPath = FileTools.GetSubFolders(prefab_path);
        foreach (string folderPath in AllPrefabPath) {
            string prefabName = folderPath.Replace(prefab_path, "prefab/");
            string folder_path = path + folderPath.Replace(prefab_path, "");
            UpdateAssetBundleName(folder_path, "*.prefab", prefabName + "/{0}.unity3d");
        }
        EditorUtility.ClearProgressBar();
        ShowTips("更新完成");
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    [MenuItem("Tools/清空AssetBundle名", false, 202)]
    /// <summary>
    /// 清空UGUI的资源assetbundle名
    /// </summary>
    public static void ClearUGUIAsset() {
        string path = "";
        // texture
        path = "Assets/UINew/texture/";
        string texture_path = Application.dataPath + "/UINew/texture/";
        List<string> AllTexturePath = FileTools.GetSubFolders(texture_path);
        foreach (string folderPath in AllTexturePath) {
            string textureName = folderPath.Replace(texture_path, "texture/");
            string folder_path = path + folderPath.Replace(texture_path, "");
            UpdateAssetBundleName(folder_path, "*.jpg", textureName + "/{0}.unity3d", true);
            UpdateAssetBundleName(folder_path, "*.png", textureName + "/{0}.unity3d", true);
        }
        // atlas，比较特殊是目录名做ab名
        path = "Assets/UINew/atlas/";
        string atlas_path = Application.dataPath + "/UINew/atlas/";
        List<string> AllAtlasPath = FileTools.GetSubFolders(atlas_path);
        foreach (string folderPath in AllAtlasPath) {
            string atlasName = folderPath.Replace(atlas_path, "atlas/");
            string folder_path = path + folderPath.Replace(atlas_path, "");
            UpdateAssetBundleName(folder_path, "*.jpg", atlasName + ".unity3d", true);
            UpdateAssetBundleName(folder_path, "*.png", atlasName + ".unity3d", true);
        }
        // prefab
        path = "Assets/UINew/prefab/";
        string prefab_path = Application.dataPath + "/UINew/prefab/";
        List<string> AllPrefabPath = FileTools.GetSubFolders(prefab_path);
        foreach (string folderPath in AllPrefabPath) {
            string prefabName = folderPath.Replace(prefab_path, "prefab/");
            string folder_path = path + folderPath.Replace(prefab_path, "");
            UpdateAssetBundleName(folder_path, "*.prefab", prefabName + "/{0}.unity3d", true);
        }
        EditorUtility.ClearProgressBar();
        ShowTips("清空完成");
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    /// <summary>
    /// 更新指定目录下的指定格式的AssetBundle名字
    /// </summary>
    /// <param name="path"></param>
    /// <param name="exformat"></param>
    /// <param name="bundlenameFormat"></param>
    /// <param name="clearABName"></param>
    public static void UpdateAssetBundleName(string path, string exformat, string bundlenameFormat, bool clearABName = false) {
        if (path.StartsWith("Assets/")) {
            path = path.Remove(0, "Assets".Length);
        }
        string dirpath = Application.dataPath + path;
        if (!string.IsNullOrEmpty(exformat)) {
            string ex = exformat.Remove(0, 1);
            string[] files = Directory.GetFiles(dirpath, exformat, SearchOption.AllDirectories);
            int index = 1;
            foreach (string fn in files) {
                string assetname = fn.Substring(dirpath.Length);
                string filename = assetname;
                filename = Path.GetFileNameWithoutExtension(filename);
                string bundlename = string.Format(bundlenameFormat, filename);
                assetname = "Assets" + path + assetname;
                SetAssetName(assetname, bundlename, clearABName);
                if (clearABName) {
                    EditorUtility.DisplayProgressBar("正在清空ab名", "旧的ab名称：" + bundlename + "(" + index + "/" + files.Length + ")", (float)index / files.Length);
                } else {
                    EditorUtility.DisplayProgressBar("正在设置ab名", "新的ab名称：" + bundlename + "(" + index + "/" + files.Length + ")", (float)index / files.Length);
                }
                index++;
            }
        } else {
            List<string> subObjPaths = FileTools.GetAllFilesExceptMeta(dirpath);
            for (int i = 0; i < subObjPaths.Count; i++) {
                string fn = subObjPaths[i];
                string assetname = fn.Substring(dirpath.Length);
                string filename = assetname;
                filename = Path.GetFileNameWithoutExtension(filename);
                string bundlename = string.Format(bundlenameFormat, filename);
                assetname = "Assets" + path + assetname;
                SetAssetName(assetname, bundlename, clearABName);
            }
        }
    }

    /// <summary>
    /// 设置assetbundle name
    /// </summary>
    /// <param name="assetname"></param>
    /// <param name="bundlename"></param>
    /// <param name="clearABName"></param>
    public static void SetAssetName(string assetname, string bundlename, bool clearABName) {
        AssetImporter ai = AssetImporter.GetAtPath(assetname);
        if (ai != null) {
            bool isDirty = false;
            if (clearABName) {
                ai.assetBundleName = "";
                ai.SaveAndReimport();
                return;
            }
            if (ai is TextureImporter) {
                string packing_name = FileTools.RemoveExName(bundlename);
                TextureImporter ti = (TextureImporter)ai;
                if (ti.textureType != TextureImporterType.Sprite) {
                    ti.textureType = TextureImporterType.Sprite;
                    isDirty = true;
                }
                if (ti.spriteImportMode != SpriteImportMode.Single) {
                    ti.spriteImportMode = SpriteImportMode.Single;
                    isDirty = true;
                }
                if (ti.spritePackingTag != packing_name) {
                    ti.spritePackingTag = packing_name;
                    isDirty = true;
                }
                if (ti.mipmapEnabled == true) {
                    ti.mipmapEnabled = false;
                    isDirty = true;
                }
            }
            bundlename = bundlename.ToLower();
            if (ai.assetBundleName != bundlename) {
                ai.assetBundleName = bundlename;
                isDirty = true;
            }
            if (isDirty) {
                ai.SaveAndReimport();
            }
        }
    }

    /// <summary>
    /// 生成AssetBundle
    /// </summary>
    [MenuItem("Tools/生成AssetBundles", false, 203)]
    public static void BuildAssetBundles() {
        // Choose the output path according to the build target.
        string outputPath = Path.Combine(Path.Combine("AssetBundles", GetPlatformFolderForAssetBundles(EditorUserBuildSettings.activeBuildTarget)), "ui");
        if (!Directory.Exists(outputPath)) {
            Directory.CreateDirectory(outputPath);
        }
        BuildPipeline.BuildAssetBundles(outputPath, BuildAssetBundleOptions.ChunkBasedCompression | BuildAssetBundleOptions.DeterministicAssetBundle | BuildAssetBundleOptions.IgnoreTypeTreeChanges, EditorUserBuildSettings.activeBuildTarget);
    }

    /// <summary>
    /// 根据平台区分路径
    /// </summary>
    /// <param name="target"></param>
    /// <returns></returns>
    public static string GetPlatformFolderForAssetBundles(BuildTarget target) {
        switch (target) {
            case BuildTarget.Android:
                return "Android";
            case BuildTarget.iOS:
                return "iOS";
            case BuildTarget.StandaloneWindows:
            case BuildTarget.StandaloneWindows64:
                return "Windows";
            case BuildTarget.StandaloneOSXIntel:
            case BuildTarget.StandaloneOSXIntel64:
            case BuildTarget.StandaloneOSXUniversal:
                return "OSX";
            // Add more build targets for your own.
            // If you add more targets, don't forget to add the same platforms to GetPlatformFolderForAssetBundles(RuntimePlatform) function.
            default:
                return null;
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