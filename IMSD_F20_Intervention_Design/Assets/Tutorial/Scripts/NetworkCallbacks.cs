using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Bolt;

[BoltGlobalBehaviour]
public class NetworkCallbacks : GlobalEventListener
{
    List<string> logMessages = new List<string>();

    void OnGUI()
    {
        // only display max the 5 latest log messages
        int maxMessages = Mathf.Min(5, logMessages.Count);

        GUILayout.BeginArea(new Rect(Screen.width / 2 - 200, Screen.height - 100, 400, 100), GUI.skin.box);

        for (int i = 0; i < maxMessages; ++i)
        {
            GUILayout.Label(logMessages[i]);
        }

        GUILayout.EndArea();
    }

    public override void SceneLoadLocalDone(string scene)
    {
        // randomize a position
        var spawnPosition = new Vector3(Random.Range(-8, 8), 0, Random.Range(-8, 8));

        // instantiate cube
        BoltNetwork.Instantiate(BoltPrefabs.Cube, spawnPosition, Quaternion.identity);
    }

    public override void OnEvent(LogEvent evnt)
    {
        logMessages.Insert(0, evnt.Message);
    }
}