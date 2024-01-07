
using System.Text;

namespace Lab1
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = UTF8Encoding.UTF8;
            double[,] p =
                { { 0, 0.35, 0 },
                { 1, 0.05, 1 },
                { 0, 0.6, 0 } };
            int smoNumber = 3;
            double N = 8;
            double [] e = new double[smoNumber];          
            double[] invertedU = new double[smoNumber];
            double[] u = new double[smoNumber];
            double[] r = new double[smoNumber];
            double[] L = new double[smoNumber];
            double[] R = new double[smoNumber];            
            r[0] = 3;
            r[1] = 1;
            r[2] = 1;
            invertedU[0] = 0.3;
            invertedU[1] = 0.9;
            invertedU[2] = 0.4;
            for (int i = 0; i < invertedU.Length; i++)
            {
                u[i] = 1/invertedU[i];
            }
            e[0] = 1;
            e[1] = -((p[1, 0] * e[0]) / (p[1, 1] + p[2,1]-1));
            e[2] = e[0]*p[2,0] + e[1] * p[2,1] + p[2,2];
            double CN = FindCN(N, e, u, r);
            for (int i = 0; i < L.Length; i++)
            {
                for (double j = r[i]+1; j <= N; j++)
                {
                    L[i] += (j - r[i]) * FindPSMOI(i, j, N, e, u, r,CN);
                }
            }
            for (int i = 0; i < R.Length; i++)
            {
                R[i] = r[i];
                for (int j = 0; j <= r[i]-1; j++)
                {
                    R[i] -= (r[i]-j)*FindPSMOI(i,j, N, e, u, r,CN);
                }
            }
            double[] M = new double[smoNumber];
            for (int i = 0; i < M.Length; i++)
            {
                M[i] = L[i] + R[i];
            }
            double[] Lambda = new double[smoNumber];
            for (int i = 0; i < Lambda.Length; i++)
            {
                Lambda[i] = R[i] * u[i];
            }
            double[] T = new double[smoNumber];
            for (int i = 0; i < T.Length; i++)
            {
                T[i] = M[i] / Lambda[i];
            }
            double[] Q = new double[smoNumber];
            for (int i = 0; i < Q.Length; i++)
            {
                Q[i] = L[i] / Lambda[i];
            }
            Console.WriteLine("Коефіцієнти передачі:");
            for (int i = 0; i < e.Length; i++)
            {
                Console.WriteLine("E{0} = {1}", i, e[i]);
            }
            Console.WriteLine("Середня кількість заявок у черзі i-ої СМО:");
            for (int i = 0; i < L.Length; i++)
            {
                Console.WriteLine("L{0} = {1}", i, L[i]);
            }
            Console.WriteLine("Середня кількість зайнятих пристроїв i-ої СМО:");
            for (int i = 0; i < R.Length; i++)
            {
                Console.WriteLine("R{0} = {1}", i, R[i]);
            }
            Console.WriteLine("Середня кількість заявок у i-ої СМО:");
            for (int i = 0; i < M.Length; i++)
            {
                Console.WriteLine("M{0} = {1}", i, M[i]);
            }
            Console.WriteLine("Інтенсивність вихідного потоку заявок:");
            for (int i = 0; i < Lambda.Length; i++)
            {
                Console.WriteLine('\u03BB' + "{0} = {1}", i, Lambda[i] );
            }
            Console.WriteLine("Середній час перебування заявки в i-ої СМО:");
            for (int i = 0; i < T.Length; i++)
            {
                Console.WriteLine("T{0} = {1}", i, T[i]);
            }
            Console.WriteLine("Середній час чекання в черзі i-ої СМО:");
            for (int i = 0; i < Q.Length; i++)
            {
                Console.WriteLine("Q{0} = {1}", i, Q[i]);
            }
        }

        private static double Factorial(double n)
        {
            if (n == 1 || n == 0) return 1;

            return n * Factorial(n - 1);
        }
        private static double PiOfK(double K,double e, double u, double r)
        {
            double res = -1;
            if (K <= r)
            {
                res = Math.Pow(e / u, K) * (1/ Factorial(K));
            }
            else
            {
                res = Math.Pow(e / u, K) *(1/(Factorial(r)*Math.Pow(r,K-r)));
            }
            return res;
        }

        private static double FindSumForCN(double N, double a, double[] e, double[] u, double[] r)
        {
            double res = 0;
            for (double b = 0; b <= N-a; b++)
            {
                res += PiOfK(b, e[1], u[1], r[1]) * PiOfK(N - a - b, e[2], u[2], r[2]);
            }
            return res;
        }
        private static double FindCN(double N, double[] e, double[] u, double[] r)
        {
            double res = 0;
            for (double a = 0; a <= N; a++)
            {
                res += PiOfK(a, e[0], u[0], r[0]) * FindSumForCN(N, a, e,u, r);
            }
            res = Math.Pow(res, -1);
            return res;
        }
        private static double FindPSMOI(int i, double j, double N, double[] e, 
            double[] u, double[] r, double CN)
        {
            double  res = 0;
            switch (i)
            {
                case 0:
                    for (int a = 0; a <= N - j; a++)
                    {
                        res += PiOfK(j, e[0], u[0], r[0]) * PiOfK(a, e[1], u[1], r[1]) *
                            PiOfK(N - j - a, e[2], u[2], r[2]);
                    }
                    break;
                case 1:
                    for (int a = 0; a <= N - j; a++)
                    {
                        res += PiOfK(a, e[0], u[0], r[0]) * PiOfK(j, e[1], u[1], r[1]) * 
                            PiOfK(N - j - a, e[2], u[2], r[2]);
                    }
                    break;
                case 2:
                    for (int a = 0; a <= N - j; a++)
                    {
                        res += PiOfK(a, e[0], u[0], r[0]) * PiOfK(N - j - a, e[1], u[1], r[1]) *
                            PiOfK(j, e[2], u[2], r[2]);
                    }
                    break;
            }

            res *= CN;
            return res;
        }
    }
}