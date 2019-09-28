using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsTerminalClipboardFixup
{
    static class Program
    {

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            var me = Process.GetCurrentProcess();
            var otherInstances = Process.GetProcessesByName(me.ProcessName).Where(a => a.Id != me.Id).ToList();

            if (otherInstances.Count > 0)
            {
                var res = MessageBox.Show("I'm already started. Do you want me to stop all instances of this program?", "Already running!", MessageBoxButtons.YesNo, MessageBoxIcon.None);
                if (res == DialogResult.Yes)
                {
                    foreach (var instance in otherInstances)
                        instance.Kill();
                }

                return;
            }


            Application.Run(new HiddenForm());
        }
    }
}
