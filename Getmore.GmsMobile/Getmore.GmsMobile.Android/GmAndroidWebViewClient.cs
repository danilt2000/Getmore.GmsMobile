using Android.Net.Http;
using Android.Webkit;
using System;
using System.Drawing;
using Xamarin.Forms;

namespace Getmore.GmsMobile.Droid
{
	internal class GmAndroidWebViewClient : Android.Webkit.WebViewClient
	{
		CustomAndroidWebViewRenderer _renderer;

		public GmAndroidWebViewClient(CustomAndroidWebViewRenderer renderer)
		{
			this._renderer = renderer ?? throw new ArgumentNullException(nameof(renderer));
		}

		public override void OnReceivedSslError(Android.Webkit.WebView view, Android.Webkit.SslErrorHandler handler, SslError error)
		{
#if DEBUG
			handler.Proceed();
#endif
		}

		//public override void OnReceivedHttpError(Android.Webkit.WebView view, Android.Webkit.IWebResourceRequest request, Android.Webkit.WebResourceResponse errorResponse)
		//{
		//	base.OnReceivedHttpError(view, request, errorResponse);
		//}

		private IWebViewController MyWebViewController => this._renderer.Element as IWebViewController;


		public override void OnPageFinished(Android.Webkit.WebView view, string url)
		{
			base.OnPageFinished(view, url);

			var source = new UrlWebViewSource { Url = url };
			var args = new WebNavigatedEventArgs(WebNavigationEvent.NewPage, source, url, WebNavigationResult.Success);
			this.MyWebViewController.SendNavigated(args);
		}

		public override void OnPageStarted(Android.Webkit.WebView view, string url, Android.Graphics.Bitmap favicon)
		{
			base.OnPageStarted(view, url, favicon);

			var args = new WebNavigatingEventArgs(WebNavigationEvent.NewPage, new UrlWebViewSource { Url = url }, url);
			this.MyWebViewController.SendNavigating(args);
		}
	}
}