using System;
using System.Collections.Generic;
using System.Text;

namespace OnTheHouse
{
    public static class MatchPairLevelSearch
    {
        public static List<string> PairMatchSearch(this string value, string left, string right)
        {
            int currentPosition = 0;
            int nextLeft = value.IndexOf(left, currentPosition);
            int level = 0;
            int start = 0;

            List<string> results = new List<string>();

            while (nextLeft > -1)
            {
                currentPosition = nextLeft + 1;
                if(level == 0)
                {
                    start = nextLeft;
                }
                level += 1;

                nextLeft = value.IndexOf(left, currentPosition);

                if (nextLeft == -1)
                {
                    nextLeft = value.Length; // reached end of string
                }
                    
                int nextRight = value.IndexOf(right, currentPosition);

                if(nextRight == -1) // end of string
                {
                    return results; // no more matches
                }

                if(nextLeft< nextRight)
                {
                    
                }

                while(nextRight < nextLeft)
                {
                    currentPosition = nextRight + 1;
                    level -= 1;

                    if(level > 0)
                    {
                        nextRight = value.IndexOf(right, currentPosition);
                        if(nextRight == -1)
                        {
                            return results; // no more matches
                        }
                    }
                    else
                    {
                        results.Add(value.Substring(start, currentPosition - start));
                        nextRight = value.IndexOf(right, currentPosition);
                        if (nextRight == -1)
                        {
                            return results;
                        }
                    }
                }
            }

            return results;
        }

    }
}
