using System;
using System.Collections.Generic;
using TimHanewich.Toolkit;
using TimHanewich.Toolkit.TextAnalysis;

namespace TimHanewich.NLP
{
    public static class NlpToolkit
    {

        //Return the index of the first found value in the find parameters
        public static int IndexOf(this string src, string[] find, int start_at)
        {
            int ToReturn = -1;
            foreach (string s in find)
            {
                int TryThis = src.IndexOf(s, start_at);
                if (TryThis > -1)
                {
                    if (ToReturn == -1) //If ToReturn is the default (-1), just replace it with whatever this is. Because this is better than the default!
                    {
                        ToReturn = TryThis;
                    }
                    else //Since ToReturn already has a value (we found someting close earlier), we must test to see if this one is closer.
                    {
                        if (TryThis < ToReturn)
                        {
                            ToReturn = TryThis;
                        }
                    }
                }
            }
            return ToReturn;
        }


        public static string[] SeparateSentences(this string src)
        {
            string[] parts = src.Split(new string[]{". ", "! ", "? ", Environment.NewLine}, StringSplitOptions.RemoveEmptyEntries);
            

            //Get a list of sentences we want to return
            List<string> ToReturn = new List<string>();
            foreach (string p in parts)
            {
                string ThisSentence = p;
                if (ThisSentence != "")
                {
                    ThisSentence = ThisSentence.Trim();
                    ToReturn.Add(ThisSentence);
                }
            }


            //Ensure each sentence has the correct puncutation at the end of it
            for (int t = 0; t < ToReturn.Count; t++)
            {
                string LastChar = ToReturn[t].Substring(ToReturn[t].Length - 1, 1);
                if (LastChar != "." && LastChar != "?" && LastChar != "!") //If it does NOT have punctuation, we need to append it to it
                {
                    int loc1 = src.IndexOf(ToReturn[t]); //Get the location of the begin
                    string ActualPunctuation = src.Substring(loc1 + ToReturn[t].Length, 1);
                    ToReturn[t] = ToReturn[t] + ActualPunctuation;
                }
            }



            return ToReturn.ToArray();
        }

        public static string[] SeparateWords(this string src)
        {
            List<string> ToReturn = new List<string>();
            foreach (string s in src.SeparateSentences())
            {
                string[] words = s.Split(new string[]{" "}, StringSplitOptions.RemoveEmptyEntries);
                foreach (string word in words)
                {
                    string WordToAdd = HanewichStringToolkit.FilterCharacters(word, "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890");
                    WordToAdd = WordToAdd.Trim();
                    ToReturn.Add(WordToAdd);
                }
            }
            return ToReturn.ToArray();
        }

    }
}