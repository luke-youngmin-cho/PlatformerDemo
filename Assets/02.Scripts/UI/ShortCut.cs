using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class ShortCut : MonoBehaviour
{
    public Image image;
    public KeyCode keyCode;
    public delegate void KeyEvent();
    public KeyEvent KE;

    private float delay = 0.5f;
    private Coroutine delayCoroutine;
    public void RegisterIconAndEvent(Sprite icon, KeyEvent keyEvent)
    {
        image.sprite = icon;
        image.color = Color.white;
        KE = keyEvent;
    }
    public void Clear()
    {
        image.sprite = null;
        image.color = Color.clear;
        KE = null;
    }
    public void TryKeyEvent()
    {
        if ((delayCoroutine == null) &&
            (KE != null))
        {
            delayCoroutine = StartCoroutine(E_Delay());
            KE();
        }
    }
    IEnumerator E_Delay()
    {
        yield return new WaitForSeconds(delay);
        delayCoroutine = null;
    }
}
