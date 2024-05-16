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

namespace WindowsFormsApp13

{
    public partial class KonveyorSistem : Form
    {

        string CalısmaMod;

        private BacgroundWorker bacgroundWorker;

        private static KonveyorSistem konvSistmInstance;

        Thread threadRead, threadWriteBool, threadReadString, threadWriteString;
        bool[] station1 = new bool[10];

        bool boolReadStatus;

        string[] station1Manuel = new string[9];
        string[] station2Manuel = new string[9];
        string[] station3Manuel = new string[9];
        string[] station4Manuel = new string[9];
        string[] station5Manuel = new string[9];
        string[] station6Manuel = new string[9];
        string[] station7Manuel = new string[9];
        string[] station8Manuel = new string[9];

        string[] station1Donanim = new string[9];
        string[] station2Donanim = new string[9];
        string[] station3Donanim = new string[9];
        string[] station4Donanim = new string[9];
        string[] station5Donanim = new string[9];
        string[] station6Donanim = new string[9];
        string[] station7Donanim = new string[9];
        string[] station8Donanim = new string[9];

        private System.Windows.Forms.Timer timerControl;

        private bool isValueChanged = false;
        bool calModValue = false;

        string setDeger = "25";

        public KonveyorSistem()
        {
            InitializeComponent();
            // Arka plan işçisini başlat

            bacgroundWorker = new BacgroundWorker();

            // KonveyorKontrolleri delegesine özel kontrol işlevini atayın
            bacgroundWorker.KonveyorKontrolleri = KonveyorKontrolleriMetodu;

        }

        public static KonveyorSistem GetInstance()
        {
            // Eğer form henüz oluşturulmadıysa veya kapatıldıysa, yeni bir örneği oluşturun
            if (konvSistmInstance == null || konvSistmInstance.IsDisposed)
            {
                konvSistmInstance = new KonveyorSistem();
            }
            return konvSistmInstance;
        }

        // Özel kontrol işlevi
        private void KonveyorKontrolleriMetodu()
        {
            // Burada konveyor kontrollerini gerçekleştirin
            for (int i = 1; i < 9; i++)
            {
                int istasyonNo = i;

                // Form oluşturulduğunda ve aktif olduğunda
                if (this.IsHandleCreated && !this.IsDisposed)
                {
                    this.Invoke((MethodInvoker)delegate
                    {
                        ManuelKontrol();
                        CheckArizaDurumu(i);
                        CheckKonveyorAktarmaDurum();
                        BariyerSensor();
                        FiksturSensor(i);
                        KonveyorSensCikis(i);
                        KonveyorSensGiris(i);
                        StoperDurum();
                        MerkezLDurum();
                        FirinDurum();
                        FanDurum();


                        // urunAyar nesnesi null değilse ve dispose edilmemişse ReadSensor'ı çağır
                        /*if (urunAyar != null && !urunAyar.IsDisposed)
                        {
                            urunAyar.ReadSensor();
                        }*/

                        // Diğer metodları da aynı şekilde kontrol edin
                    });
                }

                Thread.Sleep(100); // 1 saniye bekle
            }
        }

        private void KonveyorSistem_Load(object sender, EventArgs e)
        {
            bacgroundWorker.Baslat();

            string modSec = CallModHelper.degerKaydet();

            switch (modSec)
            {
                case "1":
                    buttonCalMod.Text = "OTOMATİK ÇALIŞMA";
                    buttonCalMod.BackColor = Color.DarkOliveGreen;
                    break;
                case "0":
                    buttonCalMod.Text = "MANUEL ÇALIŞMA";
                    buttonCalMod.BackColor = Color.Snow;
                    break;
            }

             CalısmaMod = "CalismaMod";
             nxCompoletStringWrite(CalısmaMod, modSec);

             SetDefaultValues();


             // Arka plan iş parçacığı oluştur ve başlat
             Thread backgroundThread = new Thread(() =>
             {
                 // PLC den bağlantı OK i bekle
                 while (!nxCompoletBoolRead("ConnectionOk"))
                 {
                     // Bekleme süresi
                     Thread.Sleep(100);
                 }

                 //MessageBox.Show("Geldi");

                 // Bağlantı sağlandıktan sonra gerekli işlemleri yap
                 this.Invoke((MethodInvoker)delegate
                 {

                     // SetDefaultValues ve diğer işlemler buraya gelir
                     SendValueToPLC(setDeger, 1);
                     SendValueToPLC(setDeger, 2);
                     SendValueToPLC(setDeger, 3);
                     SendValueToPLC(setDeger, 4);
                     SendValueToPLC(setDeger, 5);
                     SendValueToPLC(setDeger, 6);
                     SendValueToPLC(setDeger, 7);
                     SendValueToPLC(setDeger, 8);

                 });
             });

             backgroundThread.IsBackground = true;
             backgroundThread.Start();

        }

        private void SetDefaultValues()
        {
            for (int i = 1; i <= 8; i++)
            {
                string textBoxName = $"textBoxIst{i}Hız";
                Control[] controls = this.Controls.Find(textBoxName, true);

                if (controls.Length > 0 && controls[0] is TextBox)
                {
                    TextBox textBox = (TextBox)controls[0];
                    textBox.Text = setDeger;
                }
            }
        }

        public void ManuelKontrol()
        {
            string modSec = CallModHelper.degerKaydet();

            switch (modSec)
            {
                case "1":
                    buttonCalMod.Text = "OTOMATİK ÇALIŞMA";
                    buttonCalMod.BackColor = Color.DarkOliveGreen;
                    break;
                case "0":
                    buttonCalMod.Text = "MANUEL ÇALIŞMA";
                    buttonCalMod.BackColor = Color.Snow;
                    break;
            }
        }

        public void CheckArizaDurumu(int istasyonNo)
        {
            string donanimVariable = $"Istasyon{istasyonNo}Donanim[1]"; // İstasyon donanim değişkeni
            string arizaDurumu = nxCompoletStringRead(donanimVariable); // Donanım değişkenini oku

            if (arizaDurumu == "1")
            {
                switch (istasyonNo)
                {
                    case 1:
                        buttonIst1Arıza.BackColor = Color.LightGreen;
                        break;
                    case 2:
                        buttonIst2Arıza.BackColor = Color.LightGreen;
                        break;
                    case 3:
                        buttonIst3Arıza.BackColor = Color.LightGreen;
                        break;
                    case 4:
                        buttonIst4Arıza.BackColor = Color.LightGreen;
                        break;
                    case 5:
                        buttonIst5Arıza.BackColor = Color.LightGreen;
                        break;
                    case 6:
                        buttonIst6Arıza.BackColor = Color.LightGreen;
                        break;
                    case 7:
                        buttonIst7Arıza.BackColor = Color.LightGreen;
                        break;
                    case 8:
                        buttonIst8Arıza.BackColor = Color.LightGreen;
                        break;
                }

            }

            else if (arizaDurumu == "0")
            {
                switch (istasyonNo)
                {
                    case 1:
                        buttonIst1Arıza.BackColor = Color.LightCoral;
                        break;
                    case 2:
                        buttonIst2Arıza.BackColor = Color.LightCoral;
                        break;
                    case 3:
                        buttonIst3Arıza.BackColor = Color.LightCoral;
                        break;
                    case 4:
                        buttonIst4Arıza.BackColor = Color.LightCoral;
                        break;
                    case 5:
                        buttonIst5Arıza.BackColor = Color.LightCoral;
                        break;
                    case 6:
                        buttonIst6Arıza.BackColor = Color.LightCoral;
                        break;
                    case 7:
                        buttonIst7Arıza.BackColor = Color.LightCoral;
                        break;
                    case 8:
                        buttonIst8Arıza.BackColor = Color.LightCoral;
                        break;
                }
            }
        }

