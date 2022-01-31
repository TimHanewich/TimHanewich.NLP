using System;
using System.Collections.Generic;

namespace TimHanewich.NLP
{
    public class Feature
    {
        public int Offset {get; set;} //The beginning of the element
        public int Length {get; set;} //How many characters long the element is

        //Retrieves the content from the original source text provided
        public string Text(string source)
        {
            if (source.Length < Offset)
            {
                throw new Exception("Unable to extract feature text: The source provided was not long enough.");
            }

            string ToReturn = source.Substring(Offset, Length);
            return ToReturn;
        }


        #region "toolkit"

        public int FindNextNumberIndex(string in_text, int start_at)
        {
            return in_text.IndexOf(new string[]{"0", "1", "2", "3", "4", "5", "6", "7", "8", "9"}, start_at);
        }


        #endregion

    }
}
