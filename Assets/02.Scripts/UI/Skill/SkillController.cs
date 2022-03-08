using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
// need to consider 
// BasicKey's function almost same with ShortCut , 
// But when it derives ShortCut, InventoryItem or Skill and so on also can put them on this. 
// if there have good way to prevent other short-cut relative items, 
// It would be nice way that BasicKey is derived from ShortCut
public class SkillController : MonoBehaviour, IPointerClickHandler
{
    public Skill skill;
    public Image image;
    // UI Raycast event
    GraphicRaycaster _Raycaster;
    PointerEventData _PointerEventData;
    EventSystem _EventSystem;
    PlayerStateMachineManager _StateMachineManager;
    private void Start()
    {
        _Raycaster = SkillManager.instance.transform.parent.GetComponent<GraphicRaycaster>();
        _EventSystem = FindObjectOfType<EventSystem>();
        StartCoroutine(E_Start());
    }
    IEnumerator E_Start()
    {
        yield return new WaitUntil(() =>
        {
            bool stateMachineManagerOK = false;
            Debug.Log(GameObject.FindWithTag("Player"));
            _StateMachineManager = GameObject.FindWithTag("Player").GetComponent<PlayerStateMachineManager>();
            if (_StateMachineManager != null)
                stateMachineManagerOK = true;
            return stateMachineManagerOK;
        });
        Debug.Log($"{_StateMachineManager} got it~~~~~~~~~~~");
    }
    public void OnEnable()
    {
        gameObject.GetComponent<Image>().sprite = skill.icon;
    }
    public void OnDisable()
    {
        skill = null;
        image = null;
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right)
            gameObject.SetActive(false);
        else if (eventData.button == PointerEventData.InputButton.Left)
        {
            _PointerEventData = new PointerEventData(_EventSystem);
            _PointerEventData.position = Input.mousePosition;
            List<RaycastResult> results = new List<RaycastResult>();
            _Raycaster.Raycast(_PointerEventData, results);

            ShortCut shortCut = null;
            ShortCutClone shortCutClone = null;
            CanvasRenderer canvasRenderer = null;
            foreach (RaycastResult result in results)
            {
                // Check ShortKey
                ShortCut tmpShortCut = null;
                if (result.gameObject.TryGetComponent(out tmpShortCut))
                {
                    shortCut = tmpShortCut;
                }
                // Check Shortcut clone
                ShortCutClone tmpShortCutClone = null;
                if (result.gameObject.TryGetComponent(out tmpShortCutClone))
                {
                    shortCutClone = tmpShortCutClone;
                }
                //Check All UI. (if not exist, drop item to field)
                CanvasRenderer tmpCanvasRenderer = null;
                if (result.gameObject.TryGetComponent<CanvasRenderer>(out tmpCanvasRenderer))
                {
                    if (tmpCanvasRenderer.gameObject != this.gameObject)
                        canvasRenderer = tmpCanvasRenderer;
                }
                Debug.Log(result.gameObject.name);
            }

            // Clicked on ShortcutClone
            if (shortCutClone != null)
                shortCut = shortCutClone.GetOrigin();

            // Clicked on Shortcut
            if (shortCut != null)
            {
                PlayerState state = skill.playerState;
                PlayerStateMachine tmpStateMachine = null;
                if (_StateMachineManager.TryGetStateMachineByState(state, out tmpStateMachine))
                {
                    tmpStateMachine.keyCode = shortCut._keyCode;
                    _StateMachineManager.RefreshMachineDictionaries();
                }

                shortCut.RegisterIconAndEvent(ShortCutType.Skill, skill.icon, 
                    delegate {_StateMachineManager.keyInput = shortCut._keyCode; Debug.Log($"{shortCut._keyCode} is In"); });

                gameObject.SetActive(false);
            }

            if (canvasRenderer == null)
                gameObject.SetActive(false);
            else
                Debug.Log(canvasRenderer.name);
        }
    }


}
