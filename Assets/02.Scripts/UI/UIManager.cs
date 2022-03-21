using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// managing UIs don't destroy on load.
/// </summary>
public class UIManager : MonoBehaviour
{
    public static UIManager instance;
    public bool isReady = false;
    public GameObject playerUI;
    public GameObject shortCutView;
    public GameObject menuView;
    public GameObject inventoryView;
    public GameObject skillsView;
    public GameObject statsView;
    public GameObject equipmentsView;
    public GameObject characterInfoView;


    //============================================================================
    //************************* Private Methods **********************************
    //============================================================================

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(instance);
        }
        shortCutView.SetActive(true);
        menuView.SetActive(true);
        inventoryView.SetActive(true);
        skillsView.SetActive(true);
        statsView.SetActive(true);
        equipmentsView.SetActive(true);
        characterInfoView.SetActive(true);
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
        yield return new WaitUntil(() => inventoryView.GetComponent<InventoryView>().isReady);
        inventoryView.SetActive(false);
        yield return new WaitUntil(()=> skillsView.GetComponent<SkillsView>().isReady);
        skillsView.SetActive(false);

        statsView.SetActive(false);
        equipmentsView.SetActive(false);
        characterInfoView.SetActive(false);
        isReady = true;
    }
    
}
