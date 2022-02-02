using System;
using System.Collections.Generic;

namespace TimHanewich.NLP
{
    public class PhraseProfile
    {
        public string Phrase {get; set;}
        public int Occurences {get; set;}

        public static PhraseProfile Create(string document, string phrase)
        {
            PhraseProfile pp = new PhraseProfile();
            pp.Phrase = phrase;
            pp.Occurences = document.CountIsolatedOccurences(phrase);
            return pp;
        }

        public static PhraseProfile[] Create(string document, string[] phrases)
        {
            List<PhraseProfile> ToReturn = new List<PhraseProfile>();

            //Count
            foreach (string phrase in phrases)
            {
                ToReturn.Add(PhraseProfile.Create(document, phrase));
            }

            //Arrange from most frequent to least frequent
            List<PhraseProfile> Sorted = new List<PhraseProfile>();
            while (ToReturn.Count > 0)
            {
                PhraseProfile Winner = ToReturn[0];
                foreach (PhraseProfile pp in ToReturn)
                {
                    if (pp.Occurences > Winner.Occurences)
                    {
                        Winner = pp;
                    }
                }
                Sorted.Add(Winner);
                ToReturn.Remove(Winner);
            }
            ToReturn = Sorted;

            return ToReturn.ToArray();
        }
    }
}