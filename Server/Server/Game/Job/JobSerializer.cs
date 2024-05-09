using System;
using System.Collections.Generic;
using System.Text;

namespace Server.Game
{
    // 모아둔 Job 실행하는 클래스
    public class JobSerializer
	{
		Queue<IJob> _jobQueue = new Queue<IJob>();
		// 커맨드 패턴
		// 하나의 쓰레드가 끝날 때까지 다른 쓰레드는 아무것도 못하는
		// 상호배타적락이 아니라 queue에 접근 할 때만 lock이 있는 것
		object _lock = new object();
		bool _flush = false;
		public void Push(Action action) { Push(new Job(action)); }
		public void Push<T1>(Action<T1> action, T1 t1) { Push(new Job<T1>(action, t1)); }
		public void Push<T1, T2>(Action<T1, T2> action, T1 t1, T2 t2) { Push(new Job<T1, T2>(action, t1, t2)); }
		public void Push<T1, T2, T3>(Action<T1, T2, T3> action, T1 t1, T2 t2, T3 t3) { Push(new Job<T1, T2, T3>(action, t1, t2, t3)); }
		public void Push<T1, T2, T3, T4>(Action<T1, T2, T3, T4> action, T1 t1, T2 t2, T3 t3, T4 t4) { Push(new Job<T1, T2, T3, T4>(action, t1, t2, t3, t4)); }
		public void Push(IJob job)
		{
			bool flush = false;

			lock (_lock)
			{
				_jobQueue.Enqueue(job);
				// job에 처음 들어올땐 flush 조작해 수동으로 실행해주기
				// 비유하자면 첫 쓰레드가 실행해서 주문서를 들고 왔는데
				// 주방장이 없어서 본인 주문서를 본인이 요리함
				// 다음 쓰레드가 오면 주방장이 있는 걸 보고 주문서만 던져주고 옴
				if (_flush == false)
					flush = _flush = true;
			}

			if (flush)
				Flush();
		}

		void Flush()
		{
			while (true)
			{
				IJob job = Pop();
				if (job == null)
					return;

				job.Execute();
			}
		}

		IJob Pop()
		{
			lock (_lock)
			{
				// queue에 job 다 처리하면 빠져나오기
				if (_jobQueue.Count == 0)
				{
					_flush = false;
					return null;
				}
				return _jobQueue.Dequeue();
			}
		}

	}
}
