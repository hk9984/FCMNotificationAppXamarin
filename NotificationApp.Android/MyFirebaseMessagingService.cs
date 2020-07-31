using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;

using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Util;
using Firebase.Messaging;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Android.Support.V4.App;


namespace NotificationApp.Droid
{
    [Service]
    [IntentFilter(new[] { "com.google.firebase.MESSAGING_EVENT" })]
    class MyFirebaseMessagingService : FirebaseMessagingService
    {
        const string TAG = "MyFirebaseMsgService";
        public override void OnMessageReceived(RemoteMessage message)
        {
            base.OnMessageReceived(message);
            if (message.Data.Count > 0)
            {
                Log.Debug("NotApp:", "Data Payload found");
                Console.WriteLine("Data Payload found");
                SendLocalNotification(message);
            }
        }

        void SendLocalNotification(RemoteMessage message)
        {
            var intent = new Intent(this, typeof(MainActivity));
            intent.AddFlags(ActivityFlags.ClearTop);
            //intent.PutExtra("message", body);
            var data = message.Data;
            string title = data["title"];
            string content = data["content"];
            content = content + " edited";
            //Unique request code to avoid PendingIntent collision.
            var requestCode = new Random().Next();
            var pendingIntent = PendingIntent.GetActivity(this, requestCode, intent, PendingIntentFlags.OneShot);

            var notificationBuilder = new NotificationCompat.Builder(this, MainActivity.CHANNEL_ID)
                .SetContentTitle(title)
                .SetSmallIcon(Resource.Drawable.notify_panel_notification_icon_bg)
                .SetContentText(content)
                .SetAutoCancel(true)
                .SetShowWhen(false)
                .SetContentIntent(pendingIntent);

            /*if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
            {
                notificationBuilder.SetChannelId("my_notification_channel");
            }*/

            var notificationManager = NotificationManager.FromContext(this);
            notificationManager.Notify(0, notificationBuilder.Build());
        }
    }
}