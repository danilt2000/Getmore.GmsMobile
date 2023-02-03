using Android.Webkit;
using Getmore.GmsMobile.Core.GmsInterface;
using Java.Interop;
using Org.Json;
using System;

namespace Getmore.GmsMobile.Droid
{
	internal class GmsMobileJsBridge : Java.Lang.Object, IGmsMobileJsBridge
	{
		private readonly WeakReference<CustomAndroidWebViewRenderer> _hybridWebViewRenderer;

		public GmsMobileJsBridge(CustomAndroidWebViewRenderer hybridRenderer)
		{
			this._hybridWebViewRenderer = new WeakReference<CustomAndroidWebViewRenderer>(hybridRenderer);
		}

		[Export]
		[JavascriptInterface]
		public void InvokeDisplayJSText(string text)
		{
			if (this._hybridWebViewRenderer != null && this._hybridWebViewRenderer.TryGetTarget(out CustomAndroidWebViewRenderer hybridRenderer))
			{
				((GmsMobileWebView)hybridRenderer.Element).InvokeDisplayJSTextAction(text);
			}
		}


		[Export]
		[JavascriptInterface]
		public void SendMessageToMobileApp(string messageType, string messageJSON)
		{
			if (this._hybridWebViewRenderer != null && this._hybridWebViewRenderer.TryGetTarget(out CustomAndroidWebViewRenderer hybridRenderer))
			{
				((GmsMobileWebView)hybridRenderer.Element).InvokeIncommingMessageAction(messageType, messageJSON);
			}
		}

	}
}