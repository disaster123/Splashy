using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Drawing.Drawing2D;
using System.Windows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.InteropServices;
using System.Diagnostics;

namespace Splashy
{
  public partial class Form1 : Form
  {
    FormState formState = new FormState();
    Boolean sendmsgclose = false;
    Boolean firstpaint = true;
    Brush bgbrush;

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

    public Form1()
    {
      this.Paint += new PaintEventHandler(TextureBrush1);
      InitializeComponent();
      formState.Maximize(this);
    }

    //Used for WM_COPYDATA for string messages
    public struct COPYDATASTRUCT
    {
      public IntPtr dwData;
      public int cbData;
      [MarshalAs(UnmanagedType.LPStr)]
      public string lpData;
    }

    [System.Security.Permissions.PermissionSet(System.Security.Permissions.SecurityAction.Demand, Name = "FullTrust")]
    protected override void WndProc(ref Message m)
    {

      // Listen for operating system messages.
      switch (m.Msg)
      {
        case WM_USER:
              //MessageBox.Show("Message recieved: " + m.WParam + " – " + m.LParam);
          break;
        case WM_COPYDATA:
              COPYDATASTRUCT mystr = new COPYDATASTRUCT();
              Type mytype = mystr.GetType();
              mystr = (COPYDATASTRUCT)m.GetLParam(mytype);
              if (mystr.lpData.Equals("exit;"))
              {
                sendmsgclose = true;
                Application.Exit();
              }
              // MessageBox.Show(mystr.lpData);
              Splashtext.Text = mystr.lpData;
              Splashtext.Left = (this.Width / 2) - (Splashtext.Width / 2);
              Splashtext.Top = (this.Height / 2) - (Splashtext.Height / 2);

          break;
      }
      base.WndProc(ref m);
    }


    public void TextureBrush1(object sender,PaintEventArgs e) {
      Graphics g = e.Graphics;
      int width = this.Size.Width; // form width
      int height = this.Size.Height; // form height

      if (firstpaint)
      {
        firstpaint = false;
        String path = System.IO.Path.GetDirectoryName(Application.ExecutablePath);
        Image bgimage = ResizeImage(new Bitmap(path + "\\splashscreen.jpg"), width, height);
        bgbrush = new TextureBrush(bgimage);
      }

        // make rectangle at least the size of the window form
        g.FillRectangle(bgbrush, 0, 0, width, height);
      
    }

    private Bitmap ResizeImage(Bitmap mg, int width, int height)
    {
      Size newSize = new Size(width, height);

      double ratio = 0d;
      double myThumbWidth = 0d;
      double myThumbHeight = 0d;

      int x = 0;
      int y = 0;

      bool TrimHeight = false;
      bool TrimWidth = false;

      Bitmap bp = new Bitmap(newSize.Width, newSize.Height);

      if ((mg.Width / Convert.ToDouble(newSize.Width)) > (mg.Height /
      Convert.ToDouble(newSize.Height)))
      {
        ratio = Convert.ToDouble(mg.Width) / Convert.ToDouble(newSize.Width);
        TrimHeight = true;
      }
      else
      {
        ratio = Convert.ToDouble(mg.Height) / Convert.ToDouble(newSize.Height);
        TrimWidth = true;
      }
      myThumbHeight = Math.Ceiling(mg.Height / ratio);
      myThumbWidth = Math.Ceiling(mg.Width / ratio);

      Size thumbSize = new Size((int)myThumbWidth, (int)myThumbHeight);
      if (TrimHeight)
      {
        bp = new Bitmap(newSize.Width, thumbSize.Height);
        TrimHeight = false;
      }
      if (TrimWidth)
      {
        bp = new Bitmap(thumbSize.Width, newSize.Height);
        TrimWidth = false;
      }
      x = (newSize.Width - thumbSize.Width);
      y = (newSize.Height - thumbSize.Height);
      // Had to add System.Drawing class in front of Graphics ---
      System.Drawing.Graphics g = Graphics.FromImage(bp);
      g.SmoothingMode = SmoothingMode.HighQuality;
      g.InterpolationMode = InterpolationMode.HighQualityBicubic;
      g.PixelOffsetMode = PixelOffsetMode.HighQuality;
      Rectangle rect = new Rectangle(0, 0, thumbSize.Width, thumbSize.Height);
      g.DrawImage(mg, rect, 0, 0, mg.Width, mg.Height, GraphicsUnit.Pixel);
      return bp;
    }

    private void Form1_Load(object sender, EventArgs e)
    {
      Splashtext.Text = "";
      Cursor.Hide();
    }

    private void Form1_FormClosed(object sender, FormClosedEventArgs e)
    {
      // STEFAN normally the taskbar is activated through MP again
      if (!sendmsgclose)
      {
        WinApi.ShowTaskbar();
      }
    }
  }
  /// <summary>
/// Selected Win API Function Calls
/// </summary>

public class WinApi
{
    [DllImport("user32.dll", EntryPoint = "GetSystemMetrics")]
    public static extern int GetSystemMetrics(int which);

    [DllImport("user32.dll")]
    public static extern void
        SetWindowPos(IntPtr hwnd, IntPtr hwndInsertAfter,
                     int X, int Y, int width, int height, uint flags);        

    private const int SM_CXSCREEN = 0;
    private const int SM_CYSCREEN = 1;
    private static IntPtr HWND_TOP = IntPtr.Zero;
    private const int SWP_SHOWWINDOW = 64; // 0×0040

    public static int ScreenX
    {
        get { return GetSystemMetrics(SM_CXSCREEN);}
    }

    public static int ScreenY
    {
        get { return GetSystemMetrics(SM_CYSCREEN);}
    }

    private const int SW_HIDE = 0;
    private const int SW_SHOW = 1;

    [DllImport("user32.dll")]
    private static extern int FindWindow(string className, string windowText);
    [DllImport("user32.dll")]
    private static extern int ShowWindow(int hwnd, int command);

    public static void SetWinFullScreen(IntPtr hwnd)
    {
        SetWindowPos(hwnd, HWND_TOP, 0, 0, ScreenX, ScreenY, SWP_SHOWWINDOW);
        int hWnd = FindWindow("Shell_TrayWnd", "");
        ShowWindow(hWnd, SW_HIDE);
    }
    public static void ShowTaskbar()
    {
      int hWnd = FindWindow("Shell_TrayWnd", "");
      ShowWindow(hWnd, SW_SHOW);
    }
}

/// <summary>
/// Class used to preserve / restore / maximize state of the form
/// </summary>
public class FormState
{
    private FormWindowState winState;
    private FormBorderStyle brdStyle;
    private bool topMost;
    private Rectangle bounds;

    private bool IsMaximized = false;

    public void Maximize(Form targetForm)
    {
        if (!IsMaximized)
        {
            IsMaximized = true;
            Save(targetForm);
            targetForm.WindowState = FormWindowState.Maximized;
            targetForm.FormBorderStyle = FormBorderStyle.None;
            targetForm.TopMost = true;
            WinApi.SetWinFullScreen(targetForm.Handle);
            targetForm.TopMost = true;
        }
    }

    public void Save(Form targetForm)
    {
        winState = targetForm.WindowState;
        brdStyle = targetForm.FormBorderStyle;
        topMost = targetForm.TopMost;
        bounds = targetForm.Bounds;
    }

    public void Restore(Form targetForm)
    {
        targetForm.WindowState = winState;
        targetForm.FormBorderStyle = brdStyle;
        targetForm.TopMost = topMost;
        targetForm.Bounds = bounds;
        IsMaximized = false;
    }
}
}

