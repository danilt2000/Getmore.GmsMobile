using Android.Webkit;
using Xamarin.Forms.Platform.Android;

namespace Getmore.GmsMobile.Droid
{
	internal class JavascriptWebViewClient : FormsWebViewClient
	{
		private readonly string _javascript;

		public JavascriptWebViewClient(CustomAndroidWebViewRenderer renderer, string javascript)
			: base(renderer)
		{
			this._javascript = javascript;
		}

		public override void OnPageFinished(WebView view, string url)
		{
			base.OnPageFinished(view, url);
			view.EvaluateJavascript(this._javascript, null);
		}
	}
}