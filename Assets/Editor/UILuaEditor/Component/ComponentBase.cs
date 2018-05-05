using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

namespace LuaEditor
{
    public abstract class ComponentBase
    {

        public readonly string FindPath = "";
        public readonly string SetPath = "";
        public readonly string NowType = "";
        public readonly string Space = "  ";

        public ComponentBase(string findPath, string setPath, string nowType)
        {
            this.FindPath = findPath;
            this.SetPath = setPath;
            this.NowType = nowType;
        }

        public abstract string Find(string VarName, string ComponentPath);

        public abstract string Set(string TabName, string FuncName, string VarName);

        /// <summary>
        /// 设置控件定义
        /// </summary>
        /// <param name="componentType控件类型"></param>
        /// <param name="var控件名称"></param>
        /// <returns></returns>
        public virtual string GetDefine(string componentType, string var)
        {
            string componentAnnotation = Space + "-- " + componentType + "\r\n";
            string componentDefine = Space + GetVarName(var) + "=nil,";
            return componentAnnotation + componentDefine;
        }

        public string GetVarByName(string var)
        {
            return this.GetVarName(var);
        }

        protected string GetMould(string mouldName)
        {
            string mouldPath = GetMouldPath(mouldName);

            string mouldStr = ReadFileText(mouldPath);
            if (string.IsNullOrEmpty(mouldStr))
            {
                return "";
            }
            return mouldStr;
        }

        protected string GetVarName(string var)
        {
            if (!string.IsNullOrEmpty(var))
            {
                var = UILuaTools.GetVarName(var);
                string value = var + "_" + this.NowType;
                return value;
            }
            return null;
        }

        protected string GetFuncName(string var)
        {
            if (!string.IsNullOrEmpty(var))
            {
                var = UILuaTools.GetVarName(var);
                string value = "set_" + var + "_" + this.NowType;
                return value;
            }
            return null;
        }

        protected string GetComponentPath(string var)
        {
            if (!string.IsNullOrEmpty(var))
            {
                string value = "\"" + var + "\"";
                return value;
            }
            return null;
        }

        protected virtual string GetMouldPath(string mouldName)
        {
            string mouldPath = Application.dataPath + "/Editor/UILuaEditor/WriteLua/" + mouldName + ".txt";
            return mouldPath;
        }

        protected virtual string ReadFileText(string path)
        {
            if (!File.Exists(path))
            {
                return "";
            }
            string str = File.ReadAllText(path, Encoding.Default);
            return str;
        }
    }
}