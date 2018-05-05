namespace LuaEditor
{
    public class GameObjectMould : ComponentBase
    {

        public GameObjectMould()
            : base("component/go/find", "component/go/set", ComponentType.GameObject) {

        }

        /// <summary>
        /// value1==变量名，value2==控件路径
        /// </summary>
        /// <param name="array"></param>
        /// <returns></returns>
        public override string Find(string VarName, string ComponentPath) {
            string str = "";
            string mouldStr = GetMould(base.FindPath);
            if (VarName != null && ComponentPath != null) {
                string value1 = base.GetVarName(VarName);
                string value2 = base.GetComponentPath(ComponentPath);
                if (string.IsNullOrEmpty(mouldStr)) {
                    return "";
                }
                str = StringTools.Format(mouldStr, value1, value2);
                return str;
            }
            return null;
        }

        public override string Set(string TabName, string FuncName, string VarName) {
            return null;
        }
    }
}