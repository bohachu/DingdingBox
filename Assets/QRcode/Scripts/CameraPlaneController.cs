/// <summary>
/// write by 52cwalk,if you have some question ,please contract lycwalk@gmail.com
/// </summary>

using UnityEngine;
using System.Collections;

public class CameraPlaneController : MonoBehaviour {

	public Camera _targetCam;

	ScreenOrientation orientation;
	float height = 0;
	float width = 0;
	float screenRatio = 1.0f;
	Vector3 and_pRot, and_puRot, and_lRot, and_lrRot;

	// Use this for initialization
	void Start () {
		init ();
		correctPlaneScale(1.0f);
	}

	public void init()
	{
		float Screenheight = (float)_targetCam.orthographicSize* 2.0f; 
		float Screenwidth = Screenheight * Screen.width / Screen.height;
		height = Screenheight ;
		width = Screenwidth;
		this.transform.localPosition = new Vector3(0,0,91.6f);
		
		#if UNITY_EDITOR|| UNITY_STANDALONE
		transform.localEulerAngles = new Vector3(90,180,0);
		transform.localScale = new Vector3(width/10, 1.0f, height/10);
		#elif UNITY_WEBPLAYER
		transform.localEulerAngles = new Vector3(90,180,0);
		transform.localScale = new Vector3(width/10, 1.0f, height/10);
		#endif
		assignAngleVectors ();// get the rotation by device model

		orientation = Screen.orientation;
		#if UNITY_ANDROID||UNITY_IOS
		updateRotationAndScale();
		#endif

	}

	// Update is called once per frame
	void Update () {
			if (orientation != Screen.orientation) {
			updateRotationAndScale ();
			orientation = Screen.orientation;
			correctPlaneScale(screenRatio);
		}
	}
	
	void assignAngleVectors()
	{
		//"Nexus"
		string devceName = SystemInfo.deviceModel.ToString ();
		if (devceName.ToLower().Contains("nexus") && devceName.ToLower().Contains("5x")) {
		#if UNITY_ANDROID
			and_lrRot = new Vector3(90,180,0);
			and_lRot = new Vector3(-90,0,0);
			and_pRot= new Vector3(0,90,270);
			and_puRot= new Vector3(0,270,90);
		#elif UNITY_IOS
			and_lrRot = new Vector3(-90,0,0);
			and_lRot = new Vector3(90,180,0);
			and_pRot=new Vector3(0,270,90);
			and_puRot=new Vector3(0,270,90);
		#endif
		} else {
		#if UNITY_ANDROID
			and_lrRot = new Vector3(-90,0,0);
			and_lRot = new Vector3(90,180,0);
			and_pRot= new Vector3(0,270,90);
			and_puRot=new Vector3(0,90,270);
		#elif UNITY_IOS
			and_lrRot = new Vector3(90,180,0);
			and_lRot = new Vector3(-90,0,0);
			and_pRot= new Vector3(0,90,270);
			and_puRot= new Vector3(0,270,90);
		#endif
		}
	}
	
	public void correctPlaneScale(float size)
	{
		screenRatio = size;
		#if UNITY_ANDROID|| UNITY_IOS||UNITY_METRO
		Vector3 orgVec = transform.localScale;
		
		if(screenRatio >1f)
		{
			transform.localScale = new Vector3(orgVec.x, 1.0f, orgVec.z * screenRatio);	
		}
		else if(screenRatio <1 && screenRatio >0)
		{
			transform.localScale = new Vector3(orgVec.x/screenRatio, 1.0f, orgVec.z);	
		}
		else
		{
			
		}
		#endif
	}


		void updateRotationAndScale()
		{
			int screenHeight_1 = Screen.height;
			int screenWidth_1 = Screen.width;
			if (Screen.orientation == ScreenOrientation.Portrait||
			Screen.orientation == ScreenOrientation.PortraitUpsideDown) {

			if(screenHeight_1 < screenWidth_1)
			{
				int tempvalue = screenWidth_1;
				screenWidth_1 = screenHeight_1;
				screenHeight_1 = tempvalue;
			}
			float Screenheight = (float)_targetCam.orthographicSize* 2.0f; 
			float Screenwidth = Screenheight * screenWidth_1 / screenHeight_1;
			height = Screenheight ;
			width = Screenwidth;
			#if UNITY_ANDROID
			if( Screen.orientation == ScreenOrientation.PortraitUpsideDown)
			{
				transform.localEulerAngles = and_puRot;
			}
			else
			{
				transform.localEulerAngles = and_pRot;
			}

			transform.localScale = new Vector3(height/10, 1.0f, width/10);
			#elif UNITY_IOS
			if( Screen.orientation == ScreenOrientation.PortraitUpsideDown)
			{
				transform.localEulerAngles = and_puRot;
			}
			else
			{
				transform.localEulerAngles = and_pRot;
			}

			transform.localScale = new Vector3(-1*height/10, 1.0f, width/10);
			#endif
			} else if (Screen.orientation == ScreenOrientation.Landscape||
				Screen.orientation == ScreenOrientation.LandscapeLeft) {

			if(screenHeight_1 > screenWidth_1)
			{
				int tempvalue = screenWidth_1;
				screenWidth_1 = screenHeight_1;
				screenHeight_1 = tempvalue;
			}

			float Screenheight = (float)_targetCam.orthographicSize* 2.0f; 
			float Screenwidth = Screenheight * screenWidth_1 / screenHeight_1;
			height = Screenheight ;
			width = Screenwidth;

			#if UNITY_ANDROID
			transform.localEulerAngles = and_lRot;
			transform.localScale = new Vector3(width/10, 1.0f, height/10);
			#elif UNITY_IOS
			transform.localEulerAngles = and_lRot;
			transform.localScale = new Vector3(-1*width/10, 1.0f, height/10);
			#endif
			}
			else if(Screen.orientation == ScreenOrientation.LandscapeRight)
			{
				if(screenHeight_1 > screenWidth_1)
				{
					int tempvalue = screenWidth_1;
					screenWidth_1 = screenHeight_1;
					screenHeight_1 = tempvalue;
				}

			float Screenheight = (float)_targetCam.orthographicSize* 2.0f; 
			float Screenwidth = Screenheight * screenWidth_1 / screenHeight_1;
			height = Screenheight ;
			width = Screenwidth;
			#if UNITY_ANDROID
			transform.localEulerAngles = and_lrRot;
			transform.localScale = new Vector3(width/10, 1.0f, height/10);
			#elif UNITY_IOS

			transform.localEulerAngles = and_lrRot;
			transform.localScale = new Vector3(-1*width/10, 1.0f, height/10);
			#endif
			}
		}


}
