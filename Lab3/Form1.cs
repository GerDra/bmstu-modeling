using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Net.Mime.MediaTypeNames;

namespace Lab3
{
    public partial class Form1 : Form
    {
        private const int N_MAX = 1000;
        // Размер 1-й последовательности случайных чисел 
        private const int K = 64;
        // 1-я случайная последовательность 
        private static double[] Z1;
        // Стандартный датчик
        private static Random Random;
        private static double Rnd()
        {

            // случайное число 1-й последовательности 
            double g1 = Random.NextDouble();
            // случайное число 2-й последовательности
            double g2 = Random.NextDouble();
            int m = (int)(g2 * K);
            double res = Z1[m];
            // заполнение элемента первой последовательности 
            // новым числом 
            Z1[m] = g1;
            return res;
        }

            public Form1()
        {
            InitializeComponent();
            Z1 = new double[K];
            Random = new Random();
        }

        private double Lambda(double[] x, int n, double a, double b)
        {
            double dMax = 0.0;
            for (int i = 0; i < n; i++)
            {
                double dp = Math.Abs((double)(i + 1) / n - FuncRavnRasp(a, b ,x[i]));
                double dm = Math.Abs(FuncRavnRasp(a, b, x[i]) - (double)i / n);
                if (dp > dMax) dMax = dp;
                if (dm > dMax) dMax = dm;
            }
            return dMax * Math.Sqrt(n);
        }
        public double FuncRavnRasp(double a, double b, double x)
        {
            if (x < a)
                return 0;
            else if (x > b)
                return 1;
            else
                return (x - a) / (b - a);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            FillDataGridView();
        }
        private void FillDataGridView()
        {

            int i, k, temp;
            int j = 0;
            
            

            for (i = 0; i < K; i++)
            {
                // генерация первых К случайных чисел 
                Z1[i] = Random.NextDouble();
            }

            double x;
            for (i = 0; i < N_MAX; i++)
            {
                x = Rnd();
            }
            for (j = 0,k=10; j < 3; j++,k*=10)
            {
                for (i = 0; i < 10; i++)
            {
                if(j==0)
                        dataGrid.Rows.Add();

                temp= (int)(Z1[(int)(Random.NextDouble() * K)] * k);
                    if ((j == 0 && temp<k)|| 
                        (j == 1 && temp > (k / 10) && temp < k)|| 
                        (j == 2 && temp > (k / 10) && temp < k))
                        dataGrid.Rows[i].Cells[j].Value = temp;
                    else i--;
                }
            }
            
            String[] line=new string[1000];
            try
            {
                line = File.ReadAllLines("Random.txt");
            }
            catch (Exception e) { }

            for (j = 3, k = 10; j < 6; j++, k *= 10) 
            {
                for (i = 0; i < 10; i++)
                {
                    temp = Convert.ToInt32(line[(j-3)*333+i]);
                    if ((j == 3 && temp < k) ||
                        (j == 4 && temp > (k / 10) && temp < k) ||
                        (j == 5 && temp > (k / 10) && temp < k))
                        dataGrid.Rows[i].Cells[j].Value = temp;
                    else i--;
    
                }
            }
            dataGrid.Rows.Add();
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            int i, j, m;
            double a, b, t;
            int n = 10;
            double[] x = new double[n];
            a = double.MaxValue;
            b = double.MinValue;
            
            for (i = 0; i < 7; i++)
            {
                for (j = 0; j < n; j++)
                {
                    t = Convert.ToDouble(dataGrid.Rows[j].Cells[i].Value);
                    x[j] = t;
                }
                for (j = 0; j +1< n; j++)
                {
                    for (m = 0; m +1< n - j; m++)
                        if (x[m + 1] < x[m])
                        {
                            t = x[m];
                            x[m] = x[m + 1];
                            x[m + 1] = t;
                        }
                }

                if ((i == 0 || i == 3))
                { a = 0; b = 9; }
                else if ((i == 1 || i == 4))
                { a = 10; b = 99; }
                else if (i == 2 || i == 5)
                { a = 100; b = 999; }
                else
                {
                    for (m = 0; m < n; m++)
                    {
                        a = Math.Min(x[m], a); b = Math.Max(x[m], b);
                    }
                }

                dataGrid.Rows[n].Cells[i].Value = Lambda(x, n, a, b);
            }
        }

    }
}
