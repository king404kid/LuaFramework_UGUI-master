namespace LuaEditor
{
    public class LuaUIMould : ComponentBase
    {

        public LuaUIMould()
            : base(null, "view/ui", null) {

        }

        public override string Find(string VarName, string ComponentPath) {
            return null;
        }

        public override string Set(string TabName, string FuncName, string VarName) {
            return null;
        }

        public string Set(params string[] array) {
            string str = "";
            string mouldStr = GetMould(base.SetPath);
            if (array != null && array.Length >= 6) {
                string value1 = array[0];
                string value2 = array[1];
                string value3 = array[2];
                string value4 = array[3];
                string value5 = array[4];
                string value6 = array[5];
                if (string.IsNullOrEmpty(mouldStr)) {
                    return "";
                }
                str = StringTools.Format(mouldStr, value1, value2, value3, value4, value5, value6);
                return str;
            }
            return null;
        }
    }
}