using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using System.IO.Ports;
using System.Diagnostics;
using System.Globalization;
using System.Drawing.Drawing2D;
using System.Text.RegularExpressions;

namespace 全局定位1
{
    public partial class 全局定位 : Form
    {
        Stopwatch sw = new Stopwatch();

        
        //PID暂存
        double x = 700.0;
        double y = 372;
        double p = 0;
        float w=0;

        bool drawornot = false;

        public Image imgofCar = Image.FromFile("车.PNG");

        public 全局定位()
        {
            InitializeComponent();
            //获取端口列
            pictureBox2.Image = Image.FromFile("车.PNG");
            pictureBox1.Image = Image.FromFile("比赛场地.PNG");
            serialPort1.DataReceived += DataReceivedHandler;
            sw.Start();
            Control.CheckForIllegalCrossThreadCalls = false;  //防止跨线程出错
            //定时器中断时间
            timer1.Interval = 100;
            timer2.Interval = 100;
            timer3.Interval = 300;
            
            
           
        }
        static int buffersize = 18;   //十六进制数的大小（假设为9Byte,可调整数字大小）
        byte[] buffer = new Byte[buffersize];   //创建缓冲区

        
        private void Form1_Load(object sender, EventArgs e)
        {
            comboBox1.Items.Add("1200");
            comboBox1.Items.Add("2400");
            comboBox1.Items.Add("4800");
            comboBox1.Items.Add("9600");
            comboBox1.Items.Add("14400");
            comboBox1.Items.Add("19200");
            comboBox1.Items.Add("28800");
            comboBox1.Items.Add("38400");
            comboBox1.Items.Add("115200");//常用的波特率
            try
            {
                string[] ports = SerialPort.GetPortNames();//得到接口名字
                //将端口列表添加到comboBox
                this.comboBox2.Items.AddRange(ports);
                ///设置波特率
                serialPort1.BaudRate = Convert.ToInt32(comboBox1.Text);
                
            }
            catch (Exception ex)
            {
                
            }

           
        }


        private void pictureBox1_Click(object sender, EventArgs e)//用于显示地图
        {
            
        }


        private void button2_Click(object sender, EventArgs e)//接收/暂停数据按钮
        {
            if (serialPort1.IsOpen)////(更新)如果按下按钮之前串口是开的，就断开//如果按下按钮之前 flag的内容是false 按下之后 内容改成true 然后打开串口
            {
                serialPort1.Close();
                button2.Text = "连接";
               
            }

            else
            {
                //要打开串口要看波特率 串口等有没有设置对
                bool no_error_flag = true;
                try
                {
                    serialPort1.BaudRate = Convert.ToInt32(comboBox1.SelectedItem);
                }
                catch (ArgumentException e1)
                {
                    this.errorProvider1.SetError(this.comboBox1, "不能为空");
                    no_error_flag = false;
                }
                try
                {
                    serialPort1.PortName = Convert.ToString(comboBox2.SelectedItem);
                }
                catch (ArgumentException e2)
                {
                    this.errorProvider1.SetError(this.comboBox2, "不能为空");
                    no_error_flag = false;
                }
                try
                {
                    serialPort1.Open();
                }
                catch
                {
                    MessageBox.Show("端口错误", "警告");
                    no_error_flag = false;
                }
                if (no_error_flag)
                {

                    button2.Text = "断开连接";

                 }
              }
         }

     
        private void timer3_Tick(object sender, EventArgs e)//第三定位器
        {
            if (serialPort1.IsOpen)
            {
                /*textBox1.Text = data_warehouse;
                textBox1.SelectionStart = textBox1.TextLength;//光标定位到文本最后
                textBox1.ScrollToCaret();*/
                UpdateQueueValue();
                 
            }
            
            
          }
        

               
        //串口接收完成事件——接收所有数据
        string data_warehouse = "";
        public void DataReceivedHandler(object sender, SerialDataReceivedEventArgs e)
        {
            //读取串口中的所有数据
            string readfromport = serialPort1.ReadExisting();
            data_warehouse += readfromport;
            
        }

