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

                //Find the end location of the number
                int NumberEndLocation = src.IndexOf(new string[]{" ", Environment.NewLine}, NextNumIndex);
                if (NumberEndLocation > -1)
                {
                    //Get the number text - i.e. 72%, 400, 10, etc. (NOT "million", or "billion" if it comes after)
                    string NumberTxt = src.Substring(NextNumIndex, NumberEndLocation - NextNumIndex);

                    //If the last chracter in the numebr text is a comma or a period, move it back one
                    string LastChar = NumberTxt.Substring(NumberTxt.Length - 1, 1);
                    if (LastChar == "," || LastChar == ".")
                    {
                        NumberEndLocation = NumberEndLocation - 1;

                        //No re-get the number txt, this time with the new number end location
                        NumberTxt = src.Substring(NextNumIndex, NumberEndLocation - NextNumIndex);
                    }
                    
                    //Do a quick gut-check... is this even a number? If we detected something that is NOT a number, don't even bother logging it
                    bool IsDetectedNumber = false;
                    try
                    {
                        if (NumberTxt.Contains("%"))
                        {
                            IsDetectedNumber = true;
                        }
                        else
                        {
                            Convert.ToSingle(NumberTxt);
                            IsDetectedNumber = true;
                        }
                    }
                    catch
                    {
                        IsDetectedNumber = false;   
                    }

                    //If it is a number that we detected, move on
                    if (IsDetectedNumber)
                    {
                        //Get the position of the space that comes AFTER the word that immediately comes after the number portion above
                        int NextNextSpaceLocation = src.IndexOf(" ", NumberEndLocation + 1);
                        if (NextNextSpaceLocation > -1)
                        {
                            string TrailingWord = src.Substring(NumberEndLocation, NextNextSpaceLocation - NumberEndLocation);
                            TrailingWord = TrailingWord.ToLower().Trim();
                            if (TrailingWord == "hundred" || TrailingWord == "thousand" || TrailingWord == "million" || TrailingWord == "billion" || TrailingWord == "trillion")
                            {
                                ThisNumberFeature.Length = NextNextSpaceLocation - NextNumIndex;
                            }
                        }

                        //If length is still 0, it means we did not a append a "hundred", "thousand", etc to the end. So just set the length to the number text, i.e. "400"
                        if (ThisNumberFeature.Length == 0) //If Length is still 0
                        {
                            ThisNumberFeature.Length = NumberEndLocation - NextNumIndex;
                        }


                        //Add it
                        ToReturn.Add(ThisNumberFeature);

                    }
                }
                
                

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
