using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;



namespace WindowsFormsApp1
{

    public partial class Form1 : Form
    {
        float x, a, b, scale, lambda;
        int x0, y0, W, H, n;
        Random random;
        

        public Form1()
        {
            InitializeComponent();
            random = new Random();
            scale = 50;
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        public float FuncRavnRasp(float a, float b, float x)
        {
            if (x < a)
                return 0;
            else if (x > b)
                return 1;
            else
                return (x - a) / (b - a);
        }
        public float FuncRavnRaspPlot(float a, float b, float x)
        {
            if (a <= x && x <= b)
                return 1 / (b - a);
            else
                return 0;
        }

        public float FuncPuasson(int n, float lambda, float x)
        {
            double r;
            double pr;

            for (int i = 0; i < n; i++)
                {
                    r = random.NextDouble();
                    pr = r;
                    int k;
                    for (k = 0; pr > Math.Exp(-lambda); k++)
                    {
                        r = random.NextDouble();
                        pr *= r;
                    }

                    x = k;
                }


            
            return x;
        }

            private void button1_Click(object sender, EventArgs e)
        {
            
            Graphics graphics1 = pictureBox1.CreateGraphics();
            Pen PenBlack = new Pen(Color.Black);
            Pen PenRed = new Pen(Color.Red);
            Pen PenBlue = new Pen(Color.Blue);
            Pen PenGreen = new Pen(Color.Green);
            graphics1.Clear(Color.White);

            a = float.Parse(textBox1.Text);
            b = float.Parse(textBox2.Text);
            n = Int32.Parse(textBox4.Text);

            W = pictureBox1.Width;
            H = pictureBox1.Height;
            
            x0 = W / 2; y0 = H / 2;

            graphics1.DrawLine(PenBlack, 0, y0, W, y0);

            graphics1.DrawLine(PenBlack, x0, 0, x0, H);

            for (int i = (int)a; i <= (int)b; i++)
            { graphics1.DrawLine(PenBlack, scale * i + x0, y0-5, scale * i + x0, y0+5); }

            float step = 0.01f;

            for (x = a - Math.Abs(a); x <= 1.5*b; x += step) 
            {
                float t = x + step;
                
                graphics1.DrawLine(PenGreen, scale * x + x0, -scale * FuncRavnRasp(a, b, x) + y0,
                                           scale * t + x0, -scale * FuncRavnRasp(a, b, t) + y0);
                graphics1.DrawLine(PenBlue, scale * x + x0, -scale * FuncRavnRaspPlot(a, b, x) + y0,
                                            scale * t + x0, -scale * FuncRavnRaspPlot(a, b, t) + y0);
            }
        
            for(int i=1; i <= n; i++)
            { 
                float r;

                r = (float)random.NextDouble();
                x = a + r * (b - a);

                graphics1.DrawEllipse(PenRed, scale * x + x0, -scale * FuncRavnRasp(a, b, x) + y0, 2, 2);
                graphics1.DrawEllipse(PenRed, scale * x + x0, -scale * FuncRavnRaspPlot(a, b, x) + y0, 2, 2);
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            Graphics graphics1 = pictureBox1.CreateGraphics();
            Pen PenBlack = new Pen(Color.Black);
            Pen PenRed = new Pen(Color.Red);
            Pen PenBlue = new Pen(Color.Blue);
            graphics1.Clear(Color.White);

            a = float.Parse(textBox1.Text);
            b = float.Parse(textBox2.Text);
            lambda = float.Parse(textBox3.Text);
            n = Int32.Parse(textBox4.Text);

            W = pictureBox1.Width;
            H = pictureBox1.Height;

            //scale = 30;
            x0 = W / 2; y0 = H / 2;

            graphics1.DrawLine(PenBlack, 0, y0, W, y0);
            graphics1.DrawLine(PenBlack, x0, 0, x0, H);

            for (int i = (int)a; i <= (int)b; i++)
            { graphics1.DrawLine(PenBlack, scale * i + x0, y0-5, scale * i + x0, y0+5); }

            float step = 0.01f;

            for (x = a - Math.Abs(a); x <= b*2; x += step) // O(100,85) - center
            {
                float t = x + step;
                graphics1.DrawLine(PenRed, scale * x + x0, -scale * FuncPuasson(n, lambda, x) + y0,
                                           scale * t + x0, -scale * FuncPuasson(n, lambda, t) + y0);
            }
        }

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }
    }
}
