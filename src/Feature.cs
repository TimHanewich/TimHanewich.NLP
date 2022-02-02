using System;
using System.Collections.Generic;

namespace TimHanewich.NLP
{
    public class Feature
    {
        public int Offset {get; set;} //The beginning of the element
        public int Length {get; set;} //How many characters long the element is

        //Retrieves the content from the original source text provided
        public string Read(string source)
        {
            if (source.Length < Offset)
            {
                throw new Exception("Unable to extract feature text: The source provided was not long enough.");
            }

            string ToReturn = source.Substring(Offset, Length);
            return ToReturn;
        }

        public string ReadContainingSentence(string source)
        {
            int SentenceBegins = source.LastIndexOf(NlpToolkit.SentenceTerminators, Offset);
            int SentenceEnds = source.IndexOf(NlpToolkit.SentenceTerminators, Offset + Length);
            string ContainingSentence = source.Substring(SentenceBegins + 1, SentenceEnds - SentenceBegins + 1);
            ContainingSentence = ContainingSentence.Trim();
            return ContainingSentence;
        }


        #region "Extraction Constructors"

        public static Feature[] IdentifyNumbers(string src)
        {
            List<Feature> ToReturn = new List<Feature>();
            int NextNumIndex = FindNextNumberIndex(src, 0);
            while (NextNumIndex > -1)
            {

                Feature ThisNumberFeature = new Feature();

                //Start location
                ThisNumberFeature.Offset = NextNumIndex;

                //Get a list of characters that would be considered still part of the number
                List<string> ConsideredPartOfNumber = new List<string>();
                ConsideredPartOfNumber.Add("0");
                ConsideredPartOfNumber.Add("1");
                ConsideredPartOfNumber.Add("2");
                ConsideredPartOfNumber.Add("3");
                ConsideredPartOfNumber.Add("4");
                ConsideredPartOfNumber.Add("5");
                ConsideredPartOfNumber.Add("6");
                ConsideredPartOfNumber.Add("7");
                ConsideredPartOfNumber.Add("8");
                ConsideredPartOfNumber.Add("9");
                ConsideredPartOfNumber.Add(".");

                //Continue to collect the number
                List<string> CharsInThisNumber = new List<string>();
                for (int i = NextNumIndex; i < src.Length; i++)
                {
                    string ThisChar = src[i].ToString();
                    if (ConsideredPartOfNumber.Contains(ThisChar)) //We are still in the number, so add the char
                    {
                        CharsInThisNumber.Add(ThisChar);
                    }
                    else //We are no longer in the number, so stop the loop 
                    {
                        break;
                    }
                }

                //If the final character is a period, it is probably because it is the end of a sentence, not serving as a decimal point.
                if (CharsInThisNumber.Contains("."))
                {
                    if (CharsInThisNumber[CharsInThisNumber.Count-1] == ".")
                    {
                        CharsInThisNumber.RemoveAt(CharsInThisNumber.Count-1); //Remove the last period.
                    }
                }

                //Set end location
                ThisNumberFeature.Length = CharsInThisNumber.Count;

                //Add it to the list
                ToReturn.Add(ThisNumberFeature);
                

                //Refresh next num index
                NextNumIndex = FindNextNumberIndex(src, NextNumIndex + 1);
            }

            return ToReturn.ToArray();
        }

        #endregion

        #region "toolkit"

        //Finds the next number (0, 1, 2, 3, 4, 5, 6, 7, 8, 9) in a string
        private static int FindNextNumberIndex(string in_text, int start_at)
        {
            int ToReturn = in_text.IndexOf(new string[]{" 0", " 1", " 2", " 3", " 4", " 5", " 6", " 7", " 8", " 9"}, start_at); //Places spaces in front. Because if we are looking at a number such as 4000, it would find the 4, next 0, next 0, and next 0, as the "next number". But this method is only supposed to return the start indexes of numbers that come aftet spaces.
            if (ToReturn > -1) //If we found something, add one to it. Because we actually want to return the position of the number, not the space. BUT, if it is negative 1 (no numbers were found at all), leave it at -1 and return that.
            {
                ToReturn = ToReturn + 1;
            }
            return ToReturn;
        }

        #endregion

    }
}
