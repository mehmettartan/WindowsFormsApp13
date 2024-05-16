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

namespace WindowsFormsApp13
{


    public partial class SistemAyar : Form
    {

        private SettingsHelper settingsHelper = new SettingsHelper();
        public SistemAyar()
        {
            InitializeComponent();
            InitializeSettingsManager();

        }

        private void InitializeSettingsManager()
        {
            try
            {
                // INI dosyasından okunan verileri alın
                string exeDirectory = Path.GetDirectoryName(Application.ExecutablePath);
                string iniFolderPath = Path.Combine(exeDirectory, "Models");
                //string iniFolderPath = Path.Combine(Application.StartupPath, "Models");
                string iniFileName = Path.Combine(iniFolderPath, "settings.ini");

                if (File.Exists(iniFileName))
                {
                    using (StreamReader reader = new StreamReader(iniFileName))
                    {
                        string line;
                        while ((line = reader.ReadLine()) != null)
                        {
                            string[] parts = line.Split('=');
                            if (parts.Length == 2)
                            {
                                string textBoxName = parts[0];
                                string textBoxValue = parts[1];

                                // Ekrandaki ilgili MaskedTextBox'i bulun ve değerini ayarlayın
                                MaskedTextBox textBox = Controls.Find(textBoxName, true).FirstOrDefault() as MaskedTextBox;
                                if (textBox != null)
                                {
                                    textBox.Text = textBoxValue;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("İşlem sırasında bir hata oluştu: " + ex.Message);
            }
        }

        string iniKaydet = "IniKaydet";
        string vibFikstur = "";
        private void buttonKaydet_Click(object sender, EventArgs e)
        {
            SaveToIniFile();

            settingsHelper.LoadSettingsFromIniFile();

            

            for (int i = 0; i < 60; i++)
            {
                vibFikstur = $"IniDegisken[{i}]";
                nxCompoletStringWrite(vibFikstur, settingsHelper.First11Values[i]);
            }

            nxCompoletBoolWrite(iniKaydet, true);

            this.Close();
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

        private void SaveToIniFile()
        {
            try
            {
                // .ini dosyasına kaydet
                string iniFolderPath = Path.Combine(Application.StartupPath, "Models");
                Directory.CreateDirectory(iniFolderPath); // Klasörü oluştur

                string iniFileName = Path.Combine(iniFolderPath, "settings.ini"); // .ini dosyasının tam yolunu oluştur

                using (StreamWriter writer = new StreamWriter(iniFileName))
                {
                    for (int i = 1; i <= 60; i++)
                    {
                        string textBoxName = "textBoxAyar" + i;
                        MaskedTextBox textBox = Controls.Find(textBoxName, true).FirstOrDefault() as MaskedTextBox;

                        if (textBox != null)
                        {
                            // Her bir textBox'teki değeri .ini dosyasına yaz
                            string line = $"{textBox.Name}={textBox.Text}";
                            writer.WriteLine(line);
                        }
                        else
                        {
                            MessageBox.Show($"TextBox '{textBoxName}' bulunamadı veya doğru türde değil.");
                        }
                    }
                }

                MessageBox.Show(".ini dosyası oluşturuldu ve kaydedildi.");
            }
            catch (Exception ex)
            {
                MessageBox.Show("İşlem sırasında bir hata oluştu: " + ex.Message);
            }
        }

        private void SistemAyar_Load(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Maximized;
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
    }
}
