using System;
using System.Text.RegularExpressions;

public class Utility
{
	public static bool CheckIsUrlFormat(string strValue)
	{
		return Utility.CheckIsFormat("(http://)?([\\w-]+\\.)+[\\w-]+(/[\\w- ./?%&=]*)?", strValue);
	}

	public static bool CheckIsFormat(string strRegex, string strValue)
	{
		if (strValue != null && strValue.Trim() != string.Empty)
		{
			Regex regex = new Regex(strRegex);
			return regex.IsMatch(strValue);
		}
		return false;
	}
}
