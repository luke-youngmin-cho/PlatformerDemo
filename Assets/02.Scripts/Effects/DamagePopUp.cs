using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class DamagePopUp : MonoBehaviour
{
    public static DamagePopUp Create(Vector3 pos, int damage, int layer, bool isCriticalHit)
    {
        Transform damagePopUpTransform = Instantiate(DamagePopUpAssets.instance.GetDamagePopUpTrasnformByLayer(layer,isCriticalHit), 
                                                     pos, Quaternion.identity);
        DamagePopUp damagePopUp = damagePopUpTransform.GetComponent<DamagePopUp>();
        damagePopUp.SetUp(damage);
        return damagePopUp;
    }

    private TextMeshPro textMesh;
    private float disapperTimer = 0.5f;
    private float moveSpeedY = 0.5f;
    private float disappearSpeed = 3f;
    private Color textColor;
    private Transform tr;
    private void Awake()
    {
        tr = GetComponent<Transform>();
        textMesh = tr.GetComponent<TextMeshPro>();
    }
    private void Update()
    {
        tr.position += new Vector3(0f,moveSpeedY * Time.deltaTime,0f);

        if (disapperTimer < 0)
        {
            textColor.a -= disappearSpeed * Time.deltaTime;
            textMesh.color = textColor;
            if (textColor.a < 0)
                Destroy(gameObject);
        }
        else
        {
            disapperTimer -= Time.deltaTime;
        }    
    }
    public void SetUp(int damage)
    {
        textMesh.SetText(damage.ToString());
    }
}
