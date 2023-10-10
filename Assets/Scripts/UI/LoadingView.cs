using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UnityMMO
{
    
public class LoadingView : MonoBehaviour
{
    public static LoadingView Instance;
    Slider proBar;
    Text tip;
    float curPercent;
    float nextPercent;
    float playSpeed = 0.4f;
    const float maxProWidth = 1062;
    
    void Start()
    {
        Instance = this;
        proBar = transform.Find("Slider").GetComponent<Slider>();
            //butterfly = transform.Find("butterfly") as RectTransform;
        tip = transform.Find("tip").GetComponent<Text>();
        ResetData();
    }

    public void ResetData()
    {
        curPercent = 0;
        nextPercent = 0;
        playSpeed = 0;
        proBar.value = 0;
 
    }

    public void SetPlaySpeed(float speed)
    {
        playSpeed = speed;
    }

    public void SetActive(bool isShow, float delayTime=0.0f)
    {
        CancelInvoke("Show");
        CancelInvoke("Hide");
        if (delayTime <= 0)
        {
            gameObject.SetActive(isShow);
            if (isShow)
                transform.SetAsLastSibling();
        }
        else
        {
            if (isShow)
                Invoke("Show", delayTime);
            else
                Invoke("Hide", delayTime);
        }
    }

    public void Show()
    {
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    public void SetData(float percent, string tipStr)
    {
        nextPercent = Mathf.Clamp01(percent);
        playSpeed = Mathf.Clamp(nextPercent-curPercent, 0.3f, 1);
        // Debug.Log("loading percent : "+percent+" tips:"+tipStr+" playSpeed:"+playSpeed+" "+curPercent+" track:"+ new System.Diagnostics.StackTrace().ToString());
        tip.text = tipStr;
        //proBar.value = nextPercent;
        }

        void Update()
        {
            if (curPercent == nextPercent)
                return;
            float newPercent = curPercent + playSpeed * Time.deltaTime;
            // Debug.Log("newPercent : "+newPercent+" cur:"+curPercent+" speed:"+playSpeed+" clamp:"+Mathf.Clamp(newPercent, 0, nextPercent));
            newPercent = Mathf.Clamp(newPercent, 0, nextPercent);
            curPercent = newPercent;
            proBar.value = curPercent;
        }
    }

}
