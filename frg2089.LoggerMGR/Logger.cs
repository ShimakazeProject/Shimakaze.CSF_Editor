using System;

namespace frg2089.LoggerMGR
{
    public class Logger
    {
        public Rank LoggerRank { get; private set; }
        public Logger(Rank rank)
        {
            ChangeRank(rank);
        }
        public void ChangeRank(Rank rank)
        {
            LoggerRank = rank;
        }

        public virtual void Write(Rank rank, params string[] messages)
        {

        }
    }
}
