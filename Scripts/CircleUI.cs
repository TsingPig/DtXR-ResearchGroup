using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class CircleUI : MonoBehaviour
{
    public int uiCount; int pre_uiCount;
    public float range;

    float pi=Mathf.PI;
    public GameObject buttonPrefab;
    public List<GameObject> buttonList;

    public Vector2 getPoint(float angle)
    {
        return new Vector2(Mathf.Cos(angle) * range,Mathf.Sin(angle) * range);
    }
    public void GenerateButton(int uiButtonCount)
    {
        if (buttonList.Count > 0) { return;}
        for(float i = 0; i < uiButtonCount; i++)
        {
            print(i);
            GameObject button = Instantiate(buttonPrefab, transform.position, Quaternion.identity, parent: gameObject.transform);

            #region 旋转Button，分割对应的按钮(by Runmin Ji）
            RectTransform rectTransform = button.GetComponent<RectTransform>();
            button.GetComponent<Image>().fillAmount = 0.99f / uiButtonCount;
            rectTransform.Rotate(0, 0, -150f + 360f * (i / uiButtonCount));
            rectTransform.Translate(getPoint(2 * pi * ((i + 0.5f) / uiButtonCount)), Space.World);
            #endregion

            #region 环扇近似算法(by Zhengyang Zhu)
            float R0 = 28.9f;
            float R = 50f;
            float theta = 2 * pi * (1f / uiButtonCount);

            float left, top;
            if (theta >= pi / 2)
            {
                left = 0;
                top = R * (1 - Mathf.Cos(pi - theta));
            }
            else
            {
                left = R - Mathf.Tan(theta) * (R - R0);
                top = R + (R - R0) * (R - R0) / R;
            }
            button.GetComponent<Image>().raycastPadding = new Vector4(left,0,50,top);
            #endregion

            buttonList.Add(button);
        }
    }
    private void Start()
    {
        pre_uiCount = 0;
    }
    private void Update()
    {
        
        GenerateButton(uiCount);
        if (pre_uiCount != uiCount)
        {
            foreach(var button in buttonList)
            {
                Destroy(button.gameObject);
            }
            buttonList.Clear();
            pre_uiCount = uiCount;
        }
    }
}
