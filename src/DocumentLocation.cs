using System;

namespace TimHanewich.NLP
{
    public class DocumentLocation
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

    }
}