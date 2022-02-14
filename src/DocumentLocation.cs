using System;

namespace TimHanewich.NLP
{
    public class DocumentLocation
    {
        public int Offset {get; set;} //The beginning of the element
        public int Length {get; set;} //How many characters long the element is
    }
}