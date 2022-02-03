﻿using System;
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
            int SentenceBegins = source.LastIndexOf(NlpToolkit.SentenceTerminators, Offset); //An index of -1 is okay here, in  the case it does not find an end of a sentence previously. This is becaue the sentence extraction substring below moves the index up 1
            int SentenceEnds = source.IndexOf(NlpToolkit.SentenceTerminators, Offset + Length);
            if (SentenceEnds == -1) //If we could not find the end of a sentence (for example, period with a space after), the end of the document itself must be the end of the sentence.
            {
                SentenceEnds = source.Length - 2;
            }
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
                ConsideredPartOfNumber.Add("."); //For example, in the number "1.16"
                ConsideredPartOfNumber.Add(","); //For example, in the number "100,000"

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
                string LastChar = CharsInThisNumber[CharsInThisNumber.Count-1];
                if (LastChar == "." || LastChar == ",")
                {
                    CharsInThisNumber.RemoveAt(CharsInThisNumber.Count-1); //Remove the last period of comma or whatever
                }

                //Set end location
                ThisNumberFeature.Length = CharsInThisNumber.Count;

                //Add it to the list
                ToReturn.Add(ThisNumberFeature);
                

                //Refresh next num index
                NextNumIndex = FindNextNumberIndex(src, NextNumIndex + ThisNumberFeature.Length);
            }

            return ToReturn.ToArray();
        }

        //Identifies numbers, but also includes things like $ sign, % sign, "billion", "million" after, etc.
        public static Feature[] IdentifyNumberPhrases(string src)
        {
            Feature[] NumbersOnly = IdentifyNumbers(src);
            List<Feature> ToReturn = new List<Feature>();
            foreach (Feature f in NumbersOnly)
            {
                

                //Find the starting position
                //Is the character right before the start a dollar sign? Or another sign that may be of interest to include?
                int PhraseStartAt = f.Offset; //Default is start at same location as only number
                if (f.Offset > 0)
                {
                    string CharacterBefore = src.Substring(f.Offset - 1, 1);
                    if (CharacterBefore == "$")
                    {
                        PhraseStartAt = f.Offset - 1;
                    }
                }

                //Find the ending positioon
                //Search for common phrases that might come after the number itself - "thousand", "million", etc.
                int PhraseEndAt = f.Offset + f.Length; //Default is where it ended previously.
                if (IsFollowedBy(src, f.Offset + f.Length, " hundred thousand"))
                {   
                    PhraseEndAt = PhraseEndAt + " hundred thousand".Length;
                }
                else if (IsFollowedBy(src, f.Offset + f.Length, " hundred"))
                {
                    PhraseEndAt = PhraseEndAt + " hundred".Length;
                }
                else if (IsFollowedBy(src, f.Offset + f.Length, " thousand"))
                {
                    PhraseEndAt = PhraseEndAt + " thousand".Length;
                }
                else if (IsFollowedBy(src, f.Offset + f.Length, " million"))
                {
                    PhraseEndAt = PhraseEndAt + " million".Length;
                }
                else if (IsFollowedBy(src, f.Offset + f.Length, " billion"))
                {
                    PhraseEndAt = PhraseEndAt + " billion".Length;
                }
                else if (IsFollowedBy(src, f.Offset + f.Length, " trillion"))
                {
                    PhraseEndAt = PhraseEndAt + " trillion".Length;
                }
                
                

                //Create and set the values
                Feature NumberPhrase = new Feature();
                NumberPhrase.Offset = PhraseStartAt;
                NumberPhrase.Length = PhraseEndAt - PhraseStartAt;
                ToReturn.Add(NumberPhrase);
            }
            
            return ToReturn.ToArray();
        }

        #endregion

        #region "toolkit"

        //Finds the next number (0, 1, 2, 3, 4, 5, 6, 7, 8, 9) in a string
        private static int FindNextNumberIndex(string in_text, int start_at)
        {
            int ToReturn = in_text.IndexOf(new string[]{"0", "1", "2", "3", "4", "5", "6", "7", "8", "9"}, start_at);
            return ToReturn;
        }

        private static bool IsFollowedBy(string document, int start, string followed_by)
        {
            if (document.Length >= start + followed_by.Length)
            {
                string PartAfter = document.Substring(start, followed_by.Length);
                if (PartAfter == followed_by)
                {
                    return true;
                }
            }
            return false;
        }

        #endregion

    }
}
