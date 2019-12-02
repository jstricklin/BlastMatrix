using System.Collections;
using System.Collections.Generic;
using Project.Utilities;
using UnityEngine;
using TMPro;
using Project.Networking;

namespace Project.Controllers {
    public class EndGameUIController : Singleton<EndGameUIController>
    {

        [SerializeField]
        TMP_Text playerScoreText;
        string matchResults;
        TMP_Text matchCountdown;
        float startTime;

        public void SetMatchResults(string results, int countDown)
        {
            matchResults = results.FixLineBreaks();
            playerScoreText.text = matchResults;
            StartCoroutine(NextMatchCountdown(countDown));
        }
        public void ExitToMainMenu()
        {
            FindObjectOfType<NetworkClient>().ExitToMainMenu();
        }
        IEnumerator NextMatchCountdown(int time)
        {
            if (time == 0)
                yield break;
            matchCountdown.text = "Next match in... " + time;
            time--;
            yield return new WaitForSecondsRealtime(1);
        }
    }
}
