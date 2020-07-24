using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MjpegProcessor;
using System.Threading;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ProgressBar;
using System.IO;
using System.Drawing.Imaging;

namespace WindowsFormsApplication2
{
    
    public partial class Form1 : Form
    {
        #region declarations
        Label lb = new Label();
        Label lb1 = new Label();
        Label lb2 = new Label();
        Label lb3 = new Label();
        bool isDrag = false;
        int px = 10;
        int py = 10;
        //Label lb4 = new Label();
        //Label lb5 = new Label();
        //Label lb6 = new Label();
        //Label lb7 = new Label();
        bool isMonitor = false;
        MjpegDecoder mjp;
        bool TaskRun = false;
        bool FlagTakePicture = false;
        bool FlagCheckDiff = false;
        Bitmap curBitmap;
        Bitmap beforeBitmap;
        Thread th;
        #endregion

        #region Property
        #endregion

        #region MemberFuntion
        public Form1()
        {
            InitializeComponent();
            Closing += new CancelEventHandler(Form1_Closing);
            mjp = new MjpegDecoder();
            mjp.FrameReady += mjp_ready;
            Form.CheckForIllegalCrossThreadCalls = false;
            runCheckImgTask();
            th = new Thread(StartTakePicture);
            th.Start();
        }
       
        void mjp_ready(object sender, FrameReadyEventArgs e)
        {
            beforeBitmap = (Bitmap)pictureBox1.Image;
            pictureBox1.Image = e.Bitmap;
            curBitmap = e.Bitmap;
        }

        private void Form1_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            FlagTakePicture = false;
            FlagCheckDiff = false;
            this.Close();
            Environment.Exit(Environment.ExitCode);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            mjp.ParseStream(new Uri("http://192.168.1.209:8080/?action=stream"));
            pictureBox1.Visible = !cbTakePicture.Checked;
            FlagTakePicture = true;
            FlagCheckDiff = true;
        }

        public void StartTakePicture()
        {
            while (FlagTakePicture)
            {
                if (beforeBitmap != null)
                {
                    var dt = ConvertToBitmap(DateTime.Now.ToString("yyyyMMddHHmmss")+".jpg");
                    if (dt != null)
                        Thread.Sleep(5000);
                }
            }
        }

