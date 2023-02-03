namespace Getmore.GmsMobile.Core.GmsInterface
{
	public interface IGmsMobileJsBridge
	{
		void SendMessageToMobileApp(string messageType, string messageJSON);
	}
}