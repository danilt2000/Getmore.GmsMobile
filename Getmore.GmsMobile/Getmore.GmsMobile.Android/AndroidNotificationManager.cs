using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using AndroidX.Core.App;
using System;
using Xamarin.Forms;
using AndroidApp = Android.App.Application;

[assembly: Dependency(typeof(Getmore.GmsMobile.Droid.AndroidNotificationManager))]
namespace Getmore.GmsMobile.Droid
{

	public class AndroidNotificationManager : INotificationManager
	{
		private const string channelId = "default";
		private const string channelName = "Default";
		private const string channelDescription = "The default channel for notifications.";

		public const string TitleKey = "title";
		public const string MessageKey = "message";
		private bool channelInitialized;
		private int messageId;
		private int pendingIntentId;
		private NotificationManager manager;

		public event EventHandler NotificationReceived;

		public static AndroidNotificationManager Instance { get; private set; }

		public AndroidNotificationManager()
		{
			this.Initialize();
		}

		public void Initialize()
		{
			if (Instance == null)
			{
				this.CreateNotificationChannel();
				Instance = this;
			}
		}

		public void SendNotification(string title, string message, DateTime? notifyTime = null)
		{
			if (!this.channelInitialized)
			{
				this.CreateNotificationChannel();
			}

			if (notifyTime != null)
			{
				Intent intent = new Intent(AndroidApp.Context, typeof(AlarmHandler));
				_ = intent.PutExtra(TitleKey, title);
				_ = intent.PutExtra(MessageKey, message);

				PendingIntent pendingIntent = PendingIntent.GetBroadcast(AndroidApp.Context, this.pendingIntentId++, intent, PendingIntentFlags.Immutable);
				long triggerTime = GetNotifyTime(notifyTime.Value);
				AlarmManager alarmManager = AndroidApp.Context.GetSystemService(Context.AlarmService) as AlarmManager;
				alarmManager.Set(AlarmType.RtcWakeup, triggerTime, pendingIntent);
			}
			else
			{
				this.Show(title, message);
			}
		}

		public void ReceiveNotification(string title, string message)
		{
			var args = new NotificationEventArgs()
			{
				Title = title,
				Message = message,
			};
			NotificationReceived?.Invoke(null, args);
		}

		public void Show(string title, string message)
		{
			Intent intent = new Intent(AndroidApp.Context, typeof(MainActivity));
			_ = intent.PutExtra(TitleKey, title);
			_ = intent.PutExtra(MessageKey, message);

			PendingIntent pendingIntent = PendingIntent.GetActivity(AndroidApp.Context, this.pendingIntentId++, intent, PendingIntentFlags.Immutable);

			NotificationCompat.Builder builder = new NotificationCompat.Builder(AndroidApp.Context, channelId)
			    .SetContentIntent(pendingIntent)
			    .SetContentTitle(title)
			    .SetContentText(message)
			    .SetLargeIcon(BitmapFactory.DecodeResource(AndroidApp.Context.Resources, Resource.Drawable.abc_btn_check_material))
			    .SetSmallIcon(Resource.Drawable.abc_btn_check_material)
			    .SetDefaults((int)NotificationDefaults.Sound | (int)NotificationDefaults.Vibrate);

			Notification notification = builder.Build();
			this.manager.Notify(this.messageId++, notification);
		}

		private void CreateNotificationChannel()
		{
			this.manager = (NotificationManager)AndroidApp.Context.GetSystemService(AndroidApp.NotificationService);

			if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
			{
				var channelNameJava = new Java.Lang.String(channelName);
				var channel = new NotificationChannel(channelId, channelNameJava, NotificationImportance.Default)
				{
					Description = channelDescription
				};
				this.manager.CreateNotificationChannel(channel);
			}

			this.channelInitialized = true;
		}

		private static long GetNotifyTime(DateTime notifyTime)
		{
			DateTime utcTime = TimeZoneInfo.ConvertTimeToUtc(notifyTime);
			double epochDiff = (new DateTime(1970, 1, 1) - DateTime.MinValue).TotalSeconds;
			long utcAlarmTime = utcTime.AddSeconds(-epochDiff).Ticks / 10000;
			return utcAlarmTime; // milliseconds
		}
	}

}