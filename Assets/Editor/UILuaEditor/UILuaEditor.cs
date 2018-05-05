using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using System.IO;
using LuaFramework;

namespace LuaEditor
{
    public class UILuaEditor : EditorWindow
    {
        public static string constLuaPath = Application.dataPath + "/lua/";
        public static string constPath = Application.dataPath + "/lua/view";
        public const string Symbol = "#";
		private static string buildPath = constPath;
        private static string constUIEnumPath = constLuaPath + "enum/ui_enum.lua";
        private static Dictionary<string, UnityEngine.Object> luaBehaviourDefine = new Dictionary<string, Object>();

        /// <summary>
        /// 修改lua文件前做的一些参数校验
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="filename"></param>
        /// <param name="respath"></param>
        /// <param name="alert"></param>
        static public void ModificationLuaFile(GameObject obj, string filename, string respath, bool alert) {
            if (obj == null) {
                return;
            }
            if (string.IsNullOrEmpty(filename)) {
                filename = obj.name;
            }
            if (string.IsNullOrEmpty(respath)) {
                respath = GetResPath(obj);
            }
            string buildPath = "";
            LuaBehaviour behaviour = obj.GetComponent<LuaBehaviour>();
            if (behaviour != null) {
                buildPath = constLuaPath + behaviour.luaFile;
            } else {
                buildPath = EditorUtility.OpenFilePanel("选择更改文件:", constPath, "lua");
            }
            if (string.IsNullOrEmpty(buildPath)) {
                return;
            }
            ModificationLuaFileByPath(obj, buildPath, respath, alert);
        }

        /// <summary>
        /// 修改lua文件
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="buildPath"></param>
        /// <param name="respath"></param>
        /// <param name="alert"></param>
        static public void ModificationLuaFileByPath(GameObject obj, string buildPath, string respath, bool alert) {
            string create_content = CreateNewFile(obj, buildPath);
            bool is_succ = UILuaRead.Instance.WriteLua(buildPath, create_content, alert);
            if (is_succ) {
                RegisterLuaFile(obj, buildPath, respath);
                SetLuaBehaviour(obj, buildPath);
            }
            AssetDatabase.Refresh();
        }

        /// <summary>
        /// 创建新的定义，点击绑定，设置函数，同时保存原有的逻辑（如点击逻辑，业务逻辑等）
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="filePath"></param>
        /// <returns></returns>
        static public string CreateNewFile(GameObject obj, string filePath) {
            CleanUpGameObject(obj);
            string content = UILuaRead.Instance.ReadLua(filePath);
            LuaFileData dt = GetComponentInfo(obj, filePath);
            if (dt == null)
                return null;
            Dictionary<string, string> clickFunctionDic = dt.clickFunction;
            if (!string.IsNullOrEmpty(content)) {
                string after_str = string.Empty;
                string old_str = string.Empty;
                string new_str = string.Empty;
                string auto_after_str = string.Empty;
                string custom_str = string.Empty;
                string auto_str = string.Empty;
                GetReadStr(content, ref after_str, ref old_str, ref custom_str);
                Dictionary<string, string> clickTemp = ChangeFunc(old_str, clickFunctionDic);
                dt.clickFunction = clickTemp;
                string new_content = GetAllMat(dt);
                GetReadStr(new_content, ref auto_after_str, ref new_str, ref auto_str);
                string create_content = after_str + new_str + custom_str;
                return create_content;
            }
            return null;
        }

        /// <summary>
        /// 没看懂，有点像是删除缺失的组件
        /// </summary>
        /// <param name="go"></param>
        static public void CleanUpGameObject(GameObject go) {
            // 创建一个序列化对象
            SerializedObject serializedObject = new SerializedObject(go);
            // 获取组件列表属性
            SerializedProperty prop = serializedObject.FindProperty("m_Component");
            var components = go.GetComponents<Component>();
            int r = 0;
            for (int j = 0; j < components.Length; j++) {
                // 如果组建为null
                if (components[j] == null) {
                    // 按索引删除
                    prop.DeleteArrayElementAtIndex(j - r);
                    r++;
                }
            }
            // 应用修改到对象
            serializedObject.ApplyModifiedProperties();
            // 这一行一定要加！！！
            EditorUtility.SetDirty(go);
        }

