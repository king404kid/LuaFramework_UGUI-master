namespace LuaEditor
{
    public class LuaCtrlMould : ComponentBase
    {

        public LuaCtrlMould()
            : base(null, "view/ctrl", null) {

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
            if (array != null && array.Length >= 3) {
                string value1 = array[0];
                string value2 = array[1];
                string value3 = array[2];
                if (string.IsNullOrEmpty(mouldStr)) {
                    return "";
                }
                str = StringTools.Format(mouldStr, value1, value2, value3);
                return str;
            }
            return null;
        }
    }
}