using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LuaEditor
{
    public class LuaFileData
    {
        /// <summary>
        /// tab名<类名>
        /// </summary>
        public string className;
        /// <summary>
        /// 定义变量
        /// </summary>
        public List<string> defineVar;
        /// <summary>
        /// 查找控件.tips:现在只用来注册按钮监听事件
        /// </summary>
        public List<string> findComponent;
        /// <summary>
        /// 设置控件方法
        /// </summary>
        public List<string> setFunction;
        /// <summary>
        /// 设置初始化内容
        /// </summary>
        public List<string> initMat;
        /// <summary>
        /// 点击事件回调方法
        /// </summary>
        public Dictionary<string, string> clickFunction;

        public List<string> defaultTextComponent;

        public LuaFileData()
        { }

        public LuaFileData(string className, List<string> defineVar, List<string> findComponent, List<string> setFunction, List<string> initMat, Dictionary<string, string> clickFunction, List<string> defaultTextComponent)
        {
            this.className = className;
            this.defineVar = defineVar;
            this.findComponent = findComponent;
            this.setFunction = setFunction;
            this.initMat = initMat;
            this.clickFunction = clickFunction;
            this.defaultTextComponent = defaultTextComponent;
        }

    }
}
