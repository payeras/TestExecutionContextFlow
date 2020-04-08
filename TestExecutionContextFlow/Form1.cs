using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TestExecutionContextFlow
{
    public partial class Form1 : Form
    {
        private readonly SynchronizationContext synchronizationContext;
        public Form1()
        {
            InitializeComponent();
            synchronizationContext = SynchronizationContext.Current;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Thread.CurrentPrincipal = new GenericPrincipal(new GenericIdentity("User1"), new string[] { });
            //AppDomain.CurrentDomain.SetThreadPrincipal(Thread.CurrentPrincipal);
            textBox1.Text += $"Logged User: {Thread.CurrentPrincipal.Identity.Name}" + Environment.NewLine;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            //cancelThread.Abort();
            //textBox1.Text += $"abort Thread" + Environment.NewLine;
            //Application.DoEvents();

            Thread.CurrentPrincipal = new GenericPrincipal(new GenericIdentity("User2"), new string[] { });

            textBox1.Text += $"Logged User: {Thread.CurrentPrincipal.Identity.Name}" + Environment.NewLine;
            
        }

        private async void button3_Click(object sender, EventArgs e)
        {
            textBox1.Text += "Started long tasks" + Environment.NewLine;
            try
            {
                await longTask();
            }
            catch
            { }
            textBox1.Text += $"Logged User after long task: {Thread.CurrentPrincipal.Identity.Name}" + Environment.NewLine;
        }

        Task longtask;
        Thread cancelThread;
        public  async Task longTask()
        {

            longtask= Task.Run(() =>
            {
                cancelThread = Thread.CurrentThread;
                synchronizationContext.Post(new SendOrPostCallback(o =>
                {
                    textBox1.Text += $"Logged User in Thread: {o}" + Environment.NewLine;
 
                }), Thread.CurrentPrincipal.Identity.Name);

                
                Thread.Sleep(20000);
                
            });

            await longtask;
        }
    }
}
