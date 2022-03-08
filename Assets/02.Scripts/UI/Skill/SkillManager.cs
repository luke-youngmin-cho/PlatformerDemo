using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class SkillManager : MonoBehaviour
{
    public static SkillManager instance;
    public bool isReady = false;
    
    public GameObject skillInfoPrefab;
    public Transform content;

    public SkillController skillController;

    public List<GameObject> skillSlots = new List<GameObject>();
    private Player player;
    private void Awake()
    {
        instance = this;
    }
    private void Start()
    {
        StartCoroutine(E_Start());
    }
    IEnumerator E_Start()
    {
        yield return new WaitUntil(() =>
        {
            player = GameObject.FindWithTag("Player").GetComponent<Player>();
            if (player != null)
                return true;
            else
                return false;
        });
        Debug.Log(player);
        RefreshSkillList();
        isReady = true;
    }
    
    private void Update()
    {
        if (skillController.gameObject.activeSelf)
        {
            Vector3 pos = Input.mousePosition;
            skillController.transform.position = pos;
        }
    }
    private void RefreshSkillList()
    {
        foreach (var skill in skillSlots)
            Destroy(skill.gameObject);

        skillSlots.Clear();
        foreach (var skillStats in player.skillStatsList)
        {
            Skill tmpSkill = SkillAssets.instance.GetSkillByState(skillStats.state);
            if (tmpSkill.machineType != MachineType.BasicSkill)
            {
                GameObject tmpSkillInfo = Instantiate(skillInfoPrefab, content);
                tmpSkillInfo.transform.GetChild(0).GetComponent<Image>().sprite = tmpSkill.icon;
                tmpSkillInfo.transform.GetChild(0).GetChild(0).GetComponent<Button>().onClick.AddListener(() =>
                {
                    if(tmpSkill.machineType == MachineType.ActiveSkill)
                    {
                        skillController.skill = tmpSkill;
                        skillController.gameObject.SetActive(true);
                    }
                });
                tmpSkillInfo.transform.GetChild(1).GetComponent<Text>().text = tmpSkill.name;
                tmpSkillInfo.transform.GetChild(2).GetComponent<Text>().text = "Lv." + skillStats.level;
                tmpSkillInfo.transform.GetChild(3).GetComponent<Button>().onClick.AddListener(() =>
                {
                    player.SkillLevelUp(skillStats.state);
                });
                skillSlots.Add(tmpSkillInfo);
            }
        }
    }

}
