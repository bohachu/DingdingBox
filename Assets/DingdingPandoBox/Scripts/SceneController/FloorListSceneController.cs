using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cameo.PandoBox
{
    public class FloorListSceneController : ISceneController
    {
        [SerializeField]
        private FadePageAnimator fadePageAnimator;

        [SerializeField]
        private RectTransform contentRectTrans;

        [SerializeField]
        private GameObject floorControllerPrefab;


        class PandoBoxInfoList
        {
            public int Weights = 0;
            public List<PandoBoxInfo> pandoBoxInfos = new List<PandoBoxInfo>();

            public PandoBoxInfoList(PandoBoxInfo info)
            {
                Weights = info.weights;
                pandoBoxInfos.Add(info);
            }
        }

        public override IEnumerator InitializeCoroutine()
        {
            List<PandoBoxInfoList> pandoBoxInfos = new List<PandoBoxInfoList>();

            for (int i = 0; i < DataCenter.Instance.InfoList.Count; ++i)
            {
                PandoBoxInfo newInfo = DataCenter.Instance.InfoList[i];

                if(pandoBoxInfos.Count == 0)
                {
                    PandoBoxInfoList pandoBoxInfoList = new PandoBoxInfoList(newInfo);
                    pandoBoxInfos.Add(pandoBoxInfoList);
                }
                else
                {
                    for (int j = 0; j < pandoBoxInfos.Count; ++j)
                    {
                        if(newInfo.weights == pandoBoxInfos[j].Weights)
                        {
                            pandoBoxInfos[j].pandoBoxInfos.Add(newInfo);
                        }
                        else
                        {
                            if (j > 0 && j < pandoBoxInfos.Count - 1)
                            {
                                if(newInfo.weights > pandoBoxInfos[j-1].Weights && newInfo.weights < pandoBoxInfos[j+1].Weights)
                                {
                                    PandoBoxInfoList pandoBoxInfoList = new PandoBoxInfoList(newInfo);
                                    pandoBoxInfos.Insert(j+1, pandoBoxInfoList);
                                    break;
                                }
                            }
                            else
                            {
                                if (j == 0 && newInfo.weights < pandoBoxInfos[0].Weights)
                                {
                                    PandoBoxInfoList pandoBoxInfoList = new PandoBoxInfoList(newInfo);
                                    pandoBoxInfos.Insert(0, pandoBoxInfoList);
                                    break;
                                }

                                if (j == pandoBoxInfos.Count - 1 && newInfo.weights > pandoBoxInfos[0].Weights)
                                {
                                    PandoBoxInfoList pandoBoxInfoList = new PandoBoxInfoList(newInfo);
                                    pandoBoxInfos.Add(pandoBoxInfoList);
                                    break;
                                }
                            }
                        }
                    }
                }
            }

            for (int i = 0; i < pandoBoxInfos.Count; ++i)
            {
                GameObject floorControllerObj = Instantiate(floorControllerPrefab) as GameObject;
                FloorController floorController = floorControllerObj.GetComponent<FloorController>();
                floorController.Init(pandoBoxInfos[i].pandoBoxInfos);
                floorController.transform.SetParent(contentRectTrans, false);
            }

            yield return StartCoroutine(fadePageAnimator.FadeInCoroutine());
            yield return null;
        }

        public override IEnumerator ReleaseCoroutine()
        {
            
            yield return StartCoroutine(fadePageAnimator.FadeOutCoroutine());
            yield return null;
        }

    }

}
