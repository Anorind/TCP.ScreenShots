using System;
using System.Drawing;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Windows.Forms;

namespace TCP.ScreenShots.Client
{
    public partial class Form2 : Form
    {
        private TcpClient client;
        private NetworkStream stream;
        private Thread thread;

        public Form2()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            IPAddress ipAddress = IPAddress.Parse("127.0.0.1");
            IPEndPoint ipEndPoint = new IPEndPoint(ipAddress, 8001);

            client = new TcpClient();
            client.Connect(ipEndPoint);
            stream = client.GetStream();

            thread = new Thread(() =>
            {
                try
                {
                    while (true)
                    {
                        MessageBox.Show($"Отправка нового скриншота");
                        using (Bitmap screenshot = new Bitmap(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height))
                        {
                            Graphics graphics = Graphics.FromImage(screenshot);
                            graphics.CopyFromScreen(0, 0, 0, 0, screenshot.Size);

                            using (MemoryStream memoryStream = new MemoryStream())
                            {
                                screenshot.Save(memoryStream, System.Drawing.Imaging.ImageFormat.Png);
                                byte[] bytesToSend = memoryStream.ToArray();

                                stream.Write(bytesToSend, 0, bytesToSend.Length);

                                MessageBox.Show($"Отправлено {memoryStream.Length} байт");
                            }

                            MessageBox.Show($"Скриншот отправлен");
                        }
                            Thread.Sleep(5000);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error: " + ex.Message);
                }
                finally 
                { 
                    stream.Close();
                }
            });
            thread.IsBackground = true;
            thread.Start();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            stream.Close();
            client.Close();
            Application.Exit();
        }
    }
}

