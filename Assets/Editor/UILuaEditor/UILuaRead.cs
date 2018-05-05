namespace LuaEditor
{
    public class UILuaRead
    {

        private static UILuaRead _Instance;

        public static UILuaRead Instance
        {
            get
            {
                if (_Instance == null)
                    _Instance = new UILuaRead();
                return _Instance;
            }
        }

        public string ReadLua(string path)
        {
            string str = FileTools.ReadFileText(path, true);
            return str;
        }

        public bool WriteLua(string path, string content,bool alert = true)
        {
            bool is_succ = true;
            if (string.IsNullOrEmpty(content))
            {
                is_succ = false;
                UnityEditor.EditorUtility.DisplayDialog("Create LuaUIFile", "生成失败,文件内容为空!\n" + path, "OK");
                return false;
            }
            FileTools.SaveFile(path, content, true);
            if (alert)
            {
                UnityEditor.EditorUtility.DisplayDialog("Create LuaUIFile", "生成成功!", "OK");
            }

            return is_succ;
        }

        public void Release()
        {
            _Instance = null;
        }
    }
}