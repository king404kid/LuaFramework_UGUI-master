using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using System.IO;

public class FileTools
{
    /// <summary>
    /// 获取atlas的目录相对路径
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public static string GetAtlasDir(string path) {
        if (!path.StartsWith(Constants.ATLAS_PATH)) return null;
        string d = path.Substring(0, path.LastIndexOf("/"));
        return d;
    }

    /// <summary>
    /// 由于带Editor的目录不能用Where，所以写到这里来
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public static string[] GetAllImageFiles(string path) {
        return Directory.GetFiles(path, "*.*", SearchOption.AllDirectories).Where(s => s.EndsWith(".png") || s.EndsWith(".jpg")).ToArray<string>();
    }

    /// <summary>
    /// 获取需要剔除的文件列表
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public static List<string> GetAllFilesExceptList(string path) {
        return Directory.GetFiles(path, "*.*", SearchOption.AllDirectories)
            .Where(s => !s.EndsWith(".asset") && !s.EndsWith(".meta") && !s.EndsWith(".txt")
             && !s.EndsWith(".png") && !s.EndsWith(".jpg") && !s.EndsWith(".tga") && !s.EndsWith(".psd")
             && !s.EndsWith(".dds") && !s.EndsWith(".fbx") && !s.EndsWith(".mp3") && !s.EndsWith(".wav")
             && !s.EndsWith(".wma") && !s.EndsWith(".m4a") && !s.EndsWith(".cs")).ToList<string>();
    }

    /// <summary>
    /// 获取除meta的文件列表
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public static List<string> GetAllFilesExceptMeta(string path) {
        return Directory.GetFiles(path, "*.*", SearchOption.AllDirectories)
            .Where(s => !s.EndsWith(".asset")).ToList<string>();
    }

    /// <summary>
    /// 获取相对路径
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public static string GetRelativePath(string path) {
        path = path.Replace(Application.dataPath, "Assets").Replace("\\", "/");
        return path;
    }

    /// <summary>
    /// 获取绝对路径
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public static string GetFullPath(string path) {
        string appPath = Application.dataPath;
        int lastIndex = appPath.LastIndexOf("/");
        if (lastIndex >= 0) {
            appPath = appPath.Substring(0, lastIndex);
        }
        path = appPath + "/" + path;
        return path;
    }

    /// <summary>
    /// 该目录是否存在
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public static bool IsDirectoryExist(string path) {
        if (Directory.Exists(path)) {
            return true;
        }
        return false;
    }

    /// <summary>
    /// 该文件是否存在
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public static bool IsFileExist(string path) {
        if (File.Exists(path)) {
            return true;
        }
        return false;
    }

    /// <summary>
    /// 读取文件的字符串
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public static string ReadFileText(string path, bool isUTF8 = false) {
        if (!File.Exists(path)) {
            return "";
        }
        string str = "";
        if (isUTF8) {
            str = File.ReadAllText(path, Encoding.UTF8);
        } else {
            str = File.ReadAllText(path, Encoding.Default);
        }
        return str;
    }

    /// <summary>
    /// 保存文件
    /// </summary>
    /// <param name="path"></param>
    /// <param name="content"></param>
    public static void SaveFile(string path, string content, bool needUtf8 = false) {
        CheckFileSavePath(path);
        if (needUtf8) {
            UTF8Encoding code = new UTF8Encoding(false);
            File.WriteAllText(path, content, code);
        } else {
            File.WriteAllText(path, content, Encoding.Default);
        }
    }

    /// <summary>
    /// 检查该路径的上层目录，如果不存在则创建
    /// </summary>
    /// <param name="path"></param>
    static public void CheckFileSavePath(string path) {
        string realPath = path;
        int ind = path.LastIndexOf("/");
        if (ind >= 0) {
            realPath = path.Substring(0, ind);
        } else {
            ind = path.LastIndexOf("\\");
            if (ind >= 0) {
                realPath = path.Substring(0, ind);
            }
        }
        CreateDirectoryIfNotExist(realPath);
    }

    /// <summary>
    /// 写入所有行
    /// </summary>
    /// <param name="path"></param>
    /// <param name="lines"></param>
    static public void SaveAllLines(string path, string[] lines) {
        CheckFileSavePath(path);
        File.WriteAllLines(path, lines);
    }

    /// <summary>
    /// 获取所有行
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    static public string[] ReadAllLines(string path) {
        if (!File.Exists(path)) {
            return null;
        }
        return File.ReadAllLines(path);
    }

    /// <summary>
    /// 删除文件
    /// </summary>
    /// <param name="path"></param>
    public static void DelFile(string path) {
        if (!IsFileExist(path)) {
            return;
        }
        File.Delete(path);
    }

    /// <summary>
    /// 根据绝对路径获取文件名
    /// </summary>
    /// <param name="path"></param>
    /// <param name="needEx"></param>
    /// <returns></returns>
    public static string GetFilename(string path, bool needEx) {
        if (needEx) {
            return Path.GetFileName(path);
        }
        return Path.GetFileNameWithoutExtension(path);
    }

    /// <summary>
    /// 根据相对路径获取文件名
    /// </summary>
    /// <param name="path"></param>
    /// <param name="needEx"></param>
    /// <returns></returns>
    public static string GetFilenameWithRelativePath(string path, bool needEx) {
        if (path.StartsWith("Assets") == false) {
            return "";
        }
        int index = path.Replace("\\", "/").LastIndexOf("/");
        if (index < 0) {
            return "";
        }
        string filename = path.Substring(index + 1);
        if (needEx) {
            return filename;
        }
        index = filename.IndexOf(".");
        if (index < 0) {
            return filename;
        }
        filename = filename.Substring(0, index);
        return filename;
    }

    /// <summary>
    /// 获取目录名
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public static string GetDirectoryName(string path) {
        return Path.GetDirectoryName(path);
    }

    /// <summary>
    /// 创建目录
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public static void CreateDirectoryIfNotExist(string path) {
        if (!IsDirectoryExist(path)) {
            Directory.CreateDirectory(path);
        }
    }

    /// <summary>
    /// 获得文件名
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    static public string GetFileName(string str) {
        string rexStr = @"(?<=\\)[^\\]+$|(?<=/)[^/]+$";
        return StringTools.GetFirstMatch(str, rexStr);
    }

    /// <summary>
    /// 获得扩展名
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    static public string GetExName(string str) {
        string rexStr = @"(?<=\\[^\\]+.)[^\\.]+$|(?<=/[^/]+.)[^/.]+$";
        return StringTools.GetFirstMatch(str, rexStr);

    }

    /// <summary>
    /// 去除扩展名
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    static public string RemoveExName(string str) {
        string returnStr = str;
        string rexStr = @"[^\.]+(?=\.)";
        string xStr = StringTools.GetFirstMatch(str, rexStr);
        if (!string.IsNullOrEmpty(xStr)) {
            returnStr = xStr;
        }
        return returnStr;
    }
}
