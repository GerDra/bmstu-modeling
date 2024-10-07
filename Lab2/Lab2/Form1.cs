using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace Lab2
{
    public partial class Form1 : Form
    {

        public double[,] matrix;
        double[,] matrixCoeffs;
        double[,] matrixLamda;
        double[] deltaTime;
        public double deltaT;
        private static double EPS = 1e-3;

        public Form1()
        {
            InitializeComponent();
        }
        
        private double[] CreateInitProbabilities(int arraySize)
        {
            double[] probs = new double[arraySize];
            probs[0] = 1;
            for (int i = 1; i < arraySize; i++)
            {
                probs[i] = 0;
            }
            return probs;
        }
        private double[] GetProbabilities(int matrixSize)
        {
            int i, j;
            double[] freeMembers = new double[matrixSize];
            freeMembers[matrixSize - 1] = 1;

            matrixCoeffs = new double[matrixSize, matrixSize];

            for (i = 0; i < matrixSize - 1; i++)
            {
                double sum = 0;
                for (j = 0; j < matrixSize; j++)
                {
                    sum += matrix[i, j];
                }
                for (j = 0; j < matrixSize; j++)
                {
                    if (j == i)
                    {
                        matrixCoeffs[i, j] = -sum + matrix[i, j];
                    }
                    else
                    {
                        matrixCoeffs[i, j] = matrix[j, i];
                    }
                }
            }
            for (i = 0; i < matrixSize; i++)
            {
                matrixCoeffs[matrixSize - 1, i] = 1;
            }

         var probsSteady = Gauss(matrixCoeffs, freeMembers, matrixSize);
         
         return probsSteady;
        }

        public double[,] TransportMatrix(double[,] matrixCoeffs, int matrixSize)
        {
            double t;

            for (int i = 0; i < matrixSize; i++)
            {
                for (int j = i+1; j < matrixSize; j++)
                {
                    
                    t = matrixCoeffs[j, i];
                    matrixCoeffs[j, i]= matrixCoeffs[i, j]; ;
                    matrixCoeffs[i, j] = t;
                }
            }

            return matrixCoeffs;
        }

        private double[] Gauss(double [,] matrixCoeffs, double[] freeMembers, int n)
        {
            double s = 0;
            double[,] a = new double[n, n];
            double[] b = new double[n];
            double[] x = new double[n];
            //a = matrixCoeffs;
            //b = freeMembers;
            for (int i = 0; i < n; i++)
                x[i] = 0;
            
            for (int i = 0; i < n; i++)
                for (int j = 0; j < n; j++)
                {
                    a[i, j] = matrixCoeffs[i,j];
                }

            for (int i = 0; i < n; i++)
                b[i] = freeMembers[i];

            for (int k = 0; k < n - 1; k++)
            {
                for (int i = k + 1; i < n; i++)
                {
                    for (int j = k + 1; j < n; j++)
                    {
                        a[i, j] = a[i, j] - a[k, j] * (a[i, k] / a[k, k]);
                    }
                    b[i] = b[i] - b[k] * a[i, k] / a[k, k];
                }
            }
            for (int k = n - 1; k >= 0; k--)
            {
                s = 0;
                for (int j = k + 1; j < n; j++)
                    s = s + a[k, j] * x[j];
                x[k] = (b[k] - s) / a[k, k];
            }
            return x;
        }

        private double[] SolveOde(double[] initProbs, double[,] matrixCoeffs, int matrixSize)
        {
            double[] dydt = new double[matrixSize];
            for (int i = 0; i < matrixSize; i++)
            {
                dydt[i] = 0;
                for (int j = 0; j < matrixSize; j++)
                {
                    dydt[i] += initProbs[j] * matrixCoeffs[i, j];
                }
            }
            return dydt;
        }

        private (double[], double[]) GetTimes(double[] probsSteady, bool buildGraph, int matrixSize, double dt)
        {
            int i, j;

            for (i = 0; i < matrixSize; i++)
            {
                double sum = 0;
                for (j = 0; j < matrixSize; j++)
                {
                    sum += matrix[i, j];
                }
                for (j = 0; j < matrixSize; j++)
                {
                    if (j == i)
                    {
                        matrixCoeffs[i, j] = -sum + matrix[i, j];
                    }
                    else
                    {
                        matrixCoeffs[i, j] = matrix[j, i];
                    }
                }
            }

            double start =0, stop=20;
            int cs = (int)((stop-start)/dt);
            double[]times = new double[cs];
            for(i = 0; i < cs; i++) 
            {
                times[i] = start + dt * i;
            }

            var timesSteady = new List<double>();

            return (probsSteady, timesSteady.ToArray());
        }



        private void MyGetTimes(double[] probsSteady, int matrixSize)
        {
            int i, j;

            double dt = 1;
            double start = 0, stop = 20;
            int cs = (int)((stop - start) / dt);
            double[] times = new double[cs];
            for (i = 0; i < cs; i++)
            {
                times[i] = start + dt * i;
            }

            double [] currentP = new double[matrixSize];
            double sumLabda=1;
            double sum;

            for (i = 0; i < matrixSize; i++)
                currentP[i] = probsSteady[i];


            for (i = 0; i < matrixSize; i++)
            {
                deltaTime[i] = 0;
                currentP[i] = probsSteady[i];
                
                do {
                    sum = 0;
                    sumLabda = 0;
                    for (j = 0; j < matrixSize; j++)
                    {
                        if (j != i)
                            sum += currentP[j] * matrixLamda[j, i];
                    }
                    
                    for (j = 0; j < matrixSize; j++)
                    {
                        if (j != i)
                            sumLabda += matrixLamda[i, j];
                    }
                    
                    sum -= currentP[i] * sumLabda;
                    sum /= deltaT;
                    currentP[i] = sum;
                    deltaTime[i] += deltaT;

                }while (Math.Abs(sum - probsSteady[i]) > EPS) ;
            }

            return;
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            
            dataGridMatr.Rows.Clear();
            dataGridMatr.Columns.Clear();
            int n = Convert.ToInt32(textBox1.Text);
            deltaT = Convert.ToDouble(textBox2.Text);

            if (n > 10||n<1)
            {
                // Initializes the variables to pass to the MessageBox.Show method.
                string message = "Количество состояний должно быть от 1 до 10!";
                string caption = "Неверное значение";
                MessageBoxButtons buttons = MessageBoxButtons.OK;
                DialogResult result;

                // Displays the MessageBox.
                result = MessageBox.Show(message, caption, buttons);
                if (result == System.Windows.Forms.DialogResult.Yes)
                {
                    // Closes the parent form.
                    this.Close();
                }
                return;
            }
            int i,j;
            matrix = new double[n,n];
            matrixLamda = new double[n, n];
            
            double probs, sum;
            

            for (i = 0; i < n; i++)
                dataGridMatr.Columns.Add($"column{i}", $"{i+1}");
            dataGridMatr.Rows.Add(n);

            Random random = new Random();

            for (i = 0; i < n; i++)
            {
                probs=sum = 0;
                for (j = 0; j < n; j++)
                {
                    matrix[i,j] = Math.Round(random.NextDouble(),2);
                    sum += matrix[i, j];
                }

                for (j = 0; j < n; j++)
                {   
                    matrix[i,j] = Math.Round(matrix[i, j] / sum, 2);
                    probs += matrix[i, j];

                }

                matrix[i,n-1] = 1 - probs+matrix[i,n-1];

            }
            /*matrix = new double[4, 4] { { 0.1, 0.13, 0.3, 0.47 },
                { 0.12, 0.28, 0.23, 0.37 },
                { 0.34, 0.25, 0.22, 0.15 },
                { 0.23, 0.3, 0.03, 0.45 } };
            */

            //for (i = 0; i < n; i++)
            //{
            //    for (j = 0; j < n; j++)
            //    {
            //        matrixLamda[i, j] = matrix[i, j] / deltaT;

            //    }
            //}

            for (i = 0; i < n; i++)
            {
                for (j = 0; j < n; j++)
                {
                    matrixLamda[i, j] = matrix[i, j]; // deltaT;
                }
            }


            for (i = 0; i < n; i++)
            {
                for (j = 0; j < n; j++)
                {
                    dataGridMatr.Rows[i].Cells[j].Value = matrix[i, j];
                }
            }
        }

        private void dataGridMatr_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            dataGridRes.Rows.Clear();
            dataGridRes.Columns.Clear(); 
            int i,j,n,counter;
            double []probs,dt;
            
            n = Convert.ToInt32(textBox1.Text);
            deltaTime = new double[n];

            for (i = 0; i < n; i++)
                dataGridRes.Columns.Add($"column{i}", $"{i+1}");
            dataGridRes.Rows.Add(2);
            

            for (i = 0; i < n; i++)
            {
                counter = 0;
                for (j = 0; j < n; j++, counter++)
                {
                    matrix[i, j] = Convert.ToDouble(dataGridMatr.Rows[i].Cells[j].Value);
                }

                for (j = 0; j < n; j++)
                {
                    matrix[i, j] = matrix[i, j] / counter;
                }

            }
            
            var probsSteady = GetProbabilities(n);
            probs = CreateInitProbabilities(n);            
            dt=SolveOde(probs, matrixCoeffs, n);

            List<double>  listTime = new List<double>();
            List<double> start_probabilities = new List<double>(GetStartProbabilities(n, false));
            List<double> limit_probabilities = new List<double>(probsSteady);

            listTime = calc_stabilization_times(matrix, start_probabilities, limit_probabilities);

            for (j = 0; j < n; j++)
            {
                dataGridRes.Rows[0].Cells[j].Value = Math.Round(probsSteady[j],2);
            }
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            deltaT = Convert.ToDouble(textBox2.Text);
        }


        public static double TIME_DELTA = 1e-3;
        public static int MAGIC_NUM = 10;  // >= 1

        public static List<double> dps(double[,] matrix, List<double> probabilities)
        {
            int n = matrix.GetLength(0);
            List<double> result = new List<double>();
            for (int i = 0; i < n; i++)
            {
                double sum = 0;
                for (int j = 0; j < n; j++)
                {
                    if (i == j)
                    {
                        sum += probabilities[j] * (-matrix.GetLength(1) + matrix[i, i]);
                    }
                    else
                    {
                        sum += probabilities[j] * matrix[j, i];
                    }
                }
                result.Add(TIME_DELTA * sum);
            }
            return result;
        }
        public static double[] GetStartProbabilities(int n, bool allEqual = true)
        {
            if (allEqual)
            {
                double[] result = new double[n];
                for (int i = 0; i < n; i++)
                {
                    result[i] = 1.0 / n;
                }
                return result;
            }
            else
            {
                double[] result = new double[n];
                result[0] = 1.0;
                return result;
            }
        }
        public static List<double> calc_stabilization_times(double[,] matrix, List<double> start_probabilities, List<double> limit_probabilities)
        {
            int n = matrix.GetLength(0);
            double current_time = 0;
            List<double> current_probabilities = new List<double>(start_probabilities);
            List<double> stabilization_times = new List<double>(Enumerable.Repeat(0.0, n));

            double total_lambda_sum = 0;
            for (int i = 0; i < n; i++)
            {
                total_lambda_sum += matrix.GetLength(1);
            }
            total_lambda_sum *= MAGIC_NUM;
            List<double> cool_eps = new List<double>();
            foreach (double p in limit_probabilities)
            {
                cool_eps.Add(p / total_lambda_sum);
            }
            
            while (!stabilization_times.TrueForAll(t => t > 0))
            {
                List<double> curr_dps = dps(matrix, current_probabilities);
                for (int i = 0; i < n; i++)
                {
                    double res = Math.Abs(current_probabilities[i] - limit_probabilities[i]);
                    if (stabilization_times[i] == 0 && curr_dps[i] <= EPS && res <= EPS)
                    {
                        stabilization_times[i] = current_time;
                    }
                    current_probabilities[i] += curr_dps[i];
                }
                current_time += TIME_DELTA;
            }

            return stabilization_times;
        }

        public static (List<double>, List<List<double>>) calc_probability_over_time(double[,] matrix, List<double> start_probabilities, double end_time)
        {
            int n = matrix.GetLength(0);
            double current_time = 0;
            List<double> current_probabilities = new List<double>(start_probabilities);

            List<List<double>> probabilities_over_time = new List<List<double>>();
            List<double> times = new List<double>();

            while (current_time < end_time)
            {
                probabilities_over_time.Add(new List<double>(current_probabilities));
                List<double> curr_dps = dps(matrix, current_probabilities);
                for (int i = 0; i < n; i++)
                {
                    current_probabilities[i] += curr_dps[i];
                }
                current_time += TIME_DELTA;
                times.Add(current_time);
            }

            return (times, probabilities_over_time);
        }
    }
}
