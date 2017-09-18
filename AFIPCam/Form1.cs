using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics.Eventing.Reader;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using AForge.Video;

namespace AFIPCam
{
    public partial class Form1 : Form
    {
        private MJPEGStream stream;
        private Byte [] buffer;
        private int total = 0;
        private int read;

        public Form1()
        {
            InitializeComponent();

            // Link to IP Cam feed
            var sourceURL = "http://10.128.1.31/Streaming/Channels/1/picture";
            //var sourceURL = "http://88.53.197.250/axis-cgi/mjpg/video.cgi?resolution=320×240";           

            stream = new MJPEGStream(sourceURL);
            stream.Login = "admin";
            stream.Password = "admin1234";
            stream.NewFrame += stream_NewFrame;

            buffer = new byte[100000];


            // Create HTTP request
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(sourceURL);

            // Provide credentials for connection
            req.Credentials = new NetworkCredential("admin", "admin1234");

            // Get response
            WebResponse resp = req.GetResponse();

            // Get response stream
            Stream streamer = resp.GetResponseStream();

            // Read data from stream
            while ((read = streamer.Read(buffer, total, 1000)) != 0)
            {
                total = total + read;
            }
        }

        void stream_NewFrame(object sender, NewFrameEventArgs eventArgs)
        {
            //Bitmap bmp = (Bitmap)eventArgs.Frame.Clone();
            //pictureBox1.Image = bmp;


        }

        private void button_Start_Click(object sender, EventArgs e)
        {
            
            try
            {
                stream.Start();

                // Set image to picturebox
                Bitmap bmp = (Bitmap)Bitmap.FromStream(
                    new MemoryStream(buffer, 0, total));

                pictureBox1.Image = bmp;
                //pictureBox1.Invalidate();
                //pictureBox1.Refresh();
                //pictureBox1.Update();

                MessageBox.Show("Streaming Started!");
            }
            catch (Exception ex)
            {
                MessageBox.Show("" + ex);
                throw;
            }          
            pictureBox1.Refresh();
            pictureBox1.Invalidate();
            Thread.Sleep(1000);
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                
                if (MessageBox.Show("Are you sure?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    // user clicked yes
                    stream.Stop();
                    this.Dispose();
                }
                else
                {
                    // user clicked no
                    stream.Start();
                    

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("" + ex);
                throw;
            }

        }

        private void button_Stop_Click(object sender, EventArgs e)
        {
            try
            {
                stream.Stop();
                MessageBox.Show("Streaming Stopped!");
            }
            catch (Exception ex)
            {
                MessageBox.Show("" + ex);
                throw;
            }

        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            stream.Start();
        }
    }
}
