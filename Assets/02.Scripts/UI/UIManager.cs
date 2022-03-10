using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class UIManager : MonoBehaviour
{
    public static UIManager instance;
    public bool isReady = false;
    public GameObject shortCutView;
    public GameObject menuView;
    public GameObject itemsView;
    public GameObject skillsView;
    private void Awake()
    {
        instance = this;
        shortCutView.SetActive(true);
        menuView.SetActive(true);
        itemsView.SetActive(true);
        skillsView.SetActive(true);
        
    }
    private void Start()
    {
        StartCoroutine(E_Start());
    }
    IEnumerator E_Start()
    {
        menuView.SetActive(false);
        yield return new WaitUntil(() => shortCutView.GetComponentInParent<ShortCutsView>().isReady);
        shortCutView.SetActive(false);
        yield return new WaitUntil(() => itemsView.GetComponent<InventoryView>().isReady);
        itemsView.SetActive(false);
        yield return new WaitUntil(()=> skillsView.GetComponent<SkillsView>().isReady);
        skillsView.SetActive(false);
        isReady = true;
    }
    
}
