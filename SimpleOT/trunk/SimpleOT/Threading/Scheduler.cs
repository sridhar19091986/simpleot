using System;
using System.Collections.Generic;
using System.Threading;
using SimpleOT.Collections;
using SimpleOT.IO;

namespace SimpleOT.Threading
{
    public class Scheduler
    {
        private Thread _thread;
        private readonly PriorityQueue<Schedule> _queue;
        private SchedulerState _state;
        private uint _lastScheduletId;
        private readonly ISet<uint> _scheduleIds;
        private readonly object _lock;
        private Dispatcher _dispatcher;

        public Scheduler(Dispatcher dispatcher)
        {
            if (dispatcher == null)
                throw new ArgumentNullException("dispatcher");

            _dispatcher = dispatcher;
            _lock = new object();
            _state = SchedulerState.Terminated;
            _queue = new PriorityQueue<Schedule>();
            _lastScheduletId = 0;
            _scheduleIds = new HashSet<uint>();
            _lastScheduletId = Constants.SchedulerStartId;
        }

        public void Start()
        {
            lock (_lock)
            {
                if (_state == SchedulerState.Running)
                    throw new InvalidOperationException("The scheduler is already running.");

                if (_state == SchedulerState.Closing)
                {
                    _state = SchedulerState.Running;

#if DEBUG_SCHEDULER
					Logger.Debug("Resuming scheduler.");
#endif

                }
                else
                {

#if DEBUG_SCHEDULER
                    Logger.Debug("Starting scheduler.");
#endif

                    _state = SchedulerState.Running;
                    _thread = new Thread(Run) { IsBackground = false };
                    _thread.Start();
                }
            }
        }

        public void Shutdown()
        {
            lock (_lock)
            {
                if (_state == SchedulerState.Terminated)
                    throw new InvalidOperationException("The scheduler is already terminated.");

#if DEBUG_SCHEDULER
                Logger.Debug("Shutting down scheduler.");
#endif

                _state = SchedulerState.Terminated;
                _queue.Clear();
                _scheduleIds.Clear();
                _lastScheduletId = 0;
                Monitor.Pulse(_lock);
            }
        }

        public void Stop()
        {
            lock (_lock)
            {
                if (_state != SchedulerState.Running)
                    throw new InvalidOperationException("The scheduler is already stoped.");

#if DEBUG_SCHEDULER
                Logger.Debug("Stoping scheduler.");
#endif

                _state = SchedulerState.Closing;
            }
        }

        public uint Add(long delay, Action action)
        {
            return Add(new Schedule(delay, action));
        }

        public uint Add(Schedule schedule)
        {
            lock (_lock)
            {
                if (_state != SchedulerState.Running)
                    return 0;

                if (schedule.Id == 0)
                {
                    if (_lastScheduletId == uint.MaxValue)
                        _lastScheduletId = Constants.SchedulerStartId;

                    schedule.Id = ++_lastScheduletId;
                }

                _scheduleIds.Add(schedule.Id);
                _queue.Enqueue(schedule);

                if (_queue.Peek() == schedule)
                    Monitor.Pulse(_lock);
            }

            return schedule.Id;
        }

        public bool Remove(uint scheduleId)
        {
            lock (_lock)
            {
                return _scheduleIds.Remove(scheduleId);
            }
        }

        private void Run()
        {
            while (_state != SchedulerState.Terminated)
            {
                Schedule schedule = null;
                var ret = true;
                var runTask = false;

                lock (_lock)
                {
                    if (_queue.IsEmpty)
                    {
                        Monitor.Wait(_lock);
                    }
                    else
                        ret = _queue.Peek().LifeCycle > DateTime.Now.Ticks && Monitor.Wait(_lock, _queue.Peek().WaitTime);

                    if (ret == false && _state != SchedulerState.Terminated)
                    {
                        schedule = _queue.Dequeue();

                        if (schedule != null && _scheduleIds.Remove(schedule.Id))
                            runTask = true;
                    }
                }

                if (runTask)
                {
                    _dispatcher.Add(schedule);
                }
            }
        }

        public Dispatcher Dispatcher { get { return _dispatcher; } }
    }
}
