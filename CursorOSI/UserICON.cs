using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CursorOSI
{
    public partial class UserICON : Form
    {
        public List<string> Result = new List<string>();
        private List<string> images;
        public UserICON(List<string> images)
        {
            this.images = images;
            InitializeComponent();
            label1.Text = "";
            label2.Text = "";
        }

        private string selectFile;
        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.Filter = "图片文件|*.png;*.cur;*.ani;*.ico";
            fileDialog.Multiselect = false;
            if (fileDialog.ShowDialog() == DialogResult.OK)
            {
                selectFile = fileDialog.FileName;
                FileInfo info = new FileInfo(fileDialog.FileName);
                textBox1.Text = info.Name.Replace(info.Extension, "");
                if (images.Contains(textBox1.Text))
                {
                    MessageBox.Show("当前文件名已存在，请修改文件名！","文件名已存在",MessageBoxButtons.OK,MessageBoxIcon.Error);
                }
                pictureBox1.Image = Image.FromFile(fileDialog.FileName);
                label1.Text = "宽度：50";
                label2.Text = $"高度：{pictureBox1.Image.Height / (pictureBox1.Image.Width / 50)}";
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if (textBox1.Text!="")
            {
                if (images.Contains(textBox1.Text))
                {
                    MessageBox.Show("当前文件名已存在，请修改文件名！", "文件名已存在", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    textBox1.BackColor = Color.Red;
                }
                else
                {
                    textBox1.BackColor = Color.White;
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Result.Add(selectFile);
            this.Hide();
        }

        private void UserICON_Load(object sender, EventArgs e)
        {
            this.Icon = CursorOSI.Properties.Resources.icon;
        }
    }
}