        /// <summary>
        /// 创建新的UI文件，里面保存新的定义，点击绑定，设置函数（不需要保存原来的）
        /// </summary>
        /// <param name="Obj"></param>
        /// <param name="fileName"></param>
        /// <param name="respath"></param>
        /// <returns></returns>
        static public bool CreateUIFile(GameObject Obj, string fileName, string respath) {
            if (Obj != null) {
                string objName = Obj.name;

                if (string.IsNullOrEmpty(fileName)) {
                    fileName = objName;
                }
                string buildPath = EditorUtility.SaveFilePanel("选择保存路径:", constPath, fileName, "lua");
                if (string.IsNullOrEmpty(buildPath)) {
                    return false;
                }
                LuaFileData dt = GetComponentInfo(Obj, buildPath);
                if (dt == null)
                    return false;
                string content = GetAllMat(dt);
                bool is_succ = UILuaRead.Instance.WriteLua(buildPath, content);
                if (is_succ) {
                    if (string.IsNullOrEmpty(respath)) {
                        respath = GetResPath(Obj);
                    }
                    RegisterLuaFile(Obj, buildPath, respath);
                    SetLuaBehaviour(Obj, buildPath);
                }
                return is_succ;
            }
            return false;
        }

        /// <summary>
        /// 绑定ui_enum文件
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="buildPath"></param>
        /// <param name="respath"></param>
        static public void RegisterLuaFile(GameObject obj, string buildPath, string respath) {
            string classname = Path.GetFileNameWithoutExtension(buildPath);
            string classpath = buildPath.Replace(constLuaPath, "");
            string template = "    {0} =   {{  classpath = \"{1}\" , respath = \"{2}\" }} ,";
            string filestring = UILuaRead.Instance.ReadLua(constUIEnumPath);
            string replacestr = string.Format(template, classname, classpath, respath);
            if (!string.IsNullOrEmpty(filestring)) {
                string[] files = filestring.Split('\n');
                filestring = "";
                bool replace = false;
                for (int i = 0; i < files.Length - 1; i++) {
                    if (files[i].IndexOf("    " + classname + " ") > -1) {
                        Debug.LogWarning(files[i] + "has been replaced");
                        files[i] = string.Format(template, classname, classpath, respath);
                        replace = true;
                        break;
                    }
                }
                int insertindex = -1;
                if (!replace) {
                    insertindex = Random.Range(0, files.Length - 1);
                }
                for (int i = 0; i < files.Length - 1; i++) {
                    if (!string.IsNullOrEmpty(files[i]) && files[i] != "\n") {
                        filestring += files[i] + "\n";
                    }
                    if (i == insertindex) {
                        filestring += replacestr + "\n";
                    }
                }
                filestring += files[files.Length - 1];
                UILuaRead.Instance.WriteLua(constUIEnumPath, filestring, false);
            }
        }

        /// <summary>
        /// 获取res_path路径，格式：prefab下的相对路径，除去后缀
        /// </summary>
        /// <param name="Obj"></param>
        /// <returns></returns>
        static public string GetResPath(GameObject Obj) {
            string res_path = AssetDatabase.GetAssetPath(Obj);
            if (string.IsNullOrEmpty(res_path)) {
                res_path = AssetDatabase.GetAssetPath(PrefabUtility.GetPrefabParent(Obj));
            }
            if (!string.IsNullOrEmpty(res_path)) {
                string sub_path = "UINew/prefab/";
                int index = res_path.IndexOf(sub_path);
                if (index != -1) {
                    res_path = res_path.Substring(index).Replace(sub_path, "");
                    res_path = FileTools.RemoveExName(res_path);
                }
            }
            return res_path;
        }

