using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Threading;

namespace WindowsFormsApp13
{
    public partial class UrunAyr : Form
    {
        private SettingsHelper settingsHelper = new SettingsHelper();

        private static UrunAyr urunAyrInstance; // Form örneği için statik bir referans

        private BacgroundWorker bacgroundWorker;

        string CalısmaMod = "CalismaMod";

        double actualminValue;
        double actualmaxValue;

        string variableName = "";

        Thread threadRead, threadWriteBool, threadReadString, threadWriteString;

        FormManager formManager = new FormManager();

        private List<double> minValues = new List<double>(Enumerable.Repeat(0.0, 27));
        private List<double> maxValues = new List<double>(Enumerable.Repeat(0.0, 27));

        private bool textBoxesActiveRobot2 = false; // Başlangıçta pasif
        private bool textBoxesActiveVibr = false; // Başlangıçta pasif
        private bool textBoxesActiveMakina2 = false; // Başlangıçta pasif
        private bool textBoxesActiveOperator = false; // Başlangıçta pasif
        private bool textBoxesActiveFırınIst3 = false; // Başlangıçta pasif
        private bool textBoxesActiveFırınIst4 = false; // Başlangıçta pasif
        private bool textBoxesActiveFırınIst5 = false; // Başlangıçta pasif
        private bool textBoxesActiveFırınIst6 = false; // Başlangıçta pasif
        private bool textBoxesActiveFanIst3 = false; // Başlangıçta pasif
        private bool textBoxesActiveFanIst4 = false; // Başlangıçta pasif
        private bool textBoxesActiveFanIst5 = false; // Başlangıçta pasif
        private bool textBoxesActiveFanIst6 = false; // Başlangıçta pasif
        private bool textBoxesActiveGrup1 = false; // Başlangıçta pasif
        private bool textBoxesActiveGrup2 = false; // Başlangıçta pasif

        private int myInteger;

        string connStatus;

        string[] valuesArray = new string[24];

        bool boolReadStatus;
        string variable = "";

        public UrunAyr()
        {
            InitializeComponent();


            bacgroundWorker = new BacgroundWorker();

            // KonveyorKontrolleri delegesine özel kontrol işlevini atayın
            bacgroundWorker.UrunAyrKontrolleri = UrunAyrKontrolleri;

            for (int i = 1; i <= 85; i++)
            {
                TextBox textBox = Controls.Find("textBoxEOL" + i, true).FirstOrDefault() as TextBox;
                if (textBox != null)
                {
                    textBox.KeyPress += TextBoxEOL_KeyPress;
                    textBox.MouseDown += TextBoxEOL_MouseDown;
                    textBox.Leave += TextBoxEOL_Leave;
                }
            }
        }

        public static UrunAyr GetInstance()
        {
            // Eğer form henüz oluşturulmadıysa veya kapatıldıysa, yeni bir örneği oluşturun
            if (urunAyrInstance == null || urunAyrInstance.IsDisposed)
            {
                urunAyrInstance = new UrunAyr();
            }
            return urunAyrInstance;
        }

        private Dictionary<TextBox, string> lastValidValues = new Dictionary<TextBox, string>();

        private void UrunAyrKontrolleri()
        {
            if (this.IsHandleCreated && !this.IsDisposed)
            {
                this.Invoke((MethodInvoker)delegate
                {
                    ManuelKontrol();
                    ReadSensor();
                    CheckEOLRob();

                });
            }
        }

        public void CheckEOLRob()
        {

            textBoxActuelPos1.Text = nxCompoletDoubleRead("EolRobPos[0]");
            textBoxActuelPos2.Text = nxCompoletDoubleRead("EolRobPos[1]");
            textBoxActuelPos3.Text = nxCompoletDoubleRead("EolRobPos[2]");
            textBoxActuelPos4.Text = nxCompoletDoubleRead("EolRobPos[3]");
            textBoxActuelPos5.Text = nxCompoletDoubleRead("EolRobPos[4]");
            textBoxActuelPos6.Text = nxCompoletDoubleRead("EolRobPos[5]");
        }

