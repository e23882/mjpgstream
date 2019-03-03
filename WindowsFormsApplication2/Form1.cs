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
        #endregion

        #region Property
        #endregion

        #region MemberFuntion
        public Form1()
        {
            InitializeComponent();
            mjp = new MjpegDecoder();
            mjp.FrameReady += mjp_ready;
        }
        void mjp_ready(object sender, FrameReadyEventArgs e)
        {
            pictureBox1.Image = e.Bitmap;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            mjp.ParseStream(new Uri("http://rr114.ddns.net:8080/?action=stream"));
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
            if ((sender as Button).Text.Equals("monitor"))
            {
                lb.BackColor = lb1.BackColor = lb2.BackColor = lb3.BackColor = Color.Green;
                isMonitor = true;
                button2.Text = "Cancel";
                button1.Enabled = false;
                ReadStream.Enabled = false;
                this.log("開始監控");
            }
            else if ((sender as Button).Text.Equals("Cancel"))
            {
                lb.BackColor = lb1.BackColor = lb2.BackColor = lb3.BackColor = Color.Red;
                isMonitor = false;
                button2.Text = "monitor";
                ReadStream.Enabled = true;
            }
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
            rtbLog.AppendText(String.Format("{0} : {1}", content, DateTime.Now.TimeOfDay));
        }
        #endregion
    }
}
