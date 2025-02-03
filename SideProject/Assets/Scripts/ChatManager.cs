using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

public class ChatManager : MonoBehaviour
{
    public static ChatManager Instance;
    public float chatTriggerDistance = 3f;
    public int lowChatIntent = 10;
    public List<string> currentChatContent = new List<string>();
    public static string tempConvRecord;
    public string END_CONVERSATION_MARKER = "END_CONVERSATION_MARKER";

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void LoadChatContentFromFile(string filePath)
    {
        currentChatContent.Clear();
        if (File.Exists(filePath))
        {
            currentChatContent.AddRange(File.ReadLines(filePath)); //ReadLines根據換行符號形成陣列
        }
    }

    private bool CheckEndingPointOfChatting(string line)
    {
        return line.Contains(END_CONVERSATION_MARKER);
    }

    public void StartConversation(AgentController agentA, AgentController agentB)
    {
        if ((agentA.character.chatIntent < 10 && agentB.character.chatIntent < 10) ||
            (agentA.currentAction == AgentController.ActionState.chatting || agentB.currentAction == AgentController.ActionState.chatting))
        {
            return;
        }

        agentA.character.AddRelationship(agentB.character);
        agentB.character.AddRelationship(agentA.character);

        AgentController activeAgent = agentA.character.chatIntent >= agentB.character.chatIntent ? agentA : agentB;
        AgentController passiveAgent = (activeAgent == agentA) ? agentB : agentA;
        activeAgent.SetActionState(AgentController.ActionState.chatting);
        activeAgent.SetMovementState(AgentController.MovementState.none);
        passiveAgent.SetActionState(AgentController.ActionState.chatting);
        passiveAgent.SetMovementState(AgentController.MovementState.none);
       
        LoadChatContentFromFile(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "_Meee/AISideProject/ConversationTest.txt"));
        StartCoroutine(InConversation(activeAgent, passiveAgent));
    }

    private IEnumerator InConversation(AgentController activeSpeaker, AgentController passiveSpeaker)
    {
        int index = 0;
        bool isActive = true;

        while (index< currentChatContent.Count)
        {
            string currentLine = currentChatContent[index];
            if (CheckEndingPointOfChatting(currentLine)) break;
            //AgentController currentSpeaker = isActive ? activeSpeaker : passiveSpeaker;
            //currentSpeaker.Speak(currentLine);
            if (isActive) activeSpeaker.Speak(currentLine);
            else passiveSpeaker.Respond(currentLine);
            yield return new WaitForSeconds(2f);
            isActive = !isActive;
            index++;
        }

        activeSpeaker.EndSpeaking();
        passiveSpeaker.EndSpeaking();
    }
}
