using System;

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


    }
}