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
            currentChatContent.AddRange(File.ReadLines(filePath));
            //ReadLines根據換行符號形成陣列
        }
    }

    private bool CheckEndingPointOfChatting(string line)
    {
        return line.Contains(END_CONVERSATION_MARKER);
    }

    public IEnumerator ManageConversation(AgentController agentA, AgentController agentB)
    {
        if ((agentA.character.chatIntent < 10 && agentB.character.chatIntent < 10) ||
             agentA.currentAction == AgentController.ActionState.chatting ||
             agentB.currentAction == AgentController.ActionState.chatting)
        {
            Debug.Log(agentA.character.charNameChi + "和" + agentB.character.charNameChi + "聊天意願過低，或其中一人正在說話");
            yield break;
        }

        //聊天意願高的當speaker
        AgentController speaker = agentA.character.chatIntent >= agentB.character.chatIntent ? agentA : agentB;
        AgentController listener = (speaker == agentA) ? agentB : agentA;
        speaker.SetActionState(AgentController.ActionState.chatting);
        listener.SetActionState(AgentController.ActionState.chatting);
        bool isSpeakerTurn = true;

        LoadChatContentFromFile(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "_Meee/AISideProject/ConversationTest.txt"));
        int index = 0;

        while (index < currentChatContent.Count)
        {
            if (CheckEndingPointOfChatting(currentChatContent[index])) break;
            
            if (isSpeakerTurn)
            {
                speaker.Speak(currentChatContent[index]);
            }
            else
            {
                listener.Speak(currentChatContent[index]);
            }
            isSpeakerTurn = !isSpeakerTurn;
            index++;
            yield return new WaitForSeconds(2f);
        }

        //聊天結束
        speaker.EndSpeaking();
        listener.EndSpeaking();
    }
}