        //更新队列程序
        string[] separators = { "A:", "B:", "X:", "Y:", "P:" };
        int pos = 0;
        private void UpdateQueueValue()
        {
           
            while (data_warehouse.Length > 35)
            {
                //第一个换行符的位置
                pos = data_warehouse.IndexOf('\n');
                //断句第一个换行符
                Debug.WriteLine("" + pos);
                string s = data_warehouse.Remove(pos).ToUpper();//错
                //删掉读到的第一句话
                data_warehouse = data_warehouse.Remove(0, pos + 1);
                //开始断句
                string[] words = s.Split(separators, StringSplitOptions.RemoveEmptyEntries);
                //读取断句中的数据

                try
                {
                    //float d1 = Convert.ToSingle(words[0]);
                    //float d2 = Convert.ToSingle(words[1]);

                    x = Convert.ToDouble(words[2]);
                    y = Convert.ToDouble(words[3]);
                    p = Convert.ToDouble(words[4]);
                   
                    Debug.WriteLine(words[2]);
                    Debug.WriteLine(words[3]);
                    Debug.WriteLine(words[4]);
                    w = (float)p;
                    RotateFormCenter(pictureBox2, w);//这里我需要得到陀螺仪给的角度p，任意角度旋转
                    this.pictureBox2.Refresh();  
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e.ToString());
                }
            }
        }


        private void button4_Click(object sender, EventArgs e)//开始画图按钮
        {
            //this.pictureBox2.Image.RotateFlip(RotateFlipType.Rotate90FlipXY);
            // this.pictureBox2.Image.RotateFlip(RotateFlipType.Rotate90FlipXY);



            if (data_warehouse.Length > 0)
            {


            }
            else
            {
                MessageBox.Show("数据接受失败！", "警告");

            }

            timer1.Start();
            timer2.Start();
            timer3.Start(); 
            
        }
            
 
        private void label2_Click(object sender, EventArgs e)//写文字标签
        {

        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)//读取串口以及串口刷新
        {
            string[] ports = SerialPort.GetPortNames();
            this.comboBox2.Items.Clear();
            this.comboBox2.Items.AddRange(ports);
        }

        private void textBox1_TextChanged(object sender, EventArgs e)//显示x,y坐标和角度p
        {
            
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {

        }
        public int   a = 0, b = 0;
   
        private void timer1_Tick(object sender, EventArgs e)//关于小车运动的计时器
        {
                pictureBox2.Left =Convert.ToInt16(y);
                pictureBox2.Top = Convert.ToInt16(x); ;//赋予小车坐标位置
                
        }

        private void timer2_Tick(object sender, EventArgs e)
        {

            System.DateTime currentTime = new System.DateTime();
            currentTime = System.DateTime.Now;
            textBox1.AppendText("(" + currentTime.Year + "/" + currentTime.Month + "/" + currentTime.Day + " " + currentTime.Hour + ":" + currentTime.Minute + ":" + currentTime.Second + ")\n");  //添加文本
            textBox1.AppendText("   x: " + x + " y :" + y + " p: " + p + "\n");
            textBox1.ScrollToCaret();    //自动显示至最后行

        }

        private void button1_Click(object sender, EventArgs e)
        {
            timer1.Stop();
            timer2.Stop();
            timer3.Stop();
            
        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }

        private void button5_Click(object sender, EventArgs e)
        {
            pictureBox2.Location = new Point(372, 700);//使picturebox移动
            Thread.Sleep(200);
            timer1.Stop();
            timer2.Stop();
            timer3.Stop();
            
        }

        private void groupBox2_Enter(object sender, EventArgs e)
        {

        }

        private void RotateFormCenter(PictureBox pb, float angle)//任意角度旋转的方法
        {
            Image img = imgofCar;
            int newWidth = Math.Max(img.Height, img.Width);
            Bitmap bmp = new Bitmap(newWidth, newWidth);
            Graphics g = Graphics.FromImage(bmp);
            Matrix x = new Matrix();
            PointF point = new PointF(img.Width / 2f, img.Height / 2f);
            x.RotateAt(angle, point);
            g.Transform = x;
            g.DrawImage(img, 0, 0);
            g.Dispose();
            img = bmp;
            pb.Image = img;
        }

    }
}
