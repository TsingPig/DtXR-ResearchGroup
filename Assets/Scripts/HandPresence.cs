using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
public class HandPresence : MonoBehaviour
{

    public bool showController; //��ʾ������������ģ��

    private InputDevice targetDevice;
    public InputDeviceCharacteristics controllerChararistics;   //���������� 

    public List<GameObject> controllerPrefabs;  //������Ԥ���б�
    GameObject spawnedController;   //���ɵĿ���������
    public GameObject spawnedHand;         //���ɵ��ֵĶ���
    Animator handAnimator;

    public Transform XR_Rig_transform;  //���������ƶ�
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
        //trigger��ǰ��Ĵ�������primaryButton��X/A

        //if (targetDevice.TryGetFeatureValue(CommonUsages.primaryButton, out bool primaryButton_Value) &&
        //    primaryButton_Value)   //���Կ������Ƿ���Ч�����Ұ���������
        //{

        //}
        //if (targetDevice.TryGetFeatureValue(CommonUsages.trigger, out float trigger_Value) && trigger_Value > 0.1f)
        //{
        //    triggerValueΪǰ�ô������������ĳ̶�
        //}

       

      
    } 
    void InitDevices()
    {
        
        List<InputDevice> devices = new List<InputDevice>();    //����һ���豸�б�����
                                                                //��GetDevicesWithCharacteristis(�豸Int�����豸�б�)�У�����������������豸��
        InputDevices.GetDevicesWithCharacteristics(controllerChararistics, devices);
        //    InputDeviceCharacteristics rightControllerDevice_Enum =
        //            InputDeviceCharacteristics.Right | InputDeviceCharacteristics.Controller;
        //    //����λ��������IDC.R��IDC.C����ö�ٵ�intλ�����õ����ֿ��������ǲ���&)
        //    InputDevices.GetDevicesWithCharacteristics(rightControllerDevice_Enum, devices);
        //    //�������ж�Ӧ�豸�����豸��д��devices�б�


        if (devices.Count > 0)
        {
            targetDevice = devices[0];
            #region List.Find()�÷�
            //Note:List.Find()�÷�
            /*
             List.Find()�÷�
                public T Find(Predicate<T> match);
                ����PredicateΪC#����õ�ί�У�ԭ�����£�
                public delegate bool Predicate<in T>(T obj);
                Person p1 = lstPerson.Find(pre);//1����������
                Person p2 = lstPerson.Find(delegate (Person s) { return s.Name.Equals("����"); });//2����������
                Person p3 = lstPerson.Find(s => { return s.Name.Equals("����"); });//3��Lambda���ʽ
              Person p4 = lstPerson.Find(s => s.Name.Equals("����"));//4��Lambda���ʽ�ļ��д��
             */
            #endregion
            GameObject prefab =controllerPrefabs.Find(x => x.name == targetDevice.name);
            if(prefab != null)
            {
                spawnedController=Instantiate(prefab,transform);
            }
            else
            {
                Debug.Log("û��ƥ��Ŀ��������ɣ�");
                spawnedController = Instantiate(controllerPrefabs[0],transform);   //ʹ��Ĭ�Ͽ�����
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
            //Grip��ץס�����trigger
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
