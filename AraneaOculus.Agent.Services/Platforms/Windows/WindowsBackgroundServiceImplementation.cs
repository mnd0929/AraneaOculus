using AraneaOculus.Agent.Engine;
using AraneaOculus.Core.Interfaces;
using AraneaOculus.Core.Models.Network;

namespace AraneaOculus.Agent.Services.Platforms.Windows
{
    public class WindowsBackgroundServiceImplementation : IBackgroundService
    {
        private AgentController? Controller;

        public async Task Start(object? address = null)
        {
            ConnectionAddress networkAddress = (ConnectionAddress)address!;

            Controller = new AgentController(new WindowsDataCollector());
            await Controller.Connect(networkAddress);
        }

        public void Stop() => Controller?.Disconnect();
    }
}
