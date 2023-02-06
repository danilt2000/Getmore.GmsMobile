using Getmore.GmsMobile.Core.Authentication;
using Getmore.GmsMobile.Core.Common;
using Getmore.GmsMobile.Core.Messages;
using Getmore.GmsMobile.DataWrapper;
using Getmore.GmsMobile.GmsOnlineApiWrapper;
using System;
using System.Linq;
using Xamarin.Forms;

namespace Getmore.GmsMobile
{
	public partial class MainPage : ContentPage
	{
		private INotificationManager _NotificationManager;

		private const string ConstLoginFormTemplate = @"

<html>
	<head>
	</head>
	<body>
		HIDDEN FORM:
	
		<form id='frm' method='POST' action='{0}'>
			<input type='hidden' id='tokenHex' name='tokenHex' value='{1}'>
			<input type='hidden' id='userLogin' name='userLogin' value='{2}'>
			<input type='hidden' id='deviceId' name='deviceId' value='{3}'>
		</form>
		
		<script>
			document.getElementById('frm').submit();
		</script>
	</body>
	
</html>

";

		public MainPage()
		{
			this.InitializeComponent();

			this.GmsAppWebView.Navigated += this.GmsAppWebView_Navigated;
			this.GmsAppWebView.Navigating += this.GmsAppWebView_Navigating;

			//this.GmsAppWebView.Source = "http://10.0.9.210/gms"; // iPad

			//this.GmsAppWebView.Source = "https://10.0.2.2/gms"; // android

			this.GmsAppWebView.RegisterIncommingMessageAction(this.HandleIncommingMessage);

			this._NotificationManager = DependencyService.Get<INotificationManager>();

			var instances = this.DataAccess.GetGmsInstances();

			GmsInstanceLoginData loginData = new GmsInstanceLoginData
			{
				GmsInstanceUrl = "https://demo.gmcloud.cz/Demo/",
				AccessTokenHex = "03FFA7048572C49A6B112524AEFEB78FB1493D6E4D4EA5AD3A33C20DFAB0EC6AC1551D451D332C17D20CB067CBE4419F7B121A2E0CA039CA5DDA52B64605214DAB51724AA44261E2AAF6F1A2E13EC002A24C53963CB2D5950A8E2E0A15420BF8109B40F15C557CBD411710C3B575FEC6B2DB0BF8418C94B91E3191B06032B8BE8D6DB95884E27BE836F7138648814EF71AF080F157856AA9D408162C88C3F729BDF670DB6ABA59B06252BBB22A178C5A13FE9E21A054FEFFE7E63934A8708E44F176394A9A1CCE61",
				UserLogin = "TpmManazer",
				UserPassword = null
			};

			//if (instances.Count > 0)
				this.LoginToGmsInstance(loginData);
			//else
				//this.ShowQrCodeLoginScreen();
		}

		#region event handlers

		private void GmsAppWebView_Navigating(object sender, WebNavigatingEventArgs e)
		{

		}

		private void BtnHome_Clicked(object sender, EventArgs e)
		{
			this.GmsAppWebView.SendCommandToGmsApp(Core.GmsInterface.GmsAppCommandType.NavigateHome, null);
		}

		private void BtnBack_Clicked(object sender, EventArgs e)
		{
			this.GmsAppWebView.SendCommandToGmsApp(Core.GmsInterface.GmsAppCommandType.NavigateBack, null);
		}

		private void BtnReload_Clicked(object sender, EventArgs e)
		{
			this.GmsAppWebView.Reload();
		}

		private void GmsAppWebView_Navigated(object sender, WebNavigatedEventArgs e)
		{
			if (e.Result == WebNavigationResult.Success)
				this.GmsAppWebView.SendCommandToGmsApp(Core.GmsInterface.GmsAppCommandType.SetIsGmsMobile, null);
		}

		private void BtnQrCodeScanner_Clicked(object sender, EventArgs e)
		{
			this.ShowQrCodeLoginScreen();
		}

		private async void QrCodeScanClosed(object sender, EventArgs e)
		{
			if (sender is AddInstanceQrCodeScanner page && page.MyResult != null)
			{
				var api = new GmsApiWrapper(page.MyResult.GmsInstanceUrl);
				var generateTokenResult = await api.GenerateAccessTokenAsync(page.MyResult.UserLogin, page.MyResult.AccessTokenHex, this.DeviceId);

				if (generateTokenResult != null && generateTokenResult.Success)
				{
					var newInstance = new GmsInstanceLoginData(page.MyResult.GmsInstanceUrl, page.MyResult.UserLogin, null, generateTokenResult.AccessTokenHex);
					this.DataAccess.AddGmsInstace(newInstance);

					this.LoginToGmsInstance(newInstance);
				}
			}
		}

		#endregion

		#region private methods

		private void ShowQrCodeLoginScreen()
		{
			var page = new AddInstanceQrCodeIphoneScanner();
			_ = this.Navigation.PushModalAsync(page);
			page.Disappearing += this.QrCodeScanClosed;
		}

		private void LoginToGmsInstance(GmsInstanceLoginData instance)
		{
			var action = String.Format("{0}GmsMobile/Authentication/LoginByAccessToken", instance.GmsInstanceUrl);
			var webViewContent = String.Format(ConstLoginFormTemplate, action, instance.AccessTokenHex, instance.UserLogin, this.DeviceId);

			var html = new HtmlWebViewSource
			{
				Html = webViewContent
			};

			_ = Device.InvokeOnMainThreadAsync(() => { this.GmsAppWebView.Source = html; });

			this.CurrentGmsInstance = instance;
		}

		private void HandleIncommingMessage(MessageType messageType, MessageBase message)
		{
			switch (messageType)
			{
				case MessageType.Restart:
					this.LoginToGmsInstance(this.CurrentGmsInstance);
					break;
				case MessageType.NewPushNotification:
					this._NotificationManager.SendNotification((message as NewPushNotificationMessage).Title, (message as NewPushNotificationMessage).Text);
					break;
				case MessageType.ShowDashboard:
					break;
				case MessageType.ShowModule:
					break;
			}

		}

		#endregion

		#region private properties

		private DataAccessPoint DataAccess => this._DataAccess ??= new DataAccessPoint();
		private DataAccessPoint _DataAccess;

		private string DeviceId
		{
			get
			{
				if (String.IsNullOrWhiteSpace(this._DeviceId))
				{
					var device = DependencyService.Get<IMobileDevice>();
					this._DeviceId = device.GetIdentifier();
				}

				return this._DeviceId;
			}
		}
		private string _DeviceId = null;

		private GmsInstanceLoginData CurrentGmsInstance
		{
			get;
			set;
		}

		#endregion
	}
}
