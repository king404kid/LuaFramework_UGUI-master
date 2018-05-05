using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace LuaEditor
{
    public class ComponentTypeEditor
    {
        [MenuItem("GameObject/LuaFile/AddComponentType/GameObject", false, 15)]
        static public void AddComponentGameObject()
        {
            UnityEngine.Object[] obj = GetSelectObj();
            if (obj == null)
                return;

            foreach (UnityEngine.Object temp in obj)
            {
                if (temp != null)
                {
                    UnityEngine.GameObject go = temp as GameObject;
                    string newName = CreateGameObjectName("go", go);
                    go.name = newName;
                }
            }
        }
        [MenuItem("GameObject/LuaFile/AddComponentType/RectTransform", false, 15)]
        static public void AddComponentRectTransform()
        {
            UnityEngine.Object[] obj = GetSelectObj();
            if (obj == null)
                return;

            foreach (UnityEngine.Object temp in obj)
            {
                if (temp != null)
                {
                    UnityEngine.GameObject go = temp as GameObject;
                    string newName = CreateGameObjectName("rect", go);
                    go.name = newName;
                }
            }
        }
        [MenuItem("GameObject/LuaFile/AddComponentType/Button", false, 15)]
        static public void AddComponentButton()
        {
            UnityEngine.Object[] obj = GetSelectObj();
            if (obj == null)
                return;

            foreach (UnityEngine.Object temp in obj)
            {
                if (temp != null)
                {
                    UnityEngine.GameObject go = temp as GameObject;
                    string newName = CreateGameObjectName("btn", go);
                    go.name = newName;
                }
            }
        }
        [MenuItem("GameObject/LuaFile/AddComponentType/Image", false, 15)]
        static public void AddComponentImage()
        {
            UnityEngine.Object[] obj = GetSelectObj();
            if (obj == null)
                return;

            foreach (UnityEngine.Object temp in obj)
            {
                if (temp != null)
                {
                    UnityEngine.GameObject go = temp as GameObject;
                    string newName = CreateGameObjectName("img", go);
                    go.name = newName;
                }
            }
        }
        [MenuItem("GameObject/LuaFile/AddComponentType/Text", false, 15)]
        static public void AddComponentText()
        {
            UnityEngine.Object[] obj = GetSelectObj();
            if (obj == null)
                return;

            foreach (UnityEngine.Object temp in obj)
            {
                if (temp != null)
                {
                    UnityEngine.GameObject go = temp as GameObject;
                    string newName = CreateGameObjectName("text", go);
                    go.name = newName;
                }
            }
        }
        [MenuItem("GameObject/LuaFile/AddComponentType/RawImage", false, 15)]
        static public void AddComponentRawImage()
        {
            UnityEngine.Object[] obj = GetSelectObj();
            if (obj == null)
                return;

            foreach (UnityEngine.Object temp in obj)
            {
                if (temp != null)
                {
                    UnityEngine.GameObject go = temp as GameObject;
                    string newName = CreateGameObjectName("rawimg", go);
                    go.name = newName;
                }
            }
        }
        [MenuItem("GameObject/LuaFile/AddComponentType/AsynImgage", false, 15)]
        static public void AddComponentAsynImgage()
        {
            UnityEngine.Object[] obj = GetSelectObj();
            if (obj == null)
                return;

            foreach (UnityEngine.Object temp in obj)
            {
                if (temp != null)
                {
                    UnityEngine.GameObject go = temp as GameObject;
                    string newName = CreateGameObjectName("asyn_img", go);
                    go.name = newName;
                }
            }
        }
        [MenuItem("GameObject/LuaFile/AddComponentType/Slider", false, 15)]
        static public void AddComponentSlider()
        {
            UnityEngine.Object[] obj = GetSelectObj();
            if (obj == null)
                return;

            foreach (UnityEngine.Object temp in obj)
            {
                if (temp != null)
                {
                    UnityEngine.GameObject go = temp as GameObject;
                    string newName = CreateGameObjectName("sld", go);
                    go.name = newName;
                }
            }
        }
        [MenuItem("GameObject/LuaFile/AddComponentType/Toggle", false, 15)]
        static public void AddComponentToggle()
        {
            UnityEngine.Object[] obj = GetSelectObj();
            if (obj == null)
                return;

            foreach (UnityEngine.Object temp in obj)
            {
                if (temp != null)
                {
                    UnityEngine.GameObject go = temp as GameObject;
                    string newName = CreateGameObjectName("tog", go);
                    go.name = newName;
                }
            }
        }
        [MenuItem("GameObject/LuaFile/AddComponentType/InputField", false, 15)]
        static public void AddComponentInputField()
        {
            UnityEngine.Object[] obj = GetSelectObj();
            if (obj == null)
                return;

            foreach (UnityEngine.Object temp in obj)
            {
                if (temp != null)
                {
                    UnityEngine.GameObject go = temp as GameObject;
                    string newName = CreateGameObjectName("inpt", go);
                    go.name = newName;
                }
            }
        }
        [MenuItem("GameObject/LuaFile/AddComponentType/ScrollView", false, 15)]
        static public void AddComponentScrollView()
        {
            UnityEngine.Object[] obj = GetSelectObj();
            if (obj == null)
                return;

            foreach (UnityEngine.Object temp in obj)
            {
                if (temp != null)
                {
                    UnityEngine.GameObject go = temp as GameObject;
                    string newName = CreateGameObjectName("scroll", go);
                    go.name = newName;
                }
            }
        }
        [MenuItem("GameObject/LuaFile/AddComponentType/ScrollBar", false, 15)]
        static public void AddComponentScrollBar()
        {
            UnityEngine.Object[] obj = GetSelectObj();
            if (obj == null)
                return;

            foreach (UnityEngine.Object temp in obj)
            {
                if (temp != null)
                {
                    UnityEngine.GameObject go = temp as GameObject;
                    string newName = CreateGameObjectName("bar", go);
                    go.name = newName;
                }
            }
        }
        [MenuItem("GameObject/LuaFile/AddComponentType/DoTween", false, 15)]
        static public void AddComponentDoTween()
        {
            UnityEngine.Object[] obj = GetSelectObj();
            if (obj == null)
                return;

            foreach (UnityEngine.Object temp in obj)
            {
                if (temp != null)
                {
                    UnityEngine.GameObject go = temp as GameObject;
                    string newName = CreateGameObjectName("tween", go);
                    go.name = newName;
                }
            }
        }
        [MenuItem("GameObject/LuaFile/AddComponentType/Dropdown", false, 15)]
        static public void AddComponentDropdown()
        {
            UnityEngine.Object[] obj = GetSelectObj();
            if (obj == null)
                return;

            foreach (UnityEngine.Object temp in obj)
            {
                if (temp != null)
                {
                    UnityEngine.GameObject go = temp as GameObject;
                    string newName = CreateGameObjectName("dropdown", go);
                    go.name = newName;
                }
            }
        }
        [MenuItem("GameObject/LuaFile/AddComponentType/Cube", false, 15)]
        static public void AddComponentCube()
        {
            UnityEngine.Object[] obj = GetSelectObj();
            if (obj == null)
                return;

            foreach (UnityEngine.Object temp in obj)
            {
                if (temp != null)
                {
                    UnityEngine.GameObject go = temp as GameObject;
                    string newName = CreateGameObjectName("cube", go);
                    go.name = newName;
                }
            }
        }
        [MenuItem("GameObject/LuaFile/AddComponentType/Grid", false, 15)]
        static public void AddComponentGrid()
        {
            UnityEngine.Object[] obj = GetSelectObj();
            if (obj == null)
                return;

            foreach (UnityEngine.Object temp in obj)
            {
                if (temp != null)
                {
                    UnityEngine.GameObject go = temp as GameObject;
                    string newName = CreateGameObjectName("grid", go);
                    go.name = newName;
                }
            }
        }
        [MenuItem("GameObject/LuaFile/AddComponentType/Loop", false, 15)]
        static public void AddComponentLoop()
        {
            UnityEngine.Object[] obj = GetSelectObj();
            if (obj == null)
                return;

            foreach (UnityEngine.Object temp in obj)
            {
                if (temp != null)
                {
                    UnityEngine.GameObject go = temp as GameObject;
                    string newName = CreateGameObjectName("loop", go);
                    go.name = newName;
                }
            }
        }
		[MenuItem("GameObject/LuaFile/AddComponentType/UserToggle", false, 15)]
		static public void AddComponentUserToogle()
		{
			UnityEngine.Object[] obj = GetSelectObj();
			if (obj == null)
				return;

			foreach (UnityEngine.Object temp in obj)
			{
				if (temp != null)
				{
					UnityEngine.GameObject go = temp as GameObject;
					string newName = CreateGameObjectName("user_toggle", go);
					go.name = newName;
				}
			}
		}
	

        static public UnityEngine.Object[] GetSelectObj()
        {
            UnityEngine.Object[] obj = UnityEditor.Selection.GetFiltered(typeof(UnityEngine.Object), SelectionMode.Unfiltered);
            if (obj != null && obj.Length > 0)
            {
                return obj;
            }
            return null;
        }

        static public string CreateGameObjectName(string stype, GameObject go)
        {
            string name = go.name;
            string str_type = "#" + stype;
            int index = name.IndexOf("#");
            string newName = name;
            if (index == -1)
                newName = name + str_type;
            else
            {
                string del_str = name.Substring(index);
                newName = name.Replace(del_str, str_type);
            }
            return newName;
        }
    }
}
