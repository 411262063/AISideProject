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
    private Coroutine inUseRoutine;

    public enum UsingState
    {
        idle,
        inUse,
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

    public void StartUsingByAgent(AgentController agent)
    {
        if (usingState == UsingState.inUse) return;
        agentInUse = agent;
        usingState = UsingState.inUse;
        inUseRoutine = StartCoroutine(UsingProcess());
    }

    private IEnumerator UsingProcess()
    {
        Debug.Log(agentInUse.character.charNameChi + "正在使用" + objectData.objectNameChi);
        yield return new WaitForSeconds(agentInUse.character.objectUsageTime);
        Debug.Log(agentInUse.character.charNameChi + "結束使用" + objectData.objectNameChi);
        usingState = UsingState.idle;
        agentInUse.EndUsingByCurrentObj();
        agentInUse = null;
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
