using Foundation;
using Getmore.GmsMobile;
using Getmore.GmsMobile.iOS;
using System;
using System.IO;
using System.Linq;
using UIKit;
using WebKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(GmsMobileWebView), typeof(CustomiOSWebViewRenderer))]

namespace Getmore.GmsMobile.iOS
{

	public class CustomiOSWebViewRenderer : WkWebViewRenderer, IWKScriptMessageHandler
	{
		private const string SendMessageToMobileApp = nameof(SendMessageToMobileApp);

		protected override void OnElementChanged(VisualElementChangedEventArgs e)
		{
			base.OnElementChanged(e);

			if (e.OldElement != null)
			{
				Configuration?.UserContentController?.RemoveAllUserScripts();
				Configuration?.UserContentController?.RemoveScriptMessageHandler(SendMessageToMobileApp);
				var hybridWebView = e.OldElement as GmsMobileWebView;
				hybridWebView?.Cleanup();
			}

			if (e.NewElement != null)
			{
				if (Element is not GmsMobileWebView)
					return;

				this.DeleteCachedFiles();
				
				this.Configuration.UserContentController.AddScriptMessageHandler(this, SendMessageToMobileApp);

				
			}
		}

		public void DidReceiveScriptMessage(WKUserContentController userContentController, WKScriptMessage message)
		{
			if (!(Element is GmsMobileWebView webView))
				return;

			switch (message.Name)
			{
				case SendMessageToMobileApp:
					var messageBody = message.Body.ToString();
					var arr = messageBody.Split("###~###");
					webView.InvokeIncommingMessageAction(arr[0], arr[1]);
					break;
			}
		}

		private void DeleteCachedFiles()
		{
			if (UIDevice.CurrentDevice.CheckSystemVersion(9, 0))
			{
				NSHttpCookieStorage.SharedStorage.RemoveCookiesSinceDate(NSDate.DistantPast);
				WKWebsiteDataStore.DefaultDataStore.FetchDataRecordsOfTypes(WKWebsiteDataStore.AllWebsiteDataTypes, (NSArray records) =>
				{
					for (nuint i = 0; i < records.Count; i++)
					{
						var record = records.GetItem<WKWebsiteDataRecord>(i);

						WKWebsiteDataStore.DefaultDataStore.RemoveDataOfTypes(record.DataTypes,
						    new[] { record }, () => { Console.Write($"deleted: {record.DisplayName}"); });
					}
				});

				NSUrlCache.SharedCache.RemoveAllCachedResponses();
			}
			else
			{

				// Remove the basic cache.
				NSUrlCache.SharedCache.RemoveAllCachedResponses();
				var cookies = NSHttpCookieStorage.SharedStorage.Cookies;

				foreach (var c in cookies)
				{
					NSHttpCookieStorage.SharedStorage.DeleteCookie(c);
				}
			}


			try
			{
				// Clear web cache
				DeleteLibraryFolderContents("Caches");

				// Remove all cookies stored by the site. This includes localStorage, sessionStorage, and WebSQL/IndexedDB.
				DeleteLibraryFolderContents("Cookies");

				// Removes all app cache storage.
				DeleteLibraryFolder("WebKit");

			}
			catch
			{
				//App.UnhandledException(ex, $"Error deleting cache {ex.Message}");
				throw;
			}

		}

		private void DeleteLibraryFolder(string folderName)
		{
			var manager = NSFileManager.DefaultManager;
			var library = manager.GetUrls(NSSearchPathDirectory.LibraryDirectory, NSSearchPathDomain.User).First();
			var dir = Path.Combine(library.Path, folderName);

			manager.Remove(dir, out NSError error);
			if (error != null)
			{
				//App.UnhandledException(new Exception(error.Description), error.Description);
				throw new Exception(error.Description);
			}
		}

		private void DeleteLibraryFolderContents(string folderName)
		{
			var manager = NSFileManager.DefaultManager;
			var library = manager.GetUrls(NSSearchPathDirectory.LibraryDirectory, NSSearchPathDomain.User).First();
			var dir = Path.Combine(library.Path, folderName);
			var contents = manager.GetDirectoryContent(dir, out NSError error);
			if (error != null)
			{
				//App.UnhandledException(new Exception(error.Description), error.Description);
				throw new Exception(error.Description);
			}

			foreach (var c in contents)
			{
				try
				{
					manager.Remove(Path.Combine(dir, c), out NSError errorRemove);
					if (errorRemove != null)
					{
						//App.UnhandledException(new Exception(error.Description), error.Description);
						throw new Exception(error.Description);
					}
				}
				catch
				{
					//App.UnhandledException(ex, $"Error deleting folder contents: {folderName}{Environment.NewLine}{ex.Message}");
					throw;
				}
			}
		}

		private bool _isDisposed;

		protected override void Dispose(bool disposing)
		{
			if (this._isDisposed)
				return;

			this._isDisposed = true;

			base.Dispose(disposing);
			GC.SuppressFinalize(this);
		}
	}
}