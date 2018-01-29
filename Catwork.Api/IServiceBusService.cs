using System;
using System.Threading.Tasks;

namespace Catwork.Api
{
    public interface IServiceBusService : IDisposable
    {
        Task SendMessageAsync(string message);
    }
}