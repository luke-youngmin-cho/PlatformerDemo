using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fade : MonoBehaviour
{
    public float fadeSpeed;
    public bool fadeIn = true;
    public CanvasGroup canvasGroup;
    public bool isFinished = false;
    private void OnEnable()
    {
        if (fadeIn)
        {
            canvasGroup.alpha = 0f;
        }
        else
        {
            canvasGroup.alpha = 1f;
        }
        StartCoroutine(E_Fade());
    }
    IEnumerator E_Fade()
    {
        while(isFinished == false)
        {
            if (fadeIn)
            {
                canvasGroup.alpha += fadeSpeed * Time.deltaTime;
                if(canvasGroup.alpha >= 1f)
                    isFinished = true;
            }
            else
            {
                canvasGroup.alpha -= fadeSpeed * Time.deltaTime;
                if(canvasGroup.alpha <= 0f)
                    isFinished =true;
            }
            yield return null;
        }
    }
}
