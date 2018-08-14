using UnityEngine;
using UnityEditor;
using System.Collections;
using System.IO;
using System.Linq;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

public class EditorExtension
{
	[MenuItem("Cameo/Play")]
	static void Play()
	{
        string preloaderScene = Application.dataPath + "/DingdingPandoBox/Scenes/Preloader.unity";
		playFromPreload (preloaderScene);
	}

	private static void playFromPreload(string filePath)
	{
		EditorApplication.isPlaying = false;

		if (File.Exists(filePath))
		{
			EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo ();
			Scene scene = EditorSceneManager.OpenScene (filePath);
			EditorApplication.isPlaying = true;
		}
		else
		{
			Debug.Log("[EditorUtility.PlayFromPreloader] Cannot find " + filePath);
		}
	}

	[MenuItem("Cameo/PersistentFolder")]
	static void OpenPersistentFolder()
	{
		OpenInFileBrowser.Open (Application.persistentDataPath);
	}

	[MenuItem("Cameo/Change with selected font")]
	static void ChangeFont()
	{
		Debug.Log(Selection.objects.Length);

		if(Selection.objects.Length != 1 || !Selection.objects[0] is Font)
		{
			Debug.LogWarning("Please select a font asset");
			return;
		}
		else
		{
			Font changeToFont = Selection.objects[0] as Font;

			UnityEngine.UI.Text [] objs = Component.FindObjectsOfType<UnityEngine.UI.Text>();
			for(int i=0; i<objs.Length; ++i)
			{
				objs[i].font = changeToFont;
			}
			Debug.Log("Finished");
		}
	}

}
