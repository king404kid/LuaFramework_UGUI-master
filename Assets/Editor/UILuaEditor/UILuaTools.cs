using System.Collections.Generic;
using UnityEngine;



namespace LuaEditor
{
    public class UILuaTools
    {
        static public string GetPathByRoot(Transform root, Transform widget)
        {
            if (root != null && widget != null)
            {
                string rootN = root.name;
                string widgetN = widget.name;
                string mat = string.Empty;
                List<string> info = new List<string>();
                info.Add(widgetN);
                while (rootN != widgetN)
                {
                    Transform tf = widget.parent;
                    if (tf != null)
                    {
                        widgetN = tf.name;
                        if (rootN == widgetN) break;
                        widget = tf;
                        info.Add(widgetN);
                    }
                    else break;
                }

                if (info.Count > 0)
                {
                    int count = info.Count - 1;
                    mat = info[count];
                    for (int i = count - 1; i >= 0; i--)
                    {
                        mat += "/" + info[i];
                    }
                }
                return mat;
            }

            return string.Empty;
        }

        static public string GetClassName(string n)
        {
            string cName = n.Substring(1, n.Length - 1);
            string fName = n.Substring(0, 1);
            fName = fName.ToUpper();
            cName = fName + cName;
            return cName;
        }

        static public string GetVarName(string n)
        {
            string cName = n.Substring(1, n.Length - 1);
            string fName = n.Substring(0, 1);
            fName = fName.ToLower();
            cName = fName + cName;
            return cName;
        }

        static public string GetFunName(string n)
        {
            string cName = n.Substring(1, n.Length - 1);
            string fName = n.Substring(0, 1);
            fName = fName.ToLower();
            cName = fName + cName;
            return cName;
        }

        static public string GetCtrlName(string n)
        {
            string cName = GetClassName(n) + "Ctrl";
            return cName;
        }
    }
}
