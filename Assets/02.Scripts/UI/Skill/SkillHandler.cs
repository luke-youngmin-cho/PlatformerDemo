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
/// <summary>
/// Handler for skills in sills view.
/// </summary>
public class SkillHandler : MonoBehaviour, IPointerClickHandler
{
    public Skill skill;
    public Image image;
    // UI Raycast event
    GraphicRaycaster _Raycaster;
    PointerEventData _PointerEventData;
    EventSystem _EventSystem;

    //============================================================================
    //************************* Public Methods ***********************************
    //============================================================================

    public void Clear()
    {
        transform.SetParent(SkillsView.instance.transform);
        skill = null;
        image = null;
        gameObject.SetActive(false);
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
                // reset replaced machine's keycode
                if (PlayerStateMachineManager.instance.TryGetStateMachineByKeyCode(shortCut._keyCode, out PlayerStateMachine oldStateMachine))
                {
                    if (ShortCutsView.instance.TryGetShortCut(oldStateMachine.keyCode, out ShortCut oldMachineShortCut))
                        oldMachineShortCut.Clear();
                    oldStateMachine.keyCode = KeyCode.None;
                }

                PlayerState state = skill.playerState;
                if (PlayerStateMachineManager.instance.TryGetStateMachineByState(state, out PlayerStateMachine newStateMachine))
                {
                    // if already another shortcut for the machine exist, reset it.
                    Debug.Log(newStateMachine.keyCode);
                    if (ShortCutsView.instance.TryGetShortCut(newStateMachine.keyCode, out ShortCut oldShortCut))
                        oldShortCut.Clear();

                    newStateMachine.keyCode = shortCut._keyCode;
                    Debug.Log(newStateMachine.keyCode);
                    PlayerStateMachineManager.instance.RefreshMachineDictionaries();
                }

                shortCut.RegisterIconAndEvent(ShortCutType.Skill, skill.icon,
                    delegate { PlayerStateMachineManager.instance.keyInput = shortCut._keyCode; });

                Clear();
            }

            if (canvasRenderer == null)
                gameObject.SetActive(false);
            else
                Debug.Log(canvasRenderer.name);
        }
    }


    //============================================================================
    //************************* Private Methods **********************************
    //============================================================================

    private void Start()
    {
        _Raycaster = SkillsView.instance.transform.parent.GetComponent<GraphicRaycaster>();
        _EventSystem = FindObjectOfType<EventSystem>();
    }

    private void OnEnable()
    {
        transform.SetParent(UIManager.instance.playerUI.transform);
        gameObject.GetComponent<Image>().sprite = skill.icon;
    }
}