        private void TextBoxEOL_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                CheckNumber((TextBox)sender);
            }
        }

        private void TextBoxEOL_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                CheckNumber((TextBox)sender);
            }
        }

        private void TextBoxEOL_Leave(object sender, EventArgs e)
        {
            CheckNumber((TextBox)sender);
        }

        private void SetMinMaxValues()
        {
            minValues.Clear();
            maxValues.Clear();

            // settingsHelper.First11Values listesinden değerleri alarak minValues ve maxValues listelerine ekleme yapalım
            for (int i = 0; i < settingsHelper.First11Values.Count; i += 2)
            {
                if (double.TryParse(settingsHelper.First11Values[i], out double minValue))
                {
                    minValues.Add(minValue);
                }
                if (double.TryParse(settingsHelper.First11Values[i + 1], out double maxValue))
                {
                    maxValues.Add(maxValue);
                }
            }
        }

        //int index;
        double minValue, maxValue;
        int index;

        private void CheckNumber(TextBox textBox)
        {
            if (!string.IsNullOrEmpty(textBox.Text))
            {
                int which = int.Parse(textBox.Name.Replace("textBoxEOL", ""));

                if (which <= 54)
                {
                    index = which;

                    if (index == 1 || index == 7 || index == 13 || index == 19)
                    {
                        minValue = minValues[15];
                        maxValue = maxValues[15];

                    }

                    if (index == 2 || index == 8 || index == 14 || index == 20)
                    {
                        minValue = minValues[16];
                        maxValue = maxValues[16];
                    }

                    if (index == 3 || index == 9 || index == 15 || index == 21)
                    {
                        minValue = minValues[17];
                        maxValue = maxValues[17];
                    }

                    if (index == 4 || index == 10 || index == 16 || index == 22)
                    {
                        minValue = minValues[18];
                        maxValue = maxValues[18];
                    }

                    if (index == 5 || index == 11 || index == 17 || index == 23)
                    {
                        minValue = minValues[19];
                        maxValue = maxValues[19];
                    }

                    if (index == 6 || index == 12 || index == 18 || index == 24)
                    {
                        minValue = minValues[20];
                        maxValue = maxValues[20];
                    }

                    if (index == 25 || index == 31 || index == 37 || index == 43)
                    {
                        minValue = minValues[21];
                        maxValue = maxValues[21];
                    }

                    if (index == 26 || index == 32 || index == 38 || index == 44)
                    {
                        minValue = minValues[22];
                        maxValue = maxValues[22];
                    }

                    if (index == 27 || index == 33 || index == 39 || index == 45)
                    {
                        minValue = minValues[23];
                        maxValue = maxValues[23];
                    }

                    if (index == 28 || index == 34 || index == 40 || index == 46)
                    {
                        minValue = minValues[24];
                        maxValue = maxValues[24];
                    }

                    if (index == 29 || index == 35 || index == 41 || index == 47)
                    {
                        minValue = minValues[25];
                        maxValue = maxValues[25];
                    }

                    if (index == 30 || index == 36 || index == 42 || index == 48)
                    {
                        minValue = minValues[26];
                        maxValue = maxValues[26];
                    }
                }

                else
                {
                    index = which - 55; // textBoxEOL52'den 52 çıkarılarak index değeri hesaplanır

                    // index değerine göre ilgili min ve max değerlerini alın

                    if (index >= 0 && index < 12)
                    {
                        minValue = minValues[index % 6];
                        maxValue = maxValues[index % 6];
                    }
                    else if (index == 12 || index == 18)
                    {
                        minValue = minValues[6];
                        maxValue = maxValues[6];
                    }
                    else if (index == 13 || index == 19)
                    {
                        minValue = minValues[7];
                        maxValue = maxValues[7];
                    }
                    else if (index == 14 || index == 20)
                    {
                        minValue = minValues[8];
                        maxValue = maxValues[8];
                    }
                    else if (index == 15 || index == 21)
                    {
                        minValue = minValues[9];
                        maxValue = maxValues[9];
                    }
                    else if (index == 16 || index == 22)
                    {
                        minValue = minValues[10];
                        maxValue = maxValues[10];
                    }
                    else if (index == 17 || index == 23)
                    {
                        minValue = minValues[11];
                        maxValue = maxValues[11];
                    }

                    else if (index == 27 || index == 28 || index == 29 || index == 30)
                    {
                        minValue = 0.00;
                        maxValue = 100.00;
                    }

                }

                ValidateNumber(textBox);
            }
        }

        private void ValidateNumber(TextBox textBox)
        {
            if (!double.TryParse(textBox.Text, out double num))
            {
                MessageBox.Show("INI dosyasından alınan değerler geçersiz!", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Sayısal değerler başarıyla dönüştürüldü, kontrol işlemlerini gerçekleştirin
            if (num < minValue || num > maxValue)
            {
                MessageBox.Show($"Girilen değer {minValue} ile {maxValue} arasında olmalıdır!", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                textBox.Text = lastValidValues.ContainsKey(textBox) ? lastValidValues[textBox] : "";
            }
            else
            {
                textBox.Text = num.ToString();
                lastValidValues[textBox] = textBox.Text;
            }
        }

        private void ButtonsFalse()
        {
            int i = 1;

            for (i = 1; i <= 8; i++)
            {
                string buttonName = "buttonEOL" + i;
                Button button = Controls.Find(buttonName, true).FirstOrDefault() as Button;

                if (button != null)
                {
                    button.Enabled = false;
                    button.BackColor = Color.Snow;

                    switch(i)
                    {
                        case 1:

                            for (int j = 1; j <= 6; j++)
                            {
                                string textBoxName = "textBoxEOL" + j;
                                TextBox textBox = Controls.Find(textBoxName, true).FirstOrDefault() as TextBox;

                                if (textBox != null)
                                {
                                    textBox.BackColor = Color.Snow;
                                }
                            }

                            break;

                        case 2:

                            for (int j = 7; j <= 12; j++)
                            {
                                string textBoxName = "textBoxEOL" + j;
                                TextBox textBox = Controls.Find(textBoxName, true).FirstOrDefault() as TextBox;

                                if (textBox != null)
                                {
                                    textBox.BackColor = Color.Snow;
                                }
                            }

                            break;

                        case 3:

                            for (int j = 13; j <= 18; j++)
                            {
                                string textBoxName = "textBoxEOL" + j;
                                TextBox textBox = Controls.Find(textBoxName, true).FirstOrDefault() as TextBox;

                                if (textBox != null)
                                {
                                    textBox.BackColor = Color.Snow;
                                }
                            }

                            break;

                        case 4:

                            for (int j = 19; j <= 24; j++)
                            {
                                string textBoxName = "textBoxEOL" + j;
                                TextBox textBox = Controls.Find(textBoxName, true).FirstOrDefault() as TextBox;

                                if (textBox != null)
                                {
                                    textBox.BackColor = Color.Snow;
                                }
                            }

                            break;

                        case 5:

                            for (int j = 25; j <= 30; j++)
                            {
                                string textBoxName = "textBoxEOL" + j;
                                TextBox textBox = Controls.Find(textBoxName, true).FirstOrDefault() as TextBox;

                                if (textBox != null)
                                {
                                    textBox.BackColor = Color.Snow;
                                }
                            }

                            break;

                        case 6:

                            for (int j = 31; j <= 36; j++)
                            {
                                string textBoxName = "textBoxEOL" + j;
                                TextBox textBox = Controls.Find(textBoxName, true).FirstOrDefault() as TextBox;

                                if (textBox != null)
                                {
                                    textBox.BackColor = Color.Snow;
                                }
                            }

                            break;

                        case 7:

                            for (int j = 37; j <= 42; j++)
                            {
                                string textBoxName = "textBoxEOL" + j;
                                TextBox textBox = Controls.Find(textBoxName, true).FirstOrDefault() as TextBox;

                                if (textBox != null)
                                {
                                    textBox.BackColor = Color.Snow;
                                }
                            }

                            break;

                        case 8:

                            for (int j = 43; j <= 48; j++)
                            {
                                string textBoxName = "textBoxEOL" + j;
                                TextBox textBox = Controls.Find(textBoxName, true).FirstOrDefault() as TextBox;

                                if (textBox != null)
                                {
                                    textBox.BackColor = Color.Snow;
                                }
                            }

                            break;
                    }
                }
            }
        }

        private void ButtonsTrue()
        {
            for (int i = 1; i <= 8; i++)
            {
                string buttonName = "buttonEOL" + i;
                Button button = Controls.Find(buttonName, true).FirstOrDefault() as Button;

                if (button != null)
                {
                    button.Enabled = true;

                }
            }
        }

        private void buttonYeniModel_Click(object sender, EventArgs e)
        {
            textBoxModel.Text = "";
            buttonEOLRobot.Text = "PASİF";
            buttonEOLMakina.Text = "PASİF";
            buttonVibrRobot.Text = "PASİF";
            buttonOperatorCalsma.Text = "PASİF";
            buttonVibrRobot.Enabled = true;
            buttonOperatorCalsma.Enabled = true;
            buttonEOLRobot.BackColor = Color.DarkRed;
            buttonEOLMakina.BackColor = Color.DarkRed;
            buttonVibrRobot.BackColor = Color.DarkRed;
            buttonOperatorCalsma.BackColor = Color.DarkRed;

            string[] FanFırınState = new string[8];

            for (int i = 0; i < FanFırınState.Length; i++)
            {
                switch (i)
                {
                    case 0:
                        buttonFırınIst3.Text = "PASİF";
                        buttonFırınIst3.BackColor = Color.DarkRed;
                        textBoxEOL82.Enabled = false;
                        break;
                    case 1:
                        buttonFırınIst4.Text = "PASİF";
                        buttonFırınIst4.BackColor = Color.DarkRed;
                        textBoxEOL83.Enabled = false;
                        break;
                    case 2:
                        buttonFırınIst5.Text = "PASİF";
                        buttonFırınIst5.BackColor = Color.DarkRed;
                        textBoxEOL84.Enabled = false;
                        break;
                    case 3:
                        buttonFırınIst6.Text = "PASİF";
                        buttonFırınIst6.BackColor = Color.DarkRed;
                        textBoxEOL85.Enabled = false;
                        break;
                    case 4:
                        buttonFanIst3.Text = "PASİF";
                        buttonFanIst3.BackColor = Color.DarkRed;
                        buttonFanIst3.Enabled = false;
                        break;
                    case 5:
                        buttonFanIst4.Text = "PASİF";
                        buttonFanIst4.BackColor = Color.DarkRed;
                        buttonFanIst4.Enabled = false;
                        break;
                    case 6:
                        buttonFanIst5.Text = "PASİF";
                        buttonFanIst5.BackColor = Color.DarkRed;
                        buttonFanIst5.Enabled = false;
                        break;
                    case 7:
                        buttonFanIst6.Text = "PASİF";
                        buttonFanIst6.BackColor = Color.DarkRed;
                        buttonFanIst6.Enabled = false;
                        break;
                }
            }

            ButtonsFalse();
            ButtonsFalseVibr();
            VibrTextBoxFalse();
            RobotTextBoxFalse();


            for (int i = 0; i <= 85; i++)
            {
                string textBoxName = "textBoxEOL" + i;
                TextBox textBox = Controls.Find(textBoxName, true).FirstOrDefault() as TextBox;

                if (textBox != null)
                {
                    textBox.Text = "";
                }
            }
        }


        bool ModelFieldsFilled;

        private void buttonMdlKaydet_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(textBoxModel.Text)) // Model ismi dolu mu?
            {
                ModelFieldsFilled = true;

                bool allFieldsFilled1 = true;
                bool allFieldsFilled2 = true;
                bool allFieldsFilled3 = true;
                bool allFieldsFilled4 = true;
                bool allFieldsFilled5 = true;
                bool allFieldsFilled6 = true;

                if (buttonVibrRobot.Text == "AKTİF")
                {
                    allFieldsFilled1 = CheckFields(55, 81);
                }

                if (buttonEOLRobot.Text == "AKTİF")
                {
                    allFieldsFilled2 = CheckFields(1, 54);
                }

                if(buttonFırınIst3.Text == "AKTİF")
                {
                    allFieldsFilled3 = CheckFırın(3);
                }

                if (buttonFırınIst4.Text == "AKTİF")
                {
                    allFieldsFilled4 = CheckFırın(4);
                }

                if (buttonFırınIst5.Text == "AKTİF")
                {
                    allFieldsFilled5 = CheckFırın(5);
                }

                if (buttonFırınIst6.Text == "AKTİF")
                {
                    allFieldsFilled6 = CheckFırın(6);
                }

                if (allFieldsFilled1 && allFieldsFilled2 && allFieldsFilled3 && allFieldsFilled4 && allFieldsFilled5 && allFieldsFilled6)
                {
                    SaveToIniFile();
                }
            }
            else // Model ismi dolu değilse
            {
                MessageBox.Show("Lütfen model alanını doldurun.");
            }
        }

        private bool CheckFırın(int i)
        {
            switch(i)
            {
                case 3:
                    if (textBoxEOL82 != null)
                    {
                        if (string.IsNullOrEmpty(textBoxEOL82.Text))
                        {
                            MessageBox.Show("Boş alanları doldur.");
                            return false;
                        }
                    }
                    break;
                case 4:
                    if (textBoxEOL83 != null)
                    {
                        if (string.IsNullOrEmpty(textBoxEOL83.Text))
                        {
                            MessageBox.Show("Boş alanları doldur.");
                            return false;
                        }
                    }
                    break;
                case 5:
                    if (textBoxEOL84!= null)
                    {
                        if (string.IsNullOrEmpty(textBoxEOL84.Text))
                        {
                            MessageBox.Show("Boş alanları doldur.");
                            return false;
                        }
                    }
                    break;
                case 6:
                    if (textBoxEOL85 != null)
                    {
                        if (string.IsNullOrEmpty(textBoxEOL85.Text))
                        {
                            MessageBox.Show("Boş alanları doldur.");
                            return false;
                        }
                    }
                    break;



            }
            return true;
        }

        private bool CheckFields(int start, int end)
        {
            for (int i = start; i <= end; i++)
            {
                string textBoxName = "textBoxEOL" + i;
                TextBox textBox = Controls.Find(textBoxName, true).FirstOrDefault() as TextBox;

                if (textBox != null)
                {
                    if (string.IsNullOrEmpty(textBox.Text))
                    {
                        MessageBox.Show("Boş alanları doldur.");
                        return false;
                    }
                }
            }

            return true;
        }

        private bool AllTextBoxesFilled()
        {
            // TextBox'ların içeriğini kontrol et
            if (textBoxModel.Text == "")
                return false;

            for (int i = 1; i <= 85; i++)
            {
                string textBoxName = "textBoxEOL" + i;
                TextBox textBox = Controls.Find(textBoxName, true)[0] as TextBox;

                if (textBox.Text == "")
                    return false;
            }

            return true; // Tüm TextBox'lar doluysa true döndür
        }

        private void SaveToIniFile()
        {
            // .ini dosyasına kaydet
            string iniFolderPath = Path.Combine(Application.StartupPath, "Models"); // Models klasörünün yolunu oluştur
            Directory.CreateDirectory(iniFolderPath); // Klasörü oluştur
            if (!Directory.Exists(iniFolderPath))
            {
                Directory.CreateDirectory(iniFolderPath); // Klasörü oluştur
            }

            string iniFileName = Path.Combine(iniFolderPath, textBoxModel.Text + ".ini"); // .ini dosyasının tam yolunu oluştur

            using (StreamWriter writer = new StreamWriter(iniFileName))
            {
                string modelLine = $"{textBoxModel.Name}={textBoxModel.Text}";
                //writer.WriteLine(modelLine);

                for (int i = 1; i <= 85; i++)
                {
                    string textBoxName = "textBoxEOL" + i;
                    TextBox textBox = Controls.Find(textBoxName, true)[0] as TextBox;

                    string line = $"{textBox.Name}={textBox.Text.Replace(',', '.')}"; // Nokta yerine virgülü değiştir
                    writer.WriteLine(line);

                }

                string buttonRobotLine = $"RobotButtonState={buttonEOLRobot.Text}";
                writer.WriteLine(buttonRobotLine);

                string buttonEOLMakinaLine = $"ButtonEOLMakinaState={(buttonEOLMakina.Text)}";
                writer.WriteLine(buttonEOLMakinaLine);

                string buttonGrup1 = $"ButtonGrup1State={buttonEOLGrup1.Text}";
                writer.WriteLine(buttonGrup1);

                string buttonGrup2 = $"ButtonGrup2State={buttonEOLGrup2.Text}";
                writer.WriteLine(buttonGrup2);

                string buttonVibrLine = $"ButtonVibrasyonState={buttonVibrRobot.Text}";
                writer.WriteLine(buttonVibrLine);

                string buttonOperatorLine = $"ButtonOperatorÇalışmaState={(buttonOperatorCalsma.Text)}";
                writer.WriteLine(buttonOperatorLine);

                string[] FanFırınState = new string[8];

                for (int i = 0; i < FanFırınState.Length; i++)
                {
                    switch (i)
                    {
                        case 0:
                            FanFırınState[i] = $"Ist3FırınButtonState={buttonFırınIst3.Text}";
                            break;
                        case 1:
                            FanFırınState[i] = $"Ist4FırınButtonState={buttonFırınIst4.Text}";
                            break;
                        case 2:
                            FanFırınState[i] = $"Ist5FırınButtonState={buttonFırınIst5.Text}";
                            break;
                        case 3:
                            FanFırınState[i] = $"Ist6FırınButtonState={buttonFırınIst6.Text}";
                            break;
                        case 4:
                            FanFırınState[i] = $"Ist3FanButtonState={buttonFanIst3.Text}";
                            break;
                        case 5:
                            FanFırınState[i] = $"Ist4FanButtonState={buttonFanIst4.Text}";
                            break;
                        case 6:
                            FanFırınState[i] = $"Ist5FanButtonState={buttonFanIst5.Text}";
                            break;
                        case 7:
                            FanFırınState[i] = $"Ist6FanButtonState={buttonFanIst6.Text}";
                            break;
                    }
                    writer.WriteLine(FanFırınState[i]);
                }
            }                

            MessageBox.Show("Model oluşturuldu ve PLC ye gönderildi.");

            settingsHelper.LoadUrunAyar(textBoxModel.Text);

            string settingsValue;

            //PLC'ye değerleri yaz
            for (int i = 0; i <= 99; i++)
            {
                variableName = $"SayiDegisken[{i}]";

                if (i <= 84)
                {
                    settingsValue = settingsHelper.First85Values[i];
                }
                else
                {
                    settingsValue = settingsHelper.First85Values[i] == "AKTİF" ? "1" : "0";
                }
                                 
                    nxCompoletStringWrite(variableName, settingsValue);
            }

            string sendModel = "ModelGonder";
            nxCompoletBoolWrite(sendModel, true);
        }

        private void buttonModelSec_Click(object sender, EventArgs e)
        {
            
            string modelsFolderPath = Path.Combine(Application.StartupPath, "Models");

            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.InitialDirectory = modelsFolderPath;
                openFileDialog.Filter = "Model Dosyaları (*.ini)|*.ini|Tüm Dosyalar (*.*)|*.*";
                openFileDialog.FilterIndex = 1;
                openFileDialog.RestoreDirectory = true;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string selectedModelPath = openFileDialog.FileName;

                    textBoxModel.Text = Path.GetFileNameWithoutExtension(selectedModelPath);

                    // Seçilen model dosyasının içeriğini satır satır oku
                    string[] lines = File.ReadAllLines(selectedModelPath);

                    // Toplam 48 değişken olduğunu varsayalım
                    if (lines.Length >= 98)
                    {
                        for (int i = 0; i <= 84; i++)
                        {
                            // Değişkenin adını ve değerini ayır
                            string[] parts = lines[i].Split('=');
                            if (parts.Length == 2)
                            {
                                // TextBox adını oluştur
                                string textBoxName = "textBoxEOL" + (i+1);

                                // Form içinde bu isimde bir TextBox kontrolü ara
                                TextBox textBox = Controls.Find(textBoxName, true).FirstOrDefault() as TextBox;

                                // TextBox bulunduysa, değeri ata
                                if (textBox != null)
                                {
                                    // Nokta karakterini virgüle dönüştür
                                    string value = parts[1].Replace('.', ',');
                                    textBox.Text = value;
                                }
                            }
                        }

                        // Buton durumunu yükle
                        if (lines.Length >= 98)
                        {
                            string buttonStateRobot = lines[85].Split('=')[1];
                            string buttonStateMakina = lines[86].Split('=')[1];
                            string buttonStateGrup1 = lines[87].Split('=')[1];
                            string buttonStateGrup2 = lines[88].Split('=')[1];
                            string buttonStateVibr = lines[89].Split('=')[1];
                            string buttonStateOperator = lines[90].Split('=')[1];
                            string buttonStateIst3Fırın = lines[91].Split('=')[1];
                            string buttonStateIst4Fırın = lines[92].Split('=')[1];
                            string buttonStateIst5Fırın = lines[93].Split('=')[1];
                            string buttonStateIst6Fırın = lines[94].Split('=')[1];
                            string buttonStateIst3Fan = lines[95].Split('=')[1];
                            string buttonStateIst4Fan = lines[96].Split('=')[1];
                            string buttonStateIst5Fan = lines[97].Split('=')[1];
                            string buttonStateIst6Fan = lines[98].Split('=')[1];

                            buttonEOLRobot.Text = buttonStateRobot;
                            buttonEOLRobot.BackColor = buttonStateRobot == "AKTİF" ? Color.DarkOliveGreen : Color.DarkRed;

                            buttonEOLMakina.Text = buttonStateMakina;
                            buttonEOLMakina.BackColor = buttonStateMakina == "AKTİF" ? Color.DarkOliveGreen : Color.DarkRed;

                            buttonVibrRobot.Text = buttonStateVibr;
                            buttonVibrRobot.BackColor = buttonStateVibr == "AKTİF" ? Color.DarkOliveGreen : Color.DarkRed;
                            

                            buttonOperatorCalsma.Text = buttonStateOperator;
                            buttonOperatorCalsma.BackColor = buttonStateOperator == "AKTİF" ? Color.DarkOliveGreen : Color.DarkRed;

                            buttonFırınIst3.Text = buttonStateIst3Fırın;
                            buttonFırınIst3.BackColor = buttonStateIst3Fırın == "AKTİF" ? Color.DarkOliveGreen : Color.DarkRed;
                            textBoxEOL82.Enabled = buttonStateIst3Fırın == "AKTİF" ? true : false;

                            buttonFırınIst4.Text = buttonStateIst4Fırın;
                            buttonFırınIst4.BackColor = buttonStateIst4Fırın == "AKTİF" ? Color.DarkOliveGreen : Color.DarkRed;
                            textBoxEOL83.Enabled = buttonStateIst4Fırın == "AKTİF" ? true : false;

                            buttonFırınIst5.Text = buttonStateIst5Fırın;
                            buttonFırınIst5.BackColor = buttonStateIst5Fırın == "AKTİF" ? Color.DarkOliveGreen : Color.DarkRed;
                            textBoxEOL84.Enabled = buttonStateIst5Fırın == "AKTİF" ? true : false;

                            buttonFırınIst6.Text = buttonStateIst6Fırın;
                            buttonFırınIst6.BackColor = buttonStateIst6Fırın == "AKTİF" ? Color.DarkOliveGreen : Color.DarkRed;
                            textBoxEOL85.Enabled = buttonStateIst6Fırın == "AKTİF" ? true : false;

                            buttonFanIst3.Text = buttonStateIst3Fan;
                            buttonFanIst3.BackColor = buttonStateIst3Fan == "AKTİF" ? Color.DarkOliveGreen : Color.DarkRed;

                            buttonFanIst4.Text = buttonStateIst4Fan;
                            buttonFanIst4.BackColor = buttonStateIst4Fan == "AKTİF" ? Color.DarkOliveGreen : Color.DarkRed;

                            buttonFanIst5.Text = buttonStateIst5Fan;
                            buttonFanIst5.BackColor = buttonStateIst5Fan == "AKTİF" ? Color.DarkOliveGreen : Color.DarkRed;

                            buttonFanIst6.Text = buttonStateIst6Fan;
                            buttonFanIst6.BackColor = buttonStateIst6Fan == "AKTİF" ? Color.DarkOliveGreen : Color.DarkRed;

                            if (buttonStateRobot == "AKTİF")
                            {
                                buttonEOLMakina.Enabled = true;

                                ButtonsTrue();
                                RobotTextBoxTrue();

                                if (buttonStateMakina == "AKTİF")
                                {
                                    buttonEOLMakina.Text = "AKTİF";
                                    buttonEOLMakina.BackColor = Color.DarkOliveGreen;
                                }
                                else
                                {
                                    buttonEOLMakina.Text = "PASİF";
                                    buttonEOLMakina.BackColor = Color.DarkRed;
                                }
                            }
                            else
                            {
                                buttonEOLMakina.Enabled = false;
                                buttonEOLMakina.Text = "PASİF";
                                buttonEOLMakina.BackColor = Color.DarkRed;

                                ButtonsFalse();
                                RobotTextBoxFalse();

                            }

                            if (buttonStateVibr == "AKTİF")
                            {
                                buttonOperatorCalsma.Enabled = false;
                                buttonOperatorCalsma.Text = "PASİF";
                                buttonOperatorCalsma.BackColor = Color.DarkRed;

                                ButtonsTrueVibr();
                                VibrTextBoxTrue();

                                /*if (buttonStateOperator == "AKTİF")
                                {
                                    buttonOperatorCalsma.Text = "AKTİF";
                                    buttonOperatorCalsma.BackColor = Color.DarkOliveGreen;
                                }
                                else
                                {
                                    buttonOperatorCalsma.Text = "PASİF";
                                    buttonOperatorCalsma.BackColor = Color.DarkRed;
                                }*/
                            }
                            else
                            {
                                buttonOperatorCalsma.Enabled = true;

                                if (buttonStateOperator == "AKTİF")
                                {
                                    buttonVibrRobot.Enabled = false;
                                    buttonOperatorCalsma.Text = "AKTİF";
                                    buttonOperatorCalsma.BackColor = Color.DarkOliveGreen;
                                }
                                else
                                {
                                    buttonVibrRobot.Enabled = true;
                                    buttonOperatorCalsma.Text = "PASİF";
                                    buttonOperatorCalsma.BackColor = Color.DarkRed;
                                }

                                ButtonsFalseVibr();
                                VibrTextBoxFalse();
                            }

                            if (buttonStateIst3Fırın == "AKTİF")
                            {
                                buttonFanIst3.Enabled = true;
                                if (buttonFanIst3.Text == "AKTİF")
                                    buttonFanIst3.BackColor = Color.DarkOliveGreen;
                                else
                                    buttonFanIst3.BackColor = Color.DarkRed;
                            }

                            else
                            {
                                buttonFanIst3.Enabled = false;
                                buttonFanIst3.BackColor = Color.DarkRed;
                            }

                            if (buttonStateIst4Fırın == "AKTİF")
                            {
                                buttonFanIst4.Enabled = true;
                                if (buttonFanIst4.Text == "AKTİF")
                                    buttonFanIst4.BackColor = Color.DarkOliveGreen;
                                else
                                    buttonFanIst4.BackColor = Color.DarkRed;
                            }

                            else
                            {
                                buttonFanIst4.Enabled = false;
                                buttonFanIst4.BackColor = Color.DarkRed;
                            }

                            if (buttonStateIst5Fırın == "AKTİF")
                            {
                                buttonFanIst5.Enabled = true;
                                if (buttonFanIst5.Text == "AKTİF")
                                    buttonFanIst5.BackColor = Color.DarkOliveGreen;
                                else
                                    buttonFanIst5.BackColor = Color.DarkRed;
                            }

                            else
                            {
                                buttonFanIst5.Enabled = false;
                                buttonFanIst5.BackColor = Color.DarkRed;
                            }

                            if (buttonStateIst6Fırın == "AKTİF")
                            {
                                buttonFanIst6.Enabled = true;
                                if (buttonFanIst6.Text == "AKTİF")
                                    buttonFanIst6.BackColor = Color.DarkOliveGreen;
                                else
                                    buttonFanIst6.BackColor = Color.DarkRed;
                            }

                            else
                            {
                                buttonFanIst6.Enabled = false;
                                buttonFanIst6.BackColor = Color.DarkRed;
                            }

                        }
                    }
                    else
                    {
                        MessageBox.Show("Seçilen model dosyasında yeterli sayıda değişken bulunmuyor.");
                    }
                }
            }
        }

        private void buttonEOLRobot_Click(object sender, EventArgs e)
        {
            buttonPosKaydet2.Enabled = false;
            // Bayrağı tersine çevir
            textBoxesActiveRobot2 = !textBoxesActiveRobot2;

            // Duruma göre buton metnini ayarla
            buttonEOLRobot.Text = textBoxesActiveRobot2 ? "AKTİF" : "PASİF";

            switch(textBoxesActiveRobot2)
            {
                case false:
                    buttonEOLRobot.BackColor = Color.DarkRed;
                    break;
                case true:
                    buttonEOLRobot.BackColor = Color.DarkOliveGreen;
                    break;
            }

            // Eolmakina butonunun etkinliğini ayarla
            if (textBoxesActiveRobot2 == false)
            {
                // Durum PASİF ise, Eolmakina butonu etkin değil yap
                buttonEOLMakina.Text = "PASİF";
                buttonEOLMakina.Enabled = false;
                buttonEOLMakina.ForeColor = Color.WhiteSmoke;
                buttonEOLMakina.BackColor = Color.DarkRed;

                RobotTextBoxFalse();
                ButtonsFalse();
            }
            else
            {
                // Durum AKTİF ise, Eolmakina butonu etkin yap
                buttonEOLMakina.Enabled = true;
                buttonEOLMakina.ForeColor = Color.WhiteSmoke;

                RobotTextBoxTrue();
                ButtonsTrue();
            }
        }

        private void buttonEOLMakina_Click(object sender, EventArgs e)
        {
            // Bayrağı tersine çevir
            textBoxesActiveMakina2 = !textBoxesActiveMakina2;

            // Duruma göre buton metnini ayarla
            buttonEOLMakina.Text = textBoxesActiveMakina2 ? "AKTİF" : "PASİF";

            switch (textBoxesActiveMakina2)
            {
                case false:
                    buttonEOLMakina.BackColor = Color.DarkRed;
                    break;
                case true:
                    buttonEOLMakina.BackColor = Color.DarkOliveGreen;
                    break;
            }
        }

        private void OtherButtonsPasif(int i)
        {
            for (int j = 1; j <= 8; j++)
            {
                // Butonun adını oluştur
                string buttonName = "buttonEOL" + j;

                // Form içinde bu isimde bir Button kontrolü ara
                Button button = Controls.Find(buttonName, true).FirstOrDefault() as Button;

                // Button bulunduysa, etkinlik durumunu pasif hale getir

      
                if (button != null)
                {
                        if (button.Name == ("buttonEOL" + i))
                            continue;
                        else
                            button.BackColor = Color.Snow;
                }
            }
        }

        private void OtherButtonsSnow()
        {
            for (int j = 1; j <= 8; j++)
            {
                // Butonun adını oluştur
                string buttonName = "buttonEOL" + j;

                // Form içinde bu isimde bir Button kontrolü ara
                Button button = Controls.Find(buttonName, true).FirstOrDefault() as Button;

                // Button bulunduysa, etkinlik durumunu pasif hale getir


                if (button != null)
                {
                    button.Name = ("buttonEOL" + j);
                    button.BackColor = Color.Snow;
                }
            }
        }

        private void BackgroungTurquaise(int i)
        {
            // 6 adet textbox'ın arka plan rengini değiştir
            for (int j = 1; j <= 48; j++)
            {
                // Textbox kontrolünün ismini oluştur
                string textBoxName = "textBoxEOL" + j;

                // Form içinde bu isimde bir TextBox kontrolü ara
                TextBox textBox = Controls.Find(textBoxName, true).FirstOrDefault() as TextBox;

                // TextBox bulunduysa, arka plan rengini değiştir
                if (textBox != null)
                {
                    if ((textBox.Name == "textBoxEOL" + i) || (textBox.Name == "textBoxEOL" + (i+1)) || (textBox.Name == "textBoxEOL" + (i+2)) || (textBox.Name == "textBoxEOL" + (i + 3)) || (textBox.Name == "textBoxEOL" + (i + 4)) || (textBox.Name == "textBoxEOL" + (i + 5)))
                    {
                        textBox.BackColor = Color.PaleTurquoise;
                    }
                    else
                        textBox.BackColor = Color.Snow;

                }
            }
        }

        private void buttonEOL1_Click(object sender, EventArgs e)
        {
            buttonEOL1.BackColor = Color.PaleTurquoise;
            buttonPosKaydet2.Enabled = true;

            int i = 1;
            OtherButtonsPasif(1);
            BackgroungTurquaise(i);

            myInteger = i;
        }

        private void buttonEOL2_Click(object sender, EventArgs e)
        {
            buttonEOL2.BackColor = Color.PaleTurquoise;
            buttonPosKaydet2.Enabled = true;

            int i = 7;
            OtherButtonsPasif(2);
            BackgroungTurquaise(i);

            myInteger = i;
        }

        private void buttonEOL3_Click(object sender, EventArgs e)
        {
            buttonEOL3.BackColor = Color.PaleTurquoise;
            buttonPosKaydet2.Enabled = true;

            int i = 13;
            OtherButtonsPasif(3);
            BackgroungTurquaise(i);

            myInteger = i;
        }

        private void buttonEOL4_Click(object sender, EventArgs e)
        {
            buttonEOL4.BackColor = Color.PaleTurquoise;
            buttonPosKaydet2.Enabled = true;

            int i = 19;
            OtherButtonsPasif(4);
            BackgroungTurquaise(i);

            myInteger = i;
        }

        private void buttonEOL5_Click(object sender, EventArgs e)
        {
            buttonEOL5.BackColor = Color.PaleTurquoise;
            buttonPosKaydet2.Enabled = true;

            int i = 25;
            OtherButtonsPasif(5);
            BackgroungTurquaise(i);

            myInteger = i;
        }

        private void buttonEOL6_Click(object sender, EventArgs e)
        {
            buttonEOL6.BackColor = Color.PaleTurquoise;
            buttonPosKaydet2.Enabled = true;

            int i = 31;
            OtherButtonsPasif(6);
            BackgroungTurquaise(i);

            myInteger = i;
        }

        private void buttonEOL7_Click(object sender, EventArgs e)
        {
            buttonEOL7.BackColor = Color.PaleTurquoise;
            buttonPosKaydet2.Enabled = true;

            int i = 37;
            OtherButtonsPasif(7);
            BackgroungTurquaise(i);

            myInteger = i;
        }

        private void buttonEOL8_Click(object sender, EventArgs e)
        {
            buttonEOL8.BackColor = Color.PaleTurquoise;
            buttonPosKaydet2.Enabled = true;

            int i = 43;
            OtherButtonsPasif(8);
            BackgroungTurquaise(i);

            myInteger = i;
        }

        private void buttonPosKaydet2_Click(object sender, EventArgs e)
        {
            foreach (Control control in Controls)
            {
                if (control is TextBox textBox && textBox.Name.StartsWith("textBoxEOL"))
                {
                    textBox.MouseDown += TextBoxEOL_MouseDown;
                }
            }

            int textBoxSifirla = 1;
            buttonPosKaydet2.Enabled = false;

            int jMyInteger;

            bool allTextBoxesFilled = true; // Tüm actual pos textbox'ları doldurulmuş olarak başlat

            // Diğer altı textbox'ın değerlerini kaydetme işlemi ve actual pos textbox'larının dolu olup olmadığını kontrol etme
            for (jMyInteger = myInteger; jMyInteger <= myInteger + 5; jMyInteger++)
            {
                string textBoxActuelPosName = "textBoxActuelPos" + textBoxSifirla;


                TextBox textBoxActuelPos = Controls.Find(textBoxActuelPosName, true).FirstOrDefault() as TextBox;


                if (textBoxActuelPos != null)
                {
                    // TextBox dolu mu kontrol et
                    if (string.IsNullOrEmpty(textBoxActuelPos.Text))
                    {
                        
                        allTextBoxesFilled = false;
                        MessageBox.Show("Lütfen tüm değerleri girin!", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        buttonPosKaydet2.Enabled = true;
                        break; // Eğer bir textbox boşsa döngüyü durdur
                    }
                    else
                    {
                        // Değerin minimum ve maksimum aralıkta olup olmadığını kontrol et
                        double value;  

                        switch(jMyInteger)
                        {
                            case 1:
                            case 7:
                            case 13:
                            case 19:
                                actualminValue = minValues[15];
                                actualmaxValue = maxValues[15];
                                break;
                            case 2:
                            case 8:
                            case 14:
                            case 20:
                                actualminValue = minValues[16];
                                actualmaxValue = maxValues[16];
                                break;
                            case 3:
                            case 9:
                            case 15:
                            case 21:
                                actualminValue = minValues[17];
                                actualmaxValue = maxValues[17];
                                break;
                            case 4:
                            case 10:
                            case 16:
                            case 22:
                                actualminValue = minValues[18];
                                actualmaxValue = maxValues[18];
                                break;
                            case 5:
                            case 11:
                            case 17:
                            case 23:
                                actualminValue = minValues[19];
                                actualmaxValue = maxValues[19];
                                break;
                            case 6:
                            case 12:
                            case 18:
                            case 24:
                                actualminValue = minValues[20];
                                actualmaxValue = maxValues[20];
                                break;
                            case 25:
                            case 31:
                            case 37:
                            case 43:
                                actualminValue = minValues[21];
                                actualmaxValue = maxValues[21];
                                break;
                            case 26:
                            case 32:
                            case 38:
                            case 44:
                                actualminValue = minValues[22];
                                actualmaxValue = maxValues[22];
                                break;
                            case 27:
                            case 33:
                            case 39:
                            case 45:
                                actualminValue = minValues[23];
                                actualmaxValue = maxValues[23];
                                break;
                            case 28:
                            case 34:
                            case 40:
                            case 46:
                                actualminValue = minValues[24];
                                actualmaxValue = maxValues[24];
                                break;
                            case 29:
                            case 35:
                            case 41:
                            case 47:
                                actualminValue = minValues[25];
                                actualmaxValue = maxValues[25];
                                break;
                            case 30:
                            case 36:
                            case 42:
                            case 48:
                                actualminValue = minValues[26];
                                actualmaxValue = maxValues[26];
                                break;
                        }
                        
                        if (!double.TryParse(textBoxActuelPos.Text, out value) || value < actualminValue || value > actualmaxValue)
                        {
                            MessageBox.Show("Lütfen " + actualminValue + " ile " + actualmaxValue + " arasında bir değer girin!", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            buttonPosKaydet2.Enabled = true;
                            return; // Geçerli bir sayı değil veya min-max aralığı dışındaysa işlemi durdur
                        }
                    }
                }

                textBoxSifirla++;
            }

            // Tüm actual pos textbox'ları doluysa devam et
            if (allTextBoxesFilled)
            {
                textBoxSifirla = 1; // textBoxSifirla'yı sıfırla

                // Diğer altı textbox'ın değerlerini sırasıyla textBoxEOL'lere kaydetme işlemi
                for (int j = myInteger; j <= myInteger + 5; j++)
                {                
                    
                    string textBoxEOLName = "textBoxEOL" + j;
                    TextBox textBoxEOL = Controls.Find(textBoxEOLName, true).FirstOrDefault() as TextBox;

                    string textBoxActuelPosName = "textBoxActuelPos" + textBoxSifirla;
                    TextBox textBoxActuelPos = Controls.Find(textBoxActuelPosName, true).FirstOrDefault() as TextBox;

                    if (textBoxEOL != null && textBoxActuelPos != null)
                    {
                        textBoxEOL.Text = textBoxActuelPos.Text;

                        // TextBoxEOL'e odaklan ve Enter tuşuna bas
                        textBoxEOL.Focus();
                        textBoxEOL.SelectAll();
                        SendKeys.Send("{ENTER}");

                        // textBoxSifirla değişkeninin değerini güncelle
                        textBoxSifirla++;
                    }
                }

                // Arka plan rengini ayarla ve diğer butonların durumunu güncelle
                for (int k = myInteger; k <= myInteger + 6; k++)
                {
                    string textBoxEOLName = "textBoxEOL" + k;
                    TextBox textBoxEOL = Controls.Find(textBoxEOLName, true).FirstOrDefault() as TextBox;

                    if (textBoxEOL != null)
                    {
                        textBoxEOL.BackColor = Color.Snow; // Örnek bir renk ataması
                    }
                }

                OtherButtonsSnow();

            }
        }

        private void buttonActuelPos2_Click(object sender, EventArgs e)
        {
            OtherButtonsSnow();

            buttonPosKaydet2.Enabled = false;

            for (int i = 1; i <= 48; i++)
            {
                string textBoxName = "textBoxEOL" + i;
                TextBox textBox = Controls.Find(textBoxName, true).FirstOrDefault() as TextBox;

                if (textBox != null)
                {
                    textBox.BackColor = Color.Snow;
                }
            }
        }

        private void ButtonsFalseVibr()
        {
            int i = 1;

            for (i = 1; i <= 4; i++)
            {
                string buttonName = "buttonVibrPos" + i;
                Button button = Controls.Find(buttonName, true).FirstOrDefault() as Button;

                if (button != null)
                {
                    button.Enabled = false;
                    button.BackColor = Color.Snow;

                    switch (i)
                    {
                        case 1:

                            for (int j = 55; j <= 60; j++)
                            {
                                string textBoxName = "textBoxEOL" + j;
                                TextBox textBox = Controls.Find(textBoxName, true).FirstOrDefault() as TextBox;

                                if (textBox != null)
                                {
                                    textBox.BackColor = Color.Snow;
                                }
                            }

                            break;

                        case 2:

                            for (int j = 61; j <= 66; j++)
                            {
                                string textBoxName = "textBoxEOL" + j;
                                TextBox textBox = Controls.Find(textBoxName, true).FirstOrDefault() as TextBox;

                                if (textBox != null)
                                {
                                    textBox.BackColor = Color.Snow;
                                }
                            }

                            break;

                        case 3:

                            for (int j = 67; j <= 72; j++)
                            {
                                string textBoxName = "textBoxEOL" + j;
                                TextBox textBox = Controls.Find(textBoxName, true).FirstOrDefault() as TextBox;

                                if (textBox != null)
                                {
                                    textBox.BackColor = Color.Snow;
                                }
                            }

                            break;

                        case 4:

                            for (int j = 73; j <= 78; j++)
                            {
                                string textBoxName = "textBoxEOL" + j;
                                TextBox textBox = Controls.Find(textBoxName, true).FirstOrDefault() as TextBox;

                                if (textBox != null)
                                {
                                    textBox.BackColor = Color.Snow;
                                }
                            }

                            break;
                    }

                }
            }
        }

        private void ButtonsTrueVibr()
        {
            for (int i = 1; i <= 4; i++)
            {
                string buttonName = "buttonVibrPos" + i;
                Button button = Controls.Find(buttonName, true).FirstOrDefault() as Button;

                if (button != null)
                {
                    button.Enabled = true;

                }
            }
        }

        private void VibrTextBoxTrue()
        {
            for (int i = 55; i <= 81; i++)
            {
                string textBoxName = "textBoxEOL" + i;
                TextBox textBox = Controls.Find(textBoxName, true).FirstOrDefault() as TextBox;

                if (textBox != null)
                {
                    textBox.Enabled = true;
                }
            }
        }

        private void VibrTextBoxFalse()
        {
            for (int i = 55; i <= 81; i++)
            {
                string textBoxName = "textBoxEOL" + i;
                TextBox textBox = Controls.Find(textBoxName, true).FirstOrDefault() as TextBox;

                if (textBox != null)
                {
                    textBox.Enabled = false;
                }
            }
        }

        private void RobotTextBoxTrue()
        {
            for (int i = 1; i <= 54; i++)
            {
                string textBoxName = "textBoxEOL" + i;
                TextBox textBox = Controls.Find(textBoxName, true).FirstOrDefault() as TextBox;

                if (textBox != null)
                {
                    textBox.Enabled = true;
                }
            }
        }

        private void RobotTextBoxFalse()
        {
            for (int i = 1; i <= 54; i++)
            {
                string textBoxName = "textBoxEOL" + i;
                TextBox textBox = Controls.Find(textBoxName, true).FirstOrDefault() as TextBox;

                if (textBox != null)
                {
                    textBox.Enabled = false;
                }
            }
        }

        private void buttonVibrRobot_Click(object sender, EventArgs e)
        {
            buttonPosKaydet2.Enabled = false;
            // Bayrağı tersine çevir
            textBoxesActiveVibr = !textBoxesActiveVibr;

            // Duruma göre buton metnini ayarla
            buttonVibrRobot.Text = textBoxesActiveVibr ? "AKTİF" : "PASİF";

            switch (textBoxesActiveVibr)
            {
                case false:
                    buttonVibrRobot.BackColor = Color.DarkRed;
                    break;
                case true:
                    buttonVibrRobot.BackColor = Color.DarkOliveGreen;
                    break;
            }

            if (textBoxesActiveVibr == true)
            {
                // Durum PASİF ise, Eolmakina butonu etkin değil yap
                buttonOperatorCalsma.Text = "PASİF";
                buttonOperatorCalsma.Enabled = false;
                buttonOperatorCalsma.ForeColor = Color.WhiteSmoke;
                buttonOperatorCalsma.BackColor = Color.DarkRed;

                VibrTextBoxTrue();
                ButtonsTrueVibr();
            }
            else
            {
                // Durum AKTİF ise, Eolmakina butonu etkin yap
                buttonOperatorCalsma.Enabled = true;
                buttonOperatorCalsma.ForeColor = Color.WhiteSmoke;

                ButtonsFalseVibr();
                VibrTextBoxFalse();
            }

        }

        private void buttonOperatorCalsma_Click(object sender, EventArgs e)
        {
            // Bayrağı tersine çevir
            textBoxesActiveOperator = !textBoxesActiveOperator;

            // Duruma göre buton metnini ayarla
            buttonOperatorCalsma.Text = textBoxesActiveOperator ? "AKTİF" : "PASİF";

            switch (textBoxesActiveOperator)
            {
                case false:
                    buttonOperatorCalsma.BackColor = Color.DarkRed;
                    buttonVibrRobot.Enabled = true;
                    break;
                case true:
                    buttonOperatorCalsma.BackColor = Color.DarkOliveGreen;
                    buttonVibrRobot.Enabled = false;
                    buttonVibrRobot.BackColor = Color.DarkRed;
                    buttonVibrRobot.Text = "PASİF";
                    break;
            }
        }

        private void OtherVibrButtonsPasif(int i)
        {
            for (int j = 1; j <= 4; j++)
            {
                // Butonun adını oluştur
                string buttonName = "buttonVibrPos" + j;

                // Form içinde bu isimde bir Button kontrolü ara
                Button button = Controls.Find(buttonName, true).FirstOrDefault() as Button;

                // Button bulunduysa, etkinlik durumunu pasif hale getir


                if (button != null)
                {
                    if (button.Name == ("buttonVibrPos" + i))
                        continue;
                    else
                        button.BackColor = Color.Snow;
                }
            }
        }

        private void BackgroungVibrTurquaise(int i)
        {
            // 6 adet textbox'ın arka plan rengini değiştir
            for (int j = 55; j <= 78; j++)
            {
                // Textbox kontrolünün ismini oluştur
                string textBoxName = "textBoxEOL" + j;

                // Form içinde bu isimde bir TextBox kontrolü ara
                TextBox textBox = Controls.Find(textBoxName, true).FirstOrDefault() as TextBox;

                // TextBox bulunduysa, arka plan rengini değiştir
                if (textBox != null)
                {
                    if ((textBox.Name == "textBoxEOL" + i) || (textBox.Name == "textBoxEOL" + (i + 1)) || (textBox.Name == "textBoxEOL" + (i + 2)) || (textBox.Name == "textBoxEOL" + (i + 3)) || (textBox.Name == "textBoxEOL" + (i + 4)) || (textBox.Name == "textBoxEOL" + (i + 5)))
                    {
                        textBox.BackColor = Color.PaleTurquoise;
                    }
                    else
                        textBox.BackColor = Color.Snow;

                }
            }
        }

        private void OtherVibrButtonsSnow()
        {
            for (int j = 1; j <= 4; j++)
            {
                // Butonun adını oluştur
                string buttonName = "buttonVibrPos" + j;

                // Form içinde bu isimde bir Button kontrolü ara
                Button button = Controls.Find(buttonName, true).FirstOrDefault() as Button;

                // Button bulunduysa, etkinlik durumunu pasif hale getir


                if (button != null)
                {
                    button.Name = ("buttonVibrPos" + j);
                    button.BackColor = Color.Snow;
                }
            }
        }

        private void buttonVibrPos1_Click(object sender, EventArgs e)
        {
            buttonVibrPos1.BackColor = Color.PaleTurquoise;
            buttonPosKaydet1.Enabled = true;

            int i = 55;
            OtherVibrButtonsPasif(1);
            BackgroungVibrTurquaise(i);

            myInteger = i;
        }

        private void buttonVibrPos2_Click(object sender, EventArgs e)
        {
            buttonVibrPos2.BackColor = Color.PaleTurquoise;
            buttonPosKaydet1.Enabled = true;

            int i = 61;
            OtherVibrButtonsPasif(2);
            BackgroungVibrTurquaise(i);

            myInteger = i;
        }

        private void buttonVibrPos3_Click(object sender, EventArgs e)
        {
            buttonVibrPos3.BackColor = Color.PaleTurquoise;
            buttonPosKaydet1.Enabled = true;

            int i = 67;
            OtherVibrButtonsPasif(3);
            BackgroungVibrTurquaise(i);

            myInteger = i;
        }

        private void buttonVibrPos4_Click(object sender, EventArgs e)
        {
            buttonVibrPos4.BackColor = Color.PaleTurquoise;
            buttonPosKaydet1.Enabled = true;

            int i = 73;
            OtherVibrButtonsPasif(4);
            BackgroungVibrTurquaise(i);

            myInteger = i;
        }

        double minValueAc;
        double maxValueAc;

        private void buttonPosKaydet1_Click(object sender, EventArgs e)
        {
            foreach (Control control in Controls)
            {
                if (control is TextBox textBox && textBox.Name.StartsWith("textBoxEOL"))
                {
                    
                    
                    textBox.MouseDown += TextBoxEOL_MouseDown;
                }
            }

            int textBoxSifirla = 1;
            
            buttonPosKaydet2.Enabled = false;

            bool allTextBoxesFilled = true; // Tüm actual pos textbox'ları doldurulmuş olarak başlat

            // Diğer altı textbox'ın değerlerini kaydetme işlemi ve actual pos textbox'larının dolu olup olmadığını kontrol etme
            for (int j = myInteger; j <= myInteger + 5; j++)
            {
                string textBoxActuelPosName = "textBoxActuelPosV" + textBoxSifirla;
                TextBox textBoxActuelPosV = Controls.Find(textBoxActuelPosName, true).FirstOrDefault() as TextBox;

                if (textBoxActuelPosV != null)
                {
                    // TextBox dolu mu kontrol et
                    if (string.IsNullOrEmpty(textBoxActuelPosV.Text))
                    {

                        allTextBoxesFilled = false;
                        MessageBox.Show("Lütfen tüm değerleri girin!", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        buttonPosKaydet2.Enabled = true;
                        break; // Eğer bir textbox boşsa döngüyü durdur
                    }
                    else
                    {
                        int index = int.Parse(textBoxActuelPosV.Name.Replace("textBoxActuelPosV", "")) - 1; // Örneğin, textBoxActuelPosV1 ise, index 0 olacak

                        // index değerine göre ilgili min ve max değerlerini                        



                        if (myInteger == 55 || myInteger == 61)
                        {
                            minValueAc = minValues[index];
                            maxValueAc = maxValues[index];
                        }

                        if (myInteger == 67 || myInteger == 73)
                        {
                            minValueAc = minValues[index+6];
                            maxValueAc = maxValues[index+6];
                        }


                        
                        // TextBoxActuelPosV için uygun değeri kontrol et ve uyarı ver
                        double value;                   
                        
                        
                        if (!double.TryParse(textBoxActuelPosV.Text, out value) || value < minValueAc || value > maxValueAc)
                        {
                            MessageBox.Show($"Lütfen {minValueAc} ile {maxValueAc} arasında bir değer girin!", $"Hata: {textBoxActuelPosV.Name}", MessageBoxButtons.OK, MessageBoxIcon.Error); buttonPosKaydet2.Enabled = true;

                            // İlk textbox için uygun bir değer girilmediyse, diğer textboxları kontrol et ve uygunsa temizle
                            if (value >= minValueAc && value <= maxValueAc)
                            {
                                for (int i = myInteger + 1; i <= myInteger + 5; i++)
                                {
                                    TextBox textBoxEOL = Controls.Find("textBoxEOL" + i, true).FirstOrDefault() as TextBox;
                                    if (textBoxEOL != null)
                                    {
                                        textBoxEOL.Text = string.Empty;
                                    }
                                }
                            }

                            return; // Geçerli bir sayı değil veya min-max aralığı dışındaysa işlemi durdur
                        }
                    }
                }

                textBoxSifirla++;
            }

            // Tüm actual pos textbox'ları doluysa devam et
            if (allTextBoxesFilled)
            {
                textBoxSifirla = 1; // textBoxSifirla'yı sıfırla

                // Diğer altı textbox'ın değerlerini sırasıyla textBoxEOL'lere kaydetme işlemi
                for (int j = myInteger; j <= myInteger + 5; j++)
                {
                    string textBoxEOLName = "textBoxEOL" + j;
                    TextBox textBoxEOL = Controls.Find(textBoxEOLName, true).FirstOrDefault() as TextBox;

                    string textBoxActuelPosName = "textBoxActuelPosV" + textBoxSifirla;
                    TextBox textBoxActuelPos = Controls.Find(textBoxActuelPosName, true).FirstOrDefault() as TextBox;

                    if (textBoxEOL != null && textBoxActuelPos != null)
                    {
                        textBoxEOL.Text = textBoxActuelPos.Text;

                        // TextBoxEOL'e odaklan ve Enter tuşuna bas
                        textBoxEOL.Focus();
                        textBoxEOL.SelectAll();
                        SendKeys.Send("{ENTER}");

                        // textBoxSifirla değişkeninin değerini güncelle
                        textBoxSifirla++;
                    }
                }

                // Arka plan rengini ayarla ve diğer butonların durumunu güncelle
                for (int k = myInteger; k <= myInteger + 6; k++)
                {
                    string textBoxEOLName = "textBoxEOL" + k;
                    TextBox textBoxEOL = Controls.Find(textBoxEOLName, true).FirstOrDefault() as TextBox;

                    if (textBoxEOL != null)
                    {
                        textBoxEOL.BackColor = Color.Snow; // Örnek bir renk ataması
                    }
                }

                OtherVibrButtonsSnow();

            }
        }

        private void buttonActuelPos1_Click(object sender, EventArgs e)
        {
            OtherVibrButtonsSnow();

            buttonPosKaydet1.Enabled = false;

            for (int i = 55; i <= 78; i++)
            {
                string textBoxName = "textBoxEOL" + i;
                TextBox textBox = Controls.Find(textBoxName, true).FirstOrDefault() as TextBox;

                if (textBox != null)
                {
                    textBox.BackColor = Color.Snow;
                }
            }
        }

        private void buttonFırınIst3_Click(object sender, EventArgs e)
        {
            // Bayrağı tersine çevir
            textBoxesActiveFırınIst3 = !textBoxesActiveFırınIst3;

            // Duruma göre buton metnini ayarla
            buttonFırınIst3.Text = textBoxesActiveFırınIst3 ? "AKTİF" : "PASİF";

            switch (textBoxesActiveFırınIst3)
            {
                case false:
                    buttonFırınIst3.BackColor = Color.DarkRed;
                    buttonFanIst3.Enabled = false;
                    buttonFanIst3.Text = "PASİF";
                    buttonFanIst3.BackColor = Color.DarkRed;
                    textBoxEOL82.Enabled = false;

                    break;
                case true:
                    buttonFırınIst3.BackColor = Color.DarkOliveGreen;
                    buttonFanIst3.Enabled = true;
                    textBoxEOL82.Enabled = true;
                    break;
            }
        }

        private void buttonFırınIst4_Click(object sender, EventArgs e)
        {
            // Bayrağı tersine çevir
            textBoxesActiveFırınIst4 = !textBoxesActiveFırınIst4;

            // Duruma göre buton metnini ayarla
            buttonFırınIst4.Text = textBoxesActiveFırınIst4 ? "AKTİF" : "PASİF";

            switch (textBoxesActiveFırınIst4)
            {
                case false:
                    buttonFırınIst4.BackColor = Color.DarkRed;
                    buttonFanIst4.Enabled = false;
                    buttonFanIst4.Text = "PASİF";
                    buttonFanIst4.BackColor = Color.DarkRed;

                    textBoxEOL83.Enabled = false;

                    break;
                case true:
                    buttonFırınIst4.BackColor = Color.DarkOliveGreen;
                    buttonFanIst4.Enabled = true;

                    textBoxEOL83.Enabled = true;

                    break;
            }
        }

        private void buttonFırınIst5_Click(object sender, EventArgs e)
        {
            // Bayrağı tersine çevir
            textBoxesActiveFırınIst5 = !textBoxesActiveFırınIst5;

            // Duruma göre buton metnini ayarla
            buttonFırınIst5.Text = textBoxesActiveFırınIst5 ? "AKTİF" : "PASİF";

            switch (textBoxesActiveFırınIst5)
            {
                case false:
                    buttonFırınIst5.BackColor = Color.DarkRed;
                    buttonFanIst5.Enabled = false;
                    buttonFanIst5.Text = "PASİF";
                    buttonFanIst5.BackColor = Color.DarkRed;

                    textBoxEOL84.Enabled = false;
                    break;
                case true:
                    buttonFırınIst5.BackColor = Color.DarkOliveGreen;
                    buttonFanIst5.Enabled = true;

                    textBoxEOL84.Enabled = true;

                    break;
            }
        }

        private void buttonFırınIst6_Click(object sender, EventArgs e)
        {
            // Bayrağı tersine çevir
            textBoxesActiveFırınIst6 = !textBoxesActiveFırınIst6;

            // Duruma göre buton metnini ayarla
            buttonFırınIst6.Text = textBoxesActiveFırınIst6 ? "AKTİF" : "PASİF";

            switch (textBoxesActiveFırınIst6)
            {
                case false:
                    buttonFırınIst6.BackColor = Color.DarkRed;
                    buttonFanIst6.Enabled = false;
                    buttonFanIst6.Text = "PASİF";
                    buttonFanIst6.BackColor = Color.DarkRed;
                    
                    textBoxEOL85.Enabled = false;

                    break;
                case true:
                    buttonFırınIst6.BackColor = Color.DarkOliveGreen;
                    buttonFanIst6.Enabled = true;

                    textBoxEOL85.Enabled = true;
                    break;
            }
        }

        private void buttonFanIst3_Click(object sender, EventArgs e)
        {
            // Bayrağı tersine çevir
            textBoxesActiveFanIst3 = !textBoxesActiveFanIst3;

            // Duruma göre buton metnini ayarla
            buttonFanIst3.Text = textBoxesActiveFanIst3 ? "AKTİF" : "PASİF";

            switch (textBoxesActiveFanIst3)
            {
                case false:
                    buttonFanIst3.BackColor = Color.DarkRed;
                    break;
                case true:
                    buttonFanIst3.BackColor = Color.DarkOliveGreen;
                    break;
            }
        }

        private void buttonFanIst4_Click(object sender, EventArgs e)
        {
            // Bayrağı tersine çevir
            textBoxesActiveFanIst4 = !textBoxesActiveFanIst4;

            // Duruma göre buton metnini ayarla
            buttonFanIst4.Text = textBoxesActiveFanIst4 ? "AKTİF" : "PASİF";

            switch (textBoxesActiveFanIst4)
            {
                case false:
                    buttonFanIst4.BackColor = Color.DarkRed;
                    break;
                case true:
                    buttonFanIst4.BackColor = Color.DarkOliveGreen;
                    break;
            }
        }

        private void buttonFanIst5_Click(object sender, EventArgs e)
        {
            // Bayrağı tersine çevir
            textBoxesActiveFanIst5 = !textBoxesActiveFanIst5;

            // Duruma göre buton metnini ayarla
            buttonFanIst5.Text = textBoxesActiveFanIst5 ? "AKTİF" : "PASİF";

            switch (textBoxesActiveFanIst5)
            {
                case false:
                    buttonFanIst5.BackColor = Color.DarkRed;
                    break;
                case true:
                    buttonFanIst5.BackColor = Color.DarkOliveGreen;
                    break;
            }
        }

        private void buttonFanIst6_Click(object sender, EventArgs e)
        {
            // Bayrağı tersine çevir
            textBoxesActiveFanIst6 = !textBoxesActiveFanIst6;

            // Duruma göre buton metnini ayarla
            buttonFanIst6.Text = textBoxesActiveFanIst6 ? "AKTİF" : "PASİF";

            switch (textBoxesActiveFanIst6)
            {
                case false:
                    buttonFanIst6.BackColor = Color.DarkRed;
                    break;
                case true:
                    buttonFanIst6.BackColor = Color.DarkOliveGreen;
                    break;
            }
        }
        string vibFikstur;

        private void LoadSettingsFromIniAndSendToPLC()
        {
            // INI dosyasından ayarları yükle
            settingsHelper.LoadSettingsFromIniFile();

            string[] first11Values = settingsHelper.First11Values.Take(12).ToArray();
            string[] remainingValues = settingsHelper.First11Values.Skip(12).Take(12).ToArray();

            for (int i = 0; i < first11Values.Length; i++)
            {
                valuesArray[i] = $"VibRobFiksturCalisma[{i}]";
                vibFikstur = $"VibRobFiksturCalisma[{i}]";
                nxCompoletStringWrite(vibFikstur, first11Values[i]);
            }

            /*for (int i = 0; i < remainingValues.Length; i++)
            {
                valuesArray[i + 12] = $"VibRobVibrasyonCalisma[{i + 12}]";
                string vibFikstur = $"VibRobVibrasyonCalisma[{i + 12}]";
                nxCompoletStringWrite(vibFikstur, remainingValues[i]);
            }*/
        }


        private void UrunAyr_Load(object sender, EventArgs e)
        {
            bacgroundWorker.Baslat();

            string modSec = CallModHelper.degerKaydet();

            switch(modSec)
            {
                case "1":
                    buttonModSecim.Text = "OTOMATİK ÇALIŞMA";
                    buttonModSecim.BackColor = Color.DarkOliveGreen;
                    break;
                case "0":
                    buttonModSecim.Text = "MANUEL ÇALIŞMA";
                    buttonModSecim.BackColor = Color.Snow;
                    break;
            }              


            //PLC ye connectionStartı gönder
            connStatus = "ConnectionStart";
            nxCompoletBoolWrite(connStatus, true);

            LoadSettingsFromIniAndSendToPLC();

            SetMinMaxValues();

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

            });


            backgroundThread.IsBackground = true;
            backgroundThread.Start();


            // Çekilen değerleri bir iletişim kutusuna yazdır
            /*string valuesText = string.Join(Environment.NewLine, settingsHelper.First11Values);
            MessageBox.Show(valuesText, "Son Degerler");*/

            VibrTextBoxFalse();
            RobotTextBoxFalse();

            for (int i = 1; i <= 85; i++)
            {
                TextBox textBox = Controls.Find("textBoxEOL" + i, true).FirstOrDefault() as TextBox;
                if (textBox != null)
                {
                    textBox.KeyPress += TextBox_KeyPress;
                }
            }

            for (int j = myInteger; j <= myInteger + 5; j++)
            {
                string textBoxEOLName = "textBoxEOL" + j;
                TextBox textBoxEOL = Controls.Find(textBoxEOLName, true).FirstOrDefault() as TextBox;

                if (textBoxEOL != null)
                {
                    textBoxEOL.TextChanged += TextBoxEOL_TextChanged; // TextChanged olayına olay dinleyicisi ekle
                    textBoxEOL.Enter += TextBoxEOL_Enter; // Enter olayına olay dinleyicisi ekle
                }
            }
        }
        private void TextBoxEOL_TextChanged(object sender, EventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            HandleTextBoxChange(textBox);
        }

        private void TextBoxEOL_Enter(object sender, EventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            textBox.SelectAll(); // TextBox içindeki metni seç
            textBox.Focus(); // TextBox'a odaklan
        }

        private void HandleTextBoxChange(TextBox textBox)
        {
            if (textBox.Text.Length == textBox.MaxLength)
            {
                SendKeys.Send("{ENTER}");
            }
        }

        private void TextBox_KeyPress(object sender, KeyPressEventArgs e)
        {

            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && e.KeyChar != ',' && e.KeyChar != '-')
            {
                e.Handled = true; // Harf girilmesini engelle
            }
        }

        private void buttonKonvSistmfrUrnAyr_Click(object sender, EventArgs e)
        {

            KonveyorSistem konveyorSistemForm = KonveyorSistem.GetInstance();

            konveyorSistemForm.Show();

            HideForm();
        }

        private void buttonAnaSyfafrUrnAyr_Click(object sender, EventArgs e)
        {
            Anasayfa anaSayfaForm = Anasayfa.GetInstance();
            // Mevcut form örneğini al
            anaSayfaForm.ShowForm(); // UrunAyr formunu göster

            HideForm();
        }

        private void buttonUrnAyrfrUrnAyr_Click(object sender, EventArgs e)
        {

        }

        private void buttonEOLGrup1_Click(object sender, EventArgs e)
        {
            // Bayrağı tersine çevir
            textBoxesActiveGrup1 = !textBoxesActiveGrup1;

            // Duruma göre buton metnini ayarla
            buttonEOLGrup1.Text = textBoxesActiveGrup1 ? "AKTİF" : "PASİF";

            switch (textBoxesActiveGrup1)
            {
                case false:
                    buttonEOLGrup1.BackColor = Color.DarkRed;
                    break;
                case true:
                    buttonEOLGrup1.BackColor = Color.DarkOliveGreen;
                    break;
            }
        }

        private void buttonEOLGrup2_Click(object sender, EventArgs e)
        {
            // Bayrağı tersine çevir
            textBoxesActiveGrup2 = !textBoxesActiveGrup2;

            // Duruma göre buton metnini ayarla
            buttonEOLGrup2.Text = textBoxesActiveGrup2 ? "AKTİF" : "PASİF";

            switch (textBoxesActiveGrup2)
            {
                case false:
                    buttonEOLGrup2.BackColor = Color.DarkRed;
                    break;
                case true:
                    buttonEOLGrup2.BackColor = Color.DarkOliveGreen;
                    break;
            }
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

        private void buttonSolVakum_Click(object sender, EventArgs e)
        {
            nxCompoletBoolWrite("EolRobManuel[0]", true);

        }

        private void buttonSagVakum_Click(object sender, EventArgs e)
        {
            nxCompoletBoolWrite("EolRobManuel[1]", true);
        }

        private void ManuelKontrol()
        {
            string modSec = CallModHelper.degerKaydet();

            switch (modSec)
            {
                case "1":
                    buttonModSecim.Text = "OTOMATİK ÇALIŞMA";
                    buttonModSecim.BackColor = Color.DarkOliveGreen;
                    break;
                case "0":
                    buttonModSecim.Text = "MANUEL ÇALIŞMA";
                    buttonModSecim.BackColor = Color.Snow;
                    break;
            }
        }


    string solReadSens, sagReadSens;
        public void ReadSensor()
        {
            solReadSens = nxCompoletStringRead("EolRobDonanim[1]");

            if (solReadSens == "1")
                buttonSolVakumSensor.BackColor = Color.LightGreen;
            else
                buttonSolVakumSensor.BackColor = Color.LightCoral;

            sagReadSens = nxCompoletStringRead("EolRobDonanim[2]");

            if (sagReadSens == "1")
                buttonSagVakumSensor.BackColor = Color.LightGreen;
            else
                buttonSagVakumSensor.BackColor = Color.LightCoral;

        }

        private void buttonModSecim_Click(object sender, EventArgs e)
        {
            string result = CallModHelper.ToggleCalMod((Button)sender);

            nxCompoletStringWrite(CalısmaMod, result);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            this.DoubleBuffered = true;

            Sifre sifreAyr = new Sifre();
            formManager.ShowForm(sifreAyr);
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

        private void btnMin_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void exit_Click(object sender, EventArgs e)
        {
            bacgroundWorker.Durdur();
            Application.Exit();
        }

        public string nxCompoletDoubleRead(string variable)  //NX STRING
        {
            try
            {
                string s = Convert.ToString(nxCompolet1.ReadVariable(variable));
                return s;
            }
            catch (Exception e)
            {
                // otherConsoleAppendLine("nxCompolet Hatası" + "\nKonum : DoubleRead" + "\nvariable = " + variable, Color.Red);
                return "-1";
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
    }
}