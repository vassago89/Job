using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Net.Framework.Helper.Patterns
{
    public class PipeLine<T>
    {
        public bool IsRun => _token == null ? false : _token.IsCancellationRequested == false;
        
        public int Count { get; set; }

        CancellationToken _token;

        private ConcurrentQueue<T> _queue = new ConcurrentQueue<T>();
        protected ConcurrentQueue<T> Queue => _queue;
        private ManualResetEvent _resetEvent = new ManualResetEvent(false);

        public EventHandler<Exception> ExceptionHanlder { get; set; }
        
        private bool _islastAccess;

        public PipeLine(bool islastAccess)
        {
            _islastAccess = islastAccess;
        }

        public virtual PipeLine<T> Setup()
        {
            return this;
        }

        public Action<T> Job;
        
        public event Action PrevRun;
        public event Action PostRun;
        public event Action PrevCancle;
        public event Action PostCancle;
        
        public void Run(CancellationToken token)
        {
            _token = token;
            _token.Register(() =>
            {
                PrevCancle?.Invoke();

                _resetEvent.Set();

                PostCancle?.Invoke();
            });

            Task.Factory.StartNew(() =>
            {
                PrevRun?.Invoke();

                while (token.IsCancellationRequested == false)
                {
                    try
                    {
                        if (_queue.IsEmpty)
                        {
                            _resetEvent.Reset();
                            _resetEvent.WaitOne();
                        }

                        if (_queue.TryDequeue(out T data))
                        {
                            if (_islastAccess && Count > 1)
                            {
                                Count = _queue.Count;
                                continue;
                            }

                            Job(data);
                            Count = _queue.Count;
                        }
                    }
                    catch (Exception e)
                    {
                        ExceptionHanlder?.Invoke(this, e);
                    }
                }

                while (!_queue.IsEmpty)
                    _queue.TryDequeue(out var result);

                Count = _queue.Count;

                PostRun?.Invoke();
            }, TaskCreationOptions.LongRunning);
        }

        public virtual void Enqueue(T data)
        {
            if (_token.IsCancellationRequested)
                return;

            lock (_queue)
                _queue.Enqueue(data);

            _resetEvent.Set();
            Count = _queue.Count;
        }
    }
}
