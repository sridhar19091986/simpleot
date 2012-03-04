using System;
using System.Collections.Generic;
using System.Threading;
using NLog;

namespace SimpleOT.Threading
{
    public delegate void DispatcherEventHandler();

    public class Dispatcher
    {
		private static Logger logger = LogManager.GetCurrentClassLogger();
		
        public event DispatcherEventHandler BeforeDispatchTask;
        public event DispatcherEventHandler AfterDispatchTask;

        private Thread _thread;
        private readonly Queue<Task> _queue;
        private DispatcherState _state;

        private readonly object _lock;
		
        public Dispatcher()
        {
            _lock = new object();
            _queue = new Queue<Task>();
            _state = DispatcherState.Terminated;
        }

        public void Start()
        {
            lock (_lock)
            {
                if (_state == DispatcherState.Running)
                    throw new InvalidOperationException("The dispatcher is already running.");
				
				if(_state == DispatcherState.Closing) {
					_state = DispatcherState.Running;
				} else {
					_state = DispatcherState.Running;
	                _thread = new Thread(Run) { IsBackground = false };
                	_thread.Start();
				}
            }
        }

        public void Stop()
        {
            lock (_lock)
            {
                if (_state != DispatcherState.Running)
                    throw new InvalidOperationException("The dispatcher is already stoped.");

                _state = DispatcherState.Closing;
            }
        }

        public void Shutdown()
        {
            lock (_lock)
            {
                if (_state == DispatcherState.Terminated)
                    throw new InvalidOperationException("The dispatcher is already terminated.");

                _state = DispatcherState.Terminated;
                _queue.Clear();

                Monitor.Pulse(_lock);
            }
        }

        public void Add(Action action)
        {
            Add(new Task(action));
        }

        public void Add(Task task)
        {
            lock (_lock)
            {
                if (_state != DispatcherState.Running)
                    return;

                _queue.Enqueue(task);
                Monitor.Pulse(_lock);
            }
        }

        private void Run()
        {
            while (_state != DispatcherState.Terminated)
            {
                Task task = null;

                lock (_lock)
                {
                    if (_queue.Count == 0)
                    {
                        Monitor.Wait(_lock);
                    }

                    if (_queue.Count > 0 && _state != DispatcherState.Terminated)
                    {
                        task = _queue.Dequeue();
                    }

                }

                if (task == null)
                    continue;

                if (!task.HasExpired)
                {
                    try
                    {	
						if(BeforeDispatchTask != null)
							BeforeDispatchTask();
						
                        task.Execute();
                        
						if(AfterDispatchTask != null)
							AfterDispatchTask();
                    }
                    catch (Exception e)
                    {
                       logger.ErrorException(string.Format("Error on execute task {0}.", task.Action.Method.Name), e);
                    }
                }
            }
        }
    }
}
