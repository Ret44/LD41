using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public enum MainMenuOption { Play, Credits, Quit }

public class MainMenuController : MonoBehaviour {

    MainMenuOption current = MainMenuOption.Play;
    public Transform menuOptions;
    public Color inactiveColor;
    public Transform selector;

    public RectTransform stripe;
    public RectTransform logo;
    public RectTransform credits;

    bool isCreditsOn = false;
    bool isGameStarted = false;

    public GameObject intro;

    void Start() {
        Sequence introSeq = DOTween.Sequence();
        introSeq.Append(stripe.DOAnchorPosX(1f, 1.2f, false).SetEase(Ease.InCubic));
        introSeq.Join(logo.DOAnchorPosX(1f, 1f, false).SetEase(Ease.InCubic).SetDelay(0.6f));
        introSeq.Append(menuOptions.GetComponent<RectTransform>().DOAnchorPosY(0f, 1f, false));

        SwitchOption(0);
    }

    void Update() {
        if (!isCreditsOn && !isGameStarted) {
            if (Distance(Input.mousePosition, menuOptions.position) < 1000f) {
                if (Distance(Input.mousePosition, menuOptions.GetChild(0).position) >
                    Distance(Input.mousePosition, menuOptions.GetChild(2).position) &&
                    Distance(Input.mousePosition, menuOptions.GetChild(1).position) >
                    Distance(Input.mousePosition, menuOptions.GetChild(2).position)) {
                    SetOption(2);
                } else if (Distance(Input.mousePosition, menuOptions.GetChild(0).position) >
                    Distance(Input.mousePosition, menuOptions.GetChild(1).position)) {
                    SetOption(1);
                } else {
                    SetOption(0);
                }
            } else {
                if (Input.GetKeyDown(KeyCode.UpArrow)) {
                    SwitchOption(+1);
                } else if (Input.GetKeyDown(KeyCode.DownArrow)) {
                    SwitchOption(-1);
                }
            }

            if (Input.GetKeyDown(KeyCode.Return) || Input.GetButtonDown("Fire1")) {
                switch (current) {
                    case MainMenuOption.Play:
                        FadeMenu(false);
                        isGameStarted = true;
                        Instantiate(intro);
                        break;
                    case MainMenuOption.Credits:
                        FadeMenu(false);
                        isCreditsOn = true;
                        credits.GetComponent<Text>().DOFade(1f, 1f);
                        break;
                    case MainMenuOption.Quit:
                        Application.Quit();
                        break;
                }
            }

            SetOption((int)current);
        } else if (isCreditsOn && !isGameStarted) {
            if (Input.anyKeyDown) {
                credits.GetComponent<Text>().DOFade(0f, 1f);
                isCreditsOn = false;
                FadeMenu(true);
            }
        }
    }

    void FadeMenu(bool toState) {
        float toAlpha;
        if (toState) {
            toAlpha = 1;
        } else {
            toAlpha = 0;
        }

        Text[] menuOptionText = menuOptions.GetComponentsInChildren<Text>();
        foreach (Text t in menuOptionText) {
            t.DOFade(toAlpha, 1f);
        }
        selector.GetComponent<Image>().DOFade(toAlpha, 1f);
        logo.GetComponent<Image>().DOFade(toAlpha, 1f);
        stripe.GetComponent<Image>().DOFade(toAlpha, 1f);
    }

    float Distance(Vector2 pos1, Vector2 pos2) {
        return Mathf.Abs(pos1.x - pos2.x) + Mathf.Abs(pos1.y - pos2.y);
    }

    void SetOption(int index) {
        current = (MainMenuOption)index;
        selector.position = menuOptions.GetChild((int)current).position;
        for (int i = 0; i < menuOptions.childCount; i++) {
            menuOptions.GetChild(i).GetComponent<Text>().color = inactiveColor;
        }
        menuOptions.GetChild((int)current).GetComponent<Text>().color = Color.white;
    }

    void SwitchOption(int dir) {
        switch (dir) {
            case 0:
                SetOption((int)current);
                break;
            case +1:
                if ((int)current + 1 == Enum.GetNames(typeof(MainMenuOption)).Length) {
                    SetOption(0);
                } else {
                    SetOption((int)current + 1);
                }
                break;
            case -1:
                if ((int)current - 1 == -1) {
                    SetOption(Enum.GetNames(typeof(MainMenuOption)).Length - 1);
                } else {
                    SetOption((int)current - 1);
                }
                break;
        }
    }
}