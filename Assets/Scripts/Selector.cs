using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using TMPro;
namespace UnityEngine.XR.Interaction.Toolkit
{
    public class Selector : MonoBehaviour
    {

        private int order=0;
        private InputDevice targetDevice;
        private bool hasSelected = false;
        
        private Item selectedGameObject;

        [SerializeField]
        public InputDeviceCharacteristics inputDeviceType = InputDeviceCharacteristics.None;

        public InputDeviceCharacteristics controllerCharacteristics;
        private List<Item> itemRaycastAll;
        public TextMeshProUGUI itemRayCount;

        
        public GameObject canvasUI;
        private bool hasTriggered=false;
        private bool onTriggering=false;

        #region 初始化设备（by JiaLong Xu)
        void Start()
        {
            itemRaycastAll = new List<Item>();
            Initial();
        }
        void Initial()
        {
            List<InputDevice> devices = new List<InputDevice>();
            InputDevices.GetDevicesWithCharacteristics(inputDeviceType, devices);
            if (devices.Count > 0)
            {
                targetDevice = devices[0];

            }
        }
        #endregion
        void Update()
        {
            foreach (var item in itemRaycastAll)    //每帧初始时，清除上一帧的所有物体的状态
            {

                ExitHover(item);
            }
            #region 轮盘部分(by Zhengyang Zhu)

            if (targetDevice.TryGetFeatureValue(CommonUsages.trigger, out float triggerValue)
                && triggerValue > 0.9f && hasTriggered == false && itemRaycastAll.Count>1)
             #region 启用轮盘选中物体
            {
                onTriggering = true;
                canvasUI.SetActive(true);
                canvasUI.GetComponentInChildren<CircleUI>().uiCount = itemRaycastAll.Count;
                List<GameObject> buttonList = canvasUI.GetComponentInChildren<CircleUI>().buttonList;
                for (int i = 0; i < buttonList.Count; i++)
                {
                    if (buttonList[i].gameObject.GetComponent<ButtonHighlight>().isHighlighted == true)
                    {
                        itemRayCount.text = i.ToString();
                        order = i;
                        break;
                    }
                }
                for (int i = 0; i < buttonList.Count; i++)
                {
                    if (i == order)
                        EnterHover(itemRaycastAll[i]);
                    else
                        EnterHide(itemRaycastAll[i]);
                }

            }
            #endregion
            else if (targetDevice.TryGetFeatureValue(CommonUsages.trigger, out float triggerValue2) &&
                triggerValue2 == 0 && hasTriggered == false && onTriggering == true)
            #region 轮盘选择中
            {
                //需要更新物体对应的高亮效果
                itemRaycastAll[order].itemObject.transform.SetParent(gameObject.transform, true);
                itemRaycastAll[order].itemObject.GetComponent<Rigidbody>().useGravity = false;
                itemRaycastAll[0].itemObject.GetComponent<Rigidbody>().isKinematic = true;
                selectedGameObject = itemRaycastAll[order];

                ExitHover(selectedGameObject);
                canvasUI.SetActive(false);

                hasTriggered = true;
                onTriggering = false;
            }
            #endregion
            else if (targetDevice.TryGetFeatureValue(CommonUsages.trigger, out float triggerValue3) &&
               triggerValue3 == 0 && hasTriggered == true)
            #region 握持物体状态
            {
                GetComponent<XRInteractorLineVisual>().enabled = false;
            }
            #endregion
            else if (targetDevice.TryGetFeatureValue(CommonUsages.trigger, out float triggerValue4)
                && triggerValue4<0.5f &&triggerValue4 > 0.1f && hasTriggered == true )
            #region 轻轻触及Trigger松手，接触握持状态
            {
                GetComponent<XRInteractorLineVisual>().enabled = true;

                selectedGameObject.itemObject.transform.SetParent(null, true);
                selectedGameObject.itemObject.GetComponent<Rigidbody>().useGravity = true;
                itemRaycastAll[0].itemObject.GetComponent<Rigidbody>().isKinematic = false;
                selectedGameObject = null;
                hasTriggered = false;

            }
            #endregion

            #endregion
            #region 手柄旋钮部分(by Zhengyang Zhu,Jialong Xu)
            else if (targetDevice.TryGetFeatureValue(CommonUsages.trigger, out float triggerValue5)
                && triggerValue5 ==0 && hasTriggered==false)
            {
                //在没有启用UI轮盘的情况下，需要更新itemRaycastAll数组。
                #region 创建射线检测项目数组，排序，并显示项目数量（by Jialong Xu)
                RaycastHit[] hits = Physics.SphereCastAll(transform.position, 0.25f, transform.forward,
                    maxDistance: Mathf.Infinity, layerMask: LayerMask.GetMask("Grab"));
                //只检测Grab层的物体
                bubbleSortHits(ref hits);
                itemRaycastAll.Clear();

                for (int i = 0; i < hits.Length; i++)
                {
                    Item dummy = hits[i].transform.gameObject.GetComponent<Item>();
                    itemRaycastAll.Add(dummy);
                }
                itemRayCount.text = (order + 1).ToString() + "/" + itemRaycastAll.Count.ToString();
                #endregion
                if (itemRaycastAll.Count > 0 && itemRaycastAll.Count <= 1)
                {
                    #region 手柄旋钮选择第一个物体
                    if (!hasSelected) EnterHover(itemRaycastAll[0]);
                    //只有一个可选择目标
                    if (targetDevice.TryGetFeatureValue(CommonUsages.grip, out float gripValue))
                    {
                        if (gripValue > 0f)
                        {   //握住
                            if (hasSelected) return;
                            else
                            {
                                //没有选择
                                hasSelected = true;
                                itemRaycastAll[0].itemObject.transform.SetParent(gameObject.transform, true);
                                itemRaycastAll[0].itemObject.GetComponent<Rigidbody>().useGravity = false;
                                itemRaycastAll[0].itemObject.GetComponent<Rigidbody>().isKinematic = true;
                                selectedGameObject = itemRaycastAll[0];
                                ExitHover(selectedGameObject);
                            }
                        }
                        else
                        {
                            //松手
                            if (hasSelected) hasSelected = false;
                            selectedGameObject.itemObject.transform.SetParent(null, true);
                            selectedGameObject.itemObject.GetComponent<Rigidbody>().useGravity = true;
                            itemRaycastAll[0].itemObject.GetComponent<Rigidbody>().isKinematic = false;

                            selectedGameObject = null;
                        }
                    }
                    #endregion
                }
                else if (itemRaycastAll.Count > 1)
                {
                    #region 手柄旋钮选择多个物体
                    order = 0;
                    if (hasSelected == false)
                    {
                        if (targetDevice.TryGetFeatureValue(CommonUsages.primary2DAxis, out Vector2 primary2DAxis_Value)
                           && primary2DAxis_Value != Vector2.zero)
                        {
                            //启用轮滑
                            order = GetOrder(itemRaycastAll.Count, primary2DAxis_Value);
                        }
                        EnterHover(itemRaycastAll[order]);

                        for (int i = 0; i < itemRaycastAll.Count; i++)
                        {
                            if (i == order) continue;
                            EnterHide(itemRaycastAll[i]);
                        }
                    }

                    if (targetDevice.TryGetFeatureValue(CommonUsages.grip, out float gripValue))
                    {
                        if (gripValue > 0f)
                        {   //握住
                            if (hasSelected) return;
                            else
                            {
                                //没有选择
                                hasSelected = true;
                                itemRaycastAll[order].itemObject.transform.SetParent(gameObject.transform, true);
                                itemRaycastAll[order].itemObject.GetComponent<Rigidbody>().useGravity = false;
                                itemRaycastAll[0].itemObject.GetComponent<Rigidbody>().isKinematic = true;
                                selectedGameObject = itemRaycastAll[order];
                                ExitHover(selectedGameObject);

                            }
                        }
                        else
                        {
                            //松手
                            if (hasSelected) hasSelected = false;
                            selectedGameObject.itemObject.transform.SetParent(null, true);
                            selectedGameObject.itemObject.GetComponent<Rigidbody>().useGravity = true;
                            itemRaycastAll[0].itemObject.GetComponent<Rigidbody>().isKinematic = false;
                            selectedGameObject = null;
                        }
                    }
                    #endregion
                }
            }
            #endregion
        }
        void bubbleSortHits(ref RaycastHit[] hits)
        {
            for (int i = 0; i < hits.Length; i++)
            {
                for (int j = i; j < hits.Length - 1; j++)
                {
                    if (Vector3.Distance(hits[j].transform.position, Camera.main.transform.position) >
                        Vector3.Distance(hits[j + 1].transform.position, Camera.main.transform.position))
                    {
                        RaycastHit temp;
                        temp = hits[j + 1];
                        hits[j + 1] = hits[j];
                        hits[j] = temp;
                    }
                }
            }
        }
        #region 旋钮映射算法(by Runmin Ji)

