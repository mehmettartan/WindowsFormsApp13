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
    public class BacgroundWorker
    {
        private readonly BackgroundWorker worker;
        private bool running;

        // Her form için özel kontrol işlevlerini temsil eden delegeler
        public Action KonveyorKontrolleri { get; set; }
        public Action UrunAyrKontrolleri { get; set; }
        public Action AnasayfaKontrolleri { get; set; }
        public BacgroundWorker()
        {
            worker = new BackgroundWorker();
            worker.DoWork += KontrolleriYurut;
            running = false;
        }

        public void Baslat()
        {
            if (!running)
            {
                running = true;
                worker.RunWorkerAsync();
            }
        }

        public void Durdur()
        {
            running = false;
        }

        private void KontrolleriYurut(object sender, DoWorkEventArgs e)
        {
            while (running)
            {
                // Her formun kendi özel kontrol işlevlerini çağır
                KonveyorKontrolleri?.Invoke();
                UrunAyrKontrolleri?.Invoke();
                AnasayfaKontrolleri?.Invoke();

                Thread.Sleep(1000); // 1 saniye bekle
            }
        }
    }
}
