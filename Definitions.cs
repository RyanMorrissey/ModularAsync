//  Author: Ryan Morrissey
//  Date: 9/9/2018
//
//  This class has some constant definitions to be used by the Worker Manager and Worker classes,
//  in addition to some helper methods that only print messages to the console.  They could very easily
//  be modified to log information instead.

using System;

namespace ModularAsync
{
    class Definitions
    {
        public const int WorkStartingIndex = 0;
        public const int SecondsBetweenUpdates = 20;
        public const int MillisecondsBetweenUpdates = 20000;

        public static void PrintUsage()
        {
            Console.WriteLine("Example of Session Usage");
            Console.WriteLine("\"WorkerId\": \"Worker Id\",");
            Console.WriteLine("\"Work\": [");
            Console.WriteLine("\t\"Command A1, Command A2, Command A3\",");
            Console.WriteLine("\t\"Command B1, Command B2\",");
            Console.WriteLine("\t\"Command C1\",");
            Console.WriteLine("\"MinWorkRuntime\": 1,");
            Console.WriteLine("\"MaxWorkRuntime\": 5,");
            Console.WriteLine("\"MinTimeBetweenWork\": 20,");
            Console.WriteLine("\"MaxTimeBetweenWork\": 40,");
            Console.WriteLine("\nWorkerId and Work are required while work times are optional");
        }
    }
}
