using Android.Content;
using Getmore.GmsMobile;
using Getmore.GmsMobile.Droid;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(GmsMobileWebView), typeof(CustomAndroidWebViewRenderer))]

namespace Getmore.GmsMobile.Droid
{
	internal class CustomAndroidWebViewRenderer : WebViewRenderer
	{
		private const string ConstJsInterfaceName = "GmsMobileApp";
		// const string JAVASCRIPT_FUNCTION = @"function InvokeDisplayJSText_Wrapper(text) { GmsXamarin.InvokeDisplayJSText(text);  }";

		public CustomAndroidWebViewRenderer(Context context) : base(context)
		{

		}


		protected override void OnElementChanged(ElementChangedEventArgs<WebView> e)
		{
			base.OnElementChanged(e);

			if (this.Element is GmsMobileWebView webView)
			{
				if (e.OldElement != null)
				{
					this.Control.RemoveJavascriptInterface(CustomAndroidWebViewRenderer.ConstJsInterfaceName);
					webView.Cleanup();
				}

				if (e.NewElement != null)
				{
					this.Control.ClearCache(true);
					this.Control.ClearHistory();
					Android.Webkit.CookieManager.Instance.RemoveAllCookies(null);
					//this.Control.SetWebViewClient(new JavascriptWebViewClient(this, $"javascript: {JAVASCRIPT_FUNCTION}"));
					this.Control.AddJavascriptInterface(new GmsMobileJsBridge(this), CustomAndroidWebViewRenderer.ConstJsInterfaceName);


					this.Control.Settings.MixedContentMode = Android.Webkit.MixedContentHandling.AlwaysAllow;
					this.Control.Settings.JavaScriptEnabled = true;
					this.Control.Settings.DomStorageEnabled = true;
					this.Control.Settings.AllowContentAccess = true;
					this.Control.Settings.AllowFileAccess = true;
					this.Control.Settings.AllowFileAccessFromFileURLs = true;
					this.Control.Settings.AllowUniversalAccessFromFileURLs = true;
					this.Control.Settings.JavaScriptCanOpenWindowsAutomatically = true;
					this.Control.Settings.SetAppCacheMaxSize(long.MaxValue);
					this.Control.Settings.SetAppCacheEnabled(true);
					this.Control.Settings.LoadsImagesAutomatically = true;
					this.Control.Settings.MixedContentMode = Android.Webkit.MixedContentHandling.AlwaysAllow;
					this.Control.Settings.AllowContentAccess = true;
					//this.Control.SetWebChromeClient(new Android.Webkit.WebChromeClient() { });
					//this.Control.ClearSslPreferences();

					var c = new GmAndroidWebViewClient(this);
					this.Control.SetWebViewClient(c);


					String newUA = "Mozilla/5.0 (X11; U; Linux i686; en-US; rv:1.9.0.4) Gecko/20100101 Firefox/4.0";
					this.Control.Settings.UserAgentString = newUA;
					this.Control.Settings.LoadWithOverviewMode = true;
					this.Control.Settings.UseWideViewPort = true;
				}
			}
		}


	private bool _isDisposed;

		protected override void Dispose(bool disposing)
		{
			if (this._isDisposed)
			{
				return;
			}

			_isDisposed = true;
			base.Dispose(disposing);
			GC.SuppressFinalize(this);
		}
	}
}