        /// <summary>
        /// 获取go的所有节点下，有特殊标记对应生成的定义，点击绑定，设置函数
        /// </summary>
        /// <param name="Obj"></param>
        /// <param name="filePath"></param>
        /// <returns></returns>
        static public LuaFileData GetComponentInfo(GameObject Obj, string filePath) {
            string className = FileTools.GetFilename(filePath, false);
            Transform root = Obj.transform;
            List<Transform> tfArray = new List<Transform>();
            UITools.GetComponentsExclude<Transform>(root, tfArray, go => go.GetComponent<LuaBehaviour>() == null);
            tfArray.Sort(delegate(Transform t1, Transform t2) {
                return t1.name.CompareTo(t2.name);
            });
            List<string> defineList = new List<string>();
            List<string> find_component = new List<string>();
            List<string> set_function = new List<string>();
            List<string> defaultTextComponent = new List<string>();
            Dictionary<string, string> click_function = new Dictionary<string, string>();
            List<Transform> component_examine = new List<Transform>();
            //清空定义列表
            ClearDefine();
            foreach (Transform child in tfArray) {
                string childName = child.name;
                string obj_name = null;
                string stype = null;
                IsKeyword(childName, ref obj_name, ref stype);
                if (stype != null) {
                    string sstype = stype;
                    if (stype.Trim() != sstype) {
                        bool b = UnityEditor.EditorUtility.DisplayDialog("控件名非法", "控件名首位含有空白字符", "OK");
                        GameObject go = child.gameObject;
                        if (b && go != null) {
                            EditorGUIUtility.PingObject(go);
                            Selection.activeGameObject = go;
                        }
                        return null;
                    }
                    string widget_path = UILuaTools.GetPathByRoot(root, child);
                    Transform oldtrans = ListContains(component_examine, childName);
                    if (oldtrans != null) {
                        string componentPath = root.name + "/" + widget_path;
                        string desc = string.Format("控件{0}重复,重复路径为:{1}", childName, componentPath);
                        bool b = UnityEditor.EditorUtility.DisplayDialog("控件名重复", desc, "OK");
                        GameObject go = child.gameObject;
                        if (b && go != null) {
                            EditorGUIUtility.PingObject(go);
                            Selection.activeGameObject = go;
                        }
                        return null;
                    } else {
                        component_examine.Add(child);
                    }
                    ComponentBase component = ComponentMgr.Instance.GetObject(stype);
                    if (component != null) {
                        string var = obj_name;
                        string componentType = ComponentMgr.Instance.GetComponentType(component.NowType);
                        string define = component.GetDefine(componentType, var);
                        string defineName = component.GetVarByName(var);
                        //将对于变量名的object添加进列表，后面给luaBehaviour赋值
                        if (!string.IsNullOrEmpty(defineName)) {
                            Component c = child.gameObject.GetComponent(componentType);
                            if (c == null) {
                                AddDefine(defineName, child.gameObject);
                            } else
                                AddDefine(defineName, c);
                        }
                        string set_comp = component.Set(className, var, var);
                        //buttom特殊处理，与设置方法分开
                        if (stype == ComponentType.Button) {
                            string find_comp = component.Find(var, widget_path);
                            if (find_comp != null)
                                find_component.Add(find_comp);
                            ButtonMould button = component as ButtonMould;
                            string key = button.GetFuncName(var);
                            if (set_comp != null)
                                click_function.Add(key, set_comp);
                        } else {
                            if (set_comp != null)
                                set_function.Add(set_comp);
                        }
                        if (define != null)
                            defineList.Add(define);
                    }
                }
            }
            return new LuaFileData(className, defineList, find_component, set_function, null, click_function, defaultTextComponent);
        }

        /// <summary>
        /// 组合生成的定义，点击绑定，设置函数为一个字符串
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        static public string GetAllMat(LuaFileData dt) {
            string content = null;
            //className, defineList, find_component, set_function, null, click_function, defaultTextComponent
            if (dt != null && dt.clickFunction != null) {
                string value1 = string.Empty;
                string value2 = string.Empty;
                string value3 = string.Empty;
                string value4 = string.Empty;
                string value5 = string.Empty;
                string value6 = string.Empty;
                List<string> clickFunction = new List<string>(dt.clickFunction.Values);
                string className = dt.className;
                value1 = GetLuaString(dt.defineVar);
                value2 = GetLuaString(dt.setFunction);
                value3 = GetLuaString(dt.findComponent);
                value4 = GetLuaString(clickFunction);
                value5 = GetLuaString(dt.initMat);
                //这个暂时没用，有需求在加上
                value6 = GetLuaString(dt.defaultTextComponent);
                LuaUIMould uimould = new LuaUIMould();
                content = uimould.Set(className, value1, value2, value3, value4, value5);
            }
            return content;
        }

        /// <summary>
        /// 新增点击函数，对于旧的点击函数保留
        /// </summary>
        /// <param name="old_str"></param>
        /// <param name="clickFunctionDic"></param>
        /// <returns></returns>
        static public Dictionary<string, string> ChangeFunc(string old_str, Dictionary<string, string> clickFunctionDic) {
            int click_start = old_str.IndexOf("!@#startClick");
            int click_end = old_str.IndexOf("!@#endClick");
            if (click_start != -1 && click_end != -1) {
                int lenght_click = click_end - click_start + ("!@#endClick").Length;
                string click_str = old_str.Substring(click_start, lenght_click);
                int end_index = 0;
                Dictionary<string, string> clickTemp = new Dictionary<string, string>();
                foreach (KeyValuePair<string, string> pair in clickFunctionDic) {
                    string key = pair.Key;
                    string temp_start = "!@#" + key;
                    string temp_end = "!@#" + key;
                    int start_index = click_str.IndexOf(temp_start, 0);
                    end_index = click_str.LastIndexOf(temp_end);
                    if (start_index == -1 || end_index == -1 || start_index == end_index) {
                        clickTemp[key] = pair.Value;
                        continue;
                    }
                    int lenght = end_index - start_index + temp_end.Length;
                    string mat = click_str.Substring(start_index, lenght);
                    //加上分割符
                    clickTemp[key] = "\r\n--" + mat;
                }
                return clickTemp;
            }
            return null;
        }