        public void CheckKonveyorAktarmaDurum()
        {
            string konveyordurum1 = nxCompoletStringRead("Istasyon1Donanim[2]");
            string konveyorDurum5 = nxCompoletStringRead("Istasyon5Donanim[2]");

            switch (konveyordurum1)
            {
                case "0":
                    buttonIst1KonvAktarmaGeri .BackColor = Color.LightCoral;
                    buttonIst1KonvAktarmaGeri.Text = "AKTARMA GERİDE";
                    break;
                case "1":
                    buttonIst1KonvAktarmaGeri.BackColor = Color.LightGreen;
                    buttonIst1KonvAktarmaGeri.Text = "AKTARMA İLERİDE";
                    break;
                case "2":
                    buttonIst1KonvAktarmaGeri.BackColor = Color.LightYellow;
                    buttonIst1KonvAktarmaGeri.Text = "AKTARMA YARIDA";
                    break;
            }

            switch (konveyorDurum5)
            {
                case "0":
                    buttonIst5KonvAktarmaGeri.BackColor = Color.LightCoral;
                    buttonIst5KonvAktarmaGeri.Text = "AKTARMA GERİDE";
                    break;
                case "1":
                    buttonIst5KonvAktarmaGeri.BackColor = Color.LightGreen;
                    buttonIst5KonvAktarmaGeri.Text = "AKTARMA İLERİDE";
                    break;
                case "2":
                    buttonIst5KonvAktarmaGeri.BackColor = Color.LightYellow;
                    buttonIst5KonvAktarmaGeri.Text = "AKTARMA YARIDA";
                    break;
            }
        }

        public void BariyerSensor()
        {
            // PLC'den gelen arıza durumunu okuyun (0 veya 1 olarak varsayalım)
            string bariyerSensor = nxCompoletStringRead("Istasyon1Donanim[3]");

            // Arıza durumu 1 ise (Arıza varsa)
            if (bariyerSensor == "1")
            {
                // Kontrolün arka plan rengini kırmızı yapın
                buttonIst1Baryer.BackColor = Color.LightGreen;
            }
            // Arıza durumu 0 ise (Arıza yoksa)
            else if (bariyerSensor == "0")
            {
                buttonIst1Baryer.BackColor = Color.LightCoral;
            }
        }

        public void FiksturSensor(int istasyonNo)
        {
            string donanimVariable = $"Istasyon{istasyonNo}Donanim[4]"; // İstasyon donanim değişkeni
            string fisturSens = nxCompoletStringRead(donanimVariable); // Donanım değişkenini oku

            if (fisturSens == "1")
            {
                switch (istasyonNo)
                {
                    case 1:
                        buttonIst1Fikstur.BackColor = Color.LightGreen;
                        break;
                    case 2:
                        buttonIst2Fikstur.BackColor = Color.LightGreen;
                        break;
                    case 3:
                        buttonIst3Fikstur.BackColor = Color.LightGreen;
                        break;
                    case 4:
                        buttonIst4Fikstur.BackColor = Color.LightGreen;
                        break;
                    case 5:
                        buttonIst5Fikstur.BackColor = Color.LightGreen;
                        break;
                    case 6:
                        buttonIst6Fikstur.BackColor = Color.LightGreen;
                        break;
                    case 7:
                        buttonIst7Fikstur.BackColor = Color.LightGreen;
                        break;
                    case 8:
                        buttonIst8Fikstur.BackColor = Color.LightGreen;
                        break;
                }

            }

            else if (fisturSens == "0")
            {
                switch (istasyonNo)
                {
                    case 1:
                        buttonIst1Fikstur.BackColor = Color.LightCoral;
                        break;
                    case 2:
                        buttonIst2Fikstur.BackColor = Color.LightCoral;
                        break;
                    case 3:
                        buttonIst3Fikstur.BackColor = Color.LightCoral;
                        break;
                    case 4:
                        buttonIst4Fikstur.BackColor = Color.LightCoral;
                        break;
                    case 5:
                        buttonIst5Fikstur.BackColor = Color.LightCoral;
                        break;
                    case 6:
                        buttonIst6Fikstur.BackColor = Color.LightCoral;
                        break;
                    case 7:
                        buttonIst7Fikstur.BackColor = Color.LightCoral;
                        break;
                    case 8:
                        buttonIst8Fikstur.BackColor = Color.LightCoral;
                        break;
                }
            }
        }

        public void KonveyorSensCikis(int istasyonNo)
        {
            string donanimVariable = $"Istasyon{istasyonNo}Donanim[5]"; // İstasyon donanim değişkeni
            string konvSensCıks = nxCompoletStringRead(donanimVariable); // Donanım değişkenini oku

            if (konvSensCıks == "1")
            {
                switch (istasyonNo)
                {
                    case 1:
                        buttonIst1KonvCıkıs.BackColor = Color.LightGreen;
                        break;
                    case 5:
                        buttonIst5KonvCıkıs.BackColor = Color.LightGreen;
                        break;
                }

            }

            else if (konvSensCıks == "0")
            {
                switch (istasyonNo)
                {
                    case 1:
                        buttonIst1KonvCıkıs.BackColor = Color.LightCoral;
                        break;
                    case 5:
                        buttonIst5KonvCıkıs.BackColor = Color.LightCoral;
                        break;
                }
            }
        }

        public void KonveyorSensGiris(int istasyonNo)
        {
            string donanimVariableGiris = $"Istasyon{istasyonNo}Donanim[8]"; // İstasyon donanim değişkeni
            string konvSensGiris = nxCompoletStringRead(donanimVariableGiris); // Donanım değişkenini oku

            if (konvSensGiris == "1")
            {
                switch (istasyonNo)
                {
                    case 1:
                        buttonIst1KonvGiris.BackColor = Color.LightGreen;
                        break;
                    case 5:
                        buttonIst5KonvGiris.BackColor = Color.LightGreen;
                        break;
                }

            }

            else if (konvSensGiris == "0")
            {
                switch (istasyonNo)
                {
                    case 1:
                        buttonIst1KonvGiris.BackColor = Color.LightCoral;
                        break;
                    case 5:
                        buttonIst5KonvGiris.BackColor = Color.LightCoral;
                        break;

                }
            }
        }

