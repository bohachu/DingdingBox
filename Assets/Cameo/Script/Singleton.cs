using System;
using UnityEngine;

namespace Cameo
{
	public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
	{
		//public bool IsDontDestroyOnLoaded;

		protected static T _instance;

		protected static readonly object _synObject = new object();

		#region Property Message
		public static T Instance
		{
			get
			{
				if (_instance == null)
				{
					lock (_synObject)
					{
						if (_instance == null)
						{
							_instance = FindObjectOfType<T>();
						}
					}

					if (_instance == null)
					{
						Init();

						Debug.Log("An instance of " + typeof(T) +
							" is needed in the scene, but there is none. Created automatically");
					}
				}

				return _instance;
			}
		}

		public static  bool IsExistInstance	{get {return _instance != null;}}
		#endregion

		void Awake()
		{
			RegisterInstance ();
		}

		public static T Init()
		{
			if(_instance == null)
			{
				GameObject obj = new GameObject(typeof(T).ToString());
				_instance = obj.AddComponent<T>();
			}

			return _instance;
		}

		public void RegisterInstance () 
		{
			if (_instance == null)
			{
				_instance = GetComponent<T>();
				/*
				if ((_instance as Singleton<T>).IsDontDestroyOnLoaded) 
				{
					DontDestroyOnLoad (_instance.gameObject);
				}
				*/
			}
			else if(_instance != this)
			{
				DestroyImmediate(gameObject);
			}
		}
		/*
		//For Editor Use
		public void Destory()
		{
			if (_instance != null)
			{
				DestroyImmediate(_instance.gameObject);
				_instance = null;
			}
			T [] instances = FindObjectsOfType<T>();
			for(int i = 0; i < instances.Length; i++)
			{
				DestroyImmediate(instances[i].gameObject);
			}
		}
		*/
	}
}

