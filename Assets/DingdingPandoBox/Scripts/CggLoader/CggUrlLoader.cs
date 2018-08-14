using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Linq;
using UnityEngine;
using System.Text;
using System;
using LitJson;
using CI.HttpClient;
using Cameo;

public class CggUrlLoader {

	private Action<Boolean, string, string> callBack;

	private string apiUrl = "https://mractivitiesUAT.sogo.com.tw/TDPGW/api/cggUrl";
	private string key = "4y8yHm71GXwYw0M57XOMWhNCtCJOWxR4";
	private string iv = "GZ372ub2p1ywKI9i";
	private string responseResult = "";
	private byte[] encrypted = null;
	HttpClient httpClient = new HttpClient();
	private string result = "";
	private string actionId = "";

	// Use this for initialization
	public void Load(string actionIdIn, Action<Boolean, string, string> callBackFunc)
	{
		actionId = actionIdIn;
		callBack = callBackFunc;

		System.Net.ServicePointManager.ServerCertificateValidationCallback += (o, certificate, chain, errors) =>
		{
			return true;
		};

		Dictionary<string, string> inputData = new Dictionary<string, string>();
		inputData.Add("actionId", actionId);
		string original = JsonMapper.ToJson(inputData);
		string originalUrlEncode = WWW.EscapeURL(original);
		encrypted = EncryptStringToBytes_Aes(originalUrlEncode, Encoding.UTF8.GetBytes(key), Encoding.UTF8.GetBytes(iv));

		StringContent stringContent = new StringContent(Convert.ToBase64String(encrypted), Encoding.UTF8, "text/plain");
		httpClient.Post(new Uri(apiUrl), stringContent, (r) =>
			{
				// Raised when the download completes
				responseResult = r.Data;
				Debug.Log("[CggUrl] response result: " + responseResult);
				Decrypt();
			});

		//string testResult = HttpPost(apiUrl, Convert.ToBase64String(encrypted));
		//StartCoroutine(PostRequest(apiUrl, encrypted));

		Debug.Log("Original: " + original);
		Debug.Log("Encrypted (b64-encode): " + Convert.ToBase64String(encrypted));

	}

	private void Decrypt()
	{
		//responseResult = "Y+d6Jo/hb8EtnYbouRKRW+MhdVbjL+J5sgzcVFp4k4mLA95k9u/kiWBuf4iDwFdu1PK9+zFPknWP3Nk06ealEFNbb4fPpY0EXAgucG4VayVzKjUYjW0mTKV23m3wHo8NWEgg9FlZcZvfXIOc3ikZSap5DOad0Ih4g1PNxYT0AHFo8GBYEn4wjOAuxorvDSj+rqvwtUtMRtU8x9Gxv+TOk0E7yBViJNCqbrNYXsBQ94jaDIKYMrfTRepgmyGb/xXC1uejmdcIUCkGvsqx7GkqZ1EKE/Ojppo41R15z6+gidlomqFnNVkS6k2zAMHwqphb06z+o0cph34qBMSY+YOR0sizv0rcPzpwq4HUJqAWhZVek2qGdGNbnvT41POg3DeJfv7vwmlegLFWNtGkHB2Mdg==";
		string roundtrip = DecryptStringFromBytes_Aes(Convert.FromBase64String(responseResult), Encoding.UTF8.GetBytes(key), Encoding.UTF8.GetBytes(iv));
		Debug.Log("Round Trip: " + WWW.UnEscapeURL(roundtrip));
		result = WWW.UnEscapeURL(roundtrip);

		// 20180530 bigcookie: Fake Data Test
		//TextAsset testJson = Resources.Load<TextAsset>("json/productInfo");
		//result = testJson.text;

		callBack(true, actionId, result);
	}

	private byte[] EncryptStringToBytes_Aes(string plainText, byte[] Key, byte[] iv)
	{
		byte[] sourceBytes = Encoding.UTF8.GetBytes(plainText);
		var aes = new RijndaelManaged();
		aes.Key = Key;
		aes.IV = iv;
		aes.Mode = CipherMode.CBC;
		aes.Padding = PaddingMode.PKCS7;

		ICryptoTransform transform = aes.CreateEncryptor();

		return transform.TransformFinalBlock(sourceBytes, 0, sourceBytes.Length);
	}

	private string DecryptStringFromBytes_Aes(byte[] cipherTextCombined, byte[] Key, byte[] iv)
	{
		var aes = new RijndaelManaged();
		aes.Key = Key;
		aes.IV = iv;
		aes.Mode = CipherMode.CBC;
		aes.Padding = PaddingMode.PKCS7;
		ICryptoTransform transform = aes.CreateDecryptor();

		return Encoding.UTF8.GetString(transform.TransformFinalBlock(cipherTextCombined, 0, cipherTextCombined.Length));
	}


}
