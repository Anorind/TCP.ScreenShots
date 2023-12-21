using System;
using System.Drawing;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices.ComTypes;
using System.Threading;
using System.Windows.Forms;

namespace TCP.ScreenShots
{
    public partial class Form1 : Form
    {
        private TcpListener server;
        private Thread thread;

        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                server = new TcpListener(IPAddress.Any, 8001);
                server.Start();

                thread = new Thread(() =>
                {
                    while (true)
                    {
                        TcpClient client = server.AcceptTcpClient();
                        NetworkStream stream = client.GetStream();

                        Thread thread = new Thread(() =>
                        {
                            try
                            {
                                while (true)
                                {
                                    byte[] bytesToRead = new byte[client.ReceiveBufferSize];
                                    int bytesRead = stream.Read(bytesToRead, 0, client.ReceiveBufferSize);

                                    using (MemoryStream memoryStream = new MemoryStream(bytesToRead, 0, bytesRead))
                                    {
                                        Bitmap screenshot = new Bitmap(memoryStream);
                                        pictureBox1.Image = null; 
                                        pictureBox1.Image = screenshot; 
                                    }

                                }
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine("Error: " + ex.Message);
                            }
                            finally
                            {
                                client.Close();
                            }
                        });
                        thread.IsBackground = true;
                        thread.Start();
                    }
                });
                thread.IsBackground = true;
                thread.Start();
            }
            catch(Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            server.Stop();
            Application.Exit();
        }
    }
}