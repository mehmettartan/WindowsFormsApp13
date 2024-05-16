using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
public class SettingsHelper : Form
{
    public List<string> First11Values { get; private set; } = new List<string>(Enumerable.Repeat("", 60));
    public List<string> First85Values { get; private set; } = new List<string>(Enumerable.Repeat("", 100));
    public void LoadSettingsFromIniFile()
    {
        try
        {
            string iniFolderPath = Path.Combine(Application.StartupPath, "Models");
            string iniFileName = Path.Combine(iniFolderPath, "settings.ini");

            if (File.Exists(iniFileName))
            {
                using (StreamReader reader = new StreamReader(iniFileName))
                {
                    List<string> settingsList = new List<string>();
                    string line;
                    int count = 0;
                    while ((line = reader.ReadLine()) != null && count < 60)
                    {
                        string[] parts = line.Split('=');
                        if (parts.Length == 2)
                        {
                            string textBoxName = parts[0];
                            string textBoxValue = parts[1];
                            First11Values[count] = textBoxValue;
                            count++;
                        }
                    }
                }
            }
            else
            {
                MessageBox.Show("INI dosyası bulunamadı.");
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show("İşlem sırasında bir hata oluştu: " + ex.Message);
        }
    }

    public void LoadUrunAyar(string modelAd)
    {
        

        try
        {
            string iniFolderPath = Path.Combine(Application.StartupPath, "Models");
            // Klasördeki tüm dosyaları listele
            //string[] iniFileName = Directory.GetFiles(iniFolderPath);

            string iniFileName = Path.Combine(iniFolderPath, modelAd+".ini");

            // Dosyaların oluşturulma tarihine göre sırala ve en son dosyayı seç
            //string latestFile = iniFileName.OrderByDescending(f => new FileInfo(f).CreationTime).FirstOrDefault();

            if (File.Exists(iniFileName))
            {
                using (StreamReader reader = new StreamReader(iniFileName))
                {
                    List<string> settingsList = new List<string>();
                    string line;
                    int count = 0;
                    while ((line = reader.ReadLine()) != null && count < 100)
                    {
                        string[] parts = line.Split('=');
                        if (parts.Length == 2)
                        {
                            string textBoxValue = parts[1].Trim();
                            First85Values[count] = textBoxValue;
                            count++;
                        }
                    }

                    int length = First85Values.Count;
                }
            }
            else
            {
                MessageBox.Show("INI dosyası bulunamadı.");
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show("İşlem sırasında bir hata oluştu: " + ex.Message);
        }

    }
}