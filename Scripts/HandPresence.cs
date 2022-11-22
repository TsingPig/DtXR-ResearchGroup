using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
public class HandPresence : MonoBehaviour
{

    public bool showController; //显示控制器还是手模型

    private InputDevice targetDevice;
    public InputDeviceCharacteristics controllerChararistics;   //控制器特征 

    public List<GameObject> controllerPrefabs;  //控制器预制列表
    GameObject spawnedController;   //生成的控制器对象
    public GameObject spawnedHand;         //生成的手的对象
    Animator handAnimator;

    public Transform XR_Rig_transform;  //控制人物移动
    public float speed = 5f;
    private void Awake()
    {
        handAnimator = spawnedHand.GetComponent<Animator>();
    }
    void Start()
    {
        InitDevices();

    }
    void Update()
    {
        if (targetDevice.isValid == false)
        {
            InitDevices();
        }
        spawnedController.SetActive(showController);
        spawnedHand.SetActive(!showController);

        HandAnimator_Update();
        Move_Update();
        //handAnimator.SetFloat("Trigger", triggerValue);
        //trigger是前面的触发器，primaryButton是X/A

        //if (targetDevice.TryGetFeatureValue(CommonUsages.primaryButton, out bool primaryButton_Value) &&
        //    primaryButton_Value)   //测试控制器是否有效、并且按键被触发
        //{

        //}
        //if (targetDevice.TryGetFeatureValue(CommonUsages.trigger, out float trigger_Value) && trigger_Value > 0.1f)
        //{
        //    triggerValue为前置触发器被触发的程度
        //}

       

      
    } 
    void InitDevices()
    {
        
        List<InputDevice> devices = new List<InputDevice>();    //创建一个设备列表用于
                                                                //在GetDevicesWithCharacteristis(设备Int串，设备列表)中，存放索引到的所有设备。
        InputDevices.GetDevicesWithCharacteristics(controllerChararistics, devices);
        //    InputDeviceCharacteristics rightControllerDevice_Enum =
        //            InputDeviceCharacteristics.Right | InputDeviceCharacteristics.Controller;
        //    //运用位运算混合了IDC.R和IDC.C两个枚举的int位串，得到右手控制器（是不是&)
        //    InputDevices.GetDevicesWithCharacteristics(rightControllerDevice_Enum, devices);
        //    //检索所有对应设备串的设备，写入devices列表


        if (devices.Count > 0)
        {
            targetDevice = devices[0];
            #region List.Find()用法
            //Note:List.Find()用法
            /*
             List.Find()用法
                public T Find(Predicate<T> match);
                其中Predicate为C#定义好的委托，原型如下：
                public delegate bool Predicate<in T>(T obj);
                Person p1 = lstPerson.Find(pre);//1、命名函数
                Person p2 = lstPerson.Find(delegate (Person s) { return s.Name.Equals("王五"); });//2、匿名函数
                Person p3 = lstPerson.Find(s => { return s.Name.Equals("赵六"); });//3、Lambda表达式
              Person p4 = lstPerson.Find(s => s.Name.Equals("赵六"));//4、Lambda表达式的简洁写法
             */
            #endregion
            GameObject prefab =controllerPrefabs.Find(x => x.name == targetDevice.name);
            if(prefab != null)
            {
                spawnedController=Instantiate(prefab,transform);
            }
            else
            {
                Debug.Log("没有匹配的控制器生成！");
                spawnedController = Instantiate(controllerPrefabs[0],transform);   //使用默认控制器
            }
            
        }

    }
    void HandAnimator_Update()
    {
       

        if (targetDevice.TryGetFeatureValue(CommonUsages.trigger, out float triggerValue)
            && triggerValue > 0.1f)
        {
            handAnimator.SetFloat("Trigger",triggerValue);
        }
        else
        {
            handAnimator.SetFloat("Trigger", 0);
        }

        if (targetDevice.TryGetFeatureValue(CommonUsages.grip, out float gripValue))
        {
            //handAnimator.SetFloat("Grip", gripValue);
            //Grip是抓住物体的trigger
            handAnimator.SetFloat("Grip",gripValue);
        }
        else
        {
            handAnimator.SetFloat("Grip", 0);
        }
    }
    void Move_Update()
    {
        //if (targetDevice.TryGetFeatureValue(CommonUsages.primaryButton, out bool primaryButtonValue)
        //    && primaryButtonValue)
        //{

        //}
        
        if (targetDevice.TryGetFeatureValue(CommonUsages.primary2DAxis, out Vector2 primary2DAxis_Value))
        {
            if (primary2DAxis_Value != Vector2.zero)
            {
                XR_Rig_transform.position += new Vector3(primary2DAxis_Value.x * speed * Time.deltaTime, 0,
                    primary2DAxis_Value.y * speed * Time.deltaTime);
            }

        }
        if (targetDevice.TryGetFeatureValue(CommonUsages.primaryButton, out bool primaryButton_Value))
        {
            if (primaryButton_Value)
            {
                XR_Rig_transform.position += new Vector3(0, 0.5f*speed * Time.deltaTime, 0);
            }
        }
        if (targetDevice.TryGetFeatureValue(CommonUsages.secondaryButton, out bool secondaryButton_Value))
        {
            if (secondaryButton_Value)
            {
                XR_Rig_transform.position += new Vector3(0, (-0.5f)*speed * Time.deltaTime, 0);
            }
        }

    }
}
