using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LuaEditor
{

    public class ComponentType
    {
        public const string GameObject = "go";
        public const string RectTransform = "rect";
        public const string Button = "btn";
        public const string Image = "img";
        public const string Text = "text";
        public const string RawImage = "rawimg";
        public const string AsynImgage = "asyn_img";
        public const string Slider = "sld";
        public const string Toggle = "tog";
        public const string InputField = "inpt";
        public const string ScrollView = "scroll";
        public const string ScrollBar = "bar";
        public const string DoTween = "tween";
        public const string Dropdown = "dropdown";
        public const string Cube = "cube";
        public const string Grid = "grid";
        public const string Loop = "loop";
        public const string UserToggle = "user_toggle";
    }

    public class ComponentMgr
    {

        static private ComponentMgr _Instance;

        public Dictionary<string, ComponentBase> Dic = null;

        public Dictionary<string, string> TypeDic = null;

        static public ComponentMgr Instance
        {
            get
            {
                if (_Instance == null)
                {
                    _Instance = new ComponentMgr();
                }
                return _Instance;
            }
        }

        public ComponentMgr()
        {
            Dic = new Dictionary<string, ComponentBase>();
            TypeDic = new Dictionary<string, string>();
            this.Register();
            this.RegisetrType();
        }

        ~ComponentMgr()
        {
            if (Dic != null)
            {
                Dic.Clear();
                Dic = null;
            }
            if (TypeDic != null)
            {
                TypeDic.Clear();
                Dic = null;
            }
        }

        public void Register()
        {
            Dic[ComponentType.GameObject] = new GameObjectMould();
            Dic[ComponentType.RectTransform] = new RectTransformMould();
            Dic[ComponentType.Button] = new ButtonMould();
            Dic[ComponentType.Image] = new ImageMould();
            Dic[ComponentType.Text] = new TextMould();
            Dic[ComponentType.RawImage] = new RawImageMould();
            Dic[ComponentType.Slider] = new SliderMould();
            Dic[ComponentType.Toggle] = new ToggleMould();
            Dic[ComponentType.InputField] = new InputFieldMould();
            Dic[ComponentType.AsynImgage] = new AsynImageMould();
            //Dic[ComponentType.DoTween] = new DoTweenMould();
            //Dic[ComponentType.Dropdown] = new DropdownMould();
            //Dic[ComponentType.Cube] = new UICubeMould();
            //Dic[ComponentType.Grid] = new UIGridMould();
            //Dic[ComponentType.Loop] = new UILoopMould();
            //Dic[ComponentType.UserToggle] = new UIToggleMould();
        }

        public void RegisetrType()
        {
            TypeDic[ComponentType.GameObject] = "GameObject";
            TypeDic[ComponentType.RectTransform] = "RectTransform";
            TypeDic[ComponentType.Button] = typeof(UnityEngine.UI.Button).ToString();
            TypeDic[ComponentType.Image] = typeof(UnityEngine.UI.Image).ToString();
            TypeDic[ComponentType.Text] = typeof(UnityEngine.UI.Text).ToString();
            TypeDic[ComponentType.RawImage] = typeof(UnityEngine.UI.RawImage).ToString();
            TypeDic[ComponentType.Slider] = typeof(UnityEngine.UI.Slider).ToString();
            TypeDic[ComponentType.Toggle] = typeof(UnityEngine.UI.Toggle).ToString();
            TypeDic[ComponentType.InputField] = typeof(UnityEngine.UI.InputField).ToString();
            TypeDic[ComponentType.AsynImgage] = typeof(UnityEngine.UI.Image).ToString();
            //TypeDic[ComponentType.DoTween] = typeof(DG.Tweening.DOTweenAnimation).ToString();
            //TypeDic[ComponentType.Dropdown] = typeof(User.UIDropdown).ToString();
            //TypeDic[ComponentType.Cube] = typeof(User.UICube).ToString();
            //TypeDic[ComponentType.Grid] = typeof(User.UIGrid).ToString();
            //TypeDic[ComponentType.Loop] = typeof(User.UILoop).ToString();
            //TypeDic[ComponentType.UserToggle] = typeof(User.UIToggle).ToString();
        }

        public ComponentBase GetObject(string key)
        {
            if (Dic != null && Dic.ContainsKey(key))
            {
                return Dic[key];
            }
            return null;
        }

        public string GetComponentType(string key)
        {
            if (TypeDic != null && TypeDic.ContainsKey(key))
            {
                return TypeDic[key];
            }
            return string.Empty;
        }

    }
}