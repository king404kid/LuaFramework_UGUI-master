namespace LuaEditor
{
    public class TextDefaultMould : ComponentBase
    {

        public TextDefaultMould()
            : base("component/defaultText/find", "component/defaultText/set", null) {
        }

        /// <summary>
        /// VarName==变量名，ComponentPath==控件路径
        /// </summary>
        /// <param name="array"></param>
        /// <returns></returns>
        public string CreateText(string ComponentPath, string VarMat) {
            string str = "";
            string mouldStr = GetMould(base.FindPath);
            if (ComponentPath != null) {
                string value1 = base.GetComponentPath(ComponentPath);
                string value2 = base.GetComponentPath(VarMat);
                if (string.IsNullOrEmpty(mouldStr)) {
                    return "";
                }
                str = StringTools.Format(mouldStr, value1, value2);
                return str;
            }
            return null;
        }

        public override string Set(string TabName, string FuncName, string VarMat) {
            return null;
        }

        public override string Find(string VarName, string ComponentPath) {
            return null;
        }
    }
}