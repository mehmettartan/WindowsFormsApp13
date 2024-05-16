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
    public partial class Sifre : Form
    {
        public Sifre()
        {
            InitializeComponent();
        }

        private void textBox1_Click(object sender, EventArgs e)
        {
            textBox1.Text = "admin";
        }

        private void textBox2_Click(object sender, EventArgs e)
        {
            textBox2.Text = "";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox1.Text == "" || textBox2.Text == "")
            {
                MessageBox.Show("Kullanıcı Adı ve/veya Parola boş geçilemez!");
            }

            else
            {
                if (textBox1.Text =="admin" && textBox2.Text =="1234")
                {
                   /* SistemAyar sistemAyr = new SistemAyar();
                    sistemAyr.Show();*/

                    // Şifre doğru ise "Ürün Ayarları" formunu aç
                    FormManager formManager = new FormManager();
                    formManager.ShowForm(new SistemAyar());
                    this.Close(); // Şifre formunu kapat
                }
                else
                {
                    MessageBox.Show("Kullanıcı Adı ve/veya Parola hatalı!");
                }
            }
        }
    }
}
