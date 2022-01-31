using System;
using TimHanewich.NLP;

namespace testing
{
    class Program
    {
        static void Main(string[] args)
        {
            
            Feature f = new Feature();
            Console.WriteLine(f.FindNextNumberIndex("My name is Tim 2 hehe tim 2?!", 0));
            Console.WriteLine(f.FindNextNumberIndex("My name is Tim 2 hehe tim 2?!", 15 + 1));


            //string s = "Sup my name is... Tim. What is your name? Tim!";
            //Console.WriteLine(s.IndexOf(new string[]{"hippo", "monkey"}, 0));


        }
    }
}
