using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InstalingBOT
{
    internal class Words
    {
        public static List<String> translateActive = new List<string> { };
        public static List<Word> wordsList = new List<Word>();
        public static List<Word> sessionWords = new List<Word>();

        public static void InsertWords()
        {
            string fileName = "words.txt";

            using (StreamReader reader = new StreamReader(fileName))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    string[] pn = line.Split('|');

                    wordsList.Add(new Word { polish = pn[0], german = pn[1] });
                    translateActive.Add(pn[0]);
                }
            }
        }

        public static void GetNewWords()
        {
            string data = "";

            foreach (var el in wordsList)
            {
                if (el.german != "" && el.polish != "")
                {
                    data += el.polish + "|" + el.german + Environment.NewLine;
                }
            }

            File.WriteAllText("words.txt", data);
        }
    }
}