        /// <summary>
        /// 根据!@#start到!@#end对文本进行拆分成3部分
        /// </summary>
        /// <param name="content"></param>
        /// <param name="after_str"></param>
        /// <param name="old_str"></param>
        /// <param name="custom_str"></param>
        static public void GetReadStr(string content, ref string after_str, ref string old_str, ref string custom_str) {
            if (string.IsNullOrEmpty(content) == true) {
                return;
            }
            content = content.Replace("!@#startInit", "");
            content = content.Replace("!@#endI", "");
            string start_str = "!@#start";
            string end_str = "!@#end";
            int start_index = content.IndexOf(start_str);
            int end_index = content.LastIndexOf(end_str) + end_str.Length;
            if (start_index == -1 || end_index == -1) {
                return;
            }
            after_str = content.Substring(0, start_index);
            old_str = content.Substring(start_index, end_index - start_index);
            custom_str = content.Substring(end_index);
        }

        /// <summary>
        /// 组合成单条字符串
        /// </summary>
        /// <param name="lst"></param>
        /// <returns></returns>
        static public string GetLuaString(List<string> lst) {
            string value = string.Empty;
            if (lst != null && lst.Count > 0) {
                foreach (string str in lst) {
                    value = value + str + "\r\n";
                }
            }
            return value;
        }

        /// <summary>
        /// 拆分关键字
        /// </summary>
        /// <param name="str"></param>
        /// <param name="sname"></param>
        /// <param name="stype"></param>
        static public void IsKeyword(string str, ref string sname, ref string stype) {
            if (str != null) {
                int value = str.IndexOf(Symbol);
                if (value != -1) {
                    stype = str.Substring(value + 1);
                    sname = str.Substring(0, value);
                }
            }
        }

        /// <summary>
        /// 判断是否重复包含
        /// </summary>
        /// <param name="lst"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        static public Transform ListContains(List<Transform> lst, string item) {
            foreach (Transform tf in lst) {
                if (tf.name == item)
                    return tf;
            }
            return null;
        }

        /// <summary>
        /// 把拆分好的变量设置到luabehaviour里面
        /// </summary>
        /// <param name="Obj"></param>
        /// <param name="buildPath"></param>
        static private void SetLuaBehaviour(UnityEngine.GameObject Obj, string buildPath) {
            if (Obj != null) {
                LuaBehaviour luaBehaviour = Obj.transform.GetComponent<LuaBehaviour>();
                if (luaBehaviour == null) {
                    luaBehaviour = Obj.AddComponent<LuaBehaviour>();
                }
                if (luaBehaviour != null) {
                    List<BindItem> lst = new List<BindItem>();
                    if (luaBehaviourDefine != null) {
                        foreach (KeyValuePair<string, UnityEngine.Object> pair in luaBehaviourDefine) {
                            string name = pair.Key;
                            UnityEngine.Object obj = pair.Value;
                            BindItem bItem = new BindItem();
                            bItem.name = name;
                            bItem.obj = obj;
                            lst.Add(bItem);
                        }
                        string luaFilePath = buildPath.Replace(constLuaPath, "");
                        luaBehaviour.SetInitInfo(luaFilePath, lst);
                    }

                }
            }
        }

        /// <summary>
        /// 清空luabehaviour的变量
        /// </summary>
        static private void ClearDefine() {
            if (luaBehaviourDefine != null) {
                luaBehaviourDefine.Clear();
            }
        }

        /// <summary>
        /// 保存定义
        /// </summary>
        /// <param name="defineName"></param>
        /// <param name="obj"></param>
        static private void AddDefine(string defineName, UnityEngine.Object obj) {
            if (luaBehaviourDefine == null) {
                luaBehaviourDefine = new Dictionary<string, Object>();
            }
            luaBehaviourDefine[defineName] = obj;
        }
    }
}