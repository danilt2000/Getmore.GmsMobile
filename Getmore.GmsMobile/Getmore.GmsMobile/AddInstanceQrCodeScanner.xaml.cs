using Getmore.GmsMobile.Core.Authentication;
using System;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using ZXing.Mobile;

namespace Getmore.GmsMobile
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class AddInstanceQrCodeScanner : ContentPage
	{
		public AddInstanceQrCodeScanner()
		{
			this.InitializeComponent();
			this.QrCodeScanner.OnScanResult += this.QrCodeScanner_OnScanResult;

			this.QrCodeScanner.Options.PossibleFormats = new List<ZXing.BarcodeFormat>() { ZXing.BarcodeFormat.QR_CODE };
			this.QrCodeScanner.Options.TryHarder = true;
			this.QrCodeScanner.Options.CameraResolutionSelector = new MobileBarcodeScanningOptions.CameraResolutionSelectorDelegate(this.SelectLowestResolutionMatchingDisplayAspectRatio);

			this.QrCodeScanner.IsScanning = true;
			this.QrCodeScanner.IsAnalyzing = true;
		}

		public CameraResolution SelectLowestResolutionMatchingDisplayAspectRatio(List<CameraResolution> availableResolutions)
		{
			CameraResolution result = null;
			//a tolerance of 0.1 should not be visible to the user
			double aspectTolerance = 0.1;

			//-- Wir stellen sicher, dass diese Funktion im SizeChanged genutzt wird, dann haben wir width/heigt vom Control
			var displayOrientationHeight = this.QrCodeScanner.Width;
			var displayOrientationWidth = this.QrCodeScanner.Height;

			//calculating our targetRatio
			var targetRatio = displayOrientationHeight / displayOrientationWidth;
			var targetHeight = displayOrientationHeight;
			var minDiff = double.MaxValue;

			//camera API lists all available resolutions from highest to lowest, perfect for us
			//making use of this sorting, following code runs some comparisons to select the lowest resolution that matches the screen aspect ratio and lies within tolerance
			//selecting the lowest makes Qr detection actual faster most of the time
			foreach (var r in availableResolutions.Where(r => Math.Abs(((double)r.Width / r.Height) - targetRatio) < aspectTolerance))
			{
				//slowly going down the list to the lowest matching solution with the correct aspect ratio
				if (Math.Abs(r.Height - targetHeight) < minDiff)
					minDiff = Math.Abs(r.Height - targetHeight);
				result = r;
			}
			return result;
		}

		private void QrCodeScanner_OnScanResult(ZXing.Result result)
		{
			var resultArr = result.Text.Split('|');
			this.MyResult = new GmsInstanceLoginData(resultArr[0], resultArr[1], null, resultArr[2]);
			this.QrCodeScanner.IsAnalyzing = false;
			this.QrCodeScanner.IsScanning = false;
			_ = this.Navigation.PopModalAsync();
		}

		private void BtnClose_Clicked(object sender, EventArgs e)
		{
			_ = this.Navigation.PopModalAsync();
		}

		protected override void OnAppearing()
		{
			this.QrCodeScanner.IsScanning = true;
			this.QrCodeScanner.IsAnalyzing = true;

			this.QrCodeScanner.IsTorchOn = true;

			base.OnAppearing();
		}
		protected override void OnDisappearing()
		{
			this.QrCodeScanner.IsAnalyzing = false;
			this.QrCodeScanner.IsScanning = false;
			base.OnDisappearing();
		}

		private void BtnScanOnOff_Clicked(object sender, EventArgs e)
		{
			if (this.QrCodeScanner.IsAnalyzing)
			{
				this.QrCodeScanner.IsAnalyzing = false;
				this.QrCodeScanner.IsScanning = false;
			}
			else
			{
				this.QrCodeScanner.IsScanning = true;
				this.QrCodeScanner.IsAnalyzing = true;
			}
		}

		public GmsInstanceLoginData MyResult
		{
			get;
			private set;
		}
	}
}