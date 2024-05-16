using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp13
{
    public partial class Anasayfa : Form
    {
        private BacgroundWorker bacgroundWorker;

        private static Anasayfa anasayfaInstance;

        ToolTip toolTip = new ToolTip();

        public Anasayfa()
        {
            InitializeComponent();

            bacgroundWorker = new BacgroundWorker();

            // KonveyorKontrolleri delegesine özel kontrol işlevini atayın
            bacgroundWorker.AnasayfaKontrolleri = AnasayfaKontrolleriMetodu;

        }

        public static Anasayfa GetInstance()
        {
            // Eğer form henüz oluşturulmadıysa veya kapatıldıysa, yeni bir örneği oluşturun
            if (anasayfaInstance == null || anasayfaInstance.IsDisposed)
            {
                anasayfaInstance = new Anasayfa();
            }
            return anasayfaInstance;
        }

        private void AnasayfaKontrolleriMetodu()
        {
            if (this.IsHandleCreated && !this.IsDisposed)
            {
                this.Invoke((MethodInvoker)delegate
                {
                    AktartmaileriGeriKontrol();
                    Station1Control();
                    StationControl();

                    for (int i = 1; i <= 8; i++)
                    {
                        FiksturControl(i);
                    }

                    KonveyorDurum();

                });
            }
        }

        private void Anasayfa_Load(object sender, EventArgs e)
        {
            bacgroundWorker.Baslat();
        }

        private void FiksturControl(int istasyonNo)
        {
            string donanimVariable = $"Istasyon{istasyonNo}Donanim[4]"; // İstasyon donanim değişkeni
            string fiksturSens = nxCompoletStringRead(donanimVariable); // Donanım değişkenini oku

            // Eğer fikstur sensörü 0 değilse arka plan rengini ayarla
            if (fiksturSens != "0")
            {
                switch (istasyonNo)
                {
                    case 1:
                        break;
                    case 2:
                        tableLayoutPanel67.BackgroundImage = Properties.Resources.Kırmızı;
                        break;
                    case 3:
                        tableLayoutPanel66.BackgroundImage = Properties.Resources.Yeşil;
                        break;
                    case 4:
                        tableLayoutPanel65.BackgroundImage = Properties.Resources.Yeşil;
                        break;
                    case 5:
                        break;
                    case 6:
                        tableLayoutPanel70.BackgroundImage = Properties.Resources.Yeşil;
                        break;
                    case 7:
                        tableLayoutPanel71.BackgroundImage = Properties.Resources.Yeşil;
                        break;
                    case 8:
                        tableLayoutPanel72.BackgroundImage = Properties.Resources.Boş;
                        break;
                }
            }
            else
            {
                switch (istasyonNo)
                {
                    case 1:
                        break;
                    case 2:
                        tableLayoutPanel67.BackgroundImage = null;
                        break;
                    case 3:
                        tableLayoutPanel66.BackgroundImage = null;
                        break;
                    case 4:
                        tableLayoutPanel65.BackgroundImage = null;
                        break;
                    case 5:
                        break;
                    case 6:
                        tableLayoutPanel70.BackgroundImage = null;
                        break;
                    case 7:
                        tableLayoutPanel71.BackgroundImage = null;
                        break;
                    case 8:
                        tableLayoutPanel72.BackgroundImage = null;
                        break;
                }
            }
        }

        public void AktartmaileriGeriKontrol()
        {
            string konveyorDurum1 = nxCompoletStringRead("Istasyon1Donanim[2]");
            string konveyorDurum5 = nxCompoletStringRead("Istasyon5Donanim[2]");

            switch (konveyorDurum1)
            {
                case "0":
                    tableLayoutPanel31.BackColor = Color.FromArgb(31, 31, 31);
                    tableLayoutPanel32.BackColor = Color.FromArgb(31, 31, 31);
                    tableLayoutPanel33.BackColor = Color.FromArgb(31, 31, 31);
                    tableLayoutPanel34.BackColor = Color.FromArgb(31, 31, 31);
                    tableLayoutPanel68.BackgroundImage = null;

                    string donanimVariable = $"Istasyon1Donanim[4]";
                    string fiksturSens = nxCompoletStringRead(donanimVariable);

                    tableLayoutPanel73.BackgroundImage = Properties.Resources.Kırmızı;
                    tableLayoutPanel56.BackColor = Color.DarkOrange;
                    tableLayoutPanel57.BackColor = Color.LawnGreen;
                    tableLayoutPanel58.BackColor = Color.LawnGreen;
                    tableLayoutPanel59.BackColor = Color.DarkOrange;

                    if (fiksturSens =="0")
                        tableLayoutPanel73.BackgroundImage = null;

                    break;

                case "1":
                    tableLayoutPanel56.BackColor = Color.FromArgb(31, 31, 31);
                    tableLayoutPanel57.BackColor = Color.FromArgb(31, 31, 31);
                    tableLayoutPanel58.BackColor = Color.FromArgb(31, 31, 31);
                    tableLayoutPanel59.BackColor = Color.FromArgb(31, 31, 31);
                    tableLayoutPanel64.BackgroundImage = null;

                    string donanimVariable1 = $"Istasyon1Donanim[4]";
                    string fiksturSens1 = nxCompoletStringRead(donanimVariable1);

                    tableLayoutPanel68.BackgroundImage = Properties.Resources.Kırmızı;
                    tableLayoutPanel31.BackColor = Color.DarkOrange;
                    tableLayoutPanel32.BackColor = Color.LawnGreen;
                    tableLayoutPanel33.BackColor = Color.LawnGreen;
                    tableLayoutPanel34.BackColor = Color.DarkOrange;

                    if (fiksturSens1 == "0")
                        tableLayoutPanel68.BackgroundImage = null;
                    break;

                case "2":
                    tableLayoutPanel31.BackColor = Color.FromArgb(31, 31, 31);
                    tableLayoutPanel32.BackColor = Color.FromArgb(31, 31, 31);
                    tableLayoutPanel33.BackColor = Color.FromArgb(31, 31, 31);
                    tableLayoutPanel34.BackColor = Color.FromArgb(31, 31, 31);
                    tableLayoutPanel68.BackgroundImage = null;
                    tableLayoutPanel56.BackColor = Color.FromArgb(31, 31, 31);
                    tableLayoutPanel57.BackColor = Color.FromArgb(31, 31, 31);
                    tableLayoutPanel58.BackColor = Color.FromArgb(31, 31, 31);
                    tableLayoutPanel59.BackColor = Color.FromArgb(31, 31, 31);
                    tableLayoutPanel73.BackgroundImage = null;
                    break;
            }

            switch (konveyorDurum5)
            {
                case "0":
                    tableLayoutPanel11.BackColor = Color.FromArgb(31, 31, 31);
                    tableLayoutPanel12.BackColor = Color.FromArgb(31, 31, 31);
                    tableLayoutPanel13.BackColor = Color.FromArgb(31, 31, 31);
                    tableLayoutPanel15.BackColor = Color.FromArgb(31, 31, 31);
                    tableLayoutPanel64.BackgroundImage = null;

                    string donanimVariable = $"Istasyon5Donanim[4]";
                    string fiksturSens = nxCompoletStringRead(donanimVariable);

                    tableLayoutPanel69.BackgroundImage = Properties.Resources.Yeşil;
                    tableLayoutPanel36.BackColor = Color.DarkOrange;
                    tableLayoutPanel37.BackColor = Color.LawnGreen;
                    tableLayoutPanel38.BackColor = Color.LawnGreen;
                    tableLayoutPanel39.BackColor = Color.DarkOrange;

                    if (fiksturSens == "0")
                        tableLayoutPanel69.BackgroundImage = null;

                    break;

                case "1":
                    tableLayoutPanel36.BackColor = Color.FromArgb(31, 31, 31);
                    tableLayoutPanel37.BackColor = Color.FromArgb(31, 31, 31);
                    tableLayoutPanel38.BackColor = Color.FromArgb(31, 31, 31);
                    tableLayoutPanel39.BackColor = Color.FromArgb(31, 31, 31);
                    tableLayoutPanel69.BackgroundImage = null;

                    string donanimVariable1 = $"Istasyon5Donanim[4]";
                    string fiksturSens1 = nxCompoletStringRead(donanimVariable1);

                    tableLayoutPanel64.BackgroundImage = Properties.Resources.Yeşil;
                    tableLayoutPanel11.BackColor = Color.LawnGreen;
                    tableLayoutPanel12.BackColor = Color.DarkOrange;
                    tableLayoutPanel13.BackColor = Color.LawnGreen;
                    tableLayoutPanel15.BackColor = Color.DarkOrange;

                    if (fiksturSens1 == "0")
                        tableLayoutPanel64.BackgroundImage = null;

                    break;

                case "2":
                    tableLayoutPanel11.BackColor = Color.FromArgb(31, 31, 31);
                    tableLayoutPanel12.BackColor = Color.FromArgb(31, 31, 31);
                    tableLayoutPanel13.BackColor = Color.FromArgb(31, 31, 31);
                    tableLayoutPanel15.BackColor = Color.FromArgb(31, 31, 31);
                    tableLayoutPanel64.BackgroundImage = null;
                    tableLayoutPanel36.BackColor = Color.FromArgb(31, 31, 31);
                    tableLayoutPanel37.BackColor = Color.FromArgb(31, 31, 31);
                    tableLayoutPanel38.BackColor = Color.FromArgb(31, 31, 31);
                    tableLayoutPanel39.BackColor = Color.FromArgb(31, 31, 31);
                    tableLayoutPanel69.BackgroundImage = null;
                    break;
            }
        }

        public void KonveyorDurum()
        {
            string konvState = nxCompoletStringRead("KonveyorDurum");

            switch(konvState)
            {
                case "0":
                    textBoxKonvSistem.Text = "HAZIR DEĞİL";
                    break;
                case "1":
                    textBoxKonvSistem.Text = "HAZIR";
                    break;
                case "2":
                    textBoxKonvSistem.Text = "ÜRETİM YAPILIYOR";
                    break;
            }
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

        public void Station1Control()
        {
            string station1Control = nxCompoletStringRead("Istasyon1Durum");

            switch (station1Control)
            {
                case "0":
                    txtBxStation1State.Text = "İŞLEM YOK";
                    break;
                case "1":
                    txtBxStation1State.Text = "FİKSTÜR BEKLENİYOR";
                    break;
                case "2":
                    txtBxStation1State.Text = "OPERATÖR BEKLENİYOR";
                    break;
                case "3":
                    txtBxStation1State.Text = "FİKSTÜR GÖNDERİLİYOR";
                    break;
                case "4":
                    txtBxStation1State.Text = "İŞLEM DURDURULDU";
                    break;
            }
        }

        public void StationControl()
        {
            for (int stationNo = 2; stationNo <= 8; stationNo++)
            {
                string stationVariable = $"Istasyon{stationNo}Durum";
                string stationControl = nxCompoletStringRead(stationVariable);

                // Metin kutusu adını oluştur
                string textBoxName = $"txtBxStation{stationNo}State";

                // Metin kutusunu bul
                TextBox textBox = Controls.Find(textBoxName, true).FirstOrDefault() as TextBox;

                // Duruma göre metin kutusuna uygun bilgiyi yaz
                if (textBox != null)
                {
                    switch (stationControl)
                    {
                        case "0":
                            textBox.Text = "İŞLEM YOK";
                            break;
                        case "1":
                            textBox.Text = "FİKSTÜR BEKLENİYOR";
                            break;
                        case "2":
                            textBox.Text = "İŞLEM YAPILIYOR";
                            break;
                        case "3":
                            textBox.Text = "FİKSTÜR GÖNDERİLİYOR";
                            break;
                        case "4":
                            textBox.Text = "FİKSTÜR İŞLEM BİTTİ";
                            break;
                    }
                }
            }
        }

        private void buttonKonvSistemfrAnsyfa_Click(object sender, EventArgs e)
        {
            KonveyorSistem konvSistemForm = KonveyorSistem.GetInstance();
            konvSistemForm.Show();

            HideForm();
        }

        private void buttonUrunAyrfrAnsyfa_Click(object sender, EventArgs e)
        {

            UrunAyr urunAyrForm = UrunAyr.GetInstance(); // Mevcut form örneğini al
            urunAyrForm.ShowForm(); // UrunAyr formunu göster

            HideForm();
        }

        private void buttonRobotMakfrAnsyfa_Click(object sender, EventArgs e)
        {

        }

        private void buttonSystemAyrfrAnsyfa_Click(object sender, EventArgs e)
        {

        }

        private void buttonAnasayfafrAnsyfa_Click(object sender, EventArgs e)
        {

        }

        private void textBoxKonvSistem_MouseHover(object sender, EventArgs e)
        {
            // Fikstür durumlarını kontrol et
            string stoperDurum = KontrolEtVeSonucuGetir();

            // Pop-up mesajı oluştur
            toolTip.SetToolTip(textBoxKonvSistem, "Fikstür Durumları");

            // İstenen metni ve kontrolü belirterek pop-up mesajını görüntüle
            toolTip.Show(stoperDurum, textBoxKonvSistem, textBoxKonvSistem.Width, 0, 40000);
        }

        private string KontrolEtVeSonucuGetir()
        {
            StringBuilder durumlar = new StringBuilder();

            // İstasyon 1
            string aktarmadurum1 = nxCompoletStringRead("Istasyon1Donanim[2]");
            string fiksturGiris1 = nxCompoletStringRead("Istasyon1Donanim[8]");
            string fiksturCikis1 = nxCompoletStringRead("Istasyon1Donanim[5]");
            string bariyerDurum1 = nxCompoletStringRead("Istasyon1Donanim[3]");

            // İstasyon 2
            string merkezLdurum2 = nxCompoletStringRead("Istasyon2Donanim[3]");
            string stoperdurum2 = nxCompoletStringRead("Istasyon2Donanim[2]");

            // İstasyon 3
            string stoperdurum3 = nxCompoletStringRead("Istasyon3Donanim[2]");

            // İstasyon 4
            string stoperdurum4 = nxCompoletStringRead("Istasyon4Donanim[2]");

            // İstasyon 5
            string aktarmadurum5 = nxCompoletStringRead("Istasyon5Donanim[2]");
            string fiksturGiris5 = nxCompoletStringRead("Istasyon5Donanim[8]");
            string fiksturCikis5 = nxCompoletStringRead("Istasyon5Donanim[5]");

            // İstasyon 6
            string stoperdurum6 = nxCompoletStringRead("Istasyon6Donanim[2]");

            // İstasyon 7
            string merkezLdurum7 = nxCompoletStringRead("Istasyon7Donanim[3]");
            string stoperdurum7 = nxCompoletStringRead("Istasyon7Donanim[2]");

            // İstasyon 8
            string stoperdurum8 = nxCompoletStringRead("Istasyon8Donanim[2]");

            // Other Received
            string acilStop = nxCompoletStringRead("KonveyorHazirDurum[0]");
            string calısmaMod = nxCompoletStringRead("KonveyorHazirDurum[1]");
            string eolRobRef = nxCompoletStringRead("KonveyorHazirDurum[2]");



            // Durumları pop-up mesajında göstermek için birleştir
            // İstasyon 1

            if (aktarmadurum1 != "0")
                durumlar.AppendLine("İstasyon 1 Aktarma Geride Değil");
            if (fiksturGiris1 != "0")
                durumlar.AppendLine("İstasyon 1 Giriş Sensör Aktif");
            if (fiksturCikis1 != "0")
                durumlar.AppendLine("İstasyon 1 Çıkış Sensör Aktif");
            if (bariyerDurum1 != "1")
                durumlar.AppendLine("İstasyon 1 Bariyer Sensör Aktif");

            // İstasyon 2

            if (merkezLdurum2 == "2")
                durumlar.AppendLine("İstasyon 2 Merkezleme Yarıda");
            if (stoperdurum2 != "1")
                durumlar.AppendLine("İstasyon 2 Stoper Yukarıda Değil");

            // İstasyon 3

            if (stoperdurum3 != "1")
                durumlar.AppendLine("İstasyon 3 Stoper Yukarıda Değil");

            // İstasyon 4

            if (stoperdurum4 != "1")
                durumlar.AppendLine("İstasyon 4 Stoper Yukarıda Değil");

            // İstasyon 5

            if (aktarmadurum5 == "2")
                durumlar.AppendLine("İstasyon 5 Aktarma Yarıda");
            if (fiksturGiris5 != "0")
                durumlar.AppendLine("İstasyon 5 Giriş Sensör Aktif");
            if (fiksturCikis5 != "0")
                durumlar.AppendLine("İstasyon 5 Çıkış Sensör Aktif");

            // İstasyon 6

            if (stoperdurum6 != "1")
                durumlar.AppendLine("İstasyon 6 Stoper Yukarıda Değil");

            // İstasyon 7

            if (merkezLdurum7 == "2")
                durumlar.AppendLine("İstasyon 7 Merkezleme Yarıda");
            if (stoperdurum7 != "1")
                durumlar.AppendLine("İstasyon 7 Stoper Yukarıda Değil");

            // İstasyon 8

            if (stoperdurum8 != "1")
                durumlar.AppendLine("İstasyon 8 Stoper Yukarıda Değil");

            // Others

            if (acilStop != "1")
                durumlar.AppendLine("Acil Stop Basılı");
            if (calısmaMod != "1")
                durumlar.AppendLine("Otomatik Çalışma Seçili Değil");
            if (eolRobRef != "1")
                durumlar.AppendLine("EOL Robot Referansta Değil");


            return durumlar.ToString();
        }

        private void textBoxKonvSistem_MouseLeave(object sender, EventArgs e)
        {
            // Mouse çekildiğinde pop-up mesajını kapat
            toolTip.Hide(textBoxKonvSistem);
        }

        // Formu gizlemek için bir yöntem oluşturun
        public void HideForm()
        {
            this.Hide(); // Formu gizle
        }

        // Formu tekrar göstermek için bir yöntem oluşturun
        public void ShowForm()
        {
            this.Show(); // Formu göster
        }

        private void exit_Click(object sender, EventArgs e)
        {
            bacgroundWorker.Durdur();
            Application.Exit();
        }

        private void btnMin_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }
    }
}
