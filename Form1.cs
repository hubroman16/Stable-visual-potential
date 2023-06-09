using NetManager;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows.Forms;
using System.Drawing;

namespace UZVP
{
    public partial class Form1 : Form
    {
        double draw;
        double frequency;
        List<int> data1 = new List<int>();
        List<int> data2 = new List<int>();
        List<int> data3 = new List<int>();
        delegate void Add(Frame F);
        Add Deleg;
        bool hist = false;
        int disk = 0;
        int flag = 2;
        int counterboba = 0;
        int num;
        bool stim = false;
        bool start = false;
        bool fl = false;
        bool f1 = false;
        bool f2 = false;
        bool check = false;
        bool check1 = false;
        public Form1()
        {
            InitializeComponent();
            reseiveClientControl1.Client.Reseive += MessageHandler;
            Deleg += AddMeth;
            reseiveClientControl1.Client.Name = "УЗВП";
        }

        private void AddMeth(Frame f)
        {
            if (hist)
            {
                chart2.Invoke(new Action<int, Frame>(AddXY), disk, f);
            }
            if (fl)
            {
                Invoke(new UpdateTimer1(YourUpdateTimer1), true);
                Invoke(new UpdateTimer2(YourUpdateTimer2), true);
                Invoke(new UpdateTimer2(YourUpdateTimer3), true);

            }
            if (check)
            {
                listBox1.Invoke(new Action<Frame>(AddData), f);
                check = false;
                

            }
            
            if (check1)
            {
                listBox2.Invoke(new Action<Frame>(AddData), f);
                check1 = false;
            }
            if (start)
                disk += 24;
        }

        private void Clea(double i)
        {
            chart1.Series[0].Points.Clear();
            chart1.ChartAreas[0].AxisX.Maximum = i;
        }
        private void AddData(Frame f)
        {
            if (fl)
            {
                if (f1)
                {
                    for (int i = 0; i < 12; i++)
                    {
                        data1.Add(f.Data[i]);
                        listBox1.Items.Add(f.Data[i]);
                    }
                    data2.Clear();
                }
                if (f2)
                {
                    for (int i = 0; i < 12; i++)
                    {
                        data2.Add(f.Data[i]);
                        listBox2.Items.Add(f.Data[i]);
                    }
                for (int i = 0; i < data1.Count; i++)
                {
                    if(data2.Count>= data1.Count)
                    data3.Add(data1[i] + data2[i]);
                    stim = true;

                }
                data1.Clear();
                }
               
            }
        }


        private void AddXY(int x, Frame f)
        {
            for (int i = 0; i < 24; i++)
            {

                chart2.Series[0].Points.AddXY((x + i) / frequency, f.Data[24 * num + i]);

        }
                
        }
        private void AddXY1(int x)
        {
                for (int i= 0; i < data3.Count; i++)
                {
                    chart1.Series[0].Points.AddXY((x + i)/frequency, data3[i]);

                }
        }


        private void MessageHandler(object sender, EventClientMsgArgs e)
        {
            Frame f = new Frame(e.Msg);
            Deleg.Invoke(f);
        }

        private void ConnectionButton_Click(object sender, EventArgs e)
        {

            if (!reseiveClientControl1.Client.IsRunning)
            {
                reseiveClientControl1.Client.StartClient();
                frequency = Convert.ToInt32(textBox1.Text);
                draw = frequency / 4;
                chart1.ChartAreas[0].AxisX.Maximum = draw / frequency;
                ConnectionButton.Text = "Отключить";
            }
            else 
            {
                reseiveClientControl1.Client.StopClient();
                ConnectionButton.Text = "Подключить";

            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (flag % 2 == 0)
            {
                Size = new Size(1920, 1080);
                Location = new Point(0, 0);
            }
            else
            {
                Size = new Size(1046, 574);
                Location = new Point(Width/2, Height/2);
            }
            flag += 1;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Thread.Sleep(6000);
        }

        delegate void UpdateTimer1(bool b);
        delegate void UpdateTimer2(bool b);
        delegate void UpdateTimer3(bool b);

        public void YourUpdateTimer1(bool b)
        {
            timer1.Enabled = b;
        }
        
        public void YourUpdateTimer2(bool b)
        {
            timer2.Enabled = b;
        }
        public void YourUpdateTimer3(bool b)
        {
            timer3.Enabled = b;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            
            Bitmap bmp1 = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            Graphics g = Graphics.FromImage(bmp1);
            g.Clear(Color.Snow);
            pictureBox1.Image = bmp1;
            
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            
            Bitmap bmp1 = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            Graphics g = Graphics.FromImage(bmp1);
            g.Clear(Color.Transparent);
            pictureBox1.Image = bmp1;
            
        }


        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            num = Convert.ToInt16(textBox1.Text) - 1;
            chart1.Invoke(new Action<double>(Clea), (disk + draw + 1) / frequency);
        }

        private void button6_Click(object sender, EventArgs e)
        {
            if (hist)
            {
                hist = false;
            }
            else
            {
                if (!start)
                    start = true;
                hist = true;
            }
            if (fl)
            {
                fl = false;
            }
            else
            {
                fl = true;
            }
            timer3.Enabled = true;
            timer4.Enabled = true;
        }

        private void timer3_Tick(object sender, EventArgs e)
        {
                if ((disk + 25) / frequency > chart1.ChartAreas[0].AxisX.Maximum)
                {
                    chart1.Invoke(new Action<double>(Clea), (disk + draw + 1) / frequency);
                }
                chart1.Invoke(new Action<int>(AddXY1), disk);
                textBox3.Text = Convert.ToString(data3.Count);
        }

        private void button5_Click(object sender, EventArgs e)
        {
            chart2.Series[0].Points.Clear();
            data3.Clear();
        }

        private void timer4_Tick(object sender, EventArgs e)
        {
            counterboba += 1;
            if (counterboba % 2 == 1)
            {
                check = true;
                f2 = false;
                f1 = true;
            }
            else
            {
                check1 = true;
                f2 = true;
                f1 = false;
            }
        }
    }
}
