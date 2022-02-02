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
            string[] parts = src.Split(new string[]{". ", "! ", "? ", ".\u00A0", Environment.NewLine}, StringSplitOptions.RemoveEmptyEntries); //The u00A0 is a non-breaking space character (https://www.compart.com/en/unicode/U+00A0) 
            

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
                    if (loc1 > -1)
                    {
                        if (src.Length > ToReturn[t].Length)
                        {
                            string ActualPunctuation = src.Substring(loc1 + ToReturn[t].Length, 1);
                            ToReturn[t] = ToReturn[t] + ActualPunctuation;
                        }
                    }
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

        //Identifies things like products, organizations, places, etc. For example, something that deserves to be capitalized.
        public static string[] IdentifyPotentialEntities(this string src)
        {
            List<string> ToReturn = new List<string>();
            foreach (string sentence in src.SeparateSentences())
            {

                //Split into words
                string[] words = sentence.SeparateWords();

                //Buffer (in case a product is multiple words)
                List<string> ProductBuffer = new List<string>();

                //Go through each word and collect potentials
                for (int t = 1; t < words.Length; t++)
                {
                    string ThisWord = words[t];

                    //Assess if this word is potentially something important
                    bool IsImportantWord = false;
                    if (ThisWord != ThisWord.ToLower() && ThisWord.ToUpper() != ThisWord) //Crude way of checking if it is capitalized. And checking that it is not ALL capitalized (i.e. an acronym)
                    {
                        IsImportantWord = true;
                    }
                    else if (ProductBuffer.Count > 0) //Only consider these as important indicators if they are NOT the first word in the important phrase. i.e. "Dynamics 365" is important.... but "365" on its own is not.
                    {
                        if (TimHanewich.Toolkit.HanewichStringToolkit.FilterCharacters(ThisWord, "1234567890.") == ThisWord) //Checking if it is numeric
                        {
                            IsImportantWord = true;
                        }
                        else if (ThisWord == "&")
                        {
                            IsImportantWord = true;
                        }
                    }
                    
                    
                    //Is the first letter capitalized?
                    if (IsImportantWord)
                    {
                        ProductBuffer.Add(ThisWord);
                    }
                    else //The first letter is not capitalized. So dump the buffer and clear
                    {
                        if (ProductBuffer.Count > 0)
                        {
                            //Construct the full phrase
                            string ThisProductToAdd = "";
                            foreach (string s in ProductBuffer)
                            {
                                ThisProductToAdd = ThisProductToAdd + s + " ";
                            }
                            ThisProductToAdd = ThisProductToAdd.Substring(0,ThisProductToAdd.Length - 1);

                            //Ensure this phrase is actually said in the document
                            //For example, if the phrase is split by a comma, that would not have been picked up. The words would just be provided in a list
                            if (src.Contains(ThisProductToAdd))
                            {
                                ToReturn.Add(ThisProductToAdd);
                                ProductBuffer.Clear();
                            }
                        }
                    }
                }
            
                //Do a final buffer dump if there is content
                if (ProductBuffer.Count > 0)
                {
                    //Construct the full phrase
                    string ThisProductToAdd = "";
                    foreach (string s in ProductBuffer)
                    {
                        ThisProductToAdd = ThisProductToAdd + s + " ";
                    }
                    ThisProductToAdd = ThisProductToAdd.Substring(0,ThisProductToAdd.Length - 1);

                    //Ensure this phrase is actually said in the document
                    //For example, if the phrase is split by a comma, that would not have been picked up. The words would just be provided in a list
                    if (src.Contains(ThisProductToAdd))
                    {
                        ToReturn.Add(ThisProductToAdd);
                        ProductBuffer.Clear();
                    }
                }
            
            }

            //Cleanse the array to return before returning (remove duplicates, etc)
            List<string> ToReturnCleansed = new List<string>();
            foreach (string s in ToReturn)
            {
                if (ToReturnCleansed.Contains(s) == false)
                {
                    ToReturnCleansed.Add(s);
                }
            }
            ToReturn = ToReturnCleansed;


            return ToReturn.ToArray();
        }

        public static int CountOccurences(this string src, string phrase)
        {
            string[] parts = src.Split(new string[]{phrase}, StringSplitOptions.None);
            return parts.Length - 1;
        }

        //Same as above, EXCEPT: Counts the # of distinct phrases that match the phrase exactly. So a space or period has to come after the phrase for it to be "isolated". For example, if you count the number of occurances for the word "no", you would naturally also be including the number of "now", since "no" is in "now". Use this method to get around that issue.
        public static int CountIsolatedOccurences(this string src, string phrase)
        {
            string[] parts = src.Split(new string[]{phrase + " ", phrase + ".", phrase + "!", phrase + "?", phrase + ","}, StringSplitOptions.None);
            return parts.Length - 1;
        }

    }
}