        int GetOrder(int itemCount, Vector2 primary2DAxis_Value)
        {
            int tempOrder = 0;

            float x = primary2DAxis_Value.x;
            float y = primary2DAxis_Value.y;
            double angle = MathExtend.GetAtan(x, y);
            //Debug.Log(angle);

            double averageAngle = 360.0 / itemCount;

            double realangle = 90.0 - angle >= 0.0 ? 90.0 - angle : 90.0 - angle + 360.0;
            tempOrder = (int)(realangle / averageAngle);
            return tempOrder;
        }
        #endregion
        #region 高亮和隐藏效果(by Zhengyang Zhu)
        void EnterHover(Item item)
        {
            SetRenderMode.SetOpaque(item.itemObject);
            var material = item.itemObject.GetComponent<Renderer>().material;
            material.color = Color.red;
        }
        void ExitHover(Item item)
        {
            var material = item.itemObject.GetComponent<Renderer>().material;
           //SetRenderMode.SetOpaque(item.itemObject);
            material.color = item.originalColor;
        }
        void EnterHide(Item item)
        {
            var material= item.itemObject.GetComponent<Renderer>().material;
            SetRenderMode.SetTrans(item.itemObject);
            material.color*=new Color(1f,1f,1f,  0.2f);

        }

        #endregion


    }

    
}

