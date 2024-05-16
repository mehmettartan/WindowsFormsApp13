using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Threading;
using System.Windows.Forms;

namespace WindowsFormsApp13
{
    public static class BackgroundWorkerHelper
    {
        private static BackgroundWorker arkaPlanWorker;
        private static bool running = true;

        public static void StartBackgroundWorker(Form form)
        {
            arkaPlanWorker = new BackgroundWorker();
            arkaPlanWorker.DoWork += (sender, e) => ArkaPlanKontrolLoop(sender, e, form);
            arkaPlanWorker.RunWorkerAsync();
        }

        private static void ArkaPlanKontrolLoop(object sender, DoWorkEventArgs e, Form form)
        {
            SynchronizationContext synchronizationContext = SynchronizationContext.Current;

            while (running)
            {
                for (int i = 1; i < 9; i++)
                {
                    int istasyonNo = i;

                    if (!form.Disposing && !form.IsDisposed)
                    {

                    }

                    Thread.Sleep(100); // 1 saniye bekle
                }
            }
        }
    }
}