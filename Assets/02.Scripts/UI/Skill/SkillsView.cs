using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Presenter for interaction between skill <-> player skill list
/// </summary>
public class SkillsView : MonoBehaviour
{
    public static SkillsView instance;
    public bool isReady = false;    
    public GameObject skillInfoPrefab;
    public Transform content;
    public SkillHandler skillHandler;
    public List<GameObject> skillSlots = new List<GameObject>();


    //============================================================================
    //************************* Public Methods ***********************************
    //============================================================================

    public void RefreshSkillList()
    {
        foreach (var skill in skillSlots)
            Destroy(skill.gameObject);

        skillSlots.Clear();
        foreach (var skillStats in Player.instance.skillStatsList)
        {
            Skill tmpSkill = SkillAssets.instance.GetSkillByState(skillStats.state);
            if (tmpSkill.machineType != MachineType.BasicSkill)
            {
                GameObject tmpSkillInfo = Instantiate(skillInfoPrefab, content);
                tmpSkillInfo.transform.GetChild(0).GetComponent<Image>().sprite = tmpSkill.icon;
                tmpSkillInfo.transform.GetChild(0).GetChild(0).GetComponent<Button>().onClick.AddListener(() =>
                {
                    if (tmpSkill.machineType == MachineType.ActiveSkill)
                    {
                        skillHandler.skill = tmpSkill;
                        skillHandler.gameObject.SetActive(true);
                    }
                });
                tmpSkillInfo.transform.GetChild(1).GetComponent<Text>().text = tmpSkill.name;
                tmpSkillInfo.transform.GetChild(2).GetComponent<Text>().text = "Lv." + skillStats.level;
                tmpSkillInfo.transform.GetChild(3).GetComponent<Button>().onClick.AddListener(() =>
                {
                    Player.instance.SkillLevelUp(skillStats.state);
                });
                skillSlots.Add(tmpSkillInfo);
            }
        }
    }


    //============================================================================
    //************************* Private Methods **********************************
    //============================================================================

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        isReady = true;
        StartCoroutine(E_RefreshAtFirst());
    }

    IEnumerator E_RefreshAtFirst()
    {
        yield return new WaitUntil(() => DataManager.isApplied);
        RefreshSkillList();
    }

    private void Update()
    {
        if (skillHandler.gameObject.activeSelf)
        {
            Vector3 pos = Input.mousePosition;
            skillHandler.transform.position = pos;
        }
    }
}
