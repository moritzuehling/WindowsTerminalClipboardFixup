using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsTerminalClipboardFixup
{
    public partial class HiddenForm : Form
    {
        const int MIN_WHITESPACE = 30;
        const int MAX_LINE_LENGTH = 300;

        [DllImport("User32.dll")]
        public static extern IntPtr SetClipboardViewer(IntPtr hWndNewViewer);
        private const int WM_DRAWCLIPBOARD = 0x0308;

        [DllImport("User32.dll", CharSet = CharSet.Auto)]
        public static extern bool ChangeClipboardChain(IntPtr hWndRemove, IntPtr hWndNewNext);

        [DllImport("user32.dll")]
        static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll", SetLastError = true)]
        static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint processId);

        string LastClipboardText = null;
        public HiddenForm()
        {
            InitializeComponent();
            
            Hide();
            Visible = false;
            ShowInTaskbar = false;

            var nextClipboardViewer = SetClipboardViewer(this.Handle);
            FormClosing += (sender, ev) => ChangeClipboardChain(this.Handle, nextClipboardViewer);
        }

        protected override void SetVisibleCore(bool value)
        {
            base.SetVisibleCore(false);
        }

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == WM_DRAWCLIPBOARD)
            {
                try
                {
                    MonitorClipboard();
                }
                catch (Exception e)
                {
                    // We're not letting the application crash here, and we *especially* make sure
                    // that we don't crash in WndProc, lol.
                    Console.Error.WriteLine("We did an ooopsie when processing a WM_DRAWCLIPBOARD: " + e);
                }
            }

            base.WndProc(ref m);
        }

        private void MonitorClipboard()
        {
            IDataObject iData = Clipboard.GetDataObject();
            if (!iData.GetDataPresent(DataFormats.Text))
                return;

            var text = (string)iData.GetData(DataFormats.Text);

            if (text == LastClipboardText)
            {
                Debug.WriteLine("Rejecting our own clipboard update.");
                return;
            }

            var activeWindow = GetForegroundWindow();
            if (activeWindow == IntPtr.Zero)
                return;

            var threadId = GetWindowThreadProcessId(activeWindow, out var pid);
            var proc = Process.GetProcessById((int)pid);

            // not the droid we're looking for.
            if (proc.ProcessName != "WindowsTerminal")
                return;

            var resLines = new List<string>();

            var origText = text;
            var (estimatedLength, offset) = EstimateLineLength(text);
            if (estimatedLength < text.Length && offset > 0)
            {
                var firstLine = text.Substring(0, offset).TrimEnd();
                resLines.Add(firstLine);
                text = text.Substring(offset);
            }

            var lines = text.Replace("\r\n", "\n").Split('\n');

            for (var i = 0; i < lines.Length; i++)
            {
                var line = lines[i].TrimEnd();

                for (var j = 0; j < line.Length; j += estimatedLength)
                {
                    var toAdd = line.Substring(j, Math.Min(estimatedLength, line.Length - j));
                    toAdd = toAdd.TrimEnd();
                    resLines.Add(toAdd);
                }
            }

            var resText = string.Join(Environment.NewLine, resLines);

            if (resText != origText)
            {
                // We should overwrite
                
                Debug.WriteLine("Changing clipboard from" + origText.Length + " to " + resText.Length);
                LastClipboardText = resText;
                Clipboard.SetText(resText);
            }
            
        }

        private (int, int) EstimateLineLength(string text)
        {
            var lineLenghts = new List<int>();

            while (text.Length > 0)
            {
                bool found = false;
                for (var i = 0; i < text.Length; i++)
                {
                    if (text[i] == ' ')
                    {
                        var chainLength = CountChainLength(text, i);
                        if (chainLength > MIN_WHITESPACE)
                        {
                            lineLenghts.Add(i + chainLength);
                            text = text.Substring(i + chainLength);
                            found = true;
                            break;
                        }
                    }
                    if (text[i] == '\n')
                    {
                        lineLenghts.Add(i);
                        text = text.Substring(i + 1);
                        found = true;
                        break;
                    }
                }

                if (!found)
                {
                    lineLenghts.Add(text.Length);
                    break;
                }
            }

            var majority = lineLenghts.GroupBy(a => a).OrderByDescending(a => a.Count()).First().Key;

            return (majority, lineLenghts[0] % majority);
        }

        private int CountChainLength(string text, int startIndex)
        {
            for (var i = startIndex + 1; i < text.Length; i++)
            {
                if (text[i] != ' ')
                    return i - startIndex;
            }

            return text.Length - startIndex - 1;
        }
    }
}
