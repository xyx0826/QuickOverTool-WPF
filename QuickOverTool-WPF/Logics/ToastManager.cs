using Notifications.Wpf;

namespace OWorkbench
{
    class ToastManager
    {
        private static ToastManager _uniqueInstance;
        NotificationManager _manager;

        private ToastManager()
        {
            _manager = new NotificationManager();
        }

        public static ToastManager GetInstance()
        {
            if (_uniqueInstance == null) _uniqueInstance = new ToastManager();
            return _uniqueInstance;
        }

        /// <summary>
        /// Creates a system-wide toast with specified parameters.
        /// </summary>
        /// <param name="title">The title of the toast.</param>
        /// <param name="message">The message of the toast.</param>
        /// <param name="type">The type of the toast. From 0 to 3: Success, Information, Warning, Error.</param>
        public void CreateToast(string title, string message, int type)
        {
            NotificationContent toast = new NotificationContent
            {
                Title = title,
                Message = message,
            };

            switch (type)
            {
                case 0:
                    toast.Type = NotificationType.Success;
                    break;
                case 1:
                    toast.Type = NotificationType.Information;
                    break;
                case 2:
                    toast.Type = NotificationType.Warning;
                    break;
                case 3:
                    toast.Type = NotificationType.Error;
                    break;
            }

            _manager.Show(toast);
        }
    }
}
