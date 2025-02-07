using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.TextCore.Text;

public class UsableObjectController : MonoBehaviour
{
    public UsableObjectData objectData;
    public GameObject hintPanel;
    public TextMeshProUGUI hintText;
    public AgentController agentInUse;

    public enum UsingState
    {
        idle,
        inUse,
        inCoolDown,
    }
    public UsingState usingState;

    private void Start()
    {
        HideHintPanel();
    }

    public bool CanBeUse()
    {
        return usingState == UsingState.idle;
    }

    public void UsingByAgent(AgentController agent)
    {
        agentInUse = agent;
        usingState = UsingState.inUse;
        StartCoroutine(AgentUsingProcess());
    }

    private IEnumerator AgentUsingProcess()
    {
        Debug.Log(agentInUse.character.charNameChi + " 正在使用 " + objectData.objectNameChi);
        yield return new WaitForSeconds(agentInUse.character.objectUsageTime);
        Debug.Log(agentInUse.character.charNameChi + " 結束使用 " + objectData.objectNameChi);
        agentInUse.EndUsingCurrentObject();
        agentInUse = null;
        usingState = UsingState.inCoolDown;
        yield return new WaitForSeconds(objectData.coolDown);
        usingState = UsingState.idle;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log(collision.gameObject.name);
        if (collision.gameObject.CompareTag("Player"))
        {
            ShowHintPanel();
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        HideHintPanel();
    }

    private void ShowHintPanel()
    {
        hintPanel?.SetActive(true);
    }

    private void HideHintPanel()
    {
        hintPanel?.SetActive(false);
    }
}
