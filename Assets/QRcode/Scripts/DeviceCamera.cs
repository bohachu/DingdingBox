using UnityEngine;
using System.Collections;
using TBEasyWebCam;

public class DeviceCamera {

	public Texture preview
	{
		get
		{
			#if UNITY_ANDROID && !UNITY_EDITOR
			if(isUseEasyWebCam)
			{
				return EasyWebCam.WebCamPreview;
			}
			else
			{
				return webcamera;
			}
			#else
			return webcamera;
			#endif
		}
	}

	WebCamTexture webcamera;
	bool isRunning = false;
	#if UNITY_ANDROID && !UNITY_EDITOR
	bool isUseEasyWebCam = true;
	#endif
	public DeviceCamera (bool isUseEWC = true)
	{
		

		#if UNITY_ANDROID && !UNITY_EDITOR
		isUseEasyWebCam = isUseEWC;
		if(isUseEasyWebCam)
		{
			GameObject gameObject = new GameObject("EasyWebCamLib");
			gameObject.AddComponent<EasyWebCam>();
		}
		else
		{
			webcamera = new WebCamTexture (640, 480);
		}
		#else
		webcamera = new WebCamTexture (640, 480);
		#endif

	}

	public DeviceCamera (int width,int height,bool isUseEWC = true)
	{
		
		#if UNITY_ANDROID && !UNITY_EDITOR
		isUseEasyWebCam = isUseEWC;
		if(isUseEasyWebCam)
		{
			GameObject gameObject = new GameObject("EasyWebCamLib");
			gameObject.AddComponent<EasyWebCam>();
		}
		else
		{
			webcamera = new WebCamTexture (width, height);
		}
		#else
			webcamera = new WebCamTexture (width, height);
		#endif
	}


	/// <summary>
	/// open the camera
	/// </summary>
	public void Play()
	{
		if (isRunning) {
			return;
		}
		#if UNITY_ANDROID && !UNITY_EDITOR
		if(isUseEasyWebCam)
		{
			EasyWebCam.Play();
		}
		else{
			webcamera.Play ();
		}
		#else
		webcamera.Play ();
		#endif
		isRunning = true;
	}

	/// <summary>
	/// Stop this camera.
	/// </summary>
	public void Stop()
	{
		if (!isRunning) {
			return;
		}

		#if UNITY_ANDROID && !UNITY_EDITOR
		if(isUseEasyWebCam)
		{
		EasyWebCam.Stop();
		}
		else
		{
		webcamera.Stop ();
		}
		#else
		webcamera.Stop ();
		#endif
		isRunning = false;
	}
	/// <summary>
	/// Gets the size of the webcam
	/// </summary>
	/// <returns>The size.</returns>
	public Vector2 getSize()
	{
		#if UNITY_ANDROID && !UNITY_EDITOR
		if(isUseEasyWebCam)
		{
		return new Vector2(EasyWebCam.Width(), EasyWebCam.Height()); 
		}
		else
		{
		return new Vector2(webcamera.width, webcamera.height); 
		}

		#else
		return new Vector2(webcamera.width, webcamera.height); 
		#endif
	}

	/// <summary>
	/// get the width of the camera
	/// </summary>
	public int Width()
	{
		#if UNITY_ANDROID && !UNITY_EDITOR
		if(isUseEasyWebCam)
		{
		return EasyWebCam.Width(); 
		}
		else
		{
		return webcamera.width; 
		}
		#else
		return webcamera.width; 
		#endif

	}

	/// <summary>
	/// get the height of the camera
	/// </summary>
	public int Height()
	{
		#if UNITY_ANDROID && !UNITY_EDITOR
		if(isUseEasyWebCam)
		{
			return  EasyWebCam.Height(); 
		}
		else
		{
			return webcamera.height; 
		}

		#else
		return webcamera.height; 
		#endif
	}

	/// <summary>
	/// get status of the camera
	/// </summary>
	/// <returns><c>true</c>, if playing was ised, <c>false</c> otherwise.</returns>
	public bool isPlaying()
	{
		#if UNITY_ANDROID && !UNITY_EDITOR
		if(isUseEasyWebCam)
		{
		return EasyWebCam.isPlaying ();
		}
		else
		{
		return webcamera.isPlaying; 
		}
			
		#else
		return webcamera.isPlaying; 
		#endif
	}

	/// <summary>
	///  get the Pixels of the camera image
	/// </summary>
	/// <returns>The pixels.</returns>
	public Color[] GetPixels()
	{
		#if UNITY_ANDROID && !UNITY_EDITOR
		if(isUseEasyWebCam)
		{
			return EasyWebCam.WebCamPreview.GetPixels();
		}
		else
		{
		return webcamera.GetPixels(); 
		}

		#else
		return webcamera.GetPixels(); 
		#endif
	}

	/// <summary>
	/// get the pixels of the camera image by using the target rect range
	/// </summary>
	/// <returns>The pixels.</returns>
	/// <param name="x">The x coordinate.</param>
	/// <param name="y">The y coordinate.</param>
	/// <param name="targetWidth">Target width.</param>
	/// <param name="targetHeight">Target height.</param>
	public Color[] GetPixels(int x,int y,int targetWidth,int targetHeight)
	{
		#if UNITY_ANDROID && !UNITY_EDITOR
		if(isUseEasyWebCam)
		{
			return EasyWebCam.WebCamPreview.GetPixels(x,y,targetWidth,targetHeight); 
		}
		else
		{
			return webcamera.GetPixels(x,y,targetWidth,targetHeight); 
		}
			
		#else
		return webcamera.GetPixels(x,y,targetWidth,targetHeight); 
		#endif
	}

	/// <summary>
	/// Gets the pixels32 of the camera
	/// </summary>
	/// <returns>The pixels32.</returns>
	public Color32[] GetPixels32()
	{
		#if UNITY_ANDROID && !UNITY_EDITOR
		if(isUseEasyWebCam)
		{
			return EasyWebCam.WebCamPreview.GetPixels32();
		}
		else
		{
			return webcamera.GetPixels32(); 
		}
			

		#else
		return webcamera.GetPixels32(); 
		#endif
	}



}
