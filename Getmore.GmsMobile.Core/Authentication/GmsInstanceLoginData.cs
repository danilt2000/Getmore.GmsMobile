namespace Getmore.GmsMobile.Core.Authentication
{
	public class GmsInstanceLoginData
	{
		public GmsInstanceLoginData()
		{ }

		public GmsInstanceLoginData(string gmsInstanceUrl, string userLogin, string userPassword, string tmpAccessToken)
		{
			this.GmsInstanceUrl = gmsInstanceUrl; //"https://10.0.9.210/gms/";// gmsInstanceUrl;
			this.UserLogin = userLogin;
			this.UserPassword = userPassword;
			this.AccessTokenHex = tmpAccessToken;
		}

		public string GmsInstanceUrl
		{
			get;
			set;
		}

		public string UserLogin
		{
			get;
			set;
		}

		public string UserPassword
		{
			get;
			set;
		}

		public string AccessTokenHex
		{
			get;
			set;
		}
	}
}
