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
using Microsoft.Win32;
using System.Net.Sockets;
using System.Net;
using System.Threading;

namespace Socket_
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            ImageFromFile();
            SecondImageFromFile();

            label3.Visible = false;
            label4.Visible = false;

        }

        Bitmap image1;
        Bitmap image2;
        static Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        async public void ImageFromFile()
        {
            OpenFileDialog open_dialog = new OpenFileDialog(); //создание диалогового окна для выбора файла
            open_dialog.Filter = "Image Files(*.BMP;*.JPG;*.GIF;*.PNG)|*.BMP;*.JPG;*.GIF;*.PNG|All files (*.*)|*.*"; //формат загружаемого файла
            if (open_dialog.ShowDialog() == DialogResult.OK) //если в окне была нажата кнопка "ОК"
            {
                try
                {
                    image1 = new Bitmap(open_dialog.FileName);
                    //вместо pictureBox1 укажите pictureBox, в который нужно загрузить изображение 
                    this.pictureBox1.Size = image1.Size;
                    pictureBox1.Image = image1;
                    pictureBox1.Invalidate();
                }
                catch
                {
                    DialogResult rezult = MessageBox.Show("Невозможно открыть выбранный файл",
                    "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        async public void SecondImageFromFile()
        {
            OpenFileDialog open_dialog = new OpenFileDialog(); //создание диалогового окна для выбора файла
            open_dialog.Filter = "Image Files(*.BMP;*.JPG;*.GIF;*.PNG)|*.BMP;*.JPG;*.GIF;*.PNG|All files (*.*)|*.*"; //формат загружаемого файла
            if (open_dialog.ShowDialog() == DialogResult.OK) //если в окне была нажата кнопка "ОК"
            {
                try
                {
                    image2 = new Bitmap(open_dialog.FileName);
                    //вместо pictureBox1 укажите pictureBox, в который нужно загрузить изображение 
                    this.pictureBox3.Size = image2.Size;
                    pictureBox3.Image = image2;
                    pictureBox3.Invalidate();
                }
                catch
                {
                    DialogResult rezult = MessageBox.Show("Невозможно открыть выбранный файл",
                    "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

        }

        async public void ChangeColorFirstImage()
        {
            int count = 0;
            for (int y = 0; y < image1.Height; ++y)
            {
                for (int x = 0; x < image1.Width; ++x)
                {
                    image1.GetPixel(x, y);
                    checked
                    {
                        Color pixel = image1.GetPixel(x, y);
                        image1.SetPixel(x, y, Color.FromArgb((int)(pixel.R * 0.5), (int)(pixel.G * 0.5), (int)(pixel.B * 0.5)));
                    }
                    label1.Invoke((MethodInvoker)(() => label1.Text = image1.GetPixel(x, y).ToString()));
                    label4.Invoke((MethodInvoker)(() => label4.Text = count.ToString()));
                }
            }
         
            pictureBox1.Image = image1;
        }

        async public void ChangeColorSecondImage()
        {
            for (int y = 0; y < image2.Height; ++y)
            {
                for (int x = 0; x < image2.Width; ++x)
                {
                    image2.GetPixel(x, y);
                    checked
                    {
                        Color pixel = image2.GetPixel(x, y);
                        image2.SetPixel(x, y, Color.FromArgb((int)(pixel.R * 0.5), (int)(pixel.G * 0.5), (int)(pixel.B * 0.5)));
                    }
                    label2.Invoke((MethodInvoker)(() => label2.Text = image2.GetPixel(x, y).ToString()));
                    pictureBox3.Image = null;
                }

                pictureBox3.Image = image2;

            }
        }

        public void AddToRegistryBlackPixelsFirstImage()
        {
            int count = 0;
            string str = "BlackPixelsSecondImage";
            for (int y = 0; y < image2.Height; ++y)
            {
                for (int x = 0; x < image2.Width; ++x)
                {
                    if (image2.GetPixel(x, y) == Color.FromArgb(0, 0, 0))
                    {
                        ++count;
                    }
                    
                }
            }
            AddToregistry(count, str);
            Socket(count);
        }

        public void AddToRegistryBlackPixelsSecondImage()
        {
            string str = "BlackPixelsFirstImage";
            int count = 0;
            for (int y = 0; y < image2.Height; ++y)
            {
                for (int x = 0; x < image2.Width; ++x)
                {
                    label1.Invoke((MethodInvoker)(() => label1.Text = image1.GetPixel(x, y).ToString()));
                    pictureBox1.Image = null;
                    if (image1.GetPixel(x, y) == Color.FromArgb(0, 0, 0))
                    {
                        ++count;
                    }
                   
                }
            }
            AddToregistry(count, str);
            Socket(count);
        }

        public void AddToregistry(int count, string str)
        {
            RegistryKey currentUserKey = Registry.CurrentUser;
            RegistryKey helloKey = currentUserKey.CreateSubKey("PixelKey", true);
            RegistryKey subHelloKey = helloKey.CreateSubKey("SubPixelKey");
            subHelloKey.SetValue(str, count);
            subHelloKey.Close();
            helloKey.Close();
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            await Task.Delay((int)0.1);

            Thread thread1 = new Thread(ChangeColorFirstImage);
            Thread thread2 = new Thread(ChangeColorSecondImage);

            thread1.Start();
            thread2.Start();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            AddToRegistryBlackPixelsFirstImage();
            AddToRegistryBlackPixelsSecondImage();
        }

        public static void Socket(object count)
        {
            socket.Connect("127.0.0.1", 904);
            string message = count.ToString();
            byte[] buffer = Encoding.ASCII.GetBytes(message);
            socket.Send(buffer);
            Console.ReadLine();
            socket.Disconnect(true);
        }
    }
}