        public void StoperDurum()
        {
            string stoperdurum2 = nxCompoletStringRead("Istasyon2Donanim[2]");
            string stoperdurum3 = nxCompoletStringRead("Istasyon3Donanim[2]");
            string stoperdurum4 = nxCompoletStringRead("Istasyon4Donanim[2]");
            string stoperdurum6 = nxCompoletStringRead("Istasyon6Donanim[2]");
            string stoperdurum7 = nxCompoletStringRead("Istasyon7Donanim[2]");
            string stoperdurum8 = nxCompoletStringRead("Istasyon8Donanim[2]");

            switch (stoperdurum2)
            {
                case "0":
                    buttonIst2Stoper.BackColor = Color.LightCoral;
                    buttonIst2Stoper.Text = "STOPER AŞAĞIDA";
                    break;
                case "1":
                    buttonIst2Stoper.BackColor = Color.LightGreen;
                    buttonIst2Stoper.Text = "STOPER YUKARIDA";
                    break;
                case "2":
                    buttonIst2Stoper.BackColor = Color.LightYellow;
                    buttonIst2Stoper.Text = "STOPER YARIDA";
                    break;
            }

            switch (stoperdurum3)
            {
                case "0":
                    buttonIst3Stoper.BackColor = Color.LightCoral;
                    buttonIst3Stoper.Text = "STOPER AŞAĞIDA";
                    break;
                case "1":
                    buttonIst3Stoper.BackColor = Color.LightGreen;
                    buttonIst3Stoper.Text = "STOPER YUKARIDA";
                    break;
                case "2":
                    buttonIst3Stoper.BackColor = Color.LightYellow;
                    buttonIst3Stoper.Text = "STOPER YARIDA";
                    break;
            }

            switch (stoperdurum4)
            {
                case "0":
                    buttonIst4Stoper.BackColor = Color.LightCoral;
                    buttonIst4Stoper.Text = "STOPER AŞAĞIDA";
                    break;
                case "1":
                    buttonIst4Stoper.BackColor = Color.LightGreen;
                    buttonIst4Stoper.Text = "STOPER YUKARIDA";
                    break;
                case "2":
                    buttonIst4Stoper.BackColor = Color.LightYellow;
                    buttonIst4Stoper.Text = "STOPER YARIDA";
                    break;
            }

            switch (stoperdurum6)
            {
                case "0":
                    buttonIst6Stoper.BackColor = Color.LightCoral;
                    buttonIst6Stoper.Text = "STOPER AŞAĞIDA";
                    break;
                case "1":
                    buttonIst6Stoper.BackColor = Color.LightGreen;
                    buttonIst6Stoper.Text = "STOPER YUKARIDA";
                    break;
                case "2":
                    buttonIst6Stoper.BackColor = Color.LightYellow;
                    buttonIst6Stoper.Text = "STOPER YARIDA";
                    break;
            }

            switch (stoperdurum7)
            {
                case "0":
                    buttonIst7Stoper.BackColor = Color.LightCoral;
                    buttonIst7Stoper.Text = "STOPER AŞAĞIDA";
                    break;
                case "1":
                    buttonIst7Stoper.BackColor = Color.LightGreen;
                    buttonIst7Stoper.Text = "STOPER YUKARIDA";
                    break;
                case "2":
                    buttonIst7Stoper.BackColor = Color.LightYellow;
                    buttonIst7Stoper.Text = "STOPER YARIDA";
                    break;
            }

            switch (stoperdurum8)
            {
                case "0":
                    buttonIst8Stoper.BackColor = Color.LightCoral;
                    buttonIst8Stoper.Text = "STOPER AŞAĞIDA";
                    break;
                case "1":
                    buttonIst8Stoper.BackColor = Color.LightGreen;
                    buttonIst8Stoper.Text = "STOPER YUKARIDA";
                    break;
                case "2":
                    buttonIst8Stoper.BackColor = Color.LightYellow;
                    buttonIst8Stoper.Text = "STOPER YARIDA";
                    break;
            }
        }

        public void MerkezLDurum()
        {
            string merkezLDurum2 = nxCompoletStringRead("Istasyon2Donanim[3]");
            string merkezLDurum7 = nxCompoletStringRead("Istasyon7Donanim[3]");

            switch (merkezLDurum2)
            {
                case "0":
                    buttonIst2MerkezL.BackColor = Color.LightCoral;
                    buttonIst2MerkezL.Text = "MERKZ. AŞAĞIDA";
                    break;
                case "1":
                    buttonIst2MerkezL.BackColor = Color.LightGreen;
                    buttonIst2MerkezL.Text = "MERKZ. YUKARIDA";
                    break;
                case "2":
                    buttonIst2MerkezL.BackColor = Color.LightYellow;
                    buttonIst2MerkezL.Text = "MERKZ. YARIDA";
                    break;
            }

            switch (merkezLDurum7)
            {
                case "0":
                    buttonIst7MerkezL.BackColor = Color.LightCoral;
                    buttonIst7MerkezL.Text = "MERKEZL AŞAĞIDA";
                    break;
                case "1":
                    buttonIst7MerkezL.BackColor = Color.LightGreen;
                    buttonIst7MerkezL.Text = "MERKEZL YUKARIDA";
                    break;
                case "2":
                    buttonIst7MerkezL.BackColor = Color.LightYellow;
                    buttonIst7MerkezL.Text = "MERKEZL YARIDA";
                    break;
            }
        }

        public void FirinDurum()
        {
            string firinDurum3 = nxCompoletStringRead("Istasyon3Donanim[6]");
            string firinDurum4 = nxCompoletStringRead("Istasyon4Donanim[6]");
            string firinDurum5 = nxCompoletStringRead("Istasyon5Donanim[6]");
            string firinDurum6 = nxCompoletStringRead("Istasyon6Donanim[6]");

            if (firinDurum3 == "1")
            {
                // Kontrolün arka plan rengini kırmızı yapın
                buttonIst3Fırın.BackColor = Color.DarkOliveGreen;
            }
            // Arıza durumu 0 ise (Arıza yoksa)
            else if (firinDurum3 == "0")
            {
                buttonIst3Fırın.BackColor = Color.FromArgb(31, 31, 31);
            }

            if (firinDurum4 == "1")
            {
                // Kontrolün arka plan rengini kırmızı yapın
                buttonIst4Fırın.BackColor = Color.DarkOliveGreen;
            }
            // Arıza durumu 0 ise (Arıza yoksa)
            else if (firinDurum4 == "0")
            {
                buttonIst4Fırın.BackColor = Color.FromArgb(31, 31, 31);
            }

            if (firinDurum5 == "1")
            {
                // Kontrolün arka plan rengini kırmızı yapın
                buttonIst5Fırın.BackColor = Color.DarkOliveGreen;
            }
            // Arıza durumu 0 ise (Arıza yoksa)
            else if (firinDurum5 == "0")
            {
                buttonIst5Fırın.BackColor = Color.FromArgb(31, 31, 31);
            }

            if (firinDurum6 == "1")
            {
                // Kontrolün arka plan rengini kırmızı yapın
                buttonIst6Fırın.BackColor = Color.DarkOliveGreen;
            }
            // Arıza durumu 0 ise (Arıza yoksa)
            else if (firinDurum6 == "0")
            {
                buttonIst6Fırın.BackColor = Color.FromArgb(31, 31, 31);
            }
        }


