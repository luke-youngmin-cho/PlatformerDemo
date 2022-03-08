
using UnityEngine;

[CreateAssetMenu(fileName = "New Skill", menuName = "Skill/Create New Skill")]
public class Skill : ScriptableObject
{
    public PlayerState playerState;
    public MachineType machineType;
    public Sprite icon;
}
