using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.SceneManagement;

public class IntroController : MonoBehaviour {

    public Text skipText;
    int pageIndex = 0;

    void Start() {
        Sequence introStartSeq = DOTween.Sequence();
        introStartSeq.SetDelay(1f);
        introStartSeq.Append(skipText.DOFade(1f, 1f));
    }

    void Update() {
        if (Input.GetButtonDown("Fire1")) {
            if (pageIndex != transform.childCount - 2) {
                transform.GetChild(pageIndex).gameObject.SetActive(false);
                transform.GetChild(pageIndex + 1).gameObject.SetActive(true);
                pageIndex++;
            } else {
                SceneManager.LoadScene("game-core");
            }
        } else if (Input.GetKeyDown(KeyCode.Space)) {
            SceneManager.LoadScene("game-core");
        }
    }
}
