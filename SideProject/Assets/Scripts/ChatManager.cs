using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

public class ChatManager : MonoBehaviour
{
    public static ChatManager Instance;
    [Header("觸發對話最近距離")]
    public float chatTriggerDistance = 3f;

    [Header("對話意願低標")]
    public int minChatIntent = 10;

    [Header("當前對話內容(暫時由Txt匯入)")]
    public List<string> currentChatContent = new List<string>();

    [Header("對話中斷判斷")]
    public string END_CONVERSATION_MARKER = "END_CONVERSATION_MARKER";

    [Header("全局對話紀錄")]
    public static string GolbalConvRecord;
    

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
        if (!(agentA.CanStartNewChat() && agentB.CanStartNewChat())) return;

        AgentController activeAgent = agentA.character.chatIntent >= agentB.character.chatIntent ? agentA : agentB;
        AgentController passiveAgent = (activeAgent == agentA) ? agentB : agentA;
        activeAgent.Chatting();
        passiveAgent.Chatting();

        LoadChatContentFromFile(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "_Meee/AISideProject/ConversationTest.txt"));
        StartCoroutine(ConversationProcess(activeAgent, passiveAgent));
    }

    private IEnumerator ConversationProcess(AgentController activeSpeaker, AgentController passiveSpeaker)
    {
        int index = 0;
        bool isActive = true;
        string tempConvRecord = "";

        //聊天開始  => 存[人名1, 人名2]，開始時間到tempConvRecord
        tempConvRecord += "[" + activeSpeaker.character.charNameChi + "," + passiveSpeaker.character.charNameChi + "] 於 " + "" + " 開始聊天\n";

        while (index< currentChatContent.Count)
        {
            string currentLine = currentChatContent[index]; //delete after
            if (CheckEndingPointOfChatting(currentLine)) break; //delete after
    
            AgentController currentSpeaker = isActive ? activeSpeaker : passiveSpeaker;
            currentSpeaker.SpeakTo(isActive ? passiveSpeaker.character.charNameChi : activeSpeaker.character.charNameChi, currentLine);
            tempConvRecord += "[" + currentSpeaker.character.charNameChi + "] say " + currentLine + "\n";
            if (CheckEndingPointOfChatting(currentLine)) break;

            yield return new WaitForSeconds(2f);
            isActive = !isActive;
            index++;
        }

        tempConvRecord += "[" + activeSpeaker.character.charNameChi + "," + passiveSpeaker.character.charNameChi + "] 於 " + "" + " 結束聊天\n";
        GolbalConvRecord += tempConvRecord;
        activeSpeaker.SummarizeConversation(tempConvRecord);
        passiveSpeaker.SummarizeConversation(tempConvRecord);
    }
}
