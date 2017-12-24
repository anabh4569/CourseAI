using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LinearAlgebra;

namespace CourseAI.Helper
{
    public static class LinearExtensions
    {
        public static float Distance(this Vector a, Vector b)
        {
            if (a.Dimension != b.Dimension)
                throw new ArgumentException("Dimension mismatch.");
            double sum = 0;
            for (int i = 0; i < a.Dimension; i++)
            {
                sum += Math.Pow((a[i] - b[i]), 2);
            }
            return (float)Math.Sqrt(sum);
        }
    }
}
