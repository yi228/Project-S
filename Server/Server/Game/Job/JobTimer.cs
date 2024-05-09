using System;
using System.Collections.Generic;
using System.Text;
using ServerCore;

namespace Server.Game
{
    struct JobTimerElem : IComparable<JobTimerElem>
    {
        public int execTick; // 실행 시간
        public IJob job;

        public int CompareTo(JobTimerElem other)
        {
            return other.execTick - execTick;
        }
    }

    public class JobTimer
    {
        PriorityQueue<JobTimerElem> _pq = new PriorityQueue<JobTimerElem>();
        object _lock = new object();
        public void Push(IJob job, int tickAfter = 0)
        {
            JobTimerElem jobElement;
            jobElement.execTick = Environment.TickCount + tickAfter;
            jobElement.job = job;

            lock (_lock)
            {
                _pq.Push(jobElement);
            }
        }

        // while 돌려서 실행할 조건 되는지 검사하고 조건 만족시 실행
        public void Flush()
        {
            while (true)
            {
                int now = Environment.TickCount;

                JobTimerElem jobElement;

                lock (_lock)
                {
                    if (_pq.Count == 0)
                        break;

                    jobElement = _pq.Peek();
                    if (jobElement.execTick > now)
                        break;

                    _pq.Pop();
                }

                jobElement.job.Execute();
            }
        }
    }
}
