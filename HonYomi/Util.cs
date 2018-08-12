using System;
using System.Text;

namespace HonYomi
{
    public static class Util
    {
        public static string LCS(string s1, string s2)
        {
            if (string.IsNullOrEmpty(s1) && string.IsNullOrEmpty(s2))
                return "";
            if (string.IsNullOrEmpty(s1))
                return s2;
            if (string.IsNullOrEmpty(s2))
                return s1;

            int[,]        num             = new int[s1.Length, s2.Length];
            int           maxlen          = 0;
            int           lastSubsBegin   = 0;
            StringBuilder sequenceBuilder = new StringBuilder();

            for (int i = 0; i < s1.Length; i++)
            {
                for (int j = 0; j < s2.Length; j++)
                {
                    if (s1[i] != s2[j])
                        num[i, j] = 0;
                    else
                    {
                        if ((i == 0) || (j == 0))
                            num[i, j] = 1;
                        else
                            num[i, j] = 1 + num[i - 1, j - 1];

                        if (num[i, j] > maxlen)
                        {
                            maxlen = num[i, j];
                            int thisSubsBegin = i - num[i, j] + 1;
                            if (lastSubsBegin == thisSubsBegin)
                            { //if the current LCS is the same as the last time this block ran
                                sequenceBuilder.Append(s1[i]);
                            }
                            else //this block resets the string builder if a different LCS is found
                            {
                                lastSubsBegin          = thisSubsBegin;
                                sequenceBuilder.Length = 0; //clear it
                                sequenceBuilder.Append(s1.Substring(lastSubsBegin, (i + 1) - lastSubsBegin));
                            }
                        }
                    }
                }
            }
            return sequenceBuilder.ToString();
        }
    }
}