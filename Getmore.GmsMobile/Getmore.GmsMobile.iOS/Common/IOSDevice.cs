using Foundation;
using Getmore.GmsMobile.Core.Common;
using Getmore.GmsMobile.iOS.Common;
using System;
using System.Runtime.InteropServices;

[assembly: Xamarin.Forms.Dependency(typeof(IOSDevice))]
namespace Getmore.GmsMobile.iOS.Common
{
	/// <summary>
	/// http://codeworks.it/blog/?p=260
	/// https://stackoverflow.com/questions/50232847/how-to-get-device-id-in-xamarin-forms
	/// </summary>

	public class IOSDevice : IMobileDevice
	{
		[DllImport("/System/Library/Frameworks/IOKit.framework/IOKit")]
		private static extern uint IOServiceGetMatchingService(uint masterPort, IntPtr matching);

		[DllImport("/System/Library/Frameworks/IOKit.framework/IOKit", CharSet = CharSet.Unicode)]
		private static extern IntPtr IOServiceMatching(string s);

		[DllImport("/System/Library/Frameworks/IOKit.framework/IOKit")]
		private static extern IntPtr IORegistryEntryCreateCFProperty(uint entry, IntPtr key, IntPtr allocator, uint options);

		[DllImport("/System/Library/Frameworks/IOKit.framework/IOKit")]
		private static extern int IOObjectRelease(uint o);

		[Obsolete]
		public string GetIdentifier()
		{
			string serial = string.Empty;
			uint platformExpert = IOServiceGetMatchingService(0, IOServiceMatching("IOPlatformExpertDevice"));
			if (platformExpert != 0)
			{
				NSString key = (NSString)"IOPlatformSerialNumber";
				IntPtr serialNumber = IORegistryEntryCreateCFProperty(platformExpert, key.Handle, IntPtr.Zero, 0);
				if (serialNumber != IntPtr.Zero)
				{
					serial = NSString.FromHandle(serialNumber);
				}

				_ = IOObjectRelease(platformExpert);
			}

			return serial;
		}
	}

}