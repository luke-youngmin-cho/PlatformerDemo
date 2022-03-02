using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class PlayerSelection : MonoBehaviour
{
    public static PlayerSelection instance;

    public string selectedNickName;
    public Transform content;
    private Transform previewOrigin;
    List<GameObject> previewList  = new List<GameObject>();
    public InputField createPlayerInputField;
    public GameObject selectedEffect;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        previewOrigin = content.GetChild(1);
    }
    private void Start()
    {
        StartCoroutine(E_Start());
    }
    IEnumerator E_Start()
    {
        yield return new WaitUntil(() => PlayerDataManager.instance.isLoaded);
        DrawPlayerDatasUI(PlayerDataManager.instance.playerDatas);
    }
    public void DrawPlayerDatasUI(PlayerData[] datas)
    {
        for (int i = 0; i < previewList.Count; i++)
        {
            Destroy(previewList[i]);
        }
        previewList.Clear();

        for (int i = 0; i < datas.Length; i++)
        {
            Transform tr = Instantiate(previewOrigin, content);
            tr.GetChild(0).GetComponent<Text>().text = datas[i].nickName;
            tr.GetChild(1).GetComponent<Text>().text = "Lv." + datas[i].stats.Level;
            //tr.GetChild(2).GetComponent<RawImage>().texture = // preview texture
            tr.gameObject.SetActive(true);
            previewList.Add(tr.gameObject);
        }
    }
    public void DrawPlayerDataUI(PlayerData data)
    {
        Transform tr = Instantiate(previewOrigin, content);
        tr.GetChild(0).GetComponent<Text>().text = data.nickName;
        tr.GetChild(1).GetComponent<Text>().text = "Lv." + data.stats.Level;
        //tr.GetChild(2).GetComponent<RawImage>().texture = // preview texture
        tr.gameObject.SetActive(true);
    }
    public void SelectPlayer(Text nickNameText)
    {
        selectedNickName = nickNameText.text;
    }
    public void SelectEffectTo(Transform here)
    {
        selectedEffect.SetActive(true);
        selectedEffect.transform.position = here.position;
    }
    public void CreatePlayer()
    {
        createPlayerInputField.transform.parent.gameObject.SetActive(true);
    }
    public void OnCreatePlayerInputFieldOKButton()
    {
        PlayerDataManager.instance.CreatePlayerData(createPlayerInputField.text);
        createPlayerInputField.transform.parent.gameObject.SetActive(false);
        createPlayerInputField.text = "";
        DrawPlayerDataUI(PlayerDataManager.instance.currentPlayerData);
    }
    public void OnPlayButton()
    {
        PlayerDataManager.instance.LoadPlayerData(selectedNickName);
        SceneManager.LoadScene("GamePlay");
    }
    public void OnDeleteButton()
    {
        PlayerDataManager.instance.RemovePlayerData(selectedNickName);
        selectedEffect.SetActive(false);
        DrawPlayerDatasUI(PlayerDataManager.instance.playerDatas);
    }
}
