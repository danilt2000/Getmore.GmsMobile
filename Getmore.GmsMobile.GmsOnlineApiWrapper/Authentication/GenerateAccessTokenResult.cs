namespace Getmore.GmsMobile.GmsOnlineApiWrapper.Authentication
{
	public class GenerateAccessTokenResult
	{
		public bool Success
		{
			get;
			set;
		}

		public string AccessTokenHex
		{
			get;
			set;
		}

		public string ErrorMessage
		{
			get;
			set;
		}
	}
}
