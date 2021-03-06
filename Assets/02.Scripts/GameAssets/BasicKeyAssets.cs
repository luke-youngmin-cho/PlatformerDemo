using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicKeyAssets : MonoBehaviour
{
    private static BasicKeyAssets _instance;
    public static BasicKeyAssets instance
    {
        get
        {
            if (_instance == null)
                _instance = Instantiate(Resources.Load<BasicKeyAssets>("Assets/BasicKeyAssets"));
            return _instance;
        }
    }
    public List<BasicKey> basicKeys = new List<BasicKey>();

    public BasicKey GetBasicKeyBySpriteName(string name)
    {
        BasicKey tmpBasicKey = null;
        foreach (var basicKey in basicKeys)
        {
            if (basicKey.icon.name == name)
               tmpBasicKey = basicKey;
        }
        return tmpBasicKey;
    }
}
