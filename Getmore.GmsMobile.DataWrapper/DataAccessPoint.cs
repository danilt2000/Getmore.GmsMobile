using Getmore.GmsMobile.Core.Authentication;
using Json.Net;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Getmore.GmsMobile.DataWrapper
{
	public class DataAccessPoint
	{
		public IList<GmsInstanceLoginData> GetGmsInstances()
		{
			string fileName = GetInstancesFilePath();

			if (File.Exists(fileName))
			{
				var text = File.ReadAllText(fileName);
				return !String.IsNullOrWhiteSpace(text) ? JsonNet.Deserialize<List<GmsInstanceLoginData>>(text) : new List<GmsInstanceLoginData>();
			}
			else
				return new List<GmsInstanceLoginData>();
		}

		public void AddGmsInstace(GmsInstanceLoginData newInstance)
		{
			var allInstances = this.GetGmsInstances();

			var oldInstance = allInstances.FirstOrDefault(I => I.GmsInstanceUrl == newInstance.GmsInstanceUrl && I.UserLogin == newInstance.UserLogin);
			if (oldInstance != null)
				_ = allInstances.Remove(oldInstance);

			allInstances.Add(newInstance);

			var serializedInstances = JsonNet.Serialize(allInstances);
			string fileName = GetInstancesFilePath();
			File.WriteAllText(fileName, serializedInstances);
		}

		private static string GetInstancesFilePath()
		{
			return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "gms-instances.json");
		}
	}
}
