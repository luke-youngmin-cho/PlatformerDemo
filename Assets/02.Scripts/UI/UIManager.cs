using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;
    private void Awake()
    {
        instance = this;
    }
    public GameObject menuView;
    public GameObject itemsView;
    public GameObject skillsView;
}
