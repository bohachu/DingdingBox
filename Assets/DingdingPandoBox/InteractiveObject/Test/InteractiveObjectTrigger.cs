using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cameo.PandoBox
{
    public enum InteractiveObjectType
    {
        Present,
        VideoPlayer
    }

    public class InteractiveObjectTrigger : MonoBehaviour
    {
        public InteractiveObjectType Type;
        public Transform Root;

        public GameObject PresentPrefab;
        public GameObject VideoPrefab;

        public string youtubeUrl;

        private IDisplayObject displayObject;

        public void Show()
        {
            if(displayObject != null)
            {
                Close();
            }

            switch (Type)
            {
                case InteractiveObjectType.Present:
                    displayObject = showPresent();
                    break;
                case InteractiveObjectType.VideoPlayer:
                    displayObject = showVideo();
                    break;
            }
        }

        public void Close()
        {
            if(displayObject != null)
            {
                Destroy(displayObject.gameObject);
                displayObject = null;
            }
        }

        private IDisplayObject showPresent()
        {
            GameObject newObj = Instantiate(PresentPrefab, Root, false);
            displayObject = newObj.GetComponent<IDisplayObject>();
            displayObject.Show();
            return displayObject;
        }


        private IDisplayObject showVideo()
        {
            GameObject newObj = Instantiate(VideoPrefab, Root, false);
            displayObject = newObj.GetComponent<IDisplayObject>();
            displayObject.Show(youtubeUrl);
            return displayObject;
        }
    }
}

