using Android.Content;
using AraneaOculus.Core.Interfaces;
using AraneaOculus.Core.Models.Network;
using Application = Android.App.Application;

namespace AraneaOculus.Agent.Services.Platforms.Android
{
    public class AndroidBackgroundServiceImplementation : IBackgroundService
    {
        public async Task Start(object address = null!)
        {
            ConnectionAddress networkAddress = (ConnectionAddress)address;

            var intent = new Intent(Application.Context, typeof(AndroidBackgroundService));
            intent.PutExtra("address", networkAddress.Host.ToString());
            intent.PutExtra("port", networkAddress.Port);

            Application.Context.StartForegroundService(intent);
            await Task.Delay(10);
        }

        public void Stop()
        {
            var intent = new Intent(Application.Context, typeof(AndroidBackgroundService));
            Application.Context.StopService(intent);
        }
    }
}
