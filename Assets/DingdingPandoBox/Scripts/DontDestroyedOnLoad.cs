using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cameo
{
    public class DontDestroyedOnLoad : MonoBehaviour
    {
		private void Start()
		{
            DontDestroyOnLoad(gameObject);
		}
	}
}
