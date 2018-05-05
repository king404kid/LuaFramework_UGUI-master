using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

public class StringTools
{
    static private StringTools _instance;

    public static StringTools Instance {
        get {
            if (_instance == null) {
                _instance = new StringTools();
            }
            return _instance;
        }
    }

    /// <summary>
    /// 移除扩展名
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    static public string RemoveExName(string str) {
        string regexStr = @".+(?=\.)";
        string strs = GetFirstMatch(str, regexStr);
        if (string.IsNullOrEmpty(strs)) {
            strs = str;
        }
        return strs;
    }

    /// <summary>
    /// 获取第一个匹配
    /// </summary>
    /// <param name="str"></param>
    /// <param name="regexStr"></param>
    /// <returns></returns>
    static public string GetFirstMatch(string str, string regexStr) {
        if (string.IsNullOrEmpty(str) || string.IsNullOrEmpty(regexStr)) {
            return null;
        }
        Match m = Regex.Match(str, regexStr);
        if (!string.IsNullOrEmpty(m.ToString())) {
            return m.ToString();
        } else {
            return null;
        }
    }

    /// <summary>
    /// 是否包含汉字
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    public static bool isContainCN(string str) {
        return Regex.IsMatch(str, @"[\u4e00-\u9fa5]+");
    }

    /// <summary>
    /// 格式化字符串，替代{数字}内容
    /// </summary>
    /// <param name="str"></param>
    /// <param name="args"></param>
    /// <returns></returns>
    static public string Format(string str, params string[] args) {
        string newStr = str;
        string rexStr = @"\{[0-9]+\}";
        MatchCollection mc = Regex.Matches(str, rexStr);
        if (mc.Count > 0) {
            for (int i = 0; i < mc.Count; i++) {
                if (i < args.Length) {
                    string subStr = "{" + i + "}";
                    newStr = newStr.Replace(subStr, args[i]);
                }
            }
        }
        return newStr;
    }
}