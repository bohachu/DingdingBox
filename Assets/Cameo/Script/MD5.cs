using UnityEngine;
using System.Collections;
using Cameo;

public class MD5 
{	
	public static string getStrMd5String(string strToEncrypt) 
	{
		string strReturnString = "";
		System.Text.UTF8Encoding ue = new System.Text.UTF8Encoding ();
		byte[] bytes = ue.GetBytes (strToEncrypt);
		System.Security.Cryptography.MD5CryptoServiceProvider md5 = new System.Security.Cryptography.MD5CryptoServiceProvider ();
		byte[] hashBytes = md5.ComputeHash (bytes);
		string strHashString = "";
		for (int i = 0; i < hashBytes.Length; i++) {
			strHashString += System.Convert.ToString (hashBytes [i], 16).PadLeft (2, '0');
		}
		strReturnString = strHashString.PadLeft (32, '0');
		return strReturnString;
	}
}