        public Bitmap ConvertToBitmap(string fileName)
        {
         
            Bitmap bitmap = new Bitmap(beforeBitmap);
            bitmap.Save(@"Image\"+fileName);
            return bitmap;
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            button2.Enabled = true;
            button1.Enabled = false;
            pictureBox1.Controls.Clear();

            lb.BackColor = Color.Red;
            lb.Width = 10;
            lb.Top = 10;
            lb.Left = 10;
            lb.Height = 10;
            lb.MouseDown += MouseDown;
            lb.MouseUp += MouseUp;
            lb.MouseMove += MouseMove;
            pictureBox1.Controls.Add(lb);

            lb1.BackColor = Color.Red;
            lb1.Width = 10;
            lb1.Height = 10;
            lb1.Top = 10;
            lb1.Left = 80;
            lb1.MouseDown += MouseDown;
            lb1.MouseUp += MouseUp;
            lb1.MouseMove += MouseMove;
            pictureBox1.Controls.Add(lb1);

            lb2.BackColor = Color.Red;
            lb2.Width = 10;
            lb2.Height = 10;
            lb2.Top = 80;
            lb2.Left = 10;
            lb2.MouseDown += MouseDown;
            lb2.MouseUp += MouseUp;
            lb2.MouseMove += MouseMove;
            pictureBox1.Controls.Add(lb2);

            lb3.BackColor = Color.Red;
            lb3.Width = 10;
            lb3.Height = 10;
            lb3.Top = 80;
            lb3.Left = 80;
            lb3.MouseDown += MouseDown;
            lb3.MouseUp += MouseUp;
            lb3.MouseMove += MouseMove;
            pictureBox1.Controls.Add(lb3);
        }

        private void MouseDown(object sender, MouseEventArgs e)
        {
            isDrag = true;
            px = e.X;
            py = e.Y;
        }

        private void MouseUp(object sender, MouseEventArgs e)
        {
            isDrag = false;
        }

        private void MouseMove(object sender, MouseEventArgs e)
        {
            if (isDrag)
            {
                (sender as Label).Left += (e.X - px);
                (sender as Label).Top += (e.Y - py);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(textBox1.Text))
                MessageBox.Show("請輸入變動程度");
            else
            {
                if ((sender as Button).Text.Equals("monitor"))
                {
                    lb.BackColor = lb1.BackColor = lb2.BackColor = lb3.BackColor = Color.Green;
                    isMonitor = true;
                    button2.Text = "Cancel";
                    button1.Enabled = false;
                    ReadStream.Enabled = false;
                    this.log("開始監控");
                    TaskRun = true;
                }
                else if ((sender as Button).Text.Equals("Cancel"))
                {
                    lb.BackColor = lb1.BackColor = lb2.BackColor = lb3.BackColor = Color.Red;
                    isMonitor = false;
                    button2.Text = "monitor";
                    ReadStream.Enabled = true;
                    this.log("停止監控");
                    TaskRun = false;

                }
            }
        }

        public void runCheckImgTask()
        {
            Task mainTask = new Task(() =>
            {
                Task th = new Task(() =>
                {
                    Thread.CurrentThread.Name = "check Img";
                    while (FlagCheckDiff)
                    {
                        if (TaskRun)
                        {
                            if (!ImageEquals(beforeBitmap, curBitmap))
                                log("diff");
                        }
                    }
                });

                th.Start();
            });

            mainTask.Start();
        }
        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            if (isMonitor)
            {
                float[] dashValue = { 2, 2 };
                Pen blackPen = new Pen(Color.Green, 3);
                blackPen.DashPattern = dashValue;
                e.Graphics.DrawLine(blackPen, new Point(lb.Left, lb.Top), new Point(lb1.Left + 8, lb1.Top));
                e.Graphics.DrawLine(blackPen, new Point(lb.Left, lb.Top), new Point(lb2.Left, lb2.Top + 8));
                e.Graphics.DrawLine(blackPen, new Point(lb1.Left + 8, lb1.Top), new Point(lb3.Left + 8, lb3.Top + 8));
                e.Graphics.DrawLine(blackPen, new Point(lb2.Left, lb2.Top + 8), new Point(lb3.Left, lb3.Top + 8));
            }
            else
            {
                float[] dashValue = { 2, 2 };
                Pen blackPen = new Pen(Color.Red, 3);
                blackPen.DashPattern = dashValue;
                e.Graphics.DrawLine(blackPen, new Point(lb.Left, lb.Top), new Point(lb1.Left + 8, lb1.Top));
                e.Graphics.DrawLine(blackPen, new Point(lb.Left, lb.Top), new Point(lb2.Left, lb2.Top + 8));
                e.Graphics.DrawLine(blackPen, new Point(lb1.Left + 8, lb1.Top), new Point(lb3.Left + 8, lb3.Top + 8));
                e.Graphics.DrawLine(blackPen, new Point(lb2.Left, lb2.Top + 8), new Point(lb3.Left, lb3.Top + 8));
            }
        }

        public void log(string content)
        {
            rtbLog.AppendText(String.Format("{1} : {0}\r", content, DateTime.Now.ToString("MM/dd HH:mm:ss")));
        }

        private bool ImageEquals(Bitmap bmpOne, Bitmap bmpTwo)
        {
            int totalCount = 0;
            int count = 0;
            for (int i = 0; i < bmpOne.Width; i++)
            {
                for (int j = 0; j < bmpOne.Height; j++)
                {
                    totalCount ++;
                    if (bmpOne.GetPixel(i, j) != bmpTwo.GetPixel(i, j))
                        count++;
                }
            }
            if ((totalCount * int.Parse(textBox1.Text)/100) < count)
                return false;
            return true;
        }

        private void cbTakePicture_CheckedChanged(object sender, EventArgs e)
        {
            pictureBox1.Visible = !cbTakePicture.Checked;
        }
        #endregion
    }
}
