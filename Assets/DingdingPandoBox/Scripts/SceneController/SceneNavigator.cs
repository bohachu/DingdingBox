using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Cameo
{
    public class SceneNavigator : Singleton<SceneNavigator>
    {
        [SerializeField]
        private float maskTime = 2f;

        private bool enableMask;

        private AsyncOperation loadSceneAsync;

        private Dictionary<string, object> paramMapping;

        public void LoadScene(string nextScene, Dictionary<string, object> paramMapping = null, bool enableMask = true)
        {
            if(nextScene != SceneManager.GetActiveScene().name)
            {
                this.enableMask = enableMask;
                this.paramMapping = paramMapping;
                loadSceneAsync = SceneManager.LoadSceneAsync(nextScene);
                loadSceneAsync.allowSceneActivation = false;
                StartCoroutine("loadSceneCoroutine");
            }
        }

        private IEnumerator loadSceneCoroutine()
        {
            if(enableMask)
            {
                MaskUtility.Instance.Show(maskTime);
                yield return new WaitForSeconds(maskTime);
            }

            ISceneController controller = FindObjectOfType<ISceneController>();
            if (controller != null)
            {
                yield return StartCoroutine(controller.ReleaseCoroutine());
            }

            loadSceneAsync.allowSceneActivation = true;
            yield return loadSceneAsync;

            controller = FindObjectOfType<ISceneController>();
            controller.ParamMapping = paramMapping;

            if (controller != null)
            {
                yield return StartCoroutine(controller.InitializeCoroutine());
            }

            if(enableMask)
            {
                MaskUtility.Instance.Hide(maskTime);
                yield return new WaitForSeconds(maskTime);
            }

            Debug.LogFormat("Load scene: {0} completed!", SceneManager.GetActiveScene().name);
        }
	}
}
