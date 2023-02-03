namespace Getmore.GmsMobile.Core.Messages
{
	public class NewPushNotificationMessage : MessageBase
	{
		public string Title
		{
			get; set;
		}

		public string Text
		{
			get; set;
		}
	}
}
