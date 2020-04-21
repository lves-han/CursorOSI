using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CursorOSI.Properties;

namespace CursorOSI
{
    public partial class MainForm : Form
    {
        private DataTable imagesTable = new DataTable();
        public MainForm()
        {
            InitializeComponent();
        }
        private Dictionary<string, string> images = new Dictionary<string, string>();
        private void MainForm_Load(object sender, EventArgs e)
        {
            imagesTable.Columns.Add("imageName", typeof(string));
            imagesTable.Columns.Add("extension", typeof(string));
            imagesTable.Columns.Add("fullPath", typeof(string));
            this.Icon = Resources.icon;
            this.notifyIcon1.Icon = Resources.icon;
            this.Show();
            Resources res = new Resources();
            PropertyInfo[] peoperInfo = res.GetType().GetProperties(BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Instance);

            if (!Directory.Exists(Application.StartupPath + "\\img"))
            {
                Directory.CreateDirectory("img");
                foreach (PropertyInfo pro in peoperInfo)
                {
                    if (pro.PropertyType.BaseType.FullName == "System.Drawing.Image")
                    {
                        if (pro.Name.Contains("icon"))
                        {
                            continue;
                        }



                        Image img = (Image)Resources.ResourceManager.GetObject(pro.Name);

                        int height = 50;
                        int width = img.Width / (img.Height / 50);

                        using (Bitmap iconBm = new Bitmap(img, height, width))
                        {
                            //如果是windows调用，直接下面一行代码就可以了
                            //此代码不能在web程序中调用，会有安全异常抛出
                            using (Icon icon = ConvertToIcon(iconBm))
                            {
                                string Name_ = Application.StartupPath + $"/img/{pro.Name}.cur";
                                images.Add(pro.Name, Name_);
                                using (Stream stream = new System.IO.FileStream(Name_, System.IO.FileMode.Create))
                                {
                                    icon.Save(stream);
                                }
                            }
                        }
                        imagesTable.Rows.Add(new[] { pro.Name, "cur", Application.StartupPath + $"/img/{pro.Name}.cur" });
                    }

                }
            }
            else
            {
                string[] files = Directory.GetFiles(Application.StartupPath + "\\img");
                foreach (string file in files)
                {
                    FileInfo info = new FileInfo(file);
                    imagesTable.Rows.Add(new[] { info.Name.Replace(info.Extension, ""), info.Extension.Replace(".", ""), file });
                    images.Add(info.Name.Replace(info.Extension, ""), file);
                }
            }

            List<string> imgs = new List<string>(images.Keys);
            comboBox1.Items.AddRange(imgs.ToArray());
        }


        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            pictureBox1.Image = Image.FromFile(Application.StartupPath + "\\img\\" + comboBox1.Text + ".cur");
        }

