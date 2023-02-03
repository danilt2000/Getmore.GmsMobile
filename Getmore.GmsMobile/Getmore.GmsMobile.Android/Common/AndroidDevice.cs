using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Getmore.GmsMobile.Core.Common;
using Getmore.GmsMobile.Droid.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

[assembly: Xamarin.Forms.Dependency(typeof(AndroidDevice))]
namespace Getmore.GmsMobile.Droid.Common
{
	public class AndroidDevice : IMobileDevice
	{
		public string GetIdentifier()
		{
			return Android.Provider.Settings.Secure.GetString(Application.Context.ContentResolver, Android.Provider.Settings.Secure.AndroidId);
		}
	}
}