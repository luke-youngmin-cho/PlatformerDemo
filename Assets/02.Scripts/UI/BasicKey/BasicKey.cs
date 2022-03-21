using System;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Basic key base. ( keys for item view, skill view, pick up and so on...)
/// </summary>
public class BasicKey : MonoBehaviour
{
    public Sprite icon;
    public virtual void OnUse() { }
}
