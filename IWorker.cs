//  Author: Ryan Morrissey
//  Date: 9/9/2018
//
//  Basic Interface for Worker

using System.Threading.Tasks;

namespace ModularAsync
{
    public interface IWorker
    {
        // This will handle setting up what work will be run, and when
        Task Work();

        // This will complete the actual work
        Task ProcessWork();
    }
}