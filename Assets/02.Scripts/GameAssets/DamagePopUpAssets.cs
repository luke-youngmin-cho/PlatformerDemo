using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamagePopUpAssets : MonoBehaviour
{
    private static DamagePopUpAssets _instance;
    public static DamagePopUpAssets instance
    {
        get
        {
            if (_instance == null)
                _instance = Instantiate(Resources.Load<DamagePopUpAssets>("Assets/DamagePopUpAssets"));
            return _instance;
        }
    }

    public Transform GetDamagePopUpTrasnformByLayer(int layer, bool isCriticalHit)
    {
        Transform tmpTransform = damagePopUp_Enemy_Basic;
        if (isCriticalHit)
            tmpTransform = damagePopUp_Enemy_Basic_Critical;

        if (layer == LayerMask.NameToLayer("Player"))
        {
            if (isCriticalHit)
                tmpTransform = damagePopUp_Player_Basic_Critical;
            else
                tmpTransform = damagePopUp_Player_Basic;
        }

        return tmpTransform;
    }
    public Transform damagePopUp_Enemy_Basic;
    public Transform damagePopUp_Enemy_Basic_Critical;
    public Transform damagePopUp_Player_Basic;
    public Transform damagePopUp_Player_Basic_Critical;
}