        public void FanDurum()
        {
            string fanDurum3 = nxCompoletStringRead("Istasyon3Donanim[7]");
            string fanDurum4 = nxCompoletStringRead("Istasyon4Donanim[7]");
            string fanDurum5 = nxCompoletStringRead("Istasyon5Donanim[7]");
            string fanDurum6 = nxCompoletStringRead("Istasyon6Donanim[7]");

            if (fanDurum3 == "1")
            {
                // Kontrolün arka plan rengini kırmızı yapın
                buttonIst3Fan.BackColor = Color.DarkOliveGreen;
            }
            // Arıza durumu 0 ise (Arıza yoksa)
            else if (fanDurum3 == "0")
            {
                buttonIst3Fan.BackColor = Color.FromArgb(31,31,31);
            }

            if (fanDurum4 == "1")
            {
                // Kontrolün arka plan rengini kırmızı yapın
                buttonIst4Fan.BackColor = Color.DarkOliveGreen;
            }
            // Arıza durumu 0 ise (Arıza yoksa)
            else if (fanDurum4 == "0")
            {
                buttonIst4Fan.BackColor = Color.FromArgb(31, 31, 31);
            }

            if (fanDurum5 == "1")
            {
                // Kontrolün arka plan rengini kırmızı yapın
                buttonIst5Fan.BackColor = Color.DarkOliveGreen;
            }
            // Arıza durumu 0 ise (Arıza yoksa)
            else if (fanDurum5 == "0")
            {
                buttonIst5Fan.BackColor = Color.FromArgb(31, 31, 31);
            }

            if (fanDurum6 == "1")
            {
                // Kontrolün arka plan rengini kırmızı yapın
                buttonIst6Fan.BackColor = Color.DarkOliveGreen;
            }
            // Arıza durumu 0 ise (Arıza yoksa)
            else if (fanDurum6 == "0")
            {
                buttonIst6Fan.BackColor = Color.FromArgb(31, 31, 31);
            }
        }
        public void buttonIst1KonvMotrLft_MouseDown(object sender, MouseEventArgs e)
        {
            buttonIst1KonvMotrLft.BackColor = Color.DimGray;
            station1Manuel[0] = "Istasyon1Manuel[0]";

            threadWriteBool = new Thread(() => nxCompoletBoolWrite(station1Manuel[0], true));
            threadWriteBool.Start();
        }

        private void buttonIst1KonvMotrLft_MouseUp(object sender, MouseEventArgs e)
        {
            buttonIst1KonvMotrLft.BackColor = Color.FromArgb(31,31,31);

            station1Manuel[0] = "Istasyon1Manuel[0]";

            threadWriteBool = new Thread(() => nxCompoletBoolWrite(station1Manuel[0], false));
            threadWriteBool.Start();
        }

        private void buttonIst1KonvMotrRgt_MouseDown(object sender, MouseEventArgs e)
        {
            buttonIst1KonvMotrRgt.BackColor = Color.DimGray;

            station1Manuel[1] = "Istasyon1Manuel[1]";

            threadWriteBool = new Thread(() => nxCompoletBoolWrite(station1Manuel[1], true));
            threadWriteBool.Start();
        }

        private void buttonIst1KonvMotrRgt_MouseUp(object sender, MouseEventArgs e)
        {
            buttonIst1KonvMotrRgt.BackColor = Color.FromArgb(31, 31, 31);

            station1Manuel[1] = "Istasyon1Manuel[1]";

            threadWriteBool = new Thread(() => nxCompoletBoolWrite(station1Manuel[1], false));
            threadWriteBool.Start();
        }

        private void buttonIst1KonvAktrmaUp_MouseDown(object sender, MouseEventArgs e)
        {
            buttonIst1KonvAktrmaUp.BackColor = Color.DimGray;

            station1Manuel[2] = "Istasyon1Manuel[2]";

            threadWriteBool = new Thread(() => nxCompoletBoolWrite(station1Manuel[2], true));
            threadWriteBool.Start();
        }

        private void buttonIst1KonvAktrmaUp_MouseUp(object sender, MouseEventArgs e)
        {
            buttonIst1KonvAktrmaUp.BackColor = Color.FromArgb(31, 31, 31);

            station1Manuel[2] = "Istasyon1Manuel[2]";

            threadWriteBool = new Thread(() => nxCompoletBoolWrite(station1Manuel[2], false));
            threadWriteBool.Start();
        }

        private void buttonIst1KonvAktrmaDown_MouseDown(object sender, MouseEventArgs e)
        {
            buttonIst1KonvAktrmaDown.BackColor = Color.DimGray;

            station1Manuel[3] = "Istasyon1Manuel[3]";

            threadWriteBool = new Thread(() => nxCompoletBoolWrite(station1Manuel[3], true));
            threadWriteBool.Start();
        }

        private void buttonIst1KonvAktrmaDown_MouseUp(object sender, MouseEventArgs e)
        {
            buttonIst1KonvAktrmaDown.BackColor = Color.FromArgb(31, 31, 31);

            station1Manuel[3] = "Istasyon1Manuel[3]";

            threadWriteBool = new Thread(() => nxCompoletBoolWrite(station1Manuel[3], false));
            threadWriteBool.Start();
        }

        private void buttonIst2KonvMotrLft_MouseDown(object sender, MouseEventArgs e)
        {
            buttonIst2KonvMotrLft.BackColor = Color.DimGray;

            station2Manuel[0] = "Istasyon2Manuel[0]";

            threadWriteBool = new Thread(() => nxCompoletBoolWrite(station2Manuel[0], true));
            threadWriteBool.Start();
        }

        private void buttonIst2KonvMotrLft_MouseUp(object sender, MouseEventArgs e)
        {
            buttonIst2KonvMotrLft.BackColor = Color.FromArgb(31, 31, 31);

            station2Manuel[0] = "Istasyon2Manuel[0]";

            threadWriteBool = new Thread(() => nxCompoletBoolWrite(station2Manuel[0], false));
            threadWriteBool.Start();
        }

        private void buttonIst2KonvMotrRgt_MouseDown(object sender, MouseEventArgs e)
        {
            buttonIst2KonvMotrRgt.BackColor = Color.DimGray;

            station2Manuel[1] = "Istasyon2Manuel[1]";

            threadWriteBool = new Thread(() => nxCompoletBoolWrite(station2Manuel[1], true));
            threadWriteBool.Start();
        }

        private void buttonIst2KonvMotrRgt_MouseUp(object sender, MouseEventArgs e)
        {
            buttonIst2KonvMotrRgt.BackColor = Color.FromArgb(31, 31, 31);

            station2Manuel[1] = "Istasyon2Manuel[1]";

            threadWriteBool = new Thread(() => nxCompoletBoolWrite(station2Manuel[1], false));
            threadWriteBool.Start();
        }

        private void buttonIst2StoperPistUp_Click(object sender, EventArgs e)
        {
            station2Manuel[2] = "Istasyon2Manuel[2]";

            threadWriteBool = new Thread(() => nxCompoletBoolWrite(station2Manuel[2], true));
            threadWriteBool.Start();
        }

        private void buttonIst2StoperPistDown_Click(object sender, EventArgs e)
        {
            station2Manuel[3] = "Istasyon2Manuel[3]";

            threadWriteBool = new Thread(() => nxCompoletBoolWrite(station2Manuel[3], true));
            threadWriteBool.Start();
        }

