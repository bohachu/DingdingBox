/// <summary>
/// write by 52cwalk,if you have some question ,please contract lycwalk@gmail.com
/// </summary>
/// 
/// 

using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class QREncodeTest : MonoBehaviour {
	public QRCodeEncodeController e_qrController;
	public RawImage qrCodeImage;
	public InputField m_inputfield;
	public Text infoText;
	// Use this for initialization
	void Start () {
		if (e_qrController != null) {
			e_qrController.onQREncodeFinished += qrEncodeFinished;//Add Finished Event
		}
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void qrEncodeFinished(Texture2D tex)
	{
		if (tex != null && tex != null) {
			int width = tex.width;
			int height = tex.height;
			float aspect = width * 1.0f / height;
			qrCodeImage.GetComponent<RectTransform> ().sizeDelta = new Vector2 (170, 170.0f / aspect);
			qrCodeImage.texture = tex;
		} else {

		}
	}

	public void Encode()
	{
		if (e_qrController != null) {
			string valueStr = m_inputfield.text;
			int errorlog = e_qrController.Encode(valueStr);
			infoText.color = Color.red;
			if (errorlog == -13) {
				infoText.text = "Must contain 12 digits,the 13th digit is automatically added !";

			} else if (errorlog == -8) {
				infoText.text = "Must contain 7 digits,the 8th digit is automatically added !";

			} else if (errorlog == -128) {
				infoText.text = "Contents length should be between 1 and 80 characters !";

			} else if (errorlog == -1) {
				infoText.text = "Please select one code type !";
			}
			else if (errorlog == 0) {
				infoText.color = Color.green;
				infoText.text = "Encode successfully !";
			}


		}
	}

	public void ClearCode()
	{
		qrCodeImage.texture = null;
		m_inputfield.text = "";
		infoText.text = "";
	}

}
