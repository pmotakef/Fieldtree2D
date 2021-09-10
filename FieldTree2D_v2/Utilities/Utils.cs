using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FieldTree2D_v2.Utilities
{
    public static class Utils
    {

        public static Tuple<TObj, TVal> MinObj<TObj, TVal>(IEnumerable<TObj> objs, Func<TObj, TVal> func, IComparer<TVal> comparer = null)
        {
            if (objs == null) throw new ArgumentNullException("source");
            if (func == null) throw new ArgumentNullException("selector");

            comparer = comparer ?? Comparer<TVal>.Default;

            TObj minObj = default(TObj);
            TVal minVal = default(TVal);

            using (var iterator = objs.GetEnumerator())
            {
                bool first_entry = true;

                while (iterator.MoveNext())
                {
                    var obj = iterator.Current;
                    var val = func(obj);
                    if (first_entry)
                    {
                        minObj = obj;
                        minVal = val;
                        first_entry = false;
                    }
                    if (comparer.Compare(val, minVal) < 0)
                    {
                        minObj = obj;
                        minVal = val;
                    }
                }
            }
            return Tuple.Create(minObj, minVal);
        }

        public static Dictionary<TObj, TVal> MinOrUpperBound<TObj, TVal>(IEnumerable<TObj> objs, Func<TObj, TVal> func, TVal upperBound, IComparer<TVal> comparer = null)
        {
            if (objs == null) throw new ArgumentNullException("source");
            if (func == null) throw new ArgumentNullException("selector");

            comparer = comparer ?? Comparer<TVal>.Default;

            Dictionary<TObj, TVal> answer = new Dictionary<TObj, TVal>();

            if (objs == null || !objs.Any())
            {
                return answer;
            }

            TObj minObj = default(TObj);
            TVal minVal = default(TVal);
            bool used = false;

            using (var iterator = objs.GetEnumerator())
            {
                bool first_entry = true;

                while (iterator.MoveNext())
                {
                    var obj = iterator.Current;
                    var val = func(obj);
                    if (first_entry)
                    {
                        minObj = obj;
                        minVal = val;
                        first_entry = false;
                    }
                    if (comparer.Compare(val, upperBound) < 0)
                    {
                        minObj = obj;
                        minVal = val;
                        answer.Add(minObj, minVal);
                        used = true;
                    }
                    else if (comparer.Compare(val, minVal) < 0)
                    {
                        minObj = obj;
                        minVal = val;
                        used = false;
                    }
                }
                if (!used)
                {
                    answer.Add(minObj, minVal);
                }
            }
            return answer;
        }


        public static Tuple<TObj, TVal> MaxObj<TObj, TVal>(IEnumerable<TObj> objs, Func<TObj, TVal> func, IComparer<TVal> comparer = null)
        {
            if (objs == null) throw new ArgumentNullException("source");
            if (func == null) throw new ArgumentNullException("selector");

            comparer = comparer ?? Comparer<TVal>.Default;

            TObj maxObj = default(TObj);
            TVal maxVal = default(TVal);

            using (var iterator = objs.GetEnumerator())
            {
                bool first_entry = true;

                while (iterator.MoveNext())
                {
                    var obj = iterator.Current;
                    var val = func(obj);
                    if (first_entry)
                    {
                        maxObj = obj;
                        maxVal = val;
                        first_entry = false;
                    }
                    if (comparer.Compare(val, maxVal) > 0)
                    {
                        maxObj = obj;
                        maxVal = val;
                    }
                }
            }
            return Tuple.Create(maxObj, maxVal);
        }


    }
}
