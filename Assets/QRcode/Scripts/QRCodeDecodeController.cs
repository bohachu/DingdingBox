/// <summary>
/// write by 52cwalk,if you have some question ,please contract lycwalk@gmail.com
/// </summary>

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;

using ZXing;
using ZXing.Common;
using ZXing.QrCode;
using System.IO;

public class QRCodeDecodeController : MonoBehaviour
{
	public delegate void QRScanFinished(string str);  //declare a delegate to deal with the QRcode decode complete
	public event QRScanFinished onQRScanFinished;  		//declare a event with the delegate to trigger the complete event
	
	bool decoding = false;		
	bool tempDecodeing = false;
	string dataText = null;
	public DeviceCameraController e_DeviceController = null; 
	private Color[] orginalc;   	//the colors of the camera data.
	private Color32[] targetColorARR;   	//the colors of the camera data.
	private byte[] targetbyte;		//the pixels of the camera image.
	private int W, H, WxH;			//width/height of the camera image			
	int framerate = 0; 		

	#if UNITY_IOS
	int blockWidth = 450;
	#elif UNITY_ANDROID
	int blockWidth = 350;
	#else
	int blockWidth = 350;
	#endif
	bool isInit = false;
	BarcodeReader barReader;
	void Start()
	{
		barReader = new BarcodeReader ();
		barReader.AutoRotate = true;
		barReader.TryInverted = true;
		
		if (!e_DeviceController) {
			e_DeviceController = GameObject.FindObjectOfType<DeviceCameraController>();
			if(!e_DeviceController)
			{
				Debug.LogError("the Device Controller is not exsit,Please Drag DeviceCamera from project to Hierarchy");
			}
		}
	}
	
	void Update()
	{
		#if UNITY_EDITOR
		if (framerate++ % 10== 0) {
		#else
		if (framerate++ % 8== 0) {
		#endif
			if (e_DeviceController.isPlaying && !decoding)
			{
				W = e_DeviceController.dWebCam.Width();					// get the image width
				H = e_DeviceController.dWebCam.Height();			// get the image height 

				if (W < 100 || H < 100) {
					return;
				}

				if(!isInit && W>100 && H>100)
				{
					blockWidth = (int)((Math.Min(W,H)/3f) *2);
					isInit = true;
				}

				if(targetColorARR == null)
				{
					targetColorARR= new Color32[blockWidth * blockWidth];
				}

				int posx = ((W-blockWidth)>>1);//
				int posy = ((H-blockWidth)>>1);
				
				orginalc = e_DeviceController.dWebCam.GetPixels(posx,posy,blockWidth,blockWidth);// get the webcam image colors

                //convert the color(float) to color32 (byte)
				for(int i=0;i!= blockWidth;i++)
				{
					for(int j = 0;j!=blockWidth ;j++)
					{
						targetColorARR[i + j*blockWidth].r = (byte)( orginalc[i + j*blockWidth].r*255);
						targetColorARR[i + j*blockWidth].g = (byte)(orginalc[i + j*blockWidth].g*255);
						targetColorARR[i + j*blockWidth].b = (byte)(orginalc[i + j*blockWidth].b*255);
						targetColorARR[i + j*blockWidth].a = 255;
					}
				}

				// scan the qrcode 
				Loom.RunAsync(() =>
				              {
					try
					{
						Result data;
						data = barReader.Decode(targetColorARR,blockWidth,blockWidth);//start decode
						if (data != null) // if get the result success
						{
							decoding = true; 	// set the variable is true
							dataText = data.Text;	// use the variable to save the code result
						}

					}
					catch (Exception e)
					{
							Debug.LogError("Decode Error: " + e.Data.ToString());
						decoding = false;
					}
				});	
			}
			
			if(decoding)
			{
				// if the status variable is change
				if(tempDecodeing != decoding)
				{
					onQRScanFinished(dataText);//triger the scan finished event;
				}
				tempDecodeing = decoding;
			}
		}
	}
 	
	/// <summary>
	/// Reset this scan param
	/// </summary>
	public void Reset()
	{
		decoding = false;
		tempDecodeing = decoding;
	}



	/// <summary>
	/// Stops the work.
	/// </summary>
	public void StartWork()
	{
		if (e_DeviceController != null) {
			e_DeviceController.StartWork();
		}
		decoding = false;
		tempDecodeing = decoding;
	}
	
	/// <summary>
	/// Stops the work.
	/// </summary>
	public void StopWork()
	{
		decoding = true;
		tempDecodeing = decoding;
		if (e_DeviceController != null) {
			e_DeviceController.StopWork();
		}
	}
	
	/// <summary>
	/// Decodes the by static picture.
	/// </summary>
	/// <returns> return the decode result string </returns>
	/// <param name="tex">target texture.</param>
	public static string DecodeByStaticPic(Texture2D tex)
	{
		BarcodeReader codeReader = new BarcodeReader ();
		codeReader.AutoRotate = true;
		codeReader.TryInverted = true;
		
		Result data = codeReader.Decode (tex.GetPixels32 (), tex.width, tex.height);
		if (data != null) {
			return data.Text;
		} else {
			return "decode failed!";
		}
	}
	
}