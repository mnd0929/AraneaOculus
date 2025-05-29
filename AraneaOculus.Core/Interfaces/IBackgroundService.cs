namespace AraneaOculus.Core.Interfaces
{
    public interface IBackgroundService
    {
        Task Start(object parameter = null!);

        void Stop();
    }
}
