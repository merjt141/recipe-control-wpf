using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace RecipeControl.Services.Loggs
{
    public static class LoggerService
    {
        // Crear delegates para notificación a UI
        public static Action<string>? OnNotification;

        static public void NotifySystem(string message)
        {
            OnNotification?.Invoke(message);
        }
    }
}
