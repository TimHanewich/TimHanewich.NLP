using System;

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

    }
}
