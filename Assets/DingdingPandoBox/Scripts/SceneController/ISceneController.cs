using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cameo
{
    public abstract class ISceneController : MonoBehaviour
    {
        public Dictionary<string, object> ParamMapping = null;
        public abstract IEnumerator InitializeCoroutine();
        public abstract IEnumerator ReleaseCoroutine();
    }
}
