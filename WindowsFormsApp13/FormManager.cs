using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp13
{
    class FormManager
    {
        private Form currentForm;

        public void ShowForm(Form form)
        {
            if (currentForm != null)
            {
                currentForm.Hide();
            }

            currentForm = form;
            currentForm.Show();
        }
    }
}
