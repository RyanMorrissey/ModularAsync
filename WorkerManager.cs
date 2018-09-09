//  Author: Ryan Morrissey
//  Date: 9/9/2018
//
//  This class manages whatever workers we generate through the configuration file.
//  It will then ping the workers every 20 seconds to see if it is time to start/finish their work

using System;
using System.Timers;
using System.IO;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Data;
using Newtonsoft.Json;

namespace ModularAsync
{
    public class WorkerManager : IWorkerManager
    {
        private Dictionary<string, Worker> _workerDictionary = new Dictionary<string, Worker>();
        private Timer _updateTimer;

        public WorkerManager()
        {
            Initialize();
        }
        
        private void Initialize()
        {
            Console.WriteLine("Initializing...");
            ParseJson();
            _updateTimer = new Timer();
            _updateTimer.Elapsed += new ElapsedEventHandler(UpdateWorker);
            _updateTimer.Interval = Definitions.MillisecondsBetweenUpdates;
        }

        // This is a long method, but since its all parsing one file, its difficult to break it up more
        private void ParseJson()
        {
            try
            {
                DataSet configParser = JsonConvert.DeserializeObject<DataSet>(File.ReadAllText("config.json"));
                DataTable dataTable = configParser.Tables["Workers"];

                foreach (DataRow row in dataTable.Rows)
                {
                    // Default Attributes
                    int minWorkRuntime = 1;
                    int maxWorkRuntime = 5;
                    int minTimeBetweenWork = 20;
                    int maxTimeBetweenWork = 40;

                    if (!row.Table.Columns.Contains("WorkerId"))
                    {
                        throw new ArgumentException();
                    }
                    if (!row.Table.Columns.Contains("Work"))
                    {
                        throw new ArgumentException();
                    }
                    if (row.Table.Columns.Contains("MinWorkRuntime"))
                    {
                        minWorkRuntime = Convert.ToInt32(row["MinWorkRuntime"]);
                    }
                    if (row.Table.Columns.Contains("MaxWorkRuntime"))
                    {
                        maxWorkRuntime = Convert.ToInt32(row["MaxWorkRuntime"]);
                    }
                    if (row.Table.Columns.Contains("MinTimeBetweenWork"))
                    {
                        minTimeBetweenWork = Convert.ToInt32(row["MinTimeBetweenWork"]);
                    }
                    if (row.Table.Columns.Contains("MaxTimeBetweenWork"))
                    {
                        maxTimeBetweenWork = Convert.ToInt32(row["MaxTimeBetweenWork"]);
                    }

                    _workerDictionary.TryAdd(row["WorkerId"].ToString(), new Worker(row["WorkerId"].ToString(),
                        (string[])row["Work"], minWorkRuntime, maxWorkRuntime, minTimeBetweenWork, maxTimeBetweenWork));
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Console.WriteLine("\nError in json file.\n");
                Definitions.PrintUsage();
                Environment.Exit(0);
            }
        }

        public async Task Run()
        {
            _updateTimer.Enabled = true;
            Console.WriteLine("Type \'quit\' to end off all active work and exit.");
            UpdateWorker(null, null);
            while(true)
            {
                if (Console.ReadLine().ToLower() == "quit")
                    break;
                else
                    Console.WriteLine("Invalid Command - Type \'quit\' to exit.");
            }
            await EndWork();
        }

        private void UpdateWorker(object source, ElapsedEventArgs e)
        {
            Parallel.ForEach(_workerDictionary, async worker =>
            {
                await worker.Value.Update();
            });
        }

        private async Task EndWork()
        {
            Console.WriteLine("Ending all active work");
            Parallel.ForEach(_workerDictionary, async worker =>
            {
                await worker.Value.ForceEndWork();
            });
        }

    }
}
