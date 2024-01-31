using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

public class PlaceholderTextBox : TextBox
{
    private const uint ECM_FIRST = 0x1500;
    private const uint EM_SETCUEBANNER = ECM_FIRST + 1;

    [DllImport("user32.dll", CharSet = CharSet.Unicode)]
    private static extern IntPtr SendMessage(IntPtr hWnd, UInt32 Msg, IntPtr wParam, string lParam);

    private string placeholderText;

    public string PlaceholderText
    {
        get { return placeholderText; }
        set
        {
            placeholderText = value;
            SendMessage(this.Handle, EM_SETCUEBANNER, (IntPtr)1, placeholderText);
        }
    }
}
