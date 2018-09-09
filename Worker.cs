//  Author: Ryan Morrissey
//  Date: 9/9/2018
//
//  This class defines the workers, and using DateTime checks, will have each worker call a specific
//  method defined from their configuation.

using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Threading;

namespace ModularAsync
{
    public class Worker
    {
        #region Worker Attributes
        private Random _rand = new Random();
        private bool _isWorkerWorking = false;
        private DateTime _workEndTime = DateTime.MaxValue;
        private DateTime _timeToNextWork = DateTime.MinValue;
        private int _workCounter = 0;
        private int _workGroupCounter = 0;
        private List<string[]> _workList = new List<string[]>();

        // Configuration Information
        private readonly string _workerId;
        private readonly int _minWorkRuntime = 1;
        private readonly int _maxWorkRuntime = 5;
        private readonly int _minTimeBetweenWork = 20;
        private readonly int _maxTimeBetweenWork = 40;
        #endregion


        #region Constructor
        public Worker(string workerId, string[] workArray, int minWorkerRuntime, int maxWorkerRuntime, int minTimeBetweenWork, int maxTimeBetweenWork)
        {
            _workerId = workerId;
            _minWorkRuntime = minWorkerRuntime;
            _maxWorkRuntime = maxWorkerRuntime;
            _minTimeBetweenWork = minTimeBetweenWork;
            _maxTimeBetweenWork = maxTimeBetweenWork;

            foreach (string workCommands in workArray)
            {
                string[] commands = workCommands.Split(',');
                _workList.Add(commands);
            }
        }
        #endregion

        // The idea is to use two counters, with one to go through the list, and another to go through
        // the list's values.  After a pass, reset the counter to zero to start over again.
        public async Task Update()
        {
            // Check to see if it is time for the worker to either start or stop working
            if ((_isWorkerWorking && DateTime.Compare(DateTime.Now, _workEndTime) >= 0) ||
                (!_isWorkerWorking && DateTime.Compare(DateTime.Now, _timeToNextWork) >= 0))
            {
                await ProcessWork();

                // Now move through and update the counters
                _workCounter++;
                if (_workCounter == _workList[_workGroupCounter].Length)
                {
                    _workCounter = Definitions.WorkStartingIndex;
                    if (_isWorkerWorking)
                    {
                        _workGroupCounter++;
                        if (_workGroupCounter == _workList.Count)
                        {
                            _workGroupCounter = Definitions.WorkStartingIndex; // Reset back to zero to loop through the work list again
                        }
                        _isWorkerWorking = false;
                    }
                    else
                    {
                        _isWorkerWorking = true;
                    }
                }
            }
        }

        // Since this is just an example with no time-consuming work, the methods below will run serially
        public async Task ProcessWork()
        {
            if (_isWorkerWorking)
            {
                if (_workCounter == 0)
                {
                    _timeToNextWork = DateTime.Now.AddMinutes(_rand.Next(_minTimeBetweenWork, _maxTimeBetweenWork));  // Set the time between alarms
                }
                //await Method Call Start
                Console.WriteLine("{0} --- Worker {1} is ending {2} --- Starting up again at {3}", DateTime.Now, _workerId, _workList[_workGroupCounter][_workCounter], 
                    _timeToNextWork.AddSeconds(Definitions.SecondsBetweenUpdates * _workCounter));
            }
            else
            {
                if (_workCounter == 0)
                {
                    _workEndTime = DateTime.Now.AddMinutes(_rand.Next(_minWorkRuntime, _maxWorkRuntime));  // Set the alarm completion time
                }
                // await Method Call End
                Console.WriteLine("{0} --- Worker {1} is starting {2} --- Ending at {3}", DateTime.Now, _workerId, _workList[_workGroupCounter][_workCounter],
                   _workEndTime.AddSeconds(Definitions.SecondsBetweenUpdates * _workCounter));
            }
        }

        public async Task ForceEndWork()
        {
            if(_isWorkerWorking || _workCounter != 0)
            {
                for(int i = 0; i < _workList[_workGroupCounter].Length; i++)
                {
                    // Await Method call
                }
                Console.WriteLine("Worker {0} is has ended his work early.", _workerId);
            }
        }
    }
}
