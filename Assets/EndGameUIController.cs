using System.Collections;
using System.Collections.Generic;
using Project.Utilities;
using UnityEngine;
using TMPro;
using Project.Networking;
using UnityEngine.UI;

namespace Project.Controllers {
    public class EndGameUIController : Singleton<EndGameUIController>
    {

        [SerializeField]
        TMP_Text playerScoreText;
        [SerializeField]
        ScrollRect scoreScroll;
        string matchResults;
        [SerializeField]
        TMP_Text matchCountdown;
        float startTime;

        public void SetMatchResults(string results, int countDown)
        {
            matchResults = results.FixLineBreaks();
            playerScoreText.text = matchResults;
            scoreScroll.verticalNormalizedPosition = 1;
            StartCoroutine(NextMatchCountdown(countDown));
        }
        IEnumerator NextMatchCountdown(int time)
        {
            while(true) 
            {
                if (time == 0)
                    yield break;
                matchCountdown.text = "Next match in... " + time;
                time--;
                yield return new WaitForSecondsRealtime(1);
            }
        }
    }
}
