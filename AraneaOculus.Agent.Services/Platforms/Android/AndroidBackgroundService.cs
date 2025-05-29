using Android.App;
using Android.Content;
using Android.OS;
using AndroidX.Core.App;
using AraneaOculus.Agent.Engine;
using AraneaOculus.Agent.Services.Platforms.Android;
using AraneaOculus.Core.Models.Network;

namespace AraneaOculus.Agent.Services
{
    [Service]
    public class AndroidBackgroundService : Service
    {
        private const int ServiceId = 1001;

        private const string NotificationChannelId = "aranea_agent_channel";

        private AgentController? Controller;

        public override void OnDestroy()
        {
            Controller?.Disconnect();

            base.OnDestroy();
        }

        public override IBinder? OnBind(Intent? intent) => null;

        public override StartCommandResult OnStartCommand(Intent? intent, StartCommandFlags flags, int startId)
        {
            ConnectionAddress networkAddress = null!;
            if (intent?.Extras != null)
            {
                string address = intent.GetStringExtra("address") ?? "127.0.0.1";
                int port = intent.GetIntExtra("port", 1984)!;

                networkAddress = new ConnectionAddress(address, port);
            }

            CreateNotificationChannel();

            var notification = new NotificationCompat.Builder(this, NotificationChannelId)
                .SetSmallIcon(Resource.Drawable.mtrl_ic_error)
                .SetContentTitle("AraneaService")
                .SetContentText("Служба AraneaOculus работает в фоновом режиме")
                .SetPriority(NotificationCompat.PriorityDefault)
                .SetOngoing(true)
                .Build();

            StartForeground(ServiceId, notification);

            Task.Run(async () =>
            {
                Controller = new AgentController(new AndroidDataCollector());

                Controller.Disconnected += (_s, _e) =>
                {
                    ShowNotification(
                        "Потеряно соединение с сервером",
                        $"Соединение с сервером AraneaOculus ({networkAddress.Host}:{networkAddress.Port}) было закрыто"
                    );

                    OnDestroy();
                };

                Controller.Connected += (_s, _e) =>
                    ShowNotification(
                        "Успешное подключение к серверу",
                        $"Выполняется обмен данными с сервером {networkAddress.Host}:{networkAddress.Port}"
                    );

                Controller.OnNotificationReceived += (_s, _e) => 
                    ShowNotification("Серверное оповещение", _e!);

                await Controller.Connect(networkAddress);
            });

            Task.Delay(25000);

            return StartCommandResult.Sticky;
        }

        private void CreateNotificationChannel()
        {
            if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
            {
                var channel = new NotificationChannel(
                    NotificationChannelId,
                    "Aranea Service Channel",
                    NotificationImportance.High
                );

                var notificationManager = GetSystemService(NotificationService) as NotificationManager;
                notificationManager?.CreateNotificationChannel(channel);
            }
        }

        private void ShowNotification(string title, string message)
        {
            var notificationBuilder = new NotificationCompat.Builder(this, NotificationChannelId)
                .SetContentTitle(title)
                .SetContentText(message)
                .SetSmallIcon(Resource.Drawable.mtrl_ic_error)
                .SetAutoCancel(true)
                .SetPriority(NotificationCompat.PriorityHigh)
                .SetDefaults((int)(NotificationDefaults.Sound | NotificationDefaults.Vibrate));

            var notificationManager = NotificationManagerCompat.From(this);
            notificationManager.Notify(GenerateNotificationId(), notificationBuilder.Build());
        }

        private int GenerateNotificationId()
        {
            return DateTime.Now.Millisecond + new Random().Next(1000);
        }
    }
}