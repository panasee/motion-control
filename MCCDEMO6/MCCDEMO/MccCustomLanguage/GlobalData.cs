using System.Configuration;

namespace MccCustomLanguage;

public class GlobalData
{
	public static string SystemLanguage = ConfigurationManager.AppSettings["Language"];

	private static Language globalLanguage;

	public static Language GlobalLanguage
	{
		get
		{
			if (globalLanguage == null)
			{
				globalLanguage = new Language();
				return globalLanguage;
			}
			return globalLanguage;
		}
	}
}
