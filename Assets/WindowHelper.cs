using System.Diagnostics;
using System;
using System.Runtime.InteropServices;
using UnityEngine;

/**
* <see cref="https://answers.unity.com/questions/27490/minimizing-and-maximizing-by-script.html"/>
*/
public class WindowHelper
{
    [DllImport("user32.dll")]
    private static extern bool ShowWindow(IntPtr hwnd, int nCmdShow);
    [DllImport("user32.dll")]
    private static extern IntPtr GetActiveWindow();

    public static void minimizeMainWindow()
    {
        ShowWindow(GetActiveWindow(), 2);
    }
}