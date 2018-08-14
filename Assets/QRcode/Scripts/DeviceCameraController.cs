/// <summary>
/// write by 52cwalk,if you have some question ,please contract lycwalk@gmail.com
/// </summary>

using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TBEasyWebCam;

public class DeviceCameraController : MonoBehaviour {

	public DeviceCamera dWebCam
	{
		get
		{ 
			return webcam;
		}
	}

	private DeviceCamera webcam; 

	GameObject e_CameraPlaneObj;
	bool isCorrected = false;
	float screenVideoRatio = 1.0f;
	public bool isUseEasyWebCam = true;

	public bool isPlaying
	{
		get{
			if (webcam != null) {
				return webcam.isPlaying ();
			} else {
				return false;
			}
		}
	}

	void Start()
	{
		webcam = new DeviceCamera (isUseEasyWebCam);
		e_CameraPlaneObj = transform.Find ("CameraPlane").gameObject;
		StartWork ();
	}

	// Update is called once per frame  
	void Update()  
	{  
		if (webcam != null && webcam.isPlaying()) {
			if (webcam.Width() > 200 && !isCorrected) {
				correctScreenRatio();
			}

			if(e_CameraPlaneObj.activeSelf)
			{
				e_CameraPlaneObj.GetComponent<Renderer>().material.mainTexture = webcam.preview;
			}

		}
	}

	/// <summary>
	/// Stops the work.
	/// when you need to leave current scene ,you must call this func firstly
	/// </summary>
	public void StartWork()
	{
		if (this.webcam != null) {
			this.webcam.Play ();
		}
	}

	/// <summary>
	/// Stops the work.
	/// when you need to leave current scene ,you must call this func firstly
	/// </summary>
	public void StopWork()
	{
		if (this.webcam != null && this.webcam.isPlaying()) {
			this.webcam.Stop ();
		}
		if(e_CameraPlaneObj.activeSelf)
		{
			e_CameraPlaneObj.GetComponent<Renderer>().material.mainTexture = null;
		}

	}

	/// <summary>
	/// Corrects the screen ratio.
	/// </summary>
	void correctScreenRatio()
	{
		int videoWidth = 640;
		int videoHeight = 480;
		int ScreenWidth = 640;
		int ScreenHeight = 480;

		float videoRatio = 1;
		float screenRatio = 1;

		if (this.webcam != null) {
			videoWidth = webcam.Width();
			videoHeight = webcam.Height();
		}
		videoRatio = videoWidth * 1.0f / videoHeight;
		ScreenWidth = Mathf.Max (Screen.width, Screen.height);
		ScreenHeight = Mathf.Min (Screen.width, Screen.height);
		screenRatio = ScreenWidth * 1.0f / ScreenHeight;

		screenVideoRatio = screenRatio / videoRatio;
		isCorrected = true;

		if (e_CameraPlaneObj != null) {
			e_CameraPlaneObj.GetComponent<CameraPlaneController>().correctPlaneScale(screenVideoRatio);
		}
	}


}


