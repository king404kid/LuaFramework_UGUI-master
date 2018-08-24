using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Tools
{
    public class FileManager
    {
        private static FileManager _instance;
        public const string SAVE_PATH = "savePath.txt";

        public FileManager() {
            //System.Text.Encoding.GetEncoding("gb2312");
        }

        static public long GetFileSize(string path) {
            if (IsFileExists(path) == false) {
                return 0;
            }
            FileInfo info = new FileInfo(path);
            return info.Length;
        }

        /// <summary>
        /// 保存文件
        /// </summary>
        /// <param name="path"></param>
        /// <param name="content"></param>
        static public void SaveFile(string path, string content, bool needUtf8 = false) {
            CheckFileSavePath(path);
            if (needUtf8) {
                UTF8Encoding code = new UTF8Encoding(false);
                File.WriteAllText(path, content, code);
            } else {
                File.WriteAllText(path, content, Encoding.Default);
            }

        }

        static public void SaveFileByCode(string path, string content, Encoding code) {
            CheckFileSavePath(path);
            File.WriteAllText(path, content, code);
        }

        static public void SaveLine(string path, string content) {
            CheckFileSavePath(path);
            StreamWriter f = new StreamWriter(path, true);
            f.WriteLine(content);
            f.Close();
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
        /// 保存bytes
        /// </summary>
        /// <param name="path"></param>
        /// <param name="bytes"></param>
        static public void SaveBytes(string path, byte[] bytes) {
            CheckFileSavePath(path);
            File.WriteAllBytes(path, bytes);
        }

        static public void DelFolder(string path) {
            if (!IsDirectoryExists(path)) {
                return;
            }
            Directory.Delete(path, true);
        }

        static public void DelDir(string target_dir) {
            string[] files = Directory.GetFiles(target_dir);
            string[] dirs = Directory.GetDirectories(target_dir);

            foreach (string file in files) {
                File.SetAttributes(file, FileAttributes.Normal);
                File.Delete(file);
            }

            foreach (string dir in dirs) {
                DelDir(dir);
            }

            Directory.Delete(target_dir, false);
        }

        static public void DelFile(string path) {
            if (!IsFileExists(path)) {
                return;
            }
            File.Delete(path);
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
        /// 读取文件的字符串
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        static public string ReadFileText(string path, bool isUTF8 = false) {
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
        /// 读取bytes
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        static public byte[] ReadFileBytes(string path) {
            if (!File.Exists(path)) {
                return null;
            }
            byte[] str = File.ReadAllBytes(path);
            return str;
        }

        /// <summary>
        /// 检查某文件夹路径是否存在，如不存在，创建
        /// </summary>
        /// <param name="path"></param>
        static public void CheckDirection(string path) {
            if (!Directory.Exists(path)) {
                Directory.CreateDirectory(path);
            }
        }

        /// <summary>
        /// 单纯检查某个文件夹路径是否存在
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        static public bool IsDirectoryExists(string path) {
            if (Directory.Exists(path)) {
                return true;
            } else {
                return false;
            }
        }

        /// <summary>
        /// 检查某个文件是否存在
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        static public bool IsFileExists(string path) {
            if (File.Exists(path)) {
                return true;
            } else {
                return false;
            }
        }

        /// <summary>
        /// 获取某目录下所有指定类型的文件的路径
        /// </summary>
        /// <param name="path"></param>
        /// <param name="exName"></param>
        /// <returns></returns>
        static public List<string> GetAllFiles(string path, string exName) {
            if (!IsDirectoryExists(path)) {
                return null;
            }
            bool needCheckExName = true;
            exName = exName.ToLower();
            if (string.IsNullOrEmpty(exName)) {
                needCheckExName = false;
            }
            List<string> names = new List<string>();
            DirectoryInfo root = new DirectoryInfo(path);
            FileInfo[] files = root.GetFiles();
            string ex;
            for (int i = 0; i < files.Length; i++) {
                if (needCheckExName == true) {
                    ex = FilePathHelper.GetExName(files[i].FullName);
                    ex = ex.ToLower();
                    if (ex != exName) {
                        continue;
                    }
                }
                names.Add(StringTools.ChangePathFormat(files[i].FullName));
            }
            DirectoryInfo[] dirs = root.GetDirectories();
            if (dirs.Length > 0) {
                for (int i = 0; i < dirs.Length; i++) {
                    List<string> subNames = GetAllFiles(dirs[i].FullName, exName);
                    if (subNames.Count > 0) {
                        for (int j = 0; j < subNames.Count; j++) {
                            names.Add(StringTools.ChangePathFormat(subNames[j]));
                        }
                    }
                }
            }

            return names;

        }


        /// <summary>
        /// 获取某目录下所有除了指定类型外的文件的路径
        /// </summary>
        /// <param name="path"></param>
        /// <param name="exName"></param>
        /// <returns></returns>
        static public List<string> GetAllFilesExcept(string path, string exName) {
            List<string> names = new List<string>();
            DirectoryInfo root = new DirectoryInfo(path);
            FileInfo[] files = root.GetFiles();
            string ex;
            for (int i = 0; i < files.Length; i++) {
                ex = FilePathHelper.GetExName(files[i].FullName);
                if (ex == exName) {
                    continue;
                }
                names.Add(StringTools.ChangePathFormat(files[i].FullName));
            }
            DirectoryInfo[] dirs = root.GetDirectories();
            if (dirs.Length > 0) {
                for (int i = 0; i < dirs.Length; i++) {
                    List<string> subNames = GetAllFilesExcept(dirs[i].FullName, exName);
                    if (subNames.Count > 0) {
                        for (int j = 0; j < subNames.Count; j++) {
                            names.Add(StringTools.ChangePathFormat(subNames[j]));
                        }
                    }
                }
            }

            return names;

        }

        /// <summary>
        /// 获取某目录下所有除了指定类型外的文件的路径
        /// </summary>
        /// <param name="path"></param>
        /// <param name="exName"></param>
        /// <returns></returns>
        static public List<string> GetAllFilesIncludeList(string path, List<string> exName) {
            List<string> names = new List<string>();
            DirectoryInfo root = new DirectoryInfo(path);
            FileInfo[] files = root.GetFiles();
            for (int i = 0; i < exName.Count; i++) {
                exName[i] = exName[i].ToLower();
            }
            string ex;
            for (int i = 0; i < files.Length; i++) {
                ex = FilePathHelper.GetExName(files[i].FullName);
                ex = ex.ToLower();
                if (exName.IndexOf(ex) < 0) {
                    continue;
                }
                names.Add(StringTools.ChangePathFormat(files[i].FullName));
            }
            DirectoryInfo[] dirs = root.GetDirectories();
            if (dirs.Length > 0) {
                for (int i = 0; i < dirs.Length; i++) {
                    List<string> subNames = GetAllFilesIncludeList(dirs[i].FullName, exName);
                    if (subNames.Count > 0) {
                        for (int j = 0; j < subNames.Count; j++) {
                            names.Add(StringTools.ChangePathFormat(subNames[j]));
                        }
                    }
                }
            }

            return names;

        }

        /// <summary>
        /// 获取某目录下所有除了指定类型外的文件的路径
        /// </summary>
        /// <param name="path"></param>
        /// <param name="exName"></param>
        /// <returns></returns>
        static public List<string> GetAllFilesExceptList(string path, List<string> exName) {
            List<string> names = new List<string>();
            DirectoryInfo root = new DirectoryInfo(path);
            FileInfo[] files = root.GetFiles();
            for (int i = 0; i < exName.Count; i++) {
                exName[i] = exName[i].ToLower();
            }
            string ex;
            for (int i = 0; i < files.Length; i++) {
                ex = FilePathHelper.GetExName(files[i].FullName);
                ex = ex.ToLower();
                if (exName.IndexOf(ex) >= 0) {
                    continue;
                }
                names.Add(StringTools.ChangePathFormat(files[i].FullName));
            }
            DirectoryInfo[] dirs = root.GetDirectories();
            if (dirs.Length > 0) {
                for (int i = 0; i < dirs.Length; i++) {
                    List<string> subNames = GetAllFilesExceptList(dirs[i].FullName, exName);
                    if (subNames.Count > 0) {
                        for (int j = 0; j < subNames.Count; j++) {
                            names.Add(StringTools.ChangePathFormat(subNames[j]));
                        }
                    }
                }
            }

            return names;

        }

        /// <summary>
        /// 获取指定路径下第一层的子文件夹路径
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        static public List<string> GetSubFolders(string path) {
            if (!IsDirectoryExists(path)) {
                return null;
            }
            DirectoryInfo root = new DirectoryInfo(path);

            DirectoryInfo[] dirs = root.GetDirectories();
            List<string> folders = new List<string>();
            if (dirs.Length > 0) {
                for (int i = 0; i < dirs.Length; i++) {
                    folders.Add(StringTools.ChangePathFormat(dirs[i].FullName));
                }
            }

            return folders;

        }

        /// <summary>
        /// 获取指定路径下一层的指定格式的文件的路径
        /// </summary>
        /// <param name="path"></param>
        /// <param name="exName"></param>
        /// <returns></returns>
        static public List<string> GetSubFiles(string path, string exName) {
            List<string> names = new List<string>();
            if (IsDirectoryExists(path) == false) {
                return names;
            }

            DirectoryInfo root = new DirectoryInfo(path);
            FileInfo[] files = root.GetFiles();
            string ex;
            for (int i = 0; i < files.Length; i++) {
                ex = FilePathHelper.GetExName(files[i].FullName);
                if (ex != exName) {
                    continue;
                }
                names.Add(StringTools.ChangePathFormat(files[i].FullName));
            }
            return names;
        }

        /// <summary>
        /// 获取指定路径下一层的除指定格式以外的文件的路径
        /// </summary>
        /// <param name="path"></param>
        /// <param name="exName"></param>
        /// <returns></returns>
        static public List<string> GetSubFilesExcept(string path, string exName) {
            List<string> names = new List<string>();
            if (IsDirectoryExists(path) == false) {
                return names;
            }

            DirectoryInfo root = new DirectoryInfo(path);
            FileInfo[] files = root.GetFiles();
            string ex;
            for (int i = 0; i < files.Length; i++) {
                ex = FilePathHelper.GetExName(files[i].FullName);
                if (ex == exName) {
                    continue;
                }
                names.Add(StringTools.ChangePathFormat(files[i].FullName));
            }

            return names;
        }

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
            CheckDirection(realPath);
        }

        static public void CopyFile(string path, string tarPath) {
            if (!IsFileExists(path)) {
                return;
            }
            CheckFileSavePath(tarPath);
            File.Copy(path, tarPath, true);
        }

        static public void MoveFolder(string orgPath, string tarPath) {
            DirectoryInfo folder = new DirectoryInfo(orgPath);
            if (folder.Exists) {
                CheckDirection(tarPath);
                Directory.Delete(tarPath);
                folder.MoveTo(tarPath);
            }
        }

        static public void MoveFile(string orgPath, string tarPath) {
            if (File.Exists(orgPath) == false) {
                return;
            }
            File.Move(orgPath, tarPath);
        }

    }
}
