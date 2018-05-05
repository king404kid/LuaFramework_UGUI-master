namespace LuaEditor
{
    public class ButtonMould : ComponentBase
    {

        public ButtonMould()
            : base("component/btn/find", "component/btn/set", ComponentType.Button) {
        }

        /// <summary>
        /// VarName==变量名，ComponentPath==控件路径
        /// </summary>
        /// <param name="array"></param>
        /// <returns></returns>
        public override string Find(string VarName, string ComponentPath) {
            string str = "";
            string mouldStr = GetMould(base.FindPath);
            if (VarName != null && ComponentPath != null) {
                string value1 = base.GetVarName(VarName);
                // string value2 = base.GetComponentPath(ComponentPath);
                string value3 = GetFuncName(VarName);
                if (string.IsNullOrEmpty(mouldStr)) {
                    return "";
                }
                str = StringTools.Format(mouldStr, value1, value3);
                return str;
            }
            return null;
        }

        /// <summary>
        /// TabName==表名，FuncName==方法名，VarName==变量名
        /// </summary>
        /// <param name="array"></param>
        /// <returns></returns>
        public override string Set(string TabName, string FuncName, string VarName) {
            string str = "";
            string mouldStr = GetMould(base.SetPath);
            if (TabName != null && FuncName != null && VarName != null) {
                string value1 = TabName;
                string value2 = GetFuncName(FuncName);
                // string value3 = base.GetVarName(VarName);
                if (string.IsNullOrEmpty(mouldStr)) {
                    return "";
                }
                str = StringTools.Format(mouldStr, value1, value2);
                return str;
            }
            return null;
        }

        public string GetFuncName(string FuncName) {
            return UILuaTools.GetFunName(FuncName) + "Click";
        }
    }
}