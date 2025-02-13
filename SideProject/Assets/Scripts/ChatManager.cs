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
        activeAgent.StartChattingWith(passiveAgent.character.charNameChi);
        passiveAgent.StartChattingWith(activeAgent.character.charNameChi);

        LoadChatContentFromFile(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "_Meee/AISideProject/ConversationTest.txt"));
        StartCoroutine(ConversationProcess(activeAgent, passiveAgent));
    }

    private IEnumerator ConversationProcess(AgentController activeSpeaker, AgentController passiveSpeaker)
    {
        int index = 0;
        bool isActive = true;
        string tempConvRecord = "";

        //聊天開始  => 存[人名1, 人名2]，開始時間到tempConvRecord
        tempConvRecord += $"[{activeSpeaker.character.charNameChi},{passiveSpeaker.character.charNameChi}] 於 {GameManager.Instance.GetGameTime()} 開始聊天\n";

        while (index< currentChatContent.Count)
        {
            string currentLine = currentChatContent[index]; //delete after
            if (CheckEndingPointOfChatting(currentLine)) break; //delete after

            if (isActive)
            {
                activeSpeaker.SayOrRespond(currentLine);
                passiveSpeaker.Listen(currentLine);
                tempConvRecord += $"[{activeSpeaker.character.charNameChi}] say {currentLine}\n";
            }
            else
            {
                passiveSpeaker.SayOrRespond(currentLine);
                activeSpeaker.Listen(currentLine);
                tempConvRecord += $"[{passiveSpeaker.character.charNameChi}] say {currentLine}\n";
            } 
            yield return new WaitForSeconds(2f);
            isActive = !isActive;
            index++;
        }

        tempConvRecord += $"[{activeSpeaker.character.charNameChi},{passiveSpeaker.character.charNameChi}] 於 {GameManager.Instance.GetGameTime()} 結束聊天";
        GolbalConvRecord += $"{tempConvRecord}\n";
        activeSpeaker.SummarizeConversation(tempConvRecord);
        passiveSpeaker.SummarizeConversation(tempConvRecord);
    }

    public void SaveGlobalConvRecordToFile()
    {
        string folderPath = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Desktop), "_Meee/AISideProject/");
        string fileName = "GlobalConvRecord.txt";
        string filePath = Path.Combine(folderPath, fileName);

        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }

        if (File.Exists(filePath))
        {
            File.AppendAllText(filePath, GolbalConvRecord);
        }
        else
        {
            File.WriteAllText(filePath, GolbalConvRecord);
        }

        GolbalConvRecord = "";
    }
}
