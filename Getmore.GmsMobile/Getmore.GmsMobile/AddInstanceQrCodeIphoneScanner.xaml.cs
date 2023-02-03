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
				_ = this.Navigation.PopModalAsync();

			});
		}
	}
}