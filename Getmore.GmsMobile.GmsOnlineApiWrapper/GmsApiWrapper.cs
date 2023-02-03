using Getmore.GmsMobile.GmsOnlineApiWrapper.Authentication;
using RestSharp;
using System.Threading.Tasks;

namespace Getmore.GmsMobile.GmsOnlineApiWrapper
{
	public class GmsApiWrapper
	{
		public GmsApiWrapper(string gmsOnlineBaseUrl)
		{
			var options = new RestClientOptions(gmsOnlineBaseUrl)
			{
				RemoteCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true
			};

			this.Client = new RestClient(options);
		}


		public async Task<GenerateAccessTokenResult> GenerateAccessTokenAsync(string userLogin, string temporaryAccessTokenHash, string deviceId)
		{
			var request = new RestRequest("GmsMobile/Authentication/GenerateAccessToken")
			    .AddJsonBody(new { userLogin, temporaryAccessTokenHash, deviceId });

			return await this.Client.PostAsync<GenerateAccessTokenResult>(request);
		}

		private RestClient Client
		{
			get;
		}
	}
}