        private void buttonIst2MekezLUp_Click(object sender, EventArgs e)
        {
            station2Manuel[4] = "Istasyon2Manuel[4]";

            threadWriteBool = new Thread(() => nxCompoletBoolWrite(station2Manuel[4], true));
            threadWriteBool.Start();
        }

        private void buttonIst2MekezLDown_Click(object sender, EventArgs e)
        {
            station2Manuel[5] = "Istasyon2Manuel[5]";

            threadWriteBool = new Thread(() => nxCompoletBoolWrite(station2Manuel[5], true));
            threadWriteBool.Start();
        }

        // İstasyon -3

        private void buttonIst3KonvMotrLft_MouseDown(object sender, MouseEventArgs e)
        {
            buttonIst3KonvMotrLft.BackColor = Color.DimGray;

            station3Manuel[0] = "Istasyon3Manuel[0]";

            threadWriteBool = new Thread(() => nxCompoletBoolWrite(station3Manuel[0], true));
            threadWriteBool.Start();
        }

        private void buttonIst3KonvMotrLft_MouseUp(object sender, MouseEventArgs e)
        {
            buttonIst3KonvMotrLft.BackColor = Color.FromArgb(31, 31, 31);

            station3Manuel[0] = "Istasyon3Manuel[0]";

            threadWriteBool = new Thread(() => nxCompoletBoolWrite(station3Manuel[0], false));
            threadWriteBool.Start();
        }

        private void buttonIst3KonvMotrRgt_MouseDown(object sender, MouseEventArgs e)
        {
            buttonIst3KonvMotrRgt.BackColor = Color.DimGray;

            station3Manuel[1] = "Istasyon3Manuel[1]";

            threadWriteBool = new Thread(() => nxCompoletBoolWrite(station3Manuel[1], true));
            threadWriteBool.Start();
        }

        private void buttonIst3KonvMotrRgt_MouseUp(object sender, MouseEventArgs e)
        {
            buttonIst3KonvMotrRgt.BackColor = Color.FromArgb(31, 31, 31);

            station3Manuel[1] = "Istasyon3Manuel[1]";

            threadWriteBool = new Thread(() => nxCompoletBoolWrite(station3Manuel[1], false));
            threadWriteBool.Start();
        }

        private void buttonIst3StoperPistUp_Click(object sender, EventArgs e)
        {
            station3Manuel[2] = "Istasyon3Manuel[2]";

            threadWriteBool = new Thread(() => nxCompoletBoolWrite(station3Manuel[2], true));
            threadWriteBool.Start();
        }

        private void buttonIst3StoperPistDown_Click(object sender, EventArgs e)
        {
            station3Manuel[3] = "Istasyon3Manuel[3]";

            threadWriteBool = new Thread(() => nxCompoletBoolWrite(station3Manuel[3], true));
            threadWriteBool.Start();
        }

        private void buttonIst3Fırın_Click(object sender, EventArgs e)
        {
            station3Manuel[4] = "Istasyon3Manuel[4]";

            threadWriteBool = new Thread(() => nxCompoletBoolWrite(station3Manuel[4], true));
            threadWriteBool.Start();
        }

        private void buttonIst3Fan_Click(object sender, EventArgs e)
        {
            station3Manuel[5] = "Istasyon3Manuel[5]";

            threadWriteBool = new Thread(() => nxCompoletBoolWrite(station3Manuel[5], true));
            threadWriteBool.Start();
        }

        // İstasyon - 4
        private void buttonIst4KonvMotrLft_MouseDown(object sender, MouseEventArgs e)
        {
            buttonIst4KonvMotrLft.BackColor = Color.DimGray;

            station4Manuel[0] = "Istasyon4Manuel[0]";

            threadWriteBool = new Thread(() => nxCompoletBoolWrite(station4Manuel[0], true));
            threadWriteBool.Start();
        }

        private void buttonIst4KonvMotrLft_MouseUp(object sender, MouseEventArgs e)
        {
            buttonIst4KonvMotrLft.BackColor = Color.FromArgb(31, 31, 31);

            station4Manuel[0] = "Istasyon4Manuel[0]";

            threadWriteBool = new Thread(() => nxCompoletBoolWrite(station4Manuel[0], false));
            threadWriteBool.Start();
        }

        private void buttonIst4KonvMotrRgt_MouseDown(object sender, MouseEventArgs e)
        {
            buttonIst4KonvMotrRgt.BackColor = Color.DimGray;

            station4Manuel[1] = "Istasyon4Manuel[1]";

            threadWriteBool = new Thread(() => nxCompoletBoolWrite(station4Manuel[1], true));
            threadWriteBool.Start();
        }

        private void buttonIst4KonvMotrRgt_MouseUp(object sender, MouseEventArgs e)
        {
            buttonIst4KonvMotrRgt.BackColor = Color.FromArgb(31, 31, 31);

            station4Manuel[1] = "Istasyon4Manuel[1]";

            threadWriteBool = new Thread(() => nxCompoletBoolWrite(station4Manuel[1], false));
            threadWriteBool.Start();
        }

        private void buttonIst4StoperPistUp_Click(object sender, EventArgs e)
        {
            station4Manuel[2] = "Istasyon4Manuel[2]";

            threadWriteBool = new Thread(() => nxCompoletBoolWrite(station4Manuel[2], true));
            threadWriteBool.Start();
        }

        private void buttonIst4StoperPistDown_Click(object sender, EventArgs e)
        {
            station4Manuel[3] = "Istasyon4Manuel[3]";

            threadWriteBool = new Thread(() => nxCompoletBoolWrite(station4Manuel[3], true));
            threadWriteBool.Start();
        }

        private void buttonIst4Fırın_Click(object sender, EventArgs e)
        {
            station4Manuel[4] = "Istasyon4Manuel[4]";

            threadWriteBool = new Thread(() => nxCompoletBoolWrite(station4Manuel[4], true));
            threadWriteBool.Start();
        }

        private void buttonIst4Fan_Click(object sender, EventArgs e)
        {
            station4Manuel[5] = "Istasyon4Manuel[5]";

            threadWriteBool = new Thread(() => nxCompoletBoolWrite(station4Manuel[5], true));
            threadWriteBool.Start();
        }

        // İstasyon - 5
        private void buttonIst5KonvMotrLft_MouseDown(object sender, MouseEventArgs e)
        {
            buttonIst5KonvMotrLft.BackColor = Color.DimGray;

            station5Manuel[0] = "Istasyon5Manuel[0]";

            threadWriteBool = new Thread(() => nxCompoletBoolWrite(station5Manuel[0], true));
            threadWriteBool.Start();
        }

        private void buttonIst5KonvMotrLft_MouseUp(object sender, MouseEventArgs e)
        {
            buttonIst5KonvMotrLft.BackColor = Color.FromArgb(31, 31, 31);

            station5Manuel[0] = "Istasyon5Manuel[0]";

            threadWriteBool = new Thread(() => nxCompoletBoolWrite(station5Manuel[0], false));
            threadWriteBool.Start();
        }

        private void buttonIst5KonvMotrRgt_MouseDown(object sender, MouseEventArgs e)
        {;
            buttonIst5KonvMotrRgt.BackColor = Color.DimGray;

            station5Manuel[1] = "Istasyon5Manuel[1]";

            threadWriteBool = new Thread(() => nxCompoletBoolWrite(station5Manuel[1], true));
            threadWriteBool.Start();
        }