        private void comboBox1_TextChanged(object sender, EventArgs e)
        {
            if (images.ContainsKey(comboBox1.Text))
            {
                pictureBox1.Image = Image.FromFile(images[comboBox1.Text]);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (!images.ContainsKey(comboBox1.Text))
            {
                return;
            }
            IntPtr iP = LoadCursorFromFile(Application.StartupPath + "\\img\\" + comboBox1.Text + ".cur");
            bool res = SetSystemCursor(iP, OCR_NORMAL);
            if (res)
            {
                MessageBox.Show("开启成功","成功",MessageBoxButtons.OK,MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("开启失败", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            SystemParametersInfo(SPI_SETCURSORS, 0, IntPtr.Zero, SPIF_SENDWININICHANGE);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            List<string> lists = new List<string>(images.Keys);
            UserICON userIcon = new UserICON(lists);
            userIcon.ShowDialog();
            if (userIcon.Result.Count==0)
            {
                userIcon.Close();
                userIcon.Dispose();
                return;
            }
            
            FileInfo info = new FileInfo(userIcon.Result[0]);

            if (info.Extension.ToLower() == ".png")
            {

                Image img = Image.FromFile(userIcon.Result[0]);
                int height = 50;
                int width = img.Width / (img.Height / 50);

                using (Bitmap iconBm = new Bitmap(img, height, width))
                {
                    //如果是windows调用，直接下面一行代码就可以了
                    //此代码不能在web程序中调用，会有安全异常抛出
                    using (Icon icon = ConvertToIcon(iconBm))
                    {
                        string Name_ = Application.StartupPath + $"/img/{userIcon.textBox1.Text}.cur";
                        images.Add(userIcon.textBox1.Text, Name_);
                        using (Stream stream = new System.IO.FileStream(Name_, System.IO.FileMode.Create))
                        {
                            icon.Save(stream);
                        }
                    }
                }
                imagesTable.Rows.Add(new[] { userIcon.textBox1.Text, "cur", Application.StartupPath + $"/img/{userIcon.textBox1.Text}.cur" });
            }
            else
            {
                File.Copy(userIcon.Result[0], Application.StartupPath + $"/img/{userIcon.textBox1.Text}{info.Extension}");
                imagesTable.Rows.Add(new[] { userIcon.textBox1.Text, info.Extension.Replace(".", ""), Application.StartupPath + $"/img/{userIcon.textBox1.Text}{info.Extension}" });
            }
            imagesTable.WriteXml(Application.StartupPath + "\\ImageList.xml");
            images.Clear();
            foreach (DataRow dr in imagesTable.Rows)
            {
                images.Add(dr[0].ToString(), dr[1].ToString());
            }
            comboBox1.Items.Clear();
            List<string> imgs = new List<string>(images.Keys);
            comboBox1.Items.AddRange(imgs.ToArray());
            userIcon.Close();
            userIcon.Dispose();
        }
        [DllImport("user32")]
        private static extern IntPtr LoadCursorFromFile(string fileName);

        [DllImport("User32.DLL")]
        public static extern bool SetSystemCursor(IntPtr hcur, uint id);
        public const uint OCR_NORMAL = 32512;

        [DllImport("User32.DLL")]
        public static extern bool SystemParametersInfo(uint uiAction, uint uiParam, IntPtr pvParam, uint fWinIni);

        public const uint SPI_SETCURSORS = 87;
        public const uint SPIF_SENDWININICHANGE = 2;

        /// <summary>
        /// 转换Image为Icon
        /// </summary>
        /// <param name="image">要转换为图标的Image对象</param>
        /// <param name="nullTonull">当image为null时是否返回null。false则抛空引用异常</param>
        /// <exception cref="ArgumentNullException" />
        public static Icon ConvertToIcon(Image image, bool nullTonull = false)
        {
            if (image == null)
            {
                if (nullTonull) { return null; }
                throw new ArgumentNullException("image");
            }

            using (MemoryStream msImg = new MemoryStream(), msIco = new MemoryStream())
            {
                image.Save(msImg, ImageFormat.Png);

                using (var bin = new BinaryWriter(msIco))
                {
                    //写图标头部
                    bin.Write((short)0);           //0-1保留
                    bin.Write((short)1);           //2-3文件类型。1=图标, 2=光标
                    bin.Write((short)1);           //4-5图像数量（图标可以包含多个图像）

                    bin.Write((byte)image.Width);  //6图标宽度
                    bin.Write((byte)image.Height); //7图标高度
                    bin.Write((byte)0);            //8颜色数（若像素位深>=8，填0。这是显然的，达到8bpp的颜色数最少是256，byte不够表示）
                    bin.Write((byte)0);            //9保留。必须为0
                    bin.Write((short)0);           //10-11调色板
                    bin.Write((short)32);          //12-13位深
                    bin.Write((int)msImg.Length);  //14-17位图数据大小
                    bin.Write(22);                 //18-21位图数据起始字节

                    //写图像数据
                    bin.Write(msImg.ToArray());

                    bin.Flush();
                    bin.Seek(0, SeekOrigin.Begin);
                    return new Icon(msIco);
                }
            }
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            SystemParametersInfo(SPI_SETCURSORS, 0, IntPtr.Zero, SPIF_SENDWININICHANGE);
        }

        private void 退出ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SystemParametersInfo(SPI_SETCURSORS, 0, IntPtr.Zero, SPIF_SENDWININICHANGE);
        }

        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            this.Show();
            this.WindowState = FormWindowState.Normal;
        }

        private void MainForm_SizeChanged(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Normal)
            {
                this.notifyIcon1.Visible = false;
            }

            if (this.WindowState == FormWindowState.Minimized)
            {
                this.Hide();
                this.notifyIcon1.Visible = true;
            }
        }
    }
}
