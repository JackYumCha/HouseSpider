using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;

namespace DataSplitterTests
{
    static class DataUtil
    {
        public static string PropertyLocationCSVPath ()
        {
            var baseDir = new DirectoryInfo(AppContext.BaseDirectory);
            while(baseDir.Name != "Machine Learning Lecture")
            {
                baseDir = baseDir.Parent;
            }
            return $@"{baseDir}\Spider\PropertyLocations\20171215000odaddress.csv";
        }

        public static IEnumerable<List<T>> Split<T>(this IEnumerable<T> source, int batchCount)
        {
            int size = (int)Math.Ceiling((double)source.Count() / (double)batchCount);

            int index = 0;
            List<T> list = new List<T>();
            foreach(var item in source)
            {
                index += 1;
                if(index >= size)
                {
                    yield return list;
                    index = 0;
                    list = new List<T>();
                }
            }
            if (list.Count > 0)
                yield return list;
            yield break;
        }
    }
}
