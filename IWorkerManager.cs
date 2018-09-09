//  Author: Ryan Morrissey
//  Date: 9/9/2018
//
//  Basic Interface for Worker Manager

using System.Threading.Tasks;

namespace ModularAsync
{
    public interface IWorkerManager
    {
        Task Run();
    }
}