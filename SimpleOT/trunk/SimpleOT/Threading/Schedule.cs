using System;

namespace SimpleOT.Threading
{
    public class Schedule : Task, IComparable<Schedule>
    {
        protected long _lifeCycle;
        protected uint _id;

        public Schedule(long delay, Action action)
            : base(action, 0)
        {
            if (delay < Constants.SCHEDULER_MIN_TIME)
                delay = Constants.SCHEDULER_MIN_TIME;

            _lifeCycle = DateTime.Now.AddMilliseconds(delay).Ticks;
        }

        public long LifeCycle
        {
            get { return _lifeCycle; }
        }

        public uint Id
        {
            get { return _id; }
            set { _id = value; }
        }

        public TimeSpan WaitTime
        {
            get
            {
                var waitTime = TimeSpan.FromTicks(_lifeCycle - DateTime.Now.Ticks);
                return waitTime.Milliseconds < 0 ? TimeSpan.Zero : waitTime;
            }
        }

        public int CompareTo(Schedule schedule)
        {
            return _lifeCycle.CompareTo(schedule._lifeCycle);
        }
    }
}