        private void buttonIst5KonvMotrRgt_MouseUp(object sender, MouseEventArgs e)
        {
            buttonIst5KonvMotrRgt.BackColor = Color.FromArgb(31, 31, 31);

            station5Manuel[1] = "Istasyon5Manuel[1]";

            threadWriteBool = new Thread(() => nxCompoletBoolWrite(station5Manuel[1], false));
            threadWriteBool.Start();
        }

        private void buttonIst5KonvAktrmaUp_MouseDown(object sender, MouseEventArgs e)
        {
            buttonIst5KonvAktrmaUp.BackColor = Color.DimGray;

            station5Manuel[2] = "Istasyon5Manuel[2]";

            threadWriteBool = new Thread(() => nxCompoletBoolWrite(station5Manuel[2], true));
            threadWriteBool.Start();
        }

        private void buttonIst5KonvAktrmaUp_MouseUp(object sender, MouseEventArgs e)
        {
            buttonIst5KonvAktrmaUp.BackColor = Color.FromArgb(31, 31, 31);

            station5Manuel[2] = "Istasyon5Manuel[2]";

            threadWriteBool = new Thread(() => nxCompoletBoolWrite(station5Manuel[2], false));
            threadWriteBool.Start();
        }

        private void buttonIst5KonvAktrmaDown_MouseDown(object sender, MouseEventArgs e)
        {
            buttonIst5KonvAktrmaDown.BackColor = Color.DimGray;

            station5Manuel[3] = "Istasyon5Manuel[3]";

            threadWriteBool = new Thread(() => nxCompoletBoolWrite(station5Manuel[3], true));
            threadWriteBool.Start();
        }

        private void buttonIst5KonvAktrmaDown_MouseUp(object sender, MouseEventArgs e)
        {
            buttonIst5KonvAktrmaDown.BackColor = Color.FromArgb(31, 31, 31);

            station5Manuel[3] = "Istasyon5Manuel[3]";

            threadWriteBool = new Thread(() => nxCompoletBoolWrite(station5Manuel[3], false));
            threadWriteBool.Start();
        }

        private void buttonIst5Fırın_Click(object sender, EventArgs e)
        {
            station5Manuel[4] = "Istasyon5Manuel[4]";

            threadWriteBool = new Thread(() => nxCompoletBoolWrite(station5Manuel[4], true));
            threadWriteBool.Start();
        }

        private void buttonIst5Fan_Click(object sender, EventArgs e)
        {
            station5Manuel[5] = "Istasyon5Manuel[5]";

            threadWriteBool = new Thread(() => nxCompoletBoolWrite(station5Manuel[5], true));
            threadWriteBool.Start();
        }

        // İstasyon - 6
        private void buttonIst6KonvMotrLft_MouseDown(object sender, MouseEventArgs e)
        {
            buttonIst6KonvMotrLft.BackColor = Color.DimGray;

            station6Manuel[0] = "Istasyon6Manuel[0]";

            threadWriteBool = new Thread(() => nxCompoletBoolWrite(station6Manuel[0], true));
            threadWriteBool.Start();
        }

        private void buttonIst6KonvMotrLft_MouseUp(object sender, MouseEventArgs e)
        {
            buttonIst6KonvMotrLft.BackColor = Color.FromArgb(31, 31, 31);

            station6Manuel[0] = "Istasyon6Manuel[0]";

            threadWriteBool = new Thread(() => nxCompoletBoolWrite(station6Manuel[0], false));
            threadWriteBool.Start();
        }

        private void buttonIst6KonvMotrRgt_MouseDown(object sender, MouseEventArgs e)
        {
            buttonIst6KonvMotrRgt.BackColor = Color.DimGray;

            station6Manuel[1] = "Istasyon6Manuel[1]";

            threadWriteBool = new Thread(() => nxCompoletBoolWrite(station6Manuel[1], true));
            threadWriteBool.Start();
        }

        private void buttonIst6KonvMotrRgt_MouseUp(object sender, MouseEventArgs e)
        {
            buttonIst6KonvMotrRgt.BackColor = Color.FromArgb(31, 31, 31);

            station6Manuel[1] = "Istasyon6Manuel[1]";

            threadWriteBool = new Thread(() => nxCompoletBoolWrite(station6Manuel[1], false));
            threadWriteBool.Start();
        }

        private void buttonIst6StoperPistUp_Click(object sender, EventArgs e)
        {
            station6Manuel[2] = "Istasyon6Manuel[2]";

            threadWriteBool = new Thread(() => nxCompoletBoolWrite(station6Manuel[2], true));
            threadWriteBool.Start();
        }

        private void buttonIst6StoperPistDown_Click(object sender, EventArgs e)
        {
            station6Manuel[3] = "Istasyon6Manuel[3]";

            threadWriteBool = new Thread(() => nxCompoletBoolWrite(station6Manuel[3], true));
            threadWriteBool.Start();
        }

        private void buttonIst6Fırın_Click(object sender, EventArgs e)
        {
            station6Manuel[4] = "Istasyon6Manuel[4]";

            threadWriteBool = new Thread(() => nxCompoletBoolWrite(station6Manuel[4], true));
            threadWriteBool.Start();
        }

        private void buttonIst6Fan_Click(object sender, EventArgs e)
        {
            station6Manuel[5] = "Istasyon6Manuel[5]";

            threadWriteBool = new Thread(() => nxCompoletBoolWrite(station6Manuel[5], true));
            threadWriteBool.Start();
        }

        // İstasyon - 7
        private void buttonIst7KonvMotrLft_MouseDown(object sender, MouseEventArgs e)
        {
            buttonIst7KonvMotrLft.BackColor = Color.DimGray;

            station7Manuel[0] = "Istasyon7Manuel[0]";

            threadWriteBool = new Thread(() => nxCompoletBoolWrite(station7Manuel[0], true));
            threadWriteBool.Start();
        }

        private void buttonIst7KonvMotrLft_MouseUp(object sender, MouseEventArgs e)
        {
            buttonIst7KonvMotrLft.BackColor = Color.FromArgb(31, 31, 31);

            station7Manuel[0] = "Istasyon7Manuel[0]";

            threadWriteBool = new Thread(() => nxCompoletBoolWrite(station7Manuel[0], false));
            threadWriteBool.Start();
        }

        private void buttonIst7KonvMotrRgt_MouseDown(object sender, MouseEventArgs e)
        {
            buttonIst7KonvMotrRgt.BackColor = Color.DimGray;

            station7Manuel[1] = "Istasyon7Manuel[1]";

            threadWriteBool = new Thread(() => nxCompoletBoolWrite(station7Manuel[1], true));
            threadWriteBool.Start();
        }

        private void buttonIst7KonvMotrRgt_MouseUp(object sender, MouseEventArgs e)
        {
            buttonIst7KonvMotrRgt.BackColor = Color.FromArgb(31, 31, 31);

            station7Manuel[1] = "Istasyon7Manuel[1]";

            threadWriteBool = new Thread(() => nxCompoletBoolWrite(station7Manuel[1], false));
            threadWriteBool.Start();
        }

