using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using System.Diagnostics;
using System.IO;

namespace Sims2RPCUpdater
{
    public partial class Form1 : Form
    {
        bool manual = true;
        void Update1()
        {
            Update1(false);
        }
        void Update1(bool force)
        {
            string exs = "";
            var args = Environment.GetCommandLineArgs();
            foreach (var element in args)
            {
                if (element == "-silent")
                    manual = false;
            }
            Program.UpdateResult res;
            if (!manual)
            {
                try
                {
                    res = Program.Update(manual, ref exs);
                }
                catch(Exception e)
                {
                    exs += e.ToString() + Environment.NewLine;
                }
                if (exs.Length > 0)
                    File.WriteAllText("update.log", exs);
                ProcessStartInfo startInfo = new ProcessStartInfo();
                startInfo.FileName = Path.Combine(Directory.GetCurrentDirectory(),"Sims2RPC.exe");
                startInfo.Arguments = Environment.CommandLine+" -updater -noupdate";
                startInfo.WorkingDirectory = Path.GetDirectoryName(Directory.GetCurrentDirectory());
                Process.Start(startInfo);
                End();
                return;
            }
            try
            {
                res = Program.Update(!force,ref exs);
                if (exs.Length > 0)
                    File.WriteAllText("update.log", exs);
            }
            catch (Exception e)
            {
                MessageBox.Show("There was an error updating Sims2RPC: " + Environment.NewLine + e.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                End();
                return;
            }
            switch (res)
            {
                case Program.UpdateResult.ALREADY:
                    var rep = MessageBox.Show("Sims2RPC is already up to date. Repair?", "Info", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
                    if (rep == DialogResult.Yes)
                    {
                        Update1(true);
                    }
                    else
                    {
                        End();
                    }
                    return;
                case Program.UpdateResult.SUCCESS:
                    MessageBox.Show("Sims2RPC was succesfully updated.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    End();
                    return;
                case Program.UpdateResult.FAILED:
                    MessageBox.Show("Sims2RPC failed to update.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    End();
                    return;
                case Program.UpdateResult.PROBLEMS:
                    MessageBox.Show("Sims2RPC updated, but there were some issues.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    End();
                    return;
            }
        }
        public Form1()
        {
            InitializeComponent();
            ThreadStart thr = Update1;
            var upThread = new Thread(thr);
            upThread.Start();
            
        }
        void End()
        {
            Environment.Exit(0);
        }
        private void label1_Click(object sender, EventArgs e)
        {

        }
    }
}
