using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cameo.PandoBox
{
    public abstract class IDisplayObject : MonoBehaviour
    {
        public abstract void Show(params object[] param);
    }
}