using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TFOF.Areas.Core.Helpers
{
    public static class ListHelper<T> where T : class
    {
        
        public static List<List<T>> Group(List<T> list, int numberOfColumns)
        {
            List<List<T>> Lists = new List<List<T>>();

            int itemsInEachSet = (list.Count() / ((numberOfColumns == 0) ? 1: numberOfColumns));
            if((itemsInEachSet * numberOfColumns) < list.Count())
            {
                itemsInEachSet += 1;
            }
            for (int i = 0; i < numberOfColumns; i++)
            {
                try
                {
                    Lists.Add(list.GetRange((i * itemsInEachSet), itemsInEachSet));
                }
                catch
                {
                    Lists.Add(list.GetRange((i * itemsInEachSet), list.Count() - (i * itemsInEachSet)));
                }
            }
            return Lists;
        }

        public static string ListToString(Stack<String> listOfStrings, string delimiter = ", ", string lastDelimiter = " and ") {

            string last = listOfStrings.Pop();
            if (listOfStrings.Count > 0)
            {
                var returnString = string.Join(delimiter, listOfStrings);
                return (returnString += lastDelimiter + last);
            } else
            {
                return last;
            }
        }
    }
}