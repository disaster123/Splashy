﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.InteropServices;
using System.Diagnostics;

namespace SendSplashy
{
  class Program
  {
    class MessageHelper {
    [DllImport("User32.dll")]
private static extern int RegisterWindowMessage(string lpString);

[DllImport("User32.dll", EntryPoint = "FindWindow")]
public static extern Int32 FindWindow(String lpClassName, String lpWindowName);

//For use with WM_COPYDATA and COPYDATASTRUCT
[DllImport("User32.dll", EntryPoint = "SendMessage")]
public static extern int SendMessage(int hWnd, int Msg, int wParam, ref COPYDATASTRUCT lParam);

//For use with WM_COPYDATA and COPYDATASTRUCT
[DllImport("User32.dll", EntryPoint = "PostMessage")]
public static extern int PostMessage(int hWnd, int Msg, int wParam, ref COPYDATASTRUCT lParam);

[DllImport("User32.dll", EntryPoint = "SendMessage")]
public static extern int SendMessage(int hWnd, int Msg, int wParam, int lParam);

[DllImport("User32.dll", EntryPoint = "PostMessage")]
public static extern int PostMessage(int hWnd, int Msg, int wParam, int lParam);

[DllImport("User32.dll", EntryPoint = "SetForegroundWindow")]
public static extern bool SetForegroundWindow(int hWnd);

public const int WM_USER = 0x400;
public const int WM_COPYDATA = 0x4A;

//Used for WM_COPYDATA for string messages
public struct COPYDATASTRUCT
{
public IntPtr dwData;
public int cbData;
[MarshalAs(UnmanagedType.LPStr)]
public string lpData;
}

public bool bringAppToFront(int hWnd)
{
return SetForegroundWindow(hWnd);
}

public int sendWindowsStringMessage(int hWnd, int wParam, string msg)
{
int result = 0;

if (hWnd > 0)
{
byte[] sarr = System.Text.Encoding.Default.GetBytes(msg);
int len = sarr.Length;
COPYDATASTRUCT cds;
cds.dwData = (IntPtr)100;
cds.lpData = msg;
cds.cbData = len + 1;
result = SendMessage(hWnd, WM_COPYDATA, wParam, ref cds);
}

return result;
}

public int sendWindowsMessage(int hWnd, int Msg, int wParam, int lParam)
{
int result = 0;

if (hWnd > 0)
{
result = SendMessage(hWnd, Msg, wParam, lParam);
}

return result;
}

public int getWindowId(string className, string windowName)
{

return FindWindow(className, windowName);

}
  }

    static void Main(string[] args)
    {
      MessageHelper msg = new MessageHelper();
      int result = 0;
      //First param can be null
      int hWnd = msg.getWindowId(null, "Splashy");
      if (hWnd != 0)
      {
        if (args.Length > 0)
        {
          Console.WriteLine("Sending: " + args[0]);
          result = msg.sendWindowsStringMessage(hWnd, 0, args[0]);
        }
        else
        {
          Console.WriteLine("Nothing to send arg is not defined");
        }
      }
      else
      {
        Console.WriteLine("Nothing to send - window not found");
      }
    }
  }
}
