﻿using Getmore.GmsMobile.Core.Authentication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using ZXing;

namespace Getmore.GmsMobile
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class AddInstanceQrCodeIphoneScanner
	{
		public AddInstanceQrCodeIphoneScanner()
		{
			InitializeComponent();
		}
		public void Handle_OnScanResult(Result result)
		{
			Device.BeginInvokeOnMainThread(async () =>
			{
				await DisplayAlert("Scanned result", result.Text, "OK");
				//this.MyResult = new GmsInstanceLoginData(resultArr[0], resultArr[1], null, resultArr[2]);

				_ = this.Navigation.PopModalAsync();

			});
		}

		private void Button_Clicked(object sender, EventArgs e)
		{
			_ = this.Navigation.PopModalAsync();

		}
	}
}