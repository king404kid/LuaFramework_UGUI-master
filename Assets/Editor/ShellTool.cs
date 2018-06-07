using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
public class ShellTool
{
    /// <summary>
    /// 执行exe
    /// </summary>
    /// <param name="command"></param>
    /// <param name="argument"></param>
    public static void ProcessCommand(string command, string argument) {
        System.Diagnostics.ProcessStartInfo info = new System.Diagnostics.ProcessStartInfo(command);
        info.Arguments = argument;
        info.CreateNoWindow = false;
        info.ErrorDialog = true;
        info.UseShellExecute = true;
        if (info.UseShellExecute) {
            info.RedirectStandardOutput = false;
            info.RedirectStandardError = false;
            info.RedirectStandardInput = false;
        } else {
            info.RedirectStandardOutput = true;
            info.RedirectStandardError = true;
            info.RedirectStandardInput = true;
            info.StandardOutputEncoding = System.Text.UTF8Encoding.UTF8;
            info.StandardErrorEncoding = System.Text.UTF8Encoding.UTF8;
        }
        System.Diagnostics.Process process = System.Diagnostics.Process.Start(info);
        if (!info.UseShellExecute) {
            Debug.Log(process.StandardOutput);
            Debug.Log(process.StandardError);
        }
        process.WaitForExit();
        process.Close();
    }

    public static void ProcessCommandMacWithCD(string cdpath, string command, string argument) {
        System.Diagnostics.ProcessStartInfo info = new System.Diagnostics.ProcessStartInfo(command);
        info.Arguments = argument;
        info.CreateNoWindow = false;
        info.ErrorDialog = true;
        info.UseShellExecute = false;
        info.WorkingDirectory = cdpath;
        info.RedirectStandardOutput = true;
        info.RedirectStandardError = true;
        info.RedirectStandardInput = true;
        info.StandardOutputEncoding = System.Text.UTF8Encoding.UTF8;
        info.StandardErrorEncoding = System.Text.UTF8Encoding.UTF8;
        System.Diagnostics.Process process = System.Diagnostics.Process.Start(info);
        process.WaitForExit();
        StreamReader reader = process.StandardOutput;
        string line = null;
        string message = line;
        while (!reader.EndOfStream) {
            line = reader.ReadLine();
            message += line + "  ";
        }
        if (null != message) {
            Debug.Log(message);
        }
        reader = process.StandardError;
        line = null;
        message = null;
        while (!reader.EndOfStream) {
            line = reader.ReadLine();
            message += line + "  ";
        }
        if (null != message) {
            Debug.LogError(message);
        }
        process.Close();
    }

    public static void CmdWriteLine(string str) {
        Debug.Log("cmd:" + str);
        System.Diagnostics.Process p = new System.Diagnostics.Process();
        p.StartInfo.FileName = "cmd.exe";
        p.StartInfo.UseShellExecute = false;    //是否使用操作系统shell启动
        p.StartInfo.RedirectStandardInput = true;//接受来自调用程序的输入信息
        p.StartInfo.RedirectStandardOutput = true;//由调用程序获取输出信息
        p.StartInfo.RedirectStandardError = true;//重定向标准错误输出
        p.StartInfo.CreateNoWindow = true;//不显示程序窗口
        p.Start();//启动程序

        //向cmd窗口发送输入信息
        p.StandardInput.WriteLine(str + "&exit");

        p.StandardInput.AutoFlush = true;
        //p.StandardInput.WriteLine("exit");
        //向标准输入写入要执行的命令。这里使用&是批处理命令的符号，表示前面一个命令不管是否执行成功都执行后面(exit)命令，如果不执行exit命令，后面调用ReadToEnd()方法会假死
        //同类的符号还有&&和||前者表示必须前一个命令执行成功才会执行后面的命令，后者表示必须前一个命令执行失败才会执行后面的命令

        //获取cmd窗口的输出信息
        string output = p.StandardOutput.ReadToEnd();

        p.WaitForExit();//等待程序执行完退出进程
        p.Close();
        Debug.Log(output);
    }

    public static void CmdCDWriteLine(string path, string str) {
        Debug.Log("cmd:" + path + " " + str);
        string[] subStrs = path.Split(':');
        string panfu = subStrs[0];
        System.Diagnostics.Process p = new System.Diagnostics.Process();
        p.StartInfo.FileName = "cmd.exe";
        p.StartInfo.UseShellExecute = false;    //是否使用操作系统shell启动
        p.StartInfo.RedirectStandardInput = true;//接受来自调用程序的输入信息
        p.StartInfo.RedirectStandardOutput = true;//由调用程序获取输出信息
        p.StartInfo.RedirectStandardError = true;//重定向标准错误输出
        p.StartInfo.CreateNoWindow = true;//不显示程序窗口
        p.Start();//启动程序
        p.StandardInput.WriteLine(panfu + ":");
        p.StandardInput.WriteLine("cd " + path);
        //向cmd窗口发送输入信息
        p.StandardInput.WriteLine(str + "&exit");

        p.StandardInput.AutoFlush = true;
        //p.StandardInput.WriteLine("exit");
        //向标准输入写入要执行的命令。这里使用&是批处理命令的符号，表示前面一个命令不管是否执行成功都执行后面(exit)命令，如果不执行exit命令，后面调用ReadToEnd()方法会假死
        //同类的符号还有&&和||前者表示必须前一个命令执行成功才会执行后面的命令，后者表示必须前一个命令执行失败才会执行后面的命令

        //获取cmd窗口的输出信息
        string output = p.StandardOutput.ReadToEnd();

        p.WaitForExit();//等待程序执行完退出进程
        p.Close();
        Debug.Log(output);
    }

    /// <summary>
    /// 打开文件夹选中文件
    /// </summary>
    /// <param name="path"></param>
    public static void ShowAndSelectFileInExplorer(string path) {
        path = path.Replace('/', '\\');
        string arg = string.Format(@"/select,{0}", path);
        System.Diagnostics.Process.Start("explorer.exe", arg);
    }
}