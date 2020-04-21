using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CursorOSI
{
    public partial class UserICON : Form
    {
        public List<string> Result = new List<string>();
        public UserICON()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Result.Clear();
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.Filter = "图片文件|*.png;光标文件|*.cur|动态光标文件|*.ani";
            fileDialog.Multiselect = true;
            if (fileDialog.ShowDialog() == DialogResult.OK)
            {
                Result.AddRange(fileDialog.FileNames);
            }
        }
    }
}