        private void buttonIst7StoperPistUp_Click(object sender, EventArgs e)
        {
            station7Manuel[2] = "Istasyon7Manuel[2]";

            threadWriteBool = new Thread(() => nxCompoletBoolWrite(station7Manuel[2], true));
            threadWriteBool.Start();
        }

        private void buttonIst7StoperPistDown_Click(object sender, EventArgs e)
        {
            station7Manuel[3] = "Istasyon7Manuel[3]";

            threadWriteBool = new Thread(() => nxCompoletBoolWrite(station7Manuel[3], true));
            threadWriteBool.Start();
        }

        private void buttonIst7MekezLUp_Click(object sender, EventArgs e)
        {
            station7Manuel[4] = "Istasyon7Manuel[4]";

            threadWriteBool = new Thread(() => nxCompoletBoolWrite(station7Manuel[4], true));
            threadWriteBool.Start();
        }

        private void buttonIst7MekezLDown_Click(object sender, EventArgs e)
        {
            station7Manuel[5] = "Istasyon7Manuel[5]";

            threadWriteBool = new Thread(() => nxCompoletBoolWrite(station7Manuel[5], true));
            threadWriteBool.Start();
        }

        // İstasyon - 8
        private void buttonIst8KonvMotrLft_MouseDown(object sender, MouseEventArgs e)
        {
            buttonIst8KonvMotrLft.BackColor = Color.DimGray;

            station8Manuel[0] = "Istasyon8Manuel[0]";

            threadWriteBool = new Thread(() => nxCompoletBoolWrite(station8Manuel[0], true));
            threadWriteBool.Start();
        }

        private void buttonIst8KonvMotrLft_MouseUp(object sender, MouseEventArgs e)
        {
            buttonIst8KonvMotrLft.BackColor = Color.FromArgb(31, 31, 31);

            station8Manuel[0] = "Istasyon8Manuel[0]";

            threadWriteBool = new Thread(() => nxCompoletBoolWrite(station8Manuel[0], false));
            threadWriteBool.Start();
        }

        private void buttonIst8KonvMotrRgt_MouseDown(object sender, MouseEventArgs e)
        {
            buttonIst8KonvMotrRgt.BackColor = Color.DimGray;

            station8Manuel[1] = "Istasyon8Manuel[1]";

            threadWriteBool = new Thread(() => nxCompoletBoolWrite(station8Manuel[1], true));
            threadWriteBool.Start();
        }

        private void buttonIst8KonvMotrRgt_MouseUp(object sender, MouseEventArgs e)
        {
            buttonIst8KonvMotrRgt.BackColor = Color.FromArgb(31, 31, 31);

            station8Manuel[1] = "Istasyon8Manuel[1]";

            threadWriteBool = new Thread(() => nxCompoletBoolWrite(station8Manuel[1], false));
            threadWriteBool.Start();
        }

        private void buttonIst8StoperPistUp_Click(object sender, EventArgs e)
        {
            station8Manuel[2] = "Istasyon8Manuel[2]";

            threadWriteBool = new Thread(() => nxCompoletBoolWrite(station8Manuel[2], true));
            threadWriteBool.Start();
        }

        private void buttonIst8StoperPistDown_Click(object sender, EventArgs e)
        {
            station8Manuel[3] = "Istasyon8Manuel[3]";

            threadWriteBool = new Thread(() => nxCompoletBoolWrite(station8Manuel[3], true));
            threadWriteBool.Start();
        }


        private void SendValueToPLC(string value, int istasyonNo)
        {
            string yeniDeger = value;
            string donanimVariable = $"Istasyon{istasyonNo}Donanim[0]";
            threadWriteString = new Thread(() => nxCompoletStringWrite(donanimVariable, yeniDeger));
            threadWriteString.Start();
        }

        public bool nxCompoletBoolWrite(string variable, bool value)  //NX WRITE
        {
            try
            {
                nxCompolet1.WriteVariable(variable, value);
                return true;
            }
            catch
            {
                return false;
            }
        }

        private void buttonCalMod_Click(object sender, EventArgs e)
        {
            string result = CallModHelper.ToggleCalMod((Button)sender);

            nxCompoletStringWrite(CalısmaMod, result);

        }

