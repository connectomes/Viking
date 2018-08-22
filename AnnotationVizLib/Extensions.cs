﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AnnotationVizLib
{
    public static class AttributeExtensions
    {
        /// <summary>
        /// Converts attributes to a string and caches the results.  Not caching the results was causing performance issues.
        /// </summary>
        /// <returns></returns>
        public static string AttributesToString(this IDictionary<string, object> dict)
        {
            StringBuilder sb = new StringBuilder();
            List<string> keys = dict.Keys.ToList();
            keys.Sort();

            foreach (string key in keys)
            {
                sb.AppendFormat(" {0} : {1}", key, dict[key].ToString());
            }

            return sb.ToString();
        }
    }

    public static class MathStatsExtensions
    {
        public static double StdDev(this IEnumerable<double> values)
        {
            double average = values.Average();
            double sumOfSquaresOfDifferences = values.Select(val => (val - average) * (val - average)).Sum();
            double sd = Math.Sqrt(sumOfSquaresOfDifferences / values.Count());
            return sd;
        }

        public static double StdDev(this ICollection<double> values)
        {
            double average = values.Average();
            double sumOfSquaresOfDifferences = values.Select(val => (val - average) * (val - average)).Sum();
            double sd = Math.Sqrt(sumOfSquaresOfDifferences / values.Count);
            return sd;
        }
    }
}
