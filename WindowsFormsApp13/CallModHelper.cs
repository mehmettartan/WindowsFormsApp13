using System;
using System.Collections.Generic;
using System.Drawing; // Renk türü için gerekli namespace
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms; // Button türü için gerekli namespace

namespace WindowsFormsApp13
{
    class CallModHelper
    {
        private static string valueToWrite;
        public static string ToggleCalMod(Button button)
        {
            // Değişkenin değerine göre ilgili değeri yazın
            bool calModValue = (button.Tag as bool?) ?? false;
            calModValue = !calModValue;
            button.Tag = calModValue;

            valueToWrite = calModValue ? "1" : "0";

            switch (valueToWrite)
            {
                case "0":
                    button.Text = "MANUEL ÇALIŞMA";
                    button.BackColor = Color.Snow;
                    break;
                case "1":
                    button.Text = "OTOMATİK ÇALIŞMA";
                    button.BackColor = Color.DarkOliveGreen;
                    break;
            }

            return valueToWrite;
            degerKaydet();

            
        }

        public static string degerKaydet()
        {
            return valueToWrite;
        }
    }
}