        private void textBoxIst1Hız_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Eğer Enter tuşuna basıldıysa ve TextBox'ta değişiklik yapıldıysa
            if (e.KeyChar == (char)Keys.Enter)
            {
                // TextBox nesnesini al
                TextBox textBox = (TextBox)sender;

                // TextBox'ın metni kontrol et
                if (int.TryParse(textBox.Text, out int number))
                {
                    // Girilen değer 1-100 aralığında ise
                    if (number >= 1 && number <= 100)
                    {
                        // TextBox'ın metnini PLC'ye gönder
                        SendValueToPLC(textBox.Text, 1);
                    }
                    else
                    {
                        // Hata mesajı göster
                        MessageBox.Show("Lütfen 1 ile 100 arasında bir sayı girin!", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        textBox.Text = "25";
                        SendValueToPLC("25", 1);
                    }
                }
                else
                {
                    // Hata mesajı göster
                    MessageBox.Show("Lütfen geçerli bir sayı girin!", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    textBox.Text = "25";
                    SendValueToPLC("25", 1);
                }
            }
        }

        private void textBoxIst2Hız_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Eğer Enter tuşuna basıldıysa ve TextBox'ta değişiklik yapıldıysa
            if (e.KeyChar == (char)Keys.Enter)
            {
                // TextBox nesnesini al
                TextBox textBox = (TextBox)sender;

                // TextBox'ın metni kontrol et
                if (int.TryParse(textBox.Text, out int number))
                {
                    // Girilen değer 1-100 aralığında ise
                    if (number >= 1 && number <= 100)
                    {
                        // TextBox'ın metnini PLC'ye gönder
                        SendValueToPLC(textBox.Text, 2);
                    }
                    else
                    {
                        // Hata mesajı göster
                        MessageBox.Show("Lütfen 1 ile 100 arasında bir sayı girin!", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        textBox.Text = "25";
                        SendValueToPLC("25", 2);
                    }
                }
                else
                {
                    // Hata mesajı göster
                    MessageBox.Show("Lütfen geçerli bir sayı girin!", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    textBox.Text = "25";
                    SendValueToPLC("25", 2);
                }
            }
        }

        private void textBoxIst3Hız_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Eğer Enter tuşuna basıldıysa ve TextBox'ta değişiklik yapıldıysa
            if (e.KeyChar == (char)Keys.Enter)
            {
                // TextBox nesnesini al
                TextBox textBox = (TextBox)sender;

                // TextBox'ın metni kontrol et
                if (int.TryParse(textBox.Text, out int number))
                {
                    // Girilen değer 1-100 aralığında ise
                    if (number >= 1 && number <= 100)
                    {
                        // TextBox'ın metnini PLC'ye gönder
                        SendValueToPLC(textBox.Text, 3);
                    }
                    else
                    {
                        // Hata mesajı göster
                        MessageBox.Show("Lütfen 1 ile 100 arasında bir sayı girin!", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        textBox.Text = "25";
                        SendValueToPLC("25", 3);
                    }
                }
                else
                {
                    // Hata mesajı göster
                    MessageBox.Show("Lütfen geçerli bir sayı girin!", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    textBox.Text = "25";
                    SendValueToPLC("25", 3);
                }
            }
        }

        private void textBoxIst4Hız_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Eğer Enter tuşuna basıldıysa ve TextBox'ta değişiklik yapıldıysa
            if (e.KeyChar == (char)Keys.Enter)
            {
                // TextBox nesnesini al
                TextBox textBox = (TextBox)sender;

                // TextBox'ın metni kontrol et
                if (int.TryParse(textBox.Text, out int number))
                {
                    // Girilen değer 1-100 aralığında ise
                    if (number >= 1 && number <= 100)
                    {
                        // TextBox'ın metnini PLC'ye gönder
                        SendValueToPLC(textBox.Text, 4);
                    }
                    else
                    {
                        // Hata mesajı göster
                        MessageBox.Show("Lütfen 1 ile 100 arasında bir sayı girin!", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        textBox.Text = "25";
                        SendValueToPLC("25", 4);
                    }
                }
                else
                {
                    // Hata mesajı göster
                    MessageBox.Show("Lütfen geçerli bir sayı girin!", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    textBox.Text = "25";
                    SendValueToPLC("25", 4);
                }
            }
        }

        private void textBoxIst5Hız_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Eğer Enter tuşuna basıldıysa ve TextBox'ta değişiklik yapıldıysa
            if (e.KeyChar == (char)Keys.Enter)
            {
                // TextBox nesnesini al
                TextBox textBox = (TextBox)sender;

                // TextBox'ın metni kontrol et
                if (int.TryParse(textBox.Text, out int number))
                {
                    // Girilen değer 1-100 aralığında ise
                    if (number >= 1 && number <= 100)
                    {
                        // TextBox'ın metnini PLC'ye gönder
                        SendValueToPLC(textBox.Text, 5);
                    }
                    else
                    {
                        // Hata mesajı göster
                        MessageBox.Show("Lütfen 1 ile 100 arasında bir sayı girin!", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        textBox.Text = "25";
                        SendValueToPLC("25", 5);
                    }
                }
                else
                {
                    // Hata mesajı göster
                    MessageBox.Show("Lütfen geçerli bir sayı girin!", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    textBox.Text = "25";
                    SendValueToPLC("25", 5);
                }
            }
        }

        private void textBoxIst6Hız_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Eğer Enter tuşuna basıldıysa ve TextBox'ta değişiklik yapıldıysa
            if (e.KeyChar == (char)Keys.Enter)
            {
                // TextBox nesnesini al
                TextBox textBox = (TextBox)sender;

                // TextBox'ın metni kontrol et
                if (int.TryParse(textBox.Text, out int number))
                {
                    // Girilen değer 1-100 aralığında ise
                    if (number >= 1 && number <= 100)
                    {
                        // TextBox'ın metnini PLC'ye gönder
                        SendValueToPLC(textBox.Text, 6);
                    }
                    else
                    {
                        // Hata mesajı göster
                        MessageBox.Show("Lütfen 1 ile 100 arasında bir sayı girin!", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        textBox.Text = "25";
                        SendValueToPLC("25", 6);
                    }
                }
                else
                {
                    // Hata mesajı göster
                    MessageBox.Show("Lütfen geçerli bir sayı girin!", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    textBox.Text = "25";
                    SendValueToPLC("25", 6);
                }
            }
        }

        private void textBoxIst7Hız_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Eğer Enter tuşuna basıldıysa ve TextBox'ta değişiklik yapıldıysa
            if (e.KeyChar == (char)Keys.Enter)
            {
                // TextBox nesnesini al
                TextBox textBox = (TextBox)sender;

                // TextBox'ın metni kontrol et
                if (int.TryParse(textBox.Text, out int number))
                {
                    // Girilen değer 1-100 aralığında ise
                    if (number >= 1 && number <= 100)
                    {
                        // TextBox'ın metnini PLC'ye gönder
                        SendValueToPLC(textBox.Text, 7);
                    }
                    else
                    {
                        // Hata mesajı göster
                        MessageBox.Show("Lütfen 1 ile 100 arasında bir sayı girin!", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        textBox.Text = "25";
                        SendValueToPLC("25", 7);
                    }
                }
                else
                {
                    // Hata mesajı göster
                    MessageBox.Show("Lütfen geçerli bir sayı girin!", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    textBox.Text = "25";
                    SendValueToPLC("25", 7);
                }
            }
        }

        private void textBoxIst8Hız_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Eğer Enter tuşuna basıldıysa ve TextBox'ta değişiklik yapıldıysa
            if (e.KeyChar == (char)Keys.Enter)
            {
                // TextBox nesnesini al
                TextBox textBox = (TextBox)sender;

                // TextBox'ın metni kontrol et
                if (int.TryParse(textBox.Text, out int number))
                {
                    // Girilen değer 1-100 aralığında ise
                    if (number >= 1 && number <= 100)
                    {
                        // TextBox'ın metnini PLC'ye gönder
                        SendValueToPLC(textBox.Text, 8);
                    }
                    else
                    {
                        // Hata mesajı göster
                        MessageBox.Show("Lütfen 1 ile 100 arasında bir sayı girin!", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        textBox.Text = "25";
                        SendValueToPLC("25", 8);
                    }
                }
                else
                {
                    // Hata mesajı göster
                    MessageBox.Show("Lütfen geçerli bir sayı girin!", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    textBox.Text = "25";
                    SendValueToPLC("25", 8);
                }
            }
        }

        private void buttonUrunAyarfrKonvSis_Click(object sender, EventArgs e)
        {
            UrunAyr urunAyarForm = UrunAyr.GetInstance();
            urunAyarForm.Show();

            HideForm();

        }

        private void buttonAnasayfafrKonvSis_Click(object sender, EventArgs e)
        {

            Anasayfa anasayfaForm = Anasayfa.GetInstance();
            anasayfaForm.ShowForm();

            HideForm();

            
        }

        private void buttonSistmAyrfrKonvSistem_Click(object sender, EventArgs e)
        {

        }

        private void tableLayoutPanel105_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        public bool nxCompoletStringWrite(string variable, string value)  //NX WRITE
        {
            try
            {
                nxCompolet1.WriteVariable(variable, value);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool nxCompoletBoolRead(string variable)  //NX READ
        {
            try
            {
                boolReadStatus = false;
                bool staticValue = Convert.ToBoolean(nxCompolet1.ReadVariable(variable));
                return staticValue;
            }
            catch
            {
                boolReadStatus = true;
                return false;
            }
        }

        private void btnMin_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void exit_Click(object sender, EventArgs e)
        {
            bacgroundWorker.Durdur();
            Application.Exit();
        }

        public string nxCompoletStringRead(string variable)  //NX STRING
        {
            try
            {
                string staticStringValue = Convert.ToString(nxCompolet1.ReadVariable(variable));
                return staticStringValue;
            }
            catch (Exception e)
            {
                return "error";
            }

        }

        public void HideForm()
        {
            this.Hide(); // Formu gizle
        }

        // Formu tekrar göstermek için bir yöntem oluşturun
        public void ShowForm()
        {
            this.Show(); // Formu göster
        }
    }
}
