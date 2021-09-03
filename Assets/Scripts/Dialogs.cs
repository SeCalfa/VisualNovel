using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System;
using TMPro;
using UnityEngine.SceneManagement;

public class Dialogs : MonoBehaviour
{
    private enum LevelDialogQueue
    {
        Plansza3,
        Plansza4,
        Plansza5,
        Plansza6,
        Plansza7,
        Plansza85,
        Plansza9,
        Plansza10,
        Plansza11,
        Plansza12,
        Plansza13,
        Plansza14,
        Plansza15,
        Plansza16,
        Plansza17,
        Plansza18,
        Plansza19,
        Plansza20,
        Plansza21,
        Plansza22,
        Plansza23,
        Plansza24,
        Plansza26,
        Plansza27,
        Plansza27Incorrect,
        Plansza30Incorrect,
        Plansza31Incorrect,
        Plansza27Correct,
        Plansza28,
        Plansza29,
        Plansza30Correct,
        Plansza31Correct,
        Plansza32,
        Plansza33,
        Plansza34,
        Plansza35,
        Plansza36,
        Plansza37,
        Plansza38,
        Plansza39,
        Plansza40,
        Plansza41,
        Plansza42,
        Plansza425,
        Plansza43,
        Plansza44
    };

    [Header("Objects")]
    [SerializeField] private Image backgroundImage;
    [SerializeField] private GameObject dialogPanel = null;
    [SerializeField] private TextMeshProUGUI dialogText = null;
    [SerializeField] private Transform characterParent = null;
    [SerializeField] private Image nameMark = null;
    [SerializeField] private Text nameMarkText = null;
    [SerializeField] private GameObject[] papers = new GameObject[3];
    [SerializeField] private Transform leftSpawnPoint = null;
    [SerializeField] private Transform rightSpawnPoint = null;
    [SerializeField] private GameObject choise1 = null;
    [SerializeField] private GameObject choise2 = null;
    [SerializeField] private GameObject choise3 = null;
    [SerializeField] private Text choise1Text = null;
    [SerializeField] private Text choise2Text = null;
    [SerializeField] private Text choise3Text = null;
    [SerializeField] private GameObject note = null;
    [SerializeField] private AudioSource staticAudio = null;
    [SerializeField] private AudioSource dynamicAudio = null;
    [SerializeField] private SpriteElementFade backgroundElementFade1;
    [SerializeField] private SpriteElementFade backgroundElementFade2;
    [SerializeField] private SpriteElementFade backgroundElementFade3;

    [Header("Parametrs")]
    [SerializeField]
    private bool isChooseButtonDestroyed = false;
    [Space]
    [SerializeField] private bool isDialogPage = false;
    [SerializeField] private LevelDialogQueue levelPreset;
    [SerializeField] private string[] backgroundTexts;
    [SerializeField] private string[] dialogTexts;
    [SerializeField] private GameObject[] characters;
    [SerializeField] [Range(0, 0.05f)] private float backgroundAlphaSpeed = 0;
    [SerializeField] [Range(0, 0.05f)] private float dialogPanelAlphaSpeed = 0;
    [SerializeField] private string nextLevelName;
    [SerializeField] private AnimationCurve curve;
    [SerializeField] private bool isBlackFadeAppearOnStart = false;
    [SerializeField] private AudioClip[] dynamicSounds;

    [Space]
    [SerializeField]
    private BackgroundFrameAnimation singleFadeComponent;

    [Space]
    [SerializeField]
    private BackgroundFrameAnimation frameFade;


    private string dialText = null;
    private bool isTextRead = false;
    private bool isDialogPanelActive = false;
    private bool isChoiseAppear = false;
    private bool isCameraRotateStart = false; // Plansza 6
    private float choiseAppear = 0.0f;
    private float dialogPanelAlpha = 0;
    private float backgroundAlpha = 1.0f;
    private float nameMarkAlpha = 0;
    private float cameraRotateAlpha = 0; // Plansza 6
    private float timer = 0;
    private int dialogRead = 0;
    private int dialogMax = 0;
    private int currentPaper = 0;
    private Vector3 endLocLeft2 = new Vector3(500, 0, 0);
    private Vector3 endLocLeft15 = new Vector3(600, 0, 0);
    private Vector3 endLocRight2 = new Vector3(-500, 0, 0);
    private Vector3 endLocRight15 = new Vector3(-600, 0, 0);
    private Vector3 bgPositionOnStart;
    private Vector3 bgRotationOnStart;
    private Vector3 bgScaleOnStart;
    private Vector3 newBgPosition = new Vector3(100, -65, 0); // Plansza 6
    private Vector3 newBgRotation = new Vector3(0, 0, 10); // Plansza 6
    private Fade fade;
    private float choise1StartX;
    private float choise2StartX;
    private Dictionary<string, GameObject> allCharactersInScene = new Dictionary<string, GameObject>();
    private int[] choosenDialogElements = new int[3] { 0, 0, 0 };
    private int chooseNumber = 0;
    private bool isChooseNeedBeDesappeared = true;
    private bool isDialogNeedBeStopped = false;

    private bool button1Choosen = false;
    private bool button2Choosen = false;
    private bool button3Choosen = false;

    private void Awake()
    {
        SettingsOnAwake();
    }

    private void Start()
    {
        if (!isBlackFadeAppearOnStart)
            fade.FadeDisappear();
        else
            fade.BlackFadeDisappear();

        if (isDialogPage)
        {
            Invoke("DialogOpacityTimerVoid", 1f);
            ChooseDisappear();
        }
    }

    private void Update()
    {
        timer += Time.deltaTime;
        CameraRotate();

        if ((Input.GetKeyDown(KeyCode.Mouse0) || Input.GetKeyDown(KeyCode.Space)) && !isDialogNeedBeStopped)
        {
            if (!isDialogPage)
            {
                if (fade.isFadeDisappeared && timer > 5.0f)
                {
                    PaperDrop();
                    timer = 0;
                }
            }
            else if (isDialogPage && !isTextRead && isDialogPanelActive)
            {
                isTextRead = true;

                StopCoroutine(DialogTypingTextTimer());
                dialogText.text = dialText;
                timer = 0;
            }
            else if (isDialogPage && isTextRead && isDialogPanelActive && !isChoiseAppear && timer > 0.0f)
            {
                LevelPreset();
                isTextRead = false;

                if (dialogRead < dialogTexts.Length) // Устранение повторения последней реплики
                    StartCoroutine(DialogTypingTextTimer());

                timer = 0;
            }
        }
    }

    private void PaperDrop()
    {
        string animation = "";

        if(currentPaper >= papers.Length)
            return;
        else if (currentPaper == 0)
            animation = "PaperDisapp";
        else if (currentPaper == 1)
            animation = "PaperDisapp2";
        else if (currentPaper == 2)
            animation = "PaperDisapp3";

        papers[currentPaper].GetComponent<Animation>().Play(animation);
        currentPaper++;

        if (currentPaper == papers.Length)
            LastPaperDropped();
    }

    internal void LastPaperDropped()
    {
        fade.FadeAppear(nextLevelName);
    }

    public void DialogTextOpacityTimerForButton()
    {
        StartCoroutine(DialogOpacityTimer());
    }

    private void DialogOpacityTimerVoid()
    {
        StartCoroutine(DialogOpacityTimer());
    }

    private IEnumerator DialogOpacityTimer() // Появление
    {
        dialogPanel.SetActive(true);
        dialogPanelAlpha = 0;
    Reset:
        dialogPanelAlpha += dialogPanelAlphaSpeed;
        dialogPanel.GetComponent<Image>().color = new Color(1, 1, 1, dialogPanelAlpha);
        yield return new WaitForSeconds(0.05f);
        if (dialogPanelAlpha < 1.0f)
            goto Reset;

        StartCoroutine(DialogTypingTextTimer());
        isDialogPanelActive = true;

        if (levelPreset == LevelDialogQueue.Plansza7) // Первый диалог
        {
            nameMarkText.text = "Korav'ye";
            StartCoroutine(NameMarkAppear());
            allCharactersInScene.Add("Korav'ye", SpawnCharacter(characters[1], rightSpawnPoint, endLocRight15, false));
            allCharactersInScene["Korav'ye"].GetComponent<CharactersController>().isTalking = true;
            allCharactersInScene["Korav'ye"].GetComponent<CharactersController>().isLeftSide = false;
        }
        else if (levelPreset == LevelDialogQueue.Plansza31Incorrect)
        {
            nameMarkText.text = "Wiktor";
            StartCoroutine(NameMarkAppear());
        }
        else if (levelPreset == LevelDialogQueue.Plansza27Correct)
        {
            nameMarkText.text = "Profesor Antos";
            StartCoroutine(NameMarkAppear());
        }
        else if (levelPreset == LevelDialogQueue.Plansza32)
        {
            nameMarkText.text = "Wiktor";
            StartCoroutine(NameMarkAppear());
        }
    }

    private IEnumerator DialogOpacityTimer2() // Исчезновение
    {
    Reset:
        dialogPanelAlpha -= dialogPanelAlphaSpeed;
        dialogPanel.GetComponent<Image>().color = new Color(1, 1, 1, dialogPanelAlpha);
        yield return new WaitForSeconds(0.02f);
        if (dialogPanelAlpha > 0.0f)
            goto Reset;

        SceneManager.LoadScene(nextLevelName);
    }

    private IEnumerator DialogTypingTextTimer()
    {
        int pointNumber = 0;

        dialogText.gameObject.SetActive(true);
        dialogText.text = "";

        if (SceneManager.GetActiveScene().name == "Plansza6" && dialogRead == 0)// Когда срабатывает поворот камеры
        {
            isCameraRotateStart = true;
        }

        for (int s = 0; s < dialText.Length; s++)
        {
            dialogText.text += dialText[s];
            yield return new WaitForSeconds(0.02f);
            if (isTextRead)
                break;

            if (dialText[s].ToString() == "…")
            {
                pointNumber = 0;
                yield return new WaitForSeconds(1.0f);
            }
            else if (dialText[s].ToString() == ".")
            {
                pointNumber += 1;
            }
            else
                pointNumber = 0;

            if (pointNumber == 3)
            {
                pointNumber = 0;
                yield return new WaitForSeconds(1.0f);
            }
        }
        isTextRead = true;
    }

    private void LevelPreset()
    {
        if (isDialogPage)
        {
            if (dialogRead < dialogTexts.Length && dialogRead + 1 < dialogTexts.Length)
                dialText = dialogTexts[dialogRead + 1];

            switch (levelPreset)
            {

                #region Plansza3

                case LevelDialogQueue.Plansza3:

                    dialogRead += 1;

                    if (dialogRead == 1)
                    {
                        PlayerPrefs.SetInt("Plansza3", 1);
                        PlayDynamicSound(0);
                    }
                    else if (dialogRead == 2)
                    {
                        nameMarkText.text = "Wiktor";
                        StartCoroutine(NameMarkAppear());
                        backgroundElementFade1.ImageAppear();
                    }
                    else if (dialogRead == 3)
                    {
                        nameMarkText.text = "Profesor Antos";
                        allCharactersInScene.Add("Profesor", SpawnCharacter(characters[0], leftSpawnPoint, endLocLeft15, false));
                        allCharactersInScene["Profesor"].GetComponent<CharactersController>().isTalking = true;
                        allCharactersInScene["Profesor"].GetComponent<CharactersController>().isLeftSide = true;
                        allCharactersInScene["Profesor"].GetComponent<CharactersController>().SetEmotion(0);
                    }
                    else if (dialogRead == 4)
                    {
                        StartCoroutine(NameMarkDisappear());
                        allCharactersInScene["Profesor"].GetComponent<CharactersController>().isTalking = false;
                        allCharactersInScene.Add("Nikolai", SpawnCharacter(characters[1], leftSpawnPoint, endLocLeft2, true));
                        allCharactersInScene["Nikolai"].GetComponent<CharactersController>().isLeftSide = true;
                        allCharactersInScene["Profesor"].GetComponent<CharactersController>().isNewLocationActive = true;
                    }
                    else if (dialogRead == 5)
                    {
                        nameMarkText.text = "Profesor Antos";
                        StartCoroutine(NameMarkAppear());
                        allCharactersInScene["Profesor"].GetComponent<CharactersController>().isTalking = true;
                    }
                    else if (dialogRead == 6)
                    {
                        StartCoroutine(NameMarkDisappear());
                        allCharactersInScene["Profesor"].GetComponent<CharactersController>().isTalking = false;
                    }
                    else if (dialogRead == 7)
                    {
                        nameMarkText.text = "Profesor Antos";
                        StartCoroutine(NameMarkAppear());
                        allCharactersInScene["Profesor"].GetComponent<CharactersController>().isTalking = true;
                    }
                    else if (dialogRead == 8)
                    {
                        nameMarkText.text = "Wiktor";
                        allCharactersInScene["Profesor"].GetComponent<CharactersController>().isTalking = false;
                    }
                    else if (dialogRead == 9)
                    {
                        StartCoroutine(NameMarkDisappear());
                        backgroundElementFade2.ImageAppear();
                    }
                    else if (dialogRead == 11)
                    {
                        nameMarkText.text = "...";
                        StartCoroutine(NameMarkAppear());
                        allCharactersInScene.Add("Czukcz", SpawnCharacter(characters[2], rightSpawnPoint, endLocRight15, false));
                        allCharactersInScene["Czukcz"].GetComponent<CharactersController>().isTalking = true;
                        allCharactersInScene["Czukcz"].GetComponent<CharactersController>().isLeftSide = false;
                        allCharactersInScene["Czukcz"].GetComponent<CharactersController>().SetEmotion(0);
                    }
                    else if (dialogRead == 12)
                    {
                        StartCoroutine(NameMarkDisappear());
                        allCharactersInScene["Czukcz"].GetComponent<CharactersController>().isTalking = false;
                    }
                    else if (dialogRead == 13)
                    {
                        nameMarkText.text = "Korav'ye";
                        StartCoroutine(NameMarkAppear());
                        allCharactersInScene["Czukcz"].GetComponent<CharactersController>().isTalking = true;
                        allCharactersInScene["Czukcz"].GetComponent<CharactersController>().SetEmotion(0);
                    }
                    else if (dialogRead == 14)
                    {
                        nameMarkText.text = "Profesor Antos";
                        allCharactersInScene["Profesor"].GetComponent<CharactersController>().isTalking = true;
                        allCharactersInScene["Czukcz"].GetComponent<CharactersController>().isTalking = false;
                    }
                    else if (dialogRead == 15)
                    {
                        nameMarkText.text = "Korav'ye";
                        allCharactersInScene["Profesor"].GetComponent<CharactersController>().isTalking = false;
                        allCharactersInScene["Czukcz"].GetComponent<CharactersController>().isTalking = true;
                    }
                    else if (dialogRead == 16)
                    {
                        nameMarkText.text = "Nikolai Morozov";
                        allCharactersInScene["Nikolai"].GetComponent<CharactersController>().isTalking = true;
                        allCharactersInScene["Czukcz"].GetComponent<CharactersController>().isTalking = false;
                        allCharactersInScene["Nikolai"].GetComponent<CharactersController>().SetEmotion(0);
                    }
                    else if (dialogRead == 17)
                    {
                        StartCoroutine(NameMarkDisappear());
                        allCharactersInScene["Nikolai"].GetComponent<CharactersController>().isTalking = false;
                        allCharactersInScene["Profesor"].GetComponent<CharactersController>().SetEmotion(7);
                    }
                    else if (dialogRead == 18)
                    {
                        nameMarkText.text = "Profesor Antos";
                        StartCoroutine(NameMarkAppear());
                        allCharactersInScene["Profesor"].GetComponent<CharactersController>().isTalking = true;
                        allCharactersInScene["Profesor"].GetComponent<CharactersController>().SetEmotion(6);
                    }
                    else if (dialogRead == 19)
                    {
                        nameMarkText.text = "Korav'ye";
                        allCharactersInScene["Czukcz"].GetComponent<CharactersController>().isTalking = true;
                        allCharactersInScene["Profesor"].GetComponent<CharactersController>().isTalking = false;
                        allCharactersInScene["Profesor"].GetComponent<CharactersController>().SetEmotion(0);
                    }
                    else if (dialogRead == 20)
                    {
                        nameMarkText.text = "Nikolai Morozov";
                        allCharactersInScene["Nikolai"].GetComponent<CharactersController>().isTalking = true;
                        allCharactersInScene["Czukcz"].GetComponent<CharactersController>().isTalking = false;
                    }
                    else if (dialogRead == 21)
                    {
                        StartCoroutine(NameMarkDisappear());
                        allCharactersInScene["Nikolai"].GetComponent<CharactersController>().isTalking = false;
                        allCharactersInScene["Czukcz"].GetComponent<CharactersController>().SetEmotion(3);
                    }
                    else if (dialogRead == 22)
                    {
                        nameMarkText.text = "Wiktor";
                        StartCoroutine(NameMarkAppear());
                    }
                    else if (dialogRead == 23)
                    {
                        StartCoroutine(NameMarkDisappear());
                        allCharactersInScene["Czukcz"].GetComponent<CharactersController>().SetEmotion(0);
                    }
                    else if (dialogRead == 24)
                    {
                        nameMarkText.text = "Korav'ye";
                        StartCoroutine(NameMarkAppear());
                        allCharactersInScene["Czukcz"].GetComponent<CharactersController>().isTalking = true;
                    }
                    else if (dialogRead == 25)
                    {
                        nameMarkText.text = "Nikolai Morozov";
                        allCharactersInScene["Nikolai"].GetComponent<CharactersController>().isTalking = true;
                        allCharactersInScene["Czukcz"].GetComponent<CharactersController>().isTalking = false;
                    }
                    else if (dialogRead == 26)
                    {
                        nameMarkText.text = "Profesor Antos";
                        allCharactersInScene["Profesor"].GetComponent<CharactersController>().isTalking = true;
                        allCharactersInScene["Nikolai"].GetComponent<CharactersController>().isTalking = false;
                        allCharactersInScene["Profesor"].GetComponent<CharactersController>().SetEmotion(7);
                    }
                    else if (dialogRead == 27)
                    {
                        StartCoroutine(NameMarkDisappear());
                        allCharactersInScene["Profesor"].GetComponent<CharactersController>().isTalking = false;
                        allCharactersInScene["Nikolai"].GetComponent<CharactersController>().isLeave = true;
                        allCharactersInScene.Remove("Nikolai");
                    }
                    else if (dialogRead == 28)
                    {
                        nameMarkText.text = "Profesor Antos";
                        StartCoroutine(NameMarkAppear());
                        allCharactersInScene["Profesor"].GetComponent<CharactersController>().isTalking = true;
                        allCharactersInScene["Profesor"].GetComponent<CharactersController>().SetEmotion(0);
                    }
                    else if (dialogRead == 29)
                    {
                        nameMarkText.text = "Korav'ye";
                        allCharactersInScene["Czukcz"].GetComponent<CharactersController>().isTalking = true;
                        allCharactersInScene["Profesor"].GetComponent<CharactersController>().isTalking = false;
                    }
                    else if (dialogRead == 31)
                    {
                        StartCoroutine(NameMarkDisappear());
                        allCharactersInScene["Czukcz"].GetComponent<CharactersController>().isTalking = false;
                        allCharactersInScene["Czukcz"].GetComponent<CharactersController>().isLeave = true;
                        allCharactersInScene.Remove("Czukcz");
                        backgroundElementFade2.ImageDisappear();
                        allCharactersInScene["Profesor"].GetComponent<CharactersController>().isMiddleReturn = true;
                        ChooseAppear("Co ugryzło Nikolaja?", "Skąd się znasz z Nikolajem?");
                    }
                    else if (dialogRead == 32)
                    {
                        nameMarkText.text = "Profesor Antos";
                        StartCoroutine(NameMarkAppear());
                        allCharactersInScene["Profesor"].GetComponent<CharactersController>().isTalking = true;
                    }
                    else if (dialogRead == 33)
                    {
                        nameMarkText.text = "Wiktor";
                        allCharactersInScene["Profesor"].GetComponent<CharactersController>().isTalking = false;
                    }
                    else if (dialogRead == 34)
                    {
                        nameMarkText.text = "Profesor Antos";
                        allCharactersInScene["Profesor"].GetComponent<CharactersController>().isTalking = true;
                    }
                    else if (dialogRead == 35)
                    {
                        nameMarkText.text = "Wiktor";
                        allCharactersInScene["Profesor"].GetComponent<CharactersController>().isTalking = false;
                    }
                    else if (dialogRead == 36) // A
                    {
                        nameMarkText.text = "Profesor Antos";
                        allCharactersInScene["Profesor"].GetComponent<CharactersController>().isTalking = true;

                        if (choosenDialogElements[1] == 0) // Второй выбор не был сделан
                        {
                            dialogRead = 42;
                        }
                        else
                        {
                            dialogRead = 43;
                        }
                    }
                    else if (dialogRead == 37)
                    {
                        nameMarkText.text = "Profesor Antos";
                        StartCoroutine(NameMarkAppear());
                        allCharactersInScene["Profesor"].GetComponent<CharactersController>().isTalking = true;
                    }
                    else if (dialogRead == 38)
                    {
                        StartCoroutine(NameMarkDisappear());
                        allCharactersInScene["Profesor"].GetComponent<CharactersController>().isTalking = false;
                    }
                    else if (dialogRead == 39)
                    {
                        nameMarkText.text = "Wiktor";
                        StartCoroutine(NameMarkAppear());
                    }
                    else if (dialogRead == 40)
                    {
                        nameMarkText.text = "Profesor Antos";
                        allCharactersInScene["Profesor"].GetComponent<CharactersController>().isTalking = true;
                    }
                    else if (dialogRead == 41)
                    {
                        nameMarkText.text = "Wiktor";
                        allCharactersInScene["Profesor"].GetComponent<CharactersController>().isTalking = false;
                    }
                    else if (dialogRead == 42) // B
                    {
                        nameMarkText.text = "Profesor Antos";
                        allCharactersInScene["Profesor"].GetComponent<CharactersController>().isTalking = true;

                        if (choosenDialogElements[1] == 0) // Второй выбор не был сделан
                        {
                            dialogRead = 42;
                        }
                        else
                        {
                            dialogRead = 43;
                        }
                    }
                    else if (dialogRead == 43) // L
                    {
                        if (choosenDialogElements[0] == 1) // Певый выбор был первой репликой
                        {
                            ChooseAppear("Skąd się znasz z Nikolajem?", "Nie, to nic takiego");
                        }
                        else if (choosenDialogElements[0] == 2) // Певый выбор был второй репликой
                        {
                            ChooseAppear("Co ugryzło Nikolaja?", "Nie, to nic takiego");
                        }
                    }
                    else if (dialogRead == 46)
                    {
                        StartCoroutine(NameMarkDisappear());
                        allCharactersInScene["Profesor"].GetComponent<CharactersController>().isTalking = false;
                        allCharactersInScene["Profesor"].GetComponent<CharactersController>().isLeave = true;
                        allCharactersInScene["Profesor"].GetComponent<CharactersController>().SetEmotion(3);
                        allCharactersInScene.Remove("Profesor");
                        allCharactersInScene.Add("Reporter", SpawnCharacter(characters[4], rightSpawnPoint, endLocRight15, false));
                        allCharactersInScene["Reporter"].GetComponent<CharactersController>().isLeftSide = false;
                    }
                    else if (dialogRead == 47)
                    {
                        StartCoroutine(NameMarkAppear());
                        nameMarkText.text = "...";
                        allCharactersInScene["Reporter"].GetComponent<CharactersController>().isTalking = true;
                        allCharactersInScene["Reporter"].GetComponent<CharactersController>().SetEmotion(0);
                    }
                    else if (dialogRead == 48)
                    {
                        nameMarkText.text = "Wiktor";
                        allCharactersInScene["Reporter"].GetComponent<CharactersController>().isTalking = false;
                    }
                    else if (dialogRead == 49)
                    {
                        StartCoroutine(NameMarkDisappear());
                    }
                    else if (dialogRead == 50)
                    {
                        StartCoroutine(NameMarkAppear());
                        nameMarkText.text = "Czesław";
                        allCharactersInScene["Reporter"].GetComponent<CharactersController>().isTalking = true;
                    }
                    else if (dialogRead == 51)
                    {
                        nameMarkText.text = "Wiktor";
                        allCharactersInScene["Reporter"].GetComponent<CharactersController>().isTalking = false;
                    }
                    else if (dialogRead == 52)
                    {
                        nameMarkText.text = "Czesław";
                        allCharactersInScene["Reporter"].GetComponent<CharactersController>().isTalking = true;
                    }
                    else if (dialogRead == 53)
                    {
                        StartCoroutine(NameMarkDisappear());
                        allCharactersInScene["Reporter"].GetComponent<CharactersController>().isTalking = false;
                    }
                    else if (dialogRead == 54)
                    {
                        StartCoroutine(NameMarkAppear());
                        nameMarkText.text = "Wiktor";
                        allCharactersInScene["Reporter"].GetComponent<CharactersController>().SetEmotion(2);
                    }
                    else if (dialogRead == 55)
                    {
                        nameMarkText.text = "Czesław";
                        allCharactersInScene["Reporter"].GetComponent<CharactersController>().isTalking = true;
                        allCharactersInScene["Reporter"].GetComponent<CharactersController>().SetEmotion(1);
                    }
                    else if (dialogRead == 56)
                    {
                        StartCoroutine(NameMarkDisappear());
                        allCharactersInScene["Reporter"].GetComponent<CharactersController>().isTalking = false;
                        allCharactersInScene["Reporter"].GetComponent<CharactersController>().SetEmotion(0);
                    }
                    else if (dialogRead == 57)
                    {
                        StartCoroutine(NameMarkAppear());
                        nameMarkText.text = "...";
                        allCharactersInScene.Add("Manya", SpawnCharacter(characters[3], leftSpawnPoint, endLocLeft15, false));
                        allCharactersInScene["Manya"].GetComponent<CharactersController>().isTalking = true;
                        allCharactersInScene["Manya"].GetComponent<CharactersController>().isLeftSide = true;
                        allCharactersInScene["Manya"].GetComponent<CharactersController>().SetEmotion(0);

                    }
                    else if (dialogRead == 58)
                    {
                        nameMarkText.text = "Czesław";
                        allCharactersInScene["Reporter"].GetComponent<CharactersController>().isTalking = true;
                        allCharactersInScene["Manya"].GetComponent<CharactersController>().isTalking = false;
                        allCharactersInScene["Reporter"].GetComponent<CharactersController>().SetEmotion(2);
                    }
                    else if (dialogRead == 59)
                    {
                        backgroundElementFade3.ImageAppear();
                        StartCoroutine(NameMarkDisappear());
                        allCharactersInScene["Reporter"].GetComponent<CharactersController>().isTalking = false;
                    }
                    else if (dialogRead == 60)
                    {
                        StartCoroutine(NameMarkAppear());
                        nameMarkText.text = "Czesław";
                        allCharactersInScene["Reporter"].GetComponent<CharactersController>().isTalking = true;
                        allCharactersInScene["Reporter"].GetComponent<CharactersController>().SetEmotion(3);
                    }
                    else if (dialogRead == 61)
                    {
                        nameMarkText.text = "Manya";
                        allCharactersInScene["Reporter"].GetComponent<CharactersController>().isTalking = false;
                        allCharactersInScene["Manya"].GetComponent<CharactersController>().isTalking = true;
                        allCharactersInScene["Manya"].GetComponent<CharactersController>().SetEmotion(0);
                    }
                    else if (dialogRead == 62)
                    {
                        nameMarkText.text = "Wiktor";
                        allCharactersInScene["Manya"].GetComponent<CharactersController>().isTalking = false;
                        allCharactersInScene["Reporter"].GetComponent<CharactersController>().SetEmotion(0);
                    }
                    else if (dialogRead == 63)
                    {
                        nameMarkText.text = "Manya";
                        allCharactersInScene["Manya"].GetComponent<CharactersController>().isTalking = true;
                    }
                    else if (dialogRead == 64)
                    {
                        nameMarkText.text = "Wiktor";
                        allCharactersInScene["Manya"].GetComponent<CharactersController>().isTalking = false;
                    }
                    else if (dialogRead == 65)
                    {
                        nameMarkText.text = "Manya";
                        allCharactersInScene["Manya"].GetComponent<CharactersController>().isTalking = true;
                    }
                    else if (dialogRead == 66)
                    {
                        nameMarkText.text = "Wiktor";
                        allCharactersInScene["Manya"].GetComponent<CharactersController>().isTalking = false;
                    }
                    else if (dialogRead == 67)
                    {
                        nameMarkText.text = "Manya";
                        allCharactersInScene["Manya"].GetComponent<CharactersController>().isTalking = true;
                    }
                    else if (dialogRead == 68)
                    {
                        StartCoroutine(NameMarkDisappear());
                        allCharactersInScene["Manya"].GetComponent<CharactersController>().isTalking = false;
                        allCharactersInScene["Reporter"].GetComponent<CharactersController>().isLeave = true;
                        allCharactersInScene.Remove("Reporter");
                    }
                    else if (dialogRead == 70)
                    {
                        StartCoroutine(NameMarkAppear());
                        nameMarkText.text = "Nikolai Morozov";
                        allCharactersInScene.Add("Nikolai", SpawnCharacter(characters[1], rightSpawnPoint, endLocRight15, false));
                        allCharactersInScene["Nikolai"].GetComponent<CharactersController>().isLeftSide = false;
                        allCharactersInScene["Nikolai"].GetComponent<CharactersController>().isTalking = true;
                        allCharactersInScene["Nikolai"].GetComponent<CharactersController>().SetEmotion(4);
                    }
                    else if (dialogRead == 71)
                    {
                        StartCoroutine(NameMarkDisappear());
                        allCharactersInScene["Nikolai"].GetComponent<CharactersController>().isTalking = false;
                    }
                    else if (dialogRead == 72)
                    {
                        StartCoroutine(NameMarkAppear());
                        nameMarkText.text = "Manya";
                        allCharactersInScene["Manya"].GetComponent<CharactersController>().isTalking = true;
                        allCharactersInScene["Manya"].GetComponent<CharactersController>().SetEmotion(3);
                    }
                    else if (dialogRead == 73)
                    {
                        StartCoroutine(NameMarkDisappear());
                        allCharactersInScene["Manya"].GetComponent<CharactersController>().isTalking = false;
                    }
                    else if (dialogRead == 74)
                    {
                        nameMarkText.text = "Czesław";
                        StartCoroutine(NameMarkAppear());
                        allCharactersInScene.Add("Reporter", SpawnCharacter(characters[4], rightSpawnPoint, endLocRight2, false));
                        allCharactersInScene["Reporter"].GetComponent<CharactersController>().isTalking = true;
                        allCharactersInScene["Nikolai"].GetComponent<CharactersController>().isNewLocationActive = true;
                        allCharactersInScene["Reporter"].GetComponent<CharactersController>().SetEmotion(3);
                        allCharactersInScene["Nikolai"].GetComponent<CharactersController>().SetEmotion(0);
                    }
                    else if (dialogRead == 75)
                    {
                        nameMarkText.text = "Manya";
                        allCharactersInScene["Manya"].GetComponent<CharactersController>().isTalking = true;
                        allCharactersInScene["Reporter"].GetComponent<CharactersController>().isTalking = false;
                    }
                    else if (dialogRead == 76)
                    {
                        StartCoroutine(NameMarkDisappear());
                        allCharactersInScene.Add("Profesor", SpawnCharacter(characters[0], leftSpawnPoint, endLocLeft2, false));
                        allCharactersInScene["Profesor"].GetComponent<CharactersController>().isLeftSide = true;
                        allCharactersInScene["Manya"].GetComponent<CharactersController>().isTalking = false;
                        allCharactersInScene["Manya"].GetComponent<CharactersController>().isNewLocationActive = true;
                        allCharactersInScene["Manya"].GetComponent<CharactersController>().SetEmotion(0);
                    }
                    else if (dialogRead == 77)
                    {
                        StartCoroutine(NameMarkAppear());
                        nameMarkText.text = "Profesor Antos";
                        allCharactersInScene["Profesor"].GetComponent<CharactersController>().isTalking = true;
                        allCharactersInScene["Profesor"].GetComponent<CharactersController>().SetEmotion(0);
                    }
                    else if (dialogRead == 78)
                    {
                        StartCoroutine(NameMarkDisappear());
                        allCharactersInScene["Profesor"].GetComponent<CharactersController>().isTalking = false;
                    }
                    else if (dialogRead == 79)
                    {
                        StartCoroutine(NameMarkAppear());
                        nameMarkText.text = "Profesor Antos";
                        allCharactersInScene["Profesor"].GetComponent<CharactersController>().isTalking = true;
                    }
                    else if (dialogRead == 80) // End Level
                    {
                        allCharactersInScene["Profesor"].GetComponent<CharactersController>().isTalking = false;
                        GetComponent<Fade>().FadeAppear(nextLevelName);
                    }

                    break;

                #endregion

                #region  Plansza4

                case LevelDialogQueue.Plansza4:
                    dialogRead += 1;
                    fade.isNextLevelActive = false;

                    if (dialogRead == 1)
                    {
                        PlayerPrefs.SetInt("Plansza4", 1);
                    }
                    if (dialogRead == 5)
                    {
                        nameMarkText.text = "Profesor Antos";
                        StartCoroutine(NameMarkAppear());
                    }
                    else if (dialogRead == 6)
                    {
                        StartCoroutine(NameMarkDisappear());
                    }
                    else if (dialogRead == 8)
                    {
                        PlayDynamicSound(0);
                        frameFade.frameFadeStart = true;
                    }
                    else if (dialogRead == 13)
                    {
                        fade.BlackFadeAppear(false, "");
                        StartCoroutine(AudioOff());
                    }
                    else if (dialogRead == 17)
                    {
                        StartCoroutine(DialogOpacityTimer2());
                    }

                    break;

                #endregion

                #region Plansza5

                case LevelDialogQueue.Plansza5:
                    dialogRead += 1;

                    if (dialogRead == 1)
                    {
                        PlayerPrefs.SetInt("Plansza5", 1);
                    }
                    else if (dialogRead == 5)
                    {
                        nameMarkText.text = "Ojciec";
                        StartCoroutine(NameMarkAppear());
                    }
                    else if (dialogRead == 6)
                    {
                        StartCoroutine(NameMarkDisappear());
                    }
                    else if (dialogRead == 7)
                    {
                        nameMarkText.text = "Wiktor";
                        StartCoroutine(NameMarkAppear());
                    }
                    else if (dialogRead == 8)
                    {
                        StartCoroutine(NameMarkDisappear());
                    }
                    else if (dialogRead == 14)
                    {
                        fade.BlackFadeAppear(true, nextLevelName);
                    }

                    break;

                #endregion

                #region Plansza6

                case LevelDialogQueue.Plansza6:
                    dialogRead += 1;

                    if (dialogRead == 1)
                    {
                        PlayerPrefs.SetInt("Plansza6", 1);
                    }
                    else if (dialogRead == 4)
                    {
                        fade.FadeAppear(nextLevelName);
                    }

                    break;

                #endregion

                #region Plansza7

                case LevelDialogQueue.Plansza7:
                    dialogRead += 1;

                    if (dialogRead == 1)
                    {
                        PlayerPrefs.SetInt("Plansza7", 1);
                        nameMarkText.text = "Wiktor";
                        allCharactersInScene["Korav'ye"].GetComponent<CharactersController>().isTalking = false;
                    }
                    else if (dialogRead == 2)
                    {
                        nameMarkText.text = "Korav'ye";
                        allCharactersInScene["Korav'ye"].GetComponent<CharactersController>().isTalking = true;
                    }
                    else if (dialogRead == 3)
                    {
                        nameMarkText.text = "Wiktor";
                        allCharactersInScene["Korav'ye"].GetComponent<CharactersController>().isTalking = false;
                    }
                    else if (dialogRead == 4)
                    {
                        nameMarkText.text = "Korav'ye";
                        allCharactersInScene["Korav'ye"].GetComponent<CharactersController>().isTalking = true;
                    }
                    else if (dialogRead == 6)
                    {
                        StartCoroutine(NameMarkDisappear());
                        backgroundElementFade1.ImageAppear();
                        allCharactersInScene["Korav'ye"].GetComponent<CharactersController>().isLeave = true;
                        allCharactersInScene.Remove("Korav'ye");

                        allCharactersInScene.Add("Manya", SpawnCharacter(characters[4], leftSpawnPoint, endLocLeft15, false));
                        allCharactersInScene["Manya"].GetComponent<CharactersController>().isLeftSide = true;

                        allCharactersInScene.Add("Nikolai", SpawnCharacter(characters[3], rightSpawnPoint, endLocRight15, false));
                        allCharactersInScene["Nikolai"].GetComponent<CharactersController>().isLeftSide = false;
                        allCharactersInScene["Nikolai"].GetComponent<CharactersController>().isNewLocationActive = true;

                        allCharactersInScene.Add("Czesław", SpawnCharacter(characters[2], rightSpawnPoint, endLocRight2, false));
                        allCharactersInScene["Czesław"].GetComponent<CharactersController>().isLeftSide = false;

                        allCharactersInScene["Manya"].GetComponent<CharactersController>().SetEmotion(0);
                        allCharactersInScene["Nikolai"].GetComponent<CharactersController>().SetEmotion(0);
                        allCharactersInScene["Czesław"].GetComponent<CharactersController>().SetEmotion(0);
                    }
                    else if (dialogRead == 7)
                    {
                        allCharactersInScene["Nikolai"].GetComponent<CharactersController>().SetEmotion(1);
                    }
                    else if (dialogRead == 8)
                    {
                        nameMarkText.text = "Nikolai Morozov";
                        StartCoroutine(NameMarkAppear());
                        allCharactersInScene["Nikolai"].GetComponent<CharactersController>().isTalking = true;
                    }
                    else if (dialogRead == 9)
                    {
                        nameMarkText.text = "Wiktor";
                        allCharactersInScene["Nikolai"].GetComponent<CharactersController>().isTalking = false;
                    }
                    else if (dialogRead == 10)
                    {
                        nameMarkText.text = "Nikolai Morozov";
                        allCharactersInScene["Nikolai"].GetComponent<CharactersController>().isTalking = true;
                    }
                    else if (dialogRead == 11)
                    {
                        nameMarkText.text = "Manya";
                        allCharactersInScene["Nikolai"].GetComponent<CharactersController>().isTalking = false;
                        allCharactersInScene["Manya"].GetComponent<CharactersController>().isTalking = true;
                    }
                    else if (dialogRead == 12)
                    {
                        nameMarkText.text = "Nikolai Morozov";
                        allCharactersInScene["Manya"].GetComponent<CharactersController>().isTalking = false;
                        allCharactersInScene["Nikolai"].GetComponent<CharactersController>().isTalking = true;
                    }
                    else if (dialogRead == 13)
                    {
                        StartCoroutine(NameMarkDisappear());
                        allCharactersInScene["Nikolai"].GetComponent<CharactersController>().isTalking = false;
                    }
                    else if (dialogRead == 14)
                    {
                        nameMarkText.text = "Wiktor";
                        StartCoroutine(NameMarkAppear());
                    }
                    else if (dialogRead == 15)
                    {
                        nameMarkText.text = "Manya";
                        StartCoroutine(NameMarkAppear());
                        allCharactersInScene["Manya"].GetComponent<CharactersController>().isTalking = true;
                    }
                    else if (dialogRead == 16)
                    {
                        nameMarkText.text = "Wiktor";
                        allCharactersInScene["Manya"].GetComponent<CharactersController>().isTalking = false;
                    }
                    else if (dialogRead == 17)
                    {
                        nameMarkText.text = "Manya";
                        allCharactersInScene["Manya"].GetComponent<CharactersController>().isTalking = true;
                    }
                    else if (dialogRead == 18)
                    {
                        nameMarkText.text = "Wiktor";
                        allCharactersInScene["Manya"].GetComponent<CharactersController>().isTalking = false;
                    }
                    else if (dialogRead == 19)
                    {
                        StartCoroutine(NameMarkDisappear());
                    }
                    else if (dialogRead == 22)
                    {
                        nameMarkText.text = "Profesor Antos";
                        StartCoroutine(NameMarkAppear());
                        allCharactersInScene.Add("Profesor", SpawnCharacter(characters[0], leftSpawnPoint, endLocLeft2, false));
                        allCharactersInScene["Profesor"].GetComponent<CharactersController>().isLeftSide = true;
                        allCharactersInScene["Profesor"].GetComponent<CharactersController>().isTalking = true;
                        allCharactersInScene["Profesor"].GetComponent<CharactersController>().SetEmotion(0);
                        allCharactersInScene["Manya"].GetComponent<CharactersController>().isNewLocationActive = true;
                    }
                    else if (dialogRead == 23)
                    {
                        StartCoroutine(NameMarkDisappear());
                        allCharactersInScene["Profesor"].GetComponent<CharactersController>().isTalking = false;
                    }
                    else if (dialogRead == 24)
                    {
                        nameMarkText.text = "Profesor Antos";
                        StartCoroutine(NameMarkAppear());
                        allCharactersInScene["Profesor"].GetComponent<CharactersController>().isTalking = true;
                    }
                    else if (dialogRead == 25)
                    {
                        nameMarkText.text = "Nikolai Morozov";
                        allCharactersInScene["Profesor"].GetComponent<CharactersController>().isTalking = false;
                        allCharactersInScene["Nikolai"].GetComponent<CharactersController>().isTalking = true;
                    }
                    else if (dialogRead == 26)
                    {
                        nameMarkText.text = "Profesor Antos";
                        allCharactersInScene["Profesor"].GetComponent<CharactersController>().isTalking = true;
                        allCharactersInScene["Nikolai"].GetComponent<CharactersController>().isTalking = false;
                    }
                    else if (dialogRead == 27)
                    {
                        StartCoroutine(NameMarkDisappear());
                        allCharactersInScene["Profesor"].GetComponent<CharactersController>().isTalking = false;
                    }
                    else if (dialogRead == 30)
                    {
                        fade.FadeAppear(nextLevelName);
                    }

                    break;

                #endregion

                #region Plansza8.5

                case LevelDialogQueue.Plansza85:
                    dialogRead += 1;

                    if (dialogRead == 3)
                    {
                        fade.FadeAppear(nextLevelName);
                    }

                    break;

                #endregion

                #region Plansza9

                case LevelDialogQueue.Plansza9:
                    dialogRead += 1;

                    if (dialogRead == 1)
                    {
                        PlayerPrefs.SetInt("Plansza9", 1);
                        nameMarkText.text = "Czesław";
                        StartCoroutine(NameMarkAppear());
                        allCharactersInScene.Add("Czesław", SpawnCharacter(characters[1], rightSpawnPoint, endLocRight15, false));
                        allCharactersInScene["Czesław"].GetComponent<CharactersController>().isTalking = true;
                        allCharactersInScene["Czesław"].GetComponent<CharactersController>().isLeftSide = false;
                        allCharactersInScene["Czesław"].GetComponent<CharactersController>().SetEmotion(4);
                    }
                    else if (dialogRead == 2)
                    {
                        StartCoroutine(NameMarkDisappear());
                        allCharactersInScene["Czesław"].GetComponent<CharactersController>().isTalking = false;
                        allCharactersInScene["Czesław"].GetComponent<CharactersController>().isLeave = true;
                        allCharactersInScene.Remove("Czesław");
                        backgroundElementFade1.ImageAppear();
                    }
                    else if (dialogRead == 4)
                    {
                        nameMarkText.text = "Profesor Antos";
                        StartCoroutine(NameMarkAppear());
                        allCharactersInScene.Add("Profesor", SpawnCharacter(characters[0], leftSpawnPoint, endLocLeft15, false));
                        allCharactersInScene["Profesor"].GetComponent<CharactersController>().isTalking = true;
                        allCharactersInScene["Profesor"].GetComponent<CharactersController>().isLeftSide = true;
                        allCharactersInScene["Profesor"].GetComponent<CharactersController>().SetEmotion(0);
                    }
                    else if (dialogRead == 5)
                    {
                        StartCoroutine(NameMarkDisappear());
                        allCharactersInScene["Profesor"].GetComponent<CharactersController>().isTalking = false;
                    }
                    else if (dialogRead == 9)
                    {
                        nameMarkText.text = "Nikolai Morozov";
                        StartCoroutine(NameMarkAppear());
                        allCharactersInScene.Add("Nikolai", SpawnCharacter(characters[2], rightSpawnPoint, endLocRight15, false));
                        allCharactersInScene["Nikolai"].GetComponent<CharactersController>().isTalking = true;
                        allCharactersInScene["Nikolai"].GetComponent<CharactersController>().isLeftSide = false;
                        allCharactersInScene["Nikolai"].GetComponent<CharactersController>().SetEmotion(0);
                    }
                    else if (dialogRead == 10)
                    {
                        nameMarkText.text = "Manya";
                        allCharactersInScene.Add("Manya", SpawnCharacter(characters[3], leftSpawnPoint, endLocLeft2, false));
                        allCharactersInScene["Manya"].GetComponent<CharactersController>().isTalking = true;
                        allCharactersInScene["Manya"].GetComponent<CharactersController>().isLeftSide = true;
                        allCharactersInScene["Profesor"].GetComponent<CharactersController>().isNewLocationActive = true;
                        allCharactersInScene["Nikolai"].GetComponent<CharactersController>().isTalking = false;
                        allCharactersInScene["Manya"].GetComponent<CharactersController>().SetEmotion(0);
                    }
                    else if (dialogRead == 11)
                    {
                        nameMarkText.text = "Profesor Antos";
                        allCharactersInScene["Profesor"].GetComponent<CharactersController>().isTalking = true;
                        allCharactersInScene["Manya"].GetComponent<CharactersController>().isTalking = false;
                    }
                    else if (dialogRead == 12)
                    {
                        StartCoroutine(NameMarkDisappear());
                        allCharactersInScene["Profesor"].GetComponent<CharactersController>().isTalking = false;
                    }
                    else if (dialogRead == 13)
                    {
                        allCharactersInScene["Manya"].GetComponent<CharactersController>().isLeave = true;
                        allCharactersInScene.Remove("Manya");
                        allCharactersInScene["Nikolai"].GetComponent<CharactersController>().isLeave = true;
                        allCharactersInScene.Remove("Nikolai");
                        allCharactersInScene["Profesor"].GetComponent<CharactersController>().isMiddleReturn = true;
                    }
                    else if (dialogRead == 15)
                    {
                        nameMarkText.text = "Wiktor";
                        StartCoroutine(NameMarkAppear());
                    }
                    else if (dialogRead == 16)
                    {
                        StartCoroutine(NameMarkDisappear());
                    }
                    else if (dialogRead == 17)
                    {
                        nameMarkText.text = "Profesor Antos";
                        StartCoroutine(NameMarkAppear());
                        allCharactersInScene["Profesor"].GetComponent<CharactersController>().isTalking = true;
                        allCharactersInScene["Profesor"].GetComponent<CharactersController>().SetEmotion(7);
                    }
                    else if (dialogRead == 18)
                    {
                        nameMarkText.text = "Wiktor";
                        allCharactersInScene["Profesor"].GetComponent<CharactersController>().isTalking = false;
                    }
                    else if (dialogRead == 19)
                    {
                        StartCoroutine(NameMarkDisappear());
                    }
                    else if (dialogRead == 20)
                    {
                        nameMarkText.text = "Profesor Antos";
                        StartCoroutine(NameMarkAppear());
                        allCharactersInScene["Profesor"].GetComponent<CharactersController>().isTalking = true;
                    }
                    else if (dialogRead == 21)
                    {
                        allCharactersInScene["Profesor"].GetComponent<CharactersController>().SetEmotion(0);
                    }
                    else if (dialogRead == 22)
                    {
                        StartCoroutine(NameMarkDisappear());
                        allCharactersInScene["Profesor"].GetComponent<CharactersController>().isTalking = false;
                    }
                    else if (dialogRead == 24)
                    {
                        nameMarkText.text = "Profesor Antos";
                        StartCoroutine(NameMarkAppear());
                        allCharactersInScene["Profesor"].GetComponent<CharactersController>().isTalking = true;
                    }
                    else if (dialogRead == 25)
                    {
                        nameMarkText.text = "Wiktor";
                        allCharactersInScene["Profesor"].GetComponent<CharactersController>().isTalking = false;
                    }
                    else if (dialogRead == 26)
                    {
                        StartCoroutine(NameMarkDisappear());
                        allCharactersInScene["Profesor"].GetComponent<CharactersController>().isTalking = false;
                    }
                    else if (dialogRead == 27)
                    {
                        nameMarkText.text = "Nikolai Morozov";
                        StartCoroutine(NameMarkAppear());
                        allCharactersInScene.Add("Nikolai", SpawnCharacter(characters[2], rightSpawnPoint, endLocRight15, false));
                        allCharactersInScene["Nikolai"].GetComponent<CharactersController>().isTalking = true;
                        allCharactersInScene["Nikolai"].GetComponent<CharactersController>().isLeftSide = false;
                        allCharactersInScene["Nikolai"].GetComponent<CharactersController>().SetEmotion(0);
                    }
                    else if (dialogRead == 28)
                    {
                        nameMarkText.text = "Wiktor";
                        allCharactersInScene["Nikolai"].GetComponent<CharactersController>().isTalking = false;
                    }
                    else if (dialogRead == 29)
                    {
                        nameMarkText.text = "Nikolai Morozov";
                        allCharactersInScene["Nikolai"].GetComponent<CharactersController>().isTalking = true;
                    }
                    else if (dialogRead == 30)
                    {
                        StartCoroutine(NameMarkDisappear());
                        allCharactersInScene.Add("Czukcz", SpawnCharacter(characters[4], rightSpawnPoint, endLocRight2, false));
                        allCharactersInScene["Czukcz"].GetComponent<CharactersController>().isLeftSide = false;
                        allCharactersInScene["Nikolai"].GetComponent<CharactersController>().isTalking = false;
                        allCharactersInScene["Czukcz"].GetComponent<CharactersController>().isAlternativeCloth = true;
                        allCharactersInScene["Czukcz"].GetComponent<CharactersController>().SetEmotion(0);
                        allCharactersInScene["Nikolai"].GetComponent<CharactersController>().isNewLocationActive = true;
                        backgroundElementFade1.ImageDisappear();
                    }
                    else if (dialogRead == 31)
                    {
                        nameMarkText.text = "Korav'ye";
                        StartCoroutine(NameMarkAppear());
                        allCharactersInScene["Czukcz"].GetComponent<CharactersController>().isTalking = true;
                        allCharactersInScene["Czukcz"].GetComponent<CharactersController>().SetEmotion(3);
                    }
                    else if (dialogRead == 32)
                    {
                        nameMarkText.text = "Nikolai Morozov";
                        allCharactersInScene["Nikolai"].GetComponent<CharactersController>().isTalking = true;
                        allCharactersInScene["Czukcz"].GetComponent<CharactersController>().isTalking = false;
                        allCharactersInScene["Nikolai"].GetComponent<CharactersController>().SetEmotion(4);
                    }
                    else if (dialogRead == 33)
                    {
                        StartCoroutine(NameMarkDisappear());
                        allCharactersInScene["Nikolai"].GetComponent<CharactersController>().isTalking = false;
                    }
                    else if (dialogRead == 34)
                    {
                        nameMarkText.text = "Profesor Antos";
                        StartCoroutine(NameMarkAppear());
                        allCharactersInScene["Profesor"].GetComponent<CharactersController>().isTalking = true;
                    }
                    else if (dialogRead == 35)
                    {
                        nameMarkText.text = "Korav'ye";
                        allCharactersInScene["Czukcz"].GetComponent<CharactersController>().isTalking = true;
                        allCharactersInScene["Profesor"].GetComponent<CharactersController>().isTalking = false;
                        allCharactersInScene["Czukcz"].GetComponent<CharactersController>().SetEmotion(0);
                    }
                    else if (dialogRead == 36)
                    {
                        nameMarkText.text = "Wiktor";
                        allCharactersInScene["Czukcz"].GetComponent<CharactersController>().isTalking = false;
                    }
                    else if (dialogRead == 37)
                    {
                        StartCoroutine(NameMarkDisappear());
                    }
                    else if (dialogRead == 38)
                    {
                        nameMarkText.text = "Profesor Antos";
                        StartCoroutine(NameMarkAppear());
                        allCharactersInScene["Profesor"].GetComponent<CharactersController>().isTalking = true;
                    }
                    else if (dialogRead == 39)
                    {
                        StartCoroutine(NameMarkDisappear());
                        allCharactersInScene["Profesor"].GetComponent<CharactersController>().isTalking = false;
                    }
                    else if (dialogRead == 40)
                    {
                        nameMarkText.text = "Korav'ye";
                        StartCoroutine(NameMarkAppear());
                        allCharactersInScene["Czukcz"].GetComponent<CharactersController>().isTalking = true;
                        allCharactersInScene["Czukcz"].GetComponent<CharactersController>().SetEmotion(3);
                    }
                    else if (dialogRead == 41)
                    {
                        StartCoroutine(NameMarkDisappear());
                        allCharactersInScene["Czukcz"].GetComponent<CharactersController>().isTalking = false;
                    }
                    else if (dialogRead == 42)
                    {
                        nameMarkText.text = "Wiktor";
                        StartCoroutine(NameMarkAppear());
                    }
                    else if (dialogRead == 43)
                    {
                        StartCoroutine(NameMarkDisappear());
                        allCharactersInScene["Czukcz"].GetComponent<CharactersController>().SetEmotion(1);
                    }
                    else if (dialogRead == 44)
                    {
                        nameMarkText.text = "Nikolai Morozov";
                        StartCoroutine(NameMarkAppear());
                        allCharactersInScene["Nikolai"].GetComponent<CharactersController>().isTalking = true;
                    }
                    else if (dialogRead == 45)
                    {
                        StartCoroutine(NameMarkDisappear());
                        allCharactersInScene["Nikolai"].GetComponent<CharactersController>().isTalking = false;
                        allCharactersInScene["Czukcz"].GetComponent<CharactersController>().SetEmotion(0);
                    }
                    else if (dialogRead == 46)
                    {
                        nameMarkText.text = "Korav'ye";
                        StartCoroutine(NameMarkAppear());
                        allCharactersInScene["Czukcz"].GetComponent<CharactersController>().isTalking = true;
                    }
                    else if (dialogRead == 47)
                    {
                        allCharactersInScene["Czukcz"].GetComponent<CharactersController>().isTalking = false;
                        fade.FadeAppear(nextLevelName);
                    }

                    break;

                #endregion

                #region Plansza10

                case LevelDialogQueue.Plansza10:
                    dialogRead += 1;

                    if (dialogRead == 1)
                    {
                        PlayerPrefs.SetInt("Plansza10", 1);
                    }
                    else if (dialogRead == 3)
                    {
                        nameMarkText.text = "Wiktor";
                        StartCoroutine(NameMarkAppear());
                    }
                    else if (dialogRead == 4)
                    {
                        ChooseAppearTriple("Wspominałeś, że dostałeś od szamana jakieś instrukcje...", "Byłeś tu wcześniej?", "Widziałem medalion, który wręczył ci Aleleke.");
                        nameMarkText.text = "Korav'ye";
                        allCharactersInScene.Add("Czukcz", SpawnCharacter(characters[0], leftSpawnPoint, endLocLeft15, true));
                        allCharactersInScene["Czukcz"].GetComponent<CharactersController>().isLeftSide = true;
                        allCharactersInScene["Czukcz"].GetComponent<CharactersController>().isTalking = true;
                        allCharactersInScene["Czukcz"].GetComponent<CharactersController>().isAlternativeCloth = true;
                        allCharactersInScene["Czukcz"].GetComponent<CharactersController>().SetEmotion(3);
                    }
                    else if (dialogRead == 6)
                    {
                        nameMarkText.text = "Wiktor";
                        allCharactersInScene["Czukcz"].GetComponent<CharactersController>().isTalking = false;
                    }
                    else if (dialogRead == 7)
                    {
                        StartCoroutine(NameMarkDisappear());
                        ChooseAppearTriple("Wspominałeś, że dostałeś od szamana jakieś instrukcje...", "Byłeś tu wcześniej?", "Widziałem medalion, który wręczył ci Aleleke.");
                    }
                    else if (dialogRead == 9)
                    {
                        StartCoroutine(NameMarkDisappear());
                        allCharactersInScene["Czukcz"].GetComponent<CharactersController>().isTalking = false;
                    }
                    else if (dialogRead == 10)
                    {
                        nameMarkText.text = "Wiktor";
                        StartCoroutine(NameMarkAppear());
                    }
                    else if (dialogRead == 11)
                    {
                        nameMarkText.text = "Korav'ye";
                        allCharactersInScene["Czukcz"].GetComponent<CharactersController>().isTalking = true;
                        ChooseAppearTriple("Wspominałeś, że dostałeś od szamana jakieś instrukcje...", "Byłeś tu wcześniej?", "Widziałem medalion, który wręczył ci Aleleke.");
                    }
                    else if (dialogRead == 13)
                    {
                        StartCoroutine(NameMarkDisappear());
                        allCharactersInScene["Czukcz"].GetComponent<CharactersController>().isTalking = false;
                    }
                    else if (dialogRead == 14)
                    {
                        nameMarkText.text = "Korav'ye";
                        StartCoroutine(NameMarkAppear());
                        allCharactersInScene["Czukcz"].GetComponent<CharactersController>().isTalking = true;
                    }
                    else if (dialogRead == 15)
                    {
                        StartCoroutine(NameMarkDisappear());
                        allCharactersInScene["Czukcz"].GetComponent<CharactersController>().isTalking = false;
                    }
                    else if (dialogRead == 16)
                    {
                        nameMarkText.text = "Wiktor";
                        StartCoroutine(NameMarkAppear());
                    }
                    else if (dialogRead == 17)
                    {
                        StartCoroutine(NameMarkDisappear());
                    }
                    else if (dialogRead == 18)
                    {
                        nameMarkText.text = "Korav'ye";
                        StartCoroutine(NameMarkAppear());
                        allCharactersInScene["Czukcz"].GetComponent<CharactersController>().isTalking = true;
                        ChooseAppearTriple("Wspominałeś, że dostałeś od szamana instrukcje...", "Byłeś tu wcześniej?", "Widziałem medalion, który wręczył ci Aleleke.");
                    }
                    else if (dialogRead == 19)
                    {
                        nameMarkText.text = "Wiktor";
                        StartCoroutine(NameMarkAppear());
                        allCharactersInScene["Czukcz"].GetComponent<CharactersController>().isTalking = false;
                    }
                    else if (dialogRead == 20)
                    {
                        nameMarkText.text = "Korav'ye";
                        allCharactersInScene["Czukcz"].GetComponent<CharactersController>().isTalking = true;
                        allCharactersInScene["Czukcz"].GetComponent<CharactersController>().SetEmotion(0);
                    }
                    else if (dialogRead == 21)
                    {
                        nameMarkText.text = "Wiktor";
                        allCharactersInScene["Czukcz"].GetComponent<CharactersController>().isTalking = false;
                    }
                    else if (dialogRead == 22)
                    {
                        StartCoroutine(NameMarkDisappear());
                    }
                    else if (dialogRead == 23)
                    {
                        nameMarkText.text = "Korav'ye";
                        StartCoroutine(NameMarkAppear());
                        allCharactersInScene["Czukcz"].GetComponent<CharactersController>().isTalking = true;
                    }
                    else if (dialogRead == 24)
                    {
                        nameMarkText.text = "Wiktor";
                        allCharactersInScene["Czukcz"].GetComponent<CharactersController>().isTalking = false;
                    }
                    else if (dialogRead == 25)
                    {
                        nameMarkText.text = "Korav'ye";
                        allCharactersInScene["Czukcz"].GetComponent<CharactersController>().isTalking = true;
                        allCharactersInScene["Czukcz"].GetComponent<CharactersController>().SetEmotion(3);
                    }
                    else if (dialogRead == 26)
                    {
                        StartCoroutine(NameMarkDisappear());
                        allCharactersInScene["Czukcz"].GetComponent<CharactersController>().isTalking = false;
                    }
                    else if (dialogRead == 27)
                    {
                        nameMarkText.text = "Wiktor";
                        StartCoroutine(NameMarkAppear());
                    }
                    else if (dialogRead == 28)
                    {
                        nameMarkText.text = "Korav'ye";
                        allCharactersInScene["Czukcz"].GetComponent<CharactersController>().isTalking = true;
                    }
                    else if (dialogRead == 29)
                    {
                        StartCoroutine(NameMarkDisappear());
                        allCharactersInScene["Czukcz"].GetComponent<CharactersController>().isTalking = false;
                    }
                    else if (dialogRead == 30)
                    {
                        nameMarkText.text = "Korav'ye";
                        StartCoroutine(NameMarkAppear());
                        allCharactersInScene["Czukcz"].GetComponent<CharactersController>().isTalking = true;
                        allCharactersInScene["Czukcz"].GetComponent<CharactersController>().SetEmotion(0);
                    }
                    else if (dialogRead == 31)
                    {
                        allCharactersInScene["Czukcz"].GetComponent<CharactersController>().isTalking = false;
                        fade.FadeAppear(nextLevelName);
                    }

                    break;

                #endregion

                #region Plansza11

                case LevelDialogQueue.Plansza11:
                    dialogRead += 1;

                    if (dialogRead == 1)
                    {
                        PlayerPrefs.SetInt("Plansza11", 1);
                        nameMarkText.text = "Nikolai Morozov";
                        StartCoroutine(NameMarkAppear());
                        allCharactersInScene.Add("Nikolai", SpawnCharacter(characters[0], rightSpawnPoint, endLocRight15, false));
                        allCharactersInScene["Nikolai"].GetComponent<CharactersController>().isTalking = true;
                        allCharactersInScene["Nikolai"].GetComponent<CharactersController>().isLeftSide = false;
                        allCharactersInScene["Nikolai"].GetComponent<CharactersController>().SetEmotion(1);
                    }
                    else if (dialogRead == 2)
                    {
                        nameMarkText.text = "Profesor Antos";
                        allCharactersInScene.Add("Profesor", SpawnCharacter(characters[1], leftSpawnPoint, endLocLeft15, false));
                        allCharactersInScene["Profesor"].GetComponent<CharactersController>().isTalking = true;
                        allCharactersInScene["Profesor"].GetComponent<CharactersController>().isLeftSide = true;
                        allCharactersInScene["Nikolai"].GetComponent<CharactersController>().isTalking = false;
                        allCharactersInScene["Profesor"].GetComponent<CharactersController>().SetEmotion(5);
                    }
                    else if (dialogRead == 3)
                    {
                        nameMarkText.text = "Nikolai Morozov";
                        allCharactersInScene["Nikolai"].GetComponent<CharactersController>().isTalking = true;
                        allCharactersInScene["Profesor"].GetComponent<CharactersController>().isTalking = false;
                    }
                    else if (dialogRead == 4)
                    {
                        nameMarkText.text = "Profesor Antos";
                        allCharactersInScene["Nikolai"].GetComponent<CharactersController>().isTalking = false;
                        allCharactersInScene["Profesor"].GetComponent<CharactersController>().isTalking = true;
                    }
                    else if (dialogRead == 5)
                    {
                        nameMarkText.text = "Nikolai Morozov";
                        allCharactersInScene["Nikolai"].GetComponent<CharactersController>().isTalking = true;
                        allCharactersInScene["Profesor"].GetComponent<CharactersController>().isTalking = false;
                    }
                    else if (dialogRead == 6)
                    {
                        nameMarkText.text = "Profesor Antos";
                        allCharactersInScene["Nikolai"].GetComponent<CharactersController>().isTalking = false;
                        allCharactersInScene["Profesor"].GetComponent<CharactersController>().isTalking = true;
                    }
                    else if (dialogRead == 7)
                    {
                        nameMarkText.text = "Nikolai Morozov";
                        allCharactersInScene["Nikolai"].GetComponent<CharactersController>().isTalking = true;
                        allCharactersInScene["Profesor"].GetComponent<CharactersController>().isTalking = false;
                        allCharactersInScene["Nikolai"].GetComponent<CharactersController>().SetEmotion(0);
                    }
                    else if (dialogRead == 8)
                    {
                        StartCoroutine(NameMarkDisappear());
                        allCharactersInScene["Nikolai"].GetComponent<CharactersController>().isTalking = false;
                    }
                    else if (dialogRead == 9)
                    {
                        nameMarkText.text = "Korav'ye";
                        StartCoroutine(NameMarkAppear());
                        allCharactersInScene.Add("Czucz", SpawnCharacter(characters[2], rightSpawnPoint, endLocRight2, false));
                        allCharactersInScene["Czucz"].GetComponent<CharactersController>().isTalking = true;
                        allCharactersInScene["Czucz"].GetComponent<CharactersController>().isLeftSide = false;
                        allCharactersInScene["Czucz"].GetComponent<CharactersController>().SetEmotion(0);
                        allCharactersInScene["Nikolai"].GetComponent<CharactersController>().isNewLocationActive = true;
                    }
                    else if (dialogRead == 10)
                    {
                        nameMarkText.text = "Manya";
                        allCharactersInScene.Add("Manya", SpawnCharacter(characters[3], leftSpawnPoint, endLocLeft2, false));
                        allCharactersInScene["Manya"].GetComponent<CharactersController>().isTalking = true;
                        allCharactersInScene["Manya"].GetComponent<CharactersController>().isLeftSide = true;
                        allCharactersInScene["Manya"].GetComponent<CharactersController>().SetEmotion(0);
                        allCharactersInScene["Profesor"].GetComponent<CharactersController>().isNewLocationActive = true;
                        allCharactersInScene["Czucz"].GetComponent<CharactersController>().isTalking = false;
                    }
                    else if (dialogRead == 11)
                    {
                        nameMarkText.text = "Profesor Antos";
                        allCharactersInScene["Profesor"].GetComponent<CharactersController>().isTalking = true;
                        allCharactersInScene["Profesor"].GetComponent<CharactersController>().SetEmotion(0);
                        allCharactersInScene["Manya"].GetComponent<CharactersController>().isTalking = false;
                    }
                    else if (dialogRead == 12)
                    {
                        StartCoroutine(NameMarkDisappear());
                        allCharactersInScene["Profesor"].GetComponent<CharactersController>().isTalking = false;
                    }
                    else if (dialogRead == 14)
                    {
                        fade.FadeAppear(nextLevelName);
                    }

                    break;

                #endregion

                #region Plansza12

                case LevelDialogQueue.Plansza12:
                    dialogRead += 1;

                    if (dialogRead == 1)
                    {
                        PlayerPrefs.SetInt("Plansza12", 1);
                    }
                    else if (dialogRead == 2)
                    {
                        nameMarkText.text = "Profesor Antos";
                        StartCoroutine(NameMarkAppear());
                    }
                    else if (dialogRead == 3)
                    {
                        StartCoroutine(NameMarkDisappear());
                    }
                    else if (dialogRead == 5)
                    {
                        nameMarkText.text = "Wiktor";
                        StartCoroutine(NameMarkAppear());
                    }
                    else if (dialogRead == 6)
                    {
                        nameMarkText.text = "Profesor Antos";
                    }
                    else if (dialogRead == 7)
                    {
                        StartCoroutine(NameMarkDisappear());
                    }
                    else if (dialogRead == 8)
                    {
                        nameMarkText.text = "Nikolai Morozov";
                        StartCoroutine(NameMarkAppear());
                    }
                    else if (dialogRead == 9)
                    {
                        StartCoroutine(NameMarkDisappear());
                    }
                    else if (dialogRead == 10)
                    {
                        nameMarkText.text = "Nikolai Morozov";
                        StartCoroutine(NameMarkAppear());
                    }
                    else if (dialogRead == 11)
                    {
                        StartCoroutine(NameMarkDisappear());
                    }
                    else if (dialogRead == 13)
                    {
                        nameMarkText.text = "Wiktor";
                        StartCoroutine(NameMarkAppear());
                    }
                    else if (dialogRead == 14)
                    {
                        StartCoroutine(NameMarkDisappear());
                    }
                    else if (dialogRead == 15)
                    {
                        nameMarkText.text = "Nikolai Morozov";
                        StartCoroutine(NameMarkAppear());
                        ChooseAppear("Dlaczego tak nie znosisz Korav’ye?", "Co myślisz o wyprawie?");
                    }
                    else if (dialogRead == 17)
                    {
                        nameMarkText.text = "Wiktor";
                    }
                    else if (dialogRead == 18)
                    {
                        nameMarkText.text = "Nikolai Morozov";
                        dialogRead = 24;
                    }
                    else if (dialogRead == 20)
                    {
                        nameMarkText.text = "Wiktor";
                    }
                    else if (dialogRead == 21)
                    {
                        nameMarkText.text = "Nikolai Morozov";
                    }
                    else if (dialogRead == 22)
                    {
                        nameMarkText.text = "Wiktor";
                    }
                    else if (dialogRead == 23)
                    {
                        StartCoroutine(NameMarkDisappear());
                    }
                    else if (dialogRead == 24)
                    {
                        nameMarkText.text = "Nikolai Morozov";
                        StartCoroutine(NameMarkAppear());
                    }
                    else if (dialogRead == 25)
                    {
                        StartCoroutine(NameMarkDisappear());
                    }
                    else if (dialogRead == 26)
                    {
                        nameMarkText.text = "Manya";
                        StartCoroutine(NameMarkAppear());
                    }
                    else if (dialogRead == 27)
                    {
                        StartCoroutine(NameMarkDisappear());
                    }
                    else if (dialogRead == 29)
                    {
                        nameMarkText.text = "Profesor Antos";
                        StartCoroutine(NameMarkAppear());
                    }
                    else if (dialogRead == 30)
                    {
                        nameMarkText.text = "Czesław";
                    }
                    else if (dialogRead == 31)
                    {
                        nameMarkText.text = "Profesor Antos";
                    }
                    else if (dialogRead == 32)
                    {
                        nameMarkText.text = "Manya";
                    }
                    else if (dialogRead == 33)
                    {
                        nameMarkText.text = "Korav'ye";
                    }
                    else if (dialogRead == 34)
                    {
                        nameMarkText.text = "Profesor Antos";
                    }
                    else if (dialogRead == 35)
                    {
                        StartCoroutine(NameMarkDisappear());
                    }
                    else if (dialogRead == 36)
                    {
                        nameMarkText.text = "Czesław";
                        StartCoroutine(NameMarkAppear());
                    }
                    else if (dialogRead == 37)
                    {
                        StartCoroutine(NameMarkDisappear());
                    }
                    else if (dialogRead == 41)
                    {
                        nameMarkText.text = "Wiktor";
                        StartCoroutine(NameMarkAppear());
                    }
                    else if (dialogRead == 42)
                    {
                        StartCoroutine(NameMarkDisappear());
                    }
                    else if (dialogRead == 43)
                    {
                        nameMarkText.text = "Wiktor";
                        StartCoroutine(NameMarkAppear());
                    }
                    else if (dialogRead == 44)
                    {
                        nameMarkText.text = "Profesor Antos";
                    }
                    else if (dialogRead == 45)
                    {
                        StartCoroutine(NameMarkDisappear());
                    }
                    else if (dialogRead == 46)
                    {
                        nameMarkText.text = "Profesor Antos";
                        StartCoroutine(NameMarkAppear());
                    }
                    else if (dialogRead == 47)
                    {
                        nameMarkText.text = "Manya";
                    }
                    else if (dialogRead == 48)
                    {
                        StartCoroutine(NameMarkDisappear());
                    }
                    else if (dialogRead == 49)
                    {
                        nameMarkText.text = "Wiktor";
                        StartCoroutine(NameMarkAppear());
                    }
                    else if (dialogRead == 50)
                    {
                        nameMarkText.text = "Nikolai Morozov";
                    }
                    else if (dialogRead == 51)
                    {
                        StartCoroutine(NameMarkDisappear());
                    }
                    else if (dialogRead == 52)
                    {
                        nameMarkText.text = "Profesor Antos";
                        StartCoroutine(NameMarkAppear());
                    }
                    else if (dialogRead == 53)
                    {
                        StartCoroutine(NameMarkDisappear());
                    }
                    else if (dialogRead == 56)
                    {
                        fade.BlackFadeAppear(true, nextLevelName);
                    }

                    break;

                #endregion

                #region Plansza13

                case LevelDialogQueue.Plansza13:
                    dialogRead += 1;

                    if (dialogRead == 1)
                    {
                        PlayerPrefs.SetInt("Plansza13", 1);
                    }
                    else if (dialogRead == 2)
                    {
                        nameMarkText.text = "Wiktor";
                        StartCoroutine(NameMarkAppear());
                    }
                    else if (dialogRead == 3)
                    {
                        StartCoroutine(NameMarkDisappear());
                    }
                    else if (dialogRead == 7)
                    {
                        nameMarkText.text = "Korav'ye";
                        StartCoroutine(NameMarkAppear());
                    }
                    else if (dialogRead == 8)
                    {
                        nameMarkText.text = "Wiktor";
                    }
                    else if (dialogRead == 9)
                    {
                        StartCoroutine(NameMarkDisappear());
                    }
                    else if (dialogRead == 10)
                    {
                        fade.FadeAppear(nextLevelName);
                    }

                    break;

                #endregion

                #region Plansza14

                case LevelDialogQueue.Plansza14:
                    dialogRead += 1;

                    if (dialogRead == 1)
                    {
                        PlayerPrefs.SetInt("Plansza14", 1);
                        StartCoroutine(NameMarkAppear());
                        nameMarkText.text = "Korav'ye";
                    }
                    else if (dialogRead == 2)
                    {
                        StartCoroutine(NameMarkDisappear());
                    }
                    else if (dialogRead == 5)
                    {
                        StartCoroutine(NameMarkAppear());
                        nameMarkText.text = "Profesor Antos";
                    }
                    else if (dialogRead == 6)
                    {
                        StartCoroutine(NameMarkDisappear());
                    }
                    else if (dialogRead == 7)
                    {
                        StartCoroutine(NameMarkAppear());
                        nameMarkText.text = "Wiktor";
                    }
                    else if (dialogRead == 8)
                    {
                        nameMarkText.text = "Nikolai Morozov";
                    }
                    else if (dialogRead == 9)
                    {
                        nameMarkText.text = "Wiktor";
                    }
                    else if (dialogRead == 10)
                    {
                        nameMarkText.text = "Nikolai Morozov";
                    }
                    else if (dialogRead == 11)
                    {
                        StartCoroutine(NameMarkDisappear());
                    }
                    else if (dialogRead == 13)
                    {
                        StartCoroutine(NameMarkAppear());
                        nameMarkText.text = "Profesor Antos";
                    }
                    else if (dialogRead == 14)
                    {
                        nameMarkText.text = "Manya";
                    }
                    else if (dialogRead == 15)
                    {
                        nameMarkText.text = "Profesor Antos";
                    }
                    else if (dialogRead == 16)
                    {
                        nameMarkText.text = "Nikolai Morozov";
                    }
                    else if (dialogRead == 17)
                    {
                        nameMarkText.text = "Profesor Antos";
                    }
                    else if (dialogRead == 18)
                    {
                        StartCoroutine(NameMarkDisappear());
                    }
                    else if (dialogRead == 21)
                    {
                        StartCoroutine(NameMarkAppear());
                        nameMarkText.text = "Manya";
                    }
                    else if (dialogRead == 22)
                    {
                        nameMarkText.text = "Nikolai Morozov";
                    }
                    else if (dialogRead == 23)
                    {
                        StartCoroutine(NameMarkDisappear());
                    }
                    else if (dialogRead == 24)
                    {
                        StartCoroutine(NameMarkAppear());
                        nameMarkText.text = "Manya";
                    }
                    else if (dialogRead == 25)
                    {
                        nameMarkText.text = "Nikolai Morozov";
                    }
                    else if (dialogRead == 26)
                    {
                        nameMarkText.text = "Manya";
                    }
                    else if (dialogRead == 27)
                    {
                        nameMarkText.text = "Nikolai Morozov";
                    }
                    else if (dialogRead == 28)
                    {
                        StartCoroutine(NameMarkDisappear());
                    }
                    else if (dialogRead == 29)
                    {
                        StartCoroutine(NameMarkAppear());
                        nameMarkText.text = "Nikolai Morozov";
                    }
                    else if (dialogRead == 30)
                    {
                        StartCoroutine(NameMarkDisappear());
                    }
                    else if (dialogRead == 31)
                    {
                        StartCoroutine(NameMarkAppear());
                        nameMarkText.text = "Nikolai Morozov";
                    }
                    else if (dialogRead == 32)
                    {
                        nameMarkText.text = "Profesor Antos";
                    }
                    else if (dialogRead == 33)
                    {
                        StartCoroutine(NameMarkDisappear());
                    }
                    else if (dialogRead == 36)
                    {
                        fade.BlackFadeAppear(true, nextLevelName);
                    }

                    break;

                #endregion

                #region Plansza15

                case LevelDialogQueue.Plansza15:
                    dialogRead += 1;

                    if (dialogRead == 1)
                    {
                        PlayerPrefs.SetInt("Plansza15", 1);
                        StartCoroutine(NameMarkAppear());
                        nameMarkText.text = "Manya";
                    }
                    else if (dialogRead == 2)
                    {
                        nameMarkText.text = "Nikolai Morozov";
                    }
                    else if (dialogRead == 3)
                    {
                        StartCoroutine(NameMarkDisappear());
                    }
                    else if (dialogRead == 4)
                    {
                        StartCoroutine(NameMarkAppear());
                        nameMarkText.text = "Manya";
                    }
                    else if (dialogRead == 5)
                    {
                        nameMarkText.text = "Nikolai Morozov";
                    }
                    else if (dialogRead == 6)
                    {
                        StartCoroutine(NameMarkDisappear());
                    }
                    else if (dialogRead == 7)
                    {
                        StartCoroutine(NameMarkAppear());
                        nameMarkText.text = "Nikolai Morozov";
                    }
                    else if (dialogRead == 8)
                    {
                        nameMarkText.text = "Profesor Antos";
                    }
                    else if (dialogRead == 9)
                    {
                        StartCoroutine(NameMarkDisappear());
                    }
                    else if (dialogRead == 10)
                    {
                        StartCoroutine(NameMarkAppear());
                        nameMarkText.text = "Nikolai Morozov";
                    }
                    else if (dialogRead == 11)
                    {
                        StartCoroutine(NameMarkDisappear());
                    }
                    else if (dialogRead == 12)
                    {
                        StartCoroutine(NameMarkAppear());
                        nameMarkText.text = "Profesor Antos";
                    }
                    else if (dialogRead == 13)
                    {
                        nameMarkText.text = "Nikolai Morozov";
                    }
                    else if (dialogRead == 14)
                    {
                        StartCoroutine(NameMarkDisappear());
                    }
                    else if (dialogRead == 19)
                    {
                        fade.BlackFadeAppear(true, nextLevelName);
                    }

                    break;

                #endregion

                #region Plansza16

                case LevelDialogQueue.Plansza16:
                    dialogRead += 1;

                    if (dialogRead == 1)
                    {
                        PlayerPrefs.SetInt("Plansza16", 1);
                    }
                    else if (dialogRead == 4)
                    {
                        StartCoroutine(NameMarkAppear());
                        nameMarkText.text = "Profesor Antos";
                        allCharactersInScene.Add("Profesor", SpawnCharacter(characters[0], leftSpawnPoint, endLocLeft15, false));
                        allCharactersInScene["Profesor"].GetComponent<CharactersController>().isTalking = true;
                        allCharactersInScene["Profesor"].GetComponent<CharactersController>().isLeftSide = true;
                    }
                    else if (dialogRead == 5)
                    {
                        StartCoroutine(NameMarkDisappear());
                        allCharactersInScene["Profesor"].GetComponent<CharactersController>().isTalking = false;
                    }
                    else if (dialogRead == 7)
                    {
                        StartCoroutine(NameMarkAppear());
                        nameMarkText.text = "Manya";
                        allCharactersInScene.Add("Manya", SpawnCharacter(characters[1], leftSpawnPoint, endLocLeft2, false));
                        allCharactersInScene["Manya"].GetComponent<CharactersController>().isTalking = true;
                        allCharactersInScene["Manya"].GetComponent<CharactersController>().isLeftSide = true;
                        allCharactersInScene["Profesor"].GetComponent<CharactersController>().isNewLocationActive = true;
                    }
                    else if (dialogRead == 8)
                    {
                        StartCoroutine(NameMarkDisappear());
                        allCharactersInScene["Manya"].GetComponent<CharactersController>().isTalking = false;
                    }
                    else if (dialogRead == 9)
                    {
                        StartCoroutine(NameMarkAppear());
                        nameMarkText.text = "Nikolai Morozov";
                        allCharactersInScene.Add("Nikolai", SpawnCharacter(characters[2], rightSpawnPoint, endLocRight15, false));
                        allCharactersInScene["Nikolai"].GetComponent<CharactersController>().isTalking = true;
                        allCharactersInScene["Nikolai"].GetComponent<CharactersController>().isLeftSide = false;
                    }
                    else if (dialogRead == 10)
                    {
                        StartCoroutine(NameMarkDisappear());
                        allCharactersInScene["Nikolai Morozov"].GetComponent<CharactersController>().isTalking = false;
                    }
                    else if (dialogRead == 11)
                    {
                        StartCoroutine(NameMarkAppear());
                        nameMarkText.text = "Manya";
                        allCharactersInScene["Manya"].GetComponent<CharactersController>().isTalking = true;
                    }
                    else if (dialogRead == 12)
                    {
                        StartCoroutine(NameMarkDisappear());
                        allCharactersInScene["Manya"].GetComponent<CharactersController>().isTalking = false;
                    }
                    else if (dialogRead == 13)
                    {
                        StartCoroutine(NameMarkAppear());
                        nameMarkText.text = "Manya";
                        allCharactersInScene["Manya"].GetComponent<CharactersController>().isTalking = true;
                    }
                    else if (dialogRead == 14)
                    {
                        nameMarkText.text = "Profesor Antos";
                        allCharactersInScene["Manya"].GetComponent<CharactersController>().isTalking = false;
                        allCharactersInScene["Profesor"].GetComponent<CharactersController>().isTalking = true;
                    }
                    else if (dialogRead == 15)
                    {
                        StartCoroutine(NameMarkDisappear());
                        allCharactersInScene["Profesor"].GetComponent<CharactersController>().isTalking = false;
                    }
                    else if (dialogRead == 16)
                    {
                        StartCoroutine(NameMarkAppear());
                        nameMarkText.text = "Nikolai Morozov";
                        allCharactersInScene["Nikolai"].GetComponent<CharactersController>().isTalking = true;
                    }
                    else if (dialogRead == 17)
                    {
                        StartCoroutine(NameMarkDisappear());
                        allCharactersInScene["Nikolai"].GetComponent<CharactersController>().isTalking = false;
                    }
                    else if (dialogRead == 18)
                    {
                        StartCoroutine(NameMarkAppear());
                        nameMarkText.text = "Profesor Antos";
                        allCharactersInScene["Profesor"].GetComponent<CharactersController>().isTalking = true;
                    }
                    else if (dialogRead == 19)
                    {
                        nameMarkText.text = "Nikolai Morozov";
                        allCharactersInScene["Nikolai"].GetComponent<CharactersController>().isTalking = true;
                        allCharactersInScene["Profesor"].GetComponent<CharactersController>().isTalking = false;
                    }
                    else if (dialogRead == 20)
                    {
                        StartCoroutine(NameMarkDisappear());
                        allCharactersInScene["Nikolai"].GetComponent<CharactersController>().isTalking = false;
                    }
                    else if (dialogRead == 21)
                    {
                        StartCoroutine(NameMarkAppear());
                        nameMarkText.text = "Manya";
                        allCharactersInScene["Manya"].GetComponent<CharactersController>().isTalking = true;
                    }
                    else if (dialogRead == 22)
                    {
                        nameMarkText.text = "Wiktor";
                        allCharactersInScene["Manya"].GetComponent<CharactersController>().isTalking = false;
                    }
                    else if (dialogRead == 23)
                    {
                        StartCoroutine(NameMarkDisappear());
                    }
                    else if (dialogRead == 24)
                    {
                        StartCoroutine(NameMarkAppear());
                        nameMarkText.text = "Korav'ye";
                        allCharactersInScene.Add("Korav'ye", SpawnCharacter(characters[3], rightSpawnPoint, endLocRight2, false));
                        allCharactersInScene["Korav'ye"].GetComponent<CharactersController>().isTalking = true;
                        allCharactersInScene["Korav'ye"].GetComponent<CharactersController>().isLeftSide = false;
                        allCharactersInScene["Nikolai"].GetComponent<CharactersController>().isNewLocationActive = true;
                    }
                    else if (dialogRead == 25)
                    {
                        StartCoroutine(NameMarkDisappear());
                        allCharactersInScene["Korav'ye"].GetComponent<CharactersController>().isTalking = false;
                    }
                    else if (dialogRead == 31)
                    {
                        StartCoroutine(NameMarkAppear());
                        nameMarkText.text = "Profesor Antos";
                        allCharactersInScene["Profesor"].GetComponent<CharactersController>().isTalking = true;
                    }
                    else if (dialogRead == 32)
                    {
                        nameMarkText.text = "Wiktor";
                        allCharactersInScene["Profesor"].GetComponent<CharactersController>().isTalking = false;
                    }
                    else if (dialogRead == 33)
                    {
                        nameMarkText.text = "Profesor Antos";
                        allCharactersInScene["Profesor"].GetComponent<CharactersController>().isTalking = true;
                    }
                    else if (dialogRead == 34)
                    {
                        allCharactersInScene["Profesor"].GetComponent<CharactersController>().isTalking = false;
                        fade.BlackFadeAppear(true, nextLevelName);
                    }

                    break;

                #endregion

                #region Plansza17

                case LevelDialogQueue.Plansza17:
                    dialogRead += 1;

                    if (dialogRead == 1)
                    {
                        PlayerPrefs.SetInt("Plansza17", 1);
                    }
                    else if (dialogRead == 4)
                    {
                        StartCoroutine(NameMarkAppear());
                        nameMarkText.text = "Manya";
                        allCharactersInScene.Add("Manya", SpawnCharacter(characters[1], leftSpawnPoint, endLocLeft15, false));
                        allCharactersInScene["Manya"].GetComponent<CharactersController>().isTalking = true;
                        allCharactersInScene["Manya"].GetComponent<CharactersController>().isLeftSide = true;
                    }
                    else if (dialogRead == 5)
                    {
                        nameMarkText.text = "Profesor Antos";
                        allCharactersInScene.Add("Profesor", SpawnCharacter(characters[0], rightSpawnPoint, endLocRight15, true));
                        allCharactersInScene["Profesor"].GetComponent<CharactersController>().isLeftSide = false;
                        allCharactersInScene["Profesor"].GetComponent<CharactersController>().isTalking = true;
                        allCharactersInScene["Manya"].GetComponent<CharactersController>().isTalking = false;
                    }
                    else if (dialogRead == 6)
                    {
                        nameMarkText.text = "Manya";
                        allCharactersInScene["Manya"].GetComponent<CharactersController>().isTalking = true;
                        allCharactersInScene["Profesor"].GetComponent<CharactersController>().isTalking = false;
                    }
                    else if (dialogRead == 7)
                    {
                        nameMarkText.text = "Profesor Antos";
                        allCharactersInScene["Manya"].GetComponent<CharactersController>().isTalking = false;
                        allCharactersInScene["Profesor"].GetComponent<CharactersController>().isTalking = true;
                    }
                    else if (dialogRead == 8)
                    {
                        StartCoroutine(NameMarkDisappear());
                        allCharactersInScene["Profesor"].GetComponent<CharactersController>().isTalking = false;
                    }
                    else if (dialogRead == 9)
                    {
                        StartCoroutine(NameMarkAppear());
                        nameMarkText.text = "Manya";
                        allCharactersInScene["Manya"].GetComponent<CharactersController>().isTalking = true;
                    }
                    else if (dialogRead == 10)
                    {
                        nameMarkText.text = "Profesor Antos";
                        allCharactersInScene["Manya"].GetComponent<CharactersController>().isTalking = false;
                        allCharactersInScene["Profesor"].GetComponent<CharactersController>().isTalking = true;
                    }
                    else if (dialogRead == 11)
                    {
                        nameMarkText.text = "Manya";
                        allCharactersInScene["Manya"].GetComponent<CharactersController>().isTalking = true;
                        allCharactersInScene["Profesor"].GetComponent<CharactersController>().isTalking = false;
                    }
                    else if (dialogRead == 12)
                    {
                        nameMarkText.text = "Profesor Antos";
                        allCharactersInScene["Manya"].GetComponent<CharactersController>().isTalking = false;
                        allCharactersInScene["Profesor"].GetComponent<CharactersController>().isTalking = true;
                    }
                    else if (dialogRead == 13)
                    {
                        StartCoroutine(NameMarkDisappear());
                        allCharactersInScene["Profesor"].GetComponent<CharactersController>().isTalking = false;
                        allCharactersInScene["Profesor"].GetComponent<CharactersController>().isLeave = true;
                        allCharactersInScene["Manya"].GetComponent<CharactersController>().isLeave = true;
                        allCharactersInScene.Remove("Profesor");
                        allCharactersInScene.Remove("Manya");
                    }
                    else if (dialogRead == 15)
                    {
                        StartCoroutine(NameMarkAppear());
                        nameMarkText.text = "Wiktor";
                    }
                    else if (dialogRead == 16)
                    {
                        nameMarkText.text = "Nikolai Morozov";
                        allCharactersInScene.Add("Nikolai", SpawnCharacter(characters[2], rightSpawnPoint, endLocRight15, false));
                        allCharactersInScene["Nikolai"].GetComponent<CharactersController>().isLeftSide = false;
                        allCharactersInScene["Nikolai"].GetComponent<CharactersController>().isTalking = true;
                    }
                    else if (dialogRead == 17)
                    {
                        StartCoroutine(NameMarkDisappear());
                        allCharactersInScene["Nikolai"].GetComponent<CharactersController>().isTalking = false;
                    }
                    else if (dialogRead == 18)
                    {
                        StartCoroutine(NameMarkAppear());
                        nameMarkText.text = "Wiktor";
                    }
                    else if (dialogRead == 19)
                    {
                        nameMarkText.text = "Nikolai Morozov";
                        allCharactersInScene["Nikolai"].GetComponent<CharactersController>().isTalking = true;
                    }
                    else if (dialogRead == 20)
                    {
                        StartCoroutine(NameMarkDisappear());
                        allCharactersInScene["Nikolai"].GetComponent<CharactersController>().isTalking = false;
                    }
                    else if (dialogRead == 22)
                    {
                        StartCoroutine(NameMarkAppear());
                        nameMarkText.text = "Nikolai Morozov";
                        allCharactersInScene["Nikolai"].GetComponent<CharactersController>().isTalking = true;
                    }
                    else if (dialogRead == 23)
                    {
                        nameMarkText.text = "Wiktor";
                        allCharactersInScene["Nikolai"].GetComponent<CharactersController>().isTalking = false;
                    }
                    else if (dialogRead == 24)
                    {
                        nameMarkText.text = "Nikolai Morozov";
                        allCharactersInScene["Nikolai"].GetComponent<CharactersController>().isTalking = true;
                    }
                    else if (dialogRead == 25)
                    {
                        nameMarkText.text = "Wiktor";
                        allCharactersInScene["Nikolai"].GetComponent<CharactersController>().isTalking = false;
                    }
                    else if (dialogRead == 26)
                    {
                        nameMarkText.text = "Nikolai Morozov";
                        allCharactersInScene["Nikolai"].GetComponent<CharactersController>().isTalking = true;
                    }
                    else if (dialogRead == 28)
                    {
                        StartCoroutine(NameMarkDisappear());
                        allCharactersInScene["Nikolai"].GetComponent<CharactersController>().isTalking = false;
                    }
                    else if (dialogRead == 29)
                    {
                        StartCoroutine(NameMarkAppear());
                        nameMarkText.text = "Nikolai Morozov";
                        allCharactersInScene["Nikolai"].GetComponent<CharactersController>().isTalking = true;
                    }
                    else if (dialogRead == 30)
                    {
                        StartCoroutine(NameMarkDisappear());
                        allCharactersInScene["Nikolai"].GetComponent<CharactersController>().isTalking = false;
                    }
                    else if (dialogRead == 31)
                    {
                        StartCoroutine(NameMarkAppear());
                        nameMarkText.text = "Profesor Antos";
                    }
                    else if (dialogRead == 32)
                    {
                        fade.BlackFadeAppear(true, nextLevelName);
                    }

                    break;

                #endregion

                #region Plansza18

                case LevelDialogQueue.Plansza18:
                    dialogRead += 1;

                    if (dialogRead == 1)
                    {
                        PlayerPrefs.SetInt("Plansza18", 1);
                    }
                    else if (dialogRead == 2)
                    {
                        StartCoroutine(NameMarkAppear());
                        nameMarkText.text = "Profesor Antos";
                        allCharactersInScene.Add("Profesor", SpawnCharacter(characters[0], leftSpawnPoint, endLocLeft15, false));
                        allCharactersInScene["Profesor"].GetComponent<CharactersController>().isTalking = true;
                        allCharactersInScene["Profesor"].GetComponent<CharactersController>().isLeftSide = true;
                    }
                    else if (dialogRead == 3)
                    {
                        nameMarkText.text = "Bohater";
                        allCharactersInScene["Profesor"].GetComponent<CharactersController>().isTalking = false;
                    }
                    else if (dialogRead == 4)
                    {
                        nameMarkText.text = "Nikolai Morozov";
                        allCharactersInScene.Add("Nikolai", SpawnCharacter(characters[1], rightSpawnPoint, endLocRight15, false));
                        allCharactersInScene["Nikolai"].GetComponent<CharactersController>().isTalking = true;
                        allCharactersInScene["Nikolai"].GetComponent<CharactersController>().isLeftSide = false;
                    }
                    else if (dialogRead == 5)
                    {
                        nameMarkText.text = "Bohater";
                        allCharactersInScene["Nikolai"].GetComponent<CharactersController>().isTalking = false;
                    }
                    else if (dialogRead == 6)
                    {
                        nameMarkText.text = "Profesor Antos";
                        allCharactersInScene["Profesor"].GetComponent<CharactersController>().isTalking = true;
                    }
                    else if (dialogRead == 7)
                    {
                        nameMarkText.text = "Korav’ye";
                        allCharactersInScene.Add("Korav'ye", SpawnCharacter(characters[2], rightSpawnPoint, endLocRight2, false));
                        allCharactersInScene["Korav'ye"].GetComponent<CharactersController>().isTalking = true;
                        allCharactersInScene["Korav'ye"].GetComponent<CharactersController>().isLeftSide = false;
                        allCharactersInScene["Nikolai"].GetComponent<CharactersController>().isNewLocationActive = true;
                        allCharactersInScene["Profesor"].GetComponent<CharactersController>().isTalking = false;
                    }
                    else if (dialogRead == 9)
                    {
                        nameMarkText.text = "Profesor Antos";
                        allCharactersInScene["Korav'ye"].GetComponent<CharactersController>().isTalking = false;
                        allCharactersInScene["Profesor"].GetComponent<CharactersController>().isTalking = true;
                    }
                    else if (dialogRead == 10)
                    {
                        nameMarkText.text = "Korav’ye";
                        allCharactersInScene["Profesor"].GetComponent<CharactersController>().isTalking = false;
                        allCharactersInScene["Korav'ye"].GetComponent<CharactersController>().isTalking = true;
                    }
                    else if (dialogRead == 11)
                    {
                        nameMarkText.text = "Nikolai Morozov";
                        allCharactersInScene["Korav'ye"].GetComponent<CharactersController>().isTalking = false;
                        allCharactersInScene["Nikolai"].GetComponent<CharactersController>().isTalking = true;
                    }
                    else if (dialogRead == 12)
                    {
                        StartCoroutine(NameMarkDisappear());
                        allCharactersInScene["Nikolai"].GetComponent<CharactersController>().isTalking = false;
                    }
                    else if (dialogRead == 13)
                    {
                        StartCoroutine(NameMarkAppear());
                        nameMarkText.text = "Profesor Antos";
                        allCharactersInScene["Profesor"].GetComponent<CharactersController>().isTalking = true;
                    }
                    else if (dialogRead == 14)
                    {
                        nameMarkText.text = "Korav’ye";
                        allCharactersInScene["Profesor"].GetComponent<CharactersController>().isTalking = false;
                        allCharactersInScene["Korav'ye"].GetComponent<CharactersController>().isTalking = true;
                    }
                    else if (dialogRead == 15)
                    {
                        nameMarkText.text = "Profesor Antos";
                        allCharactersInScene["Korav'ye"].GetComponent<CharactersController>().isTalking = false;
                        allCharactersInScene["Profesor"].GetComponent<CharactersController>().isTalking = true;
                    }
                    else if (dialogRead == 16)
                    {
                        StartCoroutine(NameMarkDisappear());
                        allCharactersInScene["Profesor"].GetComponent<CharactersController>().isTalking = false;
                    }
                    else if (dialogRead == 17)
                    {
                        fade.BlackFadeAppear(true, nextLevelName);
                    }

                    break;

                #endregion

                #region Plansza19

                case LevelDialogQueue.Plansza19:
                    dialogRead += 1;

                    if (dialogRead == 1)
                    {
                        PlayerPrefs.SetInt("Plansza19", 1);
                    }
                    else if (dialogRead == 13)
                    {
                        fade.BlackFadeAppear(true, nextLevelName);
                    }

                    break;

                #endregion

                #region Plansza20

                case LevelDialogQueue.Plansza20:
                    dialogRead += 1;

                    if (dialogRead == 1)
                    {
                        PlayerPrefs.SetInt("Plansza20", 1);
                    }
                    else if (dialogRead == 7)
                    {
                        singleFadeComponent.singleFadeActivate = true;
                    }
                    else if (dialogRead == 10)
                    {
                        fade.BlackFadeAppear(true, nextLevelName);
                    }

                    break;

                #endregion

                #region Plansza21

                case LevelDialogQueue.Plansza21: // Czukcz Przewodnik, Nikolai Morozov, Korav'ye
                    dialogRead += 1;

                    if (dialogRead == 1)
                    {
                        PlayerPrefs.SetInt("Plansza21", 1);
                    }
                    else if (dialogRead == 8)
                    {
                        FindObjectOfType<SpriteElementFade>().ImageAppear();
                    }
                    else if (dialogRead == 9)
                    {
                        StartCoroutine(NameMarkAppear());
                        nameMarkText.text = "Bohater";
                    }
                    else if (dialogRead == 10)
                    {
                        nameMarkText.text = "Profesor Antos";
                        allCharactersInScene.Add("Profesor", SpawnCharacter(characters[0], leftSpawnPoint, endLocLeft15, false));
                        allCharactersInScene["Profesor"].GetComponent<CharactersController>().isTalking = true;
                        allCharactersInScene["Profesor"].GetComponent<CharactersController>().isLeftSide = true;
                    }
                    else if (dialogRead == 11)
                    {
                        nameMarkText.text = "Bohater";
                        allCharactersInScene["Profesor"].GetComponent<CharactersController>().isTalking = false;
                    }
                    else if (dialogRead == 12)
                    {
                        nameMarkText.text = "Profesor Antos";
                        allCharactersInScene["Profesor"].GetComponent<CharactersController>().isTalking = true;
                    }
                    else if (dialogRead == 13)
                    {
                        StartCoroutine(NameMarkDisappear());
                        allCharactersInScene["Profesor"].GetComponent<CharactersController>().isTalking = false;
                    }
                    else if (dialogRead == 14)
                    {
                        StartCoroutine(NameMarkAppear());
                        nameMarkText.text = "Bohater";
                    }
                    else if (dialogRead == 15)
                    {
                        StartCoroutine(NameMarkDisappear());
                    }
                    else if (dialogRead == 16)
                    {
                        StartCoroutine(NameMarkAppear());
                        nameMarkText.text = "Profesor Antos";
                        allCharactersInScene["Profesor"].GetComponent<CharactersController>().isTalking = true;
                    }
                    else if (dialogRead == 17)
                    {
                        nameMarkText.text = "Bohater";
                        allCharactersInScene["Profesor"].GetComponent<CharactersController>().isTalking = false;
                    }
                    else if (dialogRead == 18)
                    {
                        nameMarkText.text = "Profesor Antos";
                        allCharactersInScene["Profesor"].GetComponent<CharactersController>().isTalking = true;
                    }
                    else if (dialogRead == 19)
                    {
                        nameMarkText.text = "Profesor Antos";
                    }
                    else if (dialogRead == 20)
                    {
                        nameMarkText.text = "Bohater";
                        allCharactersInScene["Profesor"].GetComponent<CharactersController>().isTalking = false;
                    }
                    else if (dialogRead == 21)
                    {
                        nameMarkText.text = "Profesor Antos";
                        allCharactersInScene["Profesor"].GetComponent<CharactersController>().isTalking = true;
                    }
                    else if (dialogRead == 22)
                    {
                        StartCoroutine(NameMarkDisappear());
                        allCharactersInScene["Profesor"].GetComponent<CharactersController>().isTalking = false;
                    }
                    else if (dialogRead == 23)
                    {
                        StartCoroutine(NameMarkAppear());
                        nameMarkText.text = "Bohater";
                    }
                    else if (dialogRead == 24)
                    {
                        nameMarkText.text = "Profesor Antos";
                        allCharactersInScene["Profesor"].GetComponent<CharactersController>().isTalking = true;
                    }
                    else if (dialogRead == 25)
                    {
                        nameMarkText.text = "Bohater";
                        allCharactersInScene["Profesor"].GetComponent<CharactersController>().isTalking = false;
                    }
                    else if (dialogRead == 26)
                    {
                        StartCoroutine(NameMarkDisappear());
                    }
                    else if (dialogRead == 27)
                    {
                        StartCoroutine(NameMarkAppear());
                        nameMarkText.text = "Profesor Antos";
                        allCharactersInScene["Profesor"].GetComponent<CharactersController>().isTalking = true;
                        ChooseAppear("Korav’ye nie mówi nam wszystkiego.", "Sądzisz, że Czesław nie zginął... przypadkiem?");
                    }
                    else if (dialogRead == 29)
                    {
                        nameMarkText.text = "Bohater";
                        allCharactersInScene["Profesor"].GetComponent<CharactersController>().isTalking = false;
                    }
                    else if (dialogRead == 30) // Был сделан выбор А
                    {
                        nameMarkText.text = "Profesor Antos";
                        allCharactersInScene["Profesor"].GetComponent<CharactersController>().isTalking = true;
                        dialogRead = 33;
                    }
                    else if (dialogRead == 31)
                    {
                        StartCoroutine(NameMarkDisappear());
                        allCharactersInScene["Profesor"].GetComponent<CharactersController>().isTalking = false;
                    }
                    else if (dialogRead == 32)
                    {
                        StartCoroutine(NameMarkAppear());
                        nameMarkText.text = "Profesor Antos";
                        allCharactersInScene["Profesor"].GetComponent<CharactersController>().isTalking = true;
                    }
                    else if (dialogRead == 33)
                    {
                        nameMarkText.text = "Bohater";
                        allCharactersInScene["Profesor"].GetComponent<CharactersController>().isTalking = false;
                    }
                    else if (dialogRead == 34)
                    {
                        StartCoroutine(NameMarkDisappear());
                        allCharactersInScene["Profesor"].GetComponent<CharactersController>().isTalking = false;
                    }
                    else if (dialogRead == 36)
                    {
                        StartCoroutine(NameMarkAppear());
                        nameMarkText.text = "Bohater";
                    }
                    else if (dialogRead == 37)
                    {
                        nameMarkText.text = "Profesor Antos";
                        allCharactersInScene["Profesor"].GetComponent<CharactersController>().isTalking = true;
                    }
                    else if (dialogRead == 38)
                    {
                        allCharactersInScene["Profesor"].GetComponent<CharactersController>().isTalking = false;
                        fade.BlackFadeAppear(true, nextLevelName);
                    }

                    break;

                #endregion

                #region Plansza22

                case LevelDialogQueue.Plansza22:
                    dialogRead += 1;

                    if (dialogRead == 1)
                    {
                        PlayerPrefs.SetInt("Plansza22", 1);
                    }
                    else if (dialogRead == 3)
                    {
                        StartCoroutine(NameMarkAppear());
                        nameMarkText.text = "Nikolai Morozov";
                    }
                    else if (dialogRead == 4)
                    {
                        StartCoroutine(NameMarkDisappear());
                    }
                    else if (dialogRead == 12)
                    {
                        fade.BlackFadeAppear(true, nextLevelName);
                    }

                    break;

                #endregion

                #region Plansza23

                case LevelDialogQueue.Plansza23:
                    dialogRead += 1;

                    if (dialogRead == 1)
                    {
                        PlayerPrefs.SetInt("Plansza23", 1);
                    }
                    else if (dialogRead == 2)
                    {
                        StartCoroutine(NameMarkAppear());
                        nameMarkText.text = "Profesor Antos";
                    }
                    else if (dialogRead == 3)
                    {
                        StartCoroutine(NameMarkDisappear());
                    }
                    else if (dialogRead == 5)
                    {
                        StartCoroutine(NameMarkAppear());
                        nameMarkText.text = "Korav’ye";
                    }
                    else if (dialogRead == 6)
                    {
                        StartCoroutine(NameMarkDisappear());
                    }
                    else if (dialogRead == 7)
                    {
                        StartCoroutine(NameMarkAppear());
                        nameMarkText.text = "Bohater";
                    }
                    else if (dialogRead == 8)
                    {
                        nameMarkText.text = "Profesor Antos";
                    }
                    else if (dialogRead == 9)
                    {
                        nameMarkText.text = "Bohater";
                    }
                    else if (dialogRead == 10)
                    {
                        nameMarkText.text = "Korav’ye";
                    }
                    else if (dialogRead == 11)
                    {
                        StartCoroutine(NameMarkDisappear());
                    }
                    else if (dialogRead == 12)
                    {
                        StartCoroutine(NameMarkAppear());
                        nameMarkText.text = "Korav’ye";
                    }
                    else if (dialogRead == 13)
                    {
                        StartCoroutine(NameMarkDisappear());
                    }
                    else if (dialogRead == 14)
                    {
                        fade.BlackFadeAppear(true, nextLevelName);
                    }
                    break;

                #endregion

                #region Plansza24

                case LevelDialogQueue.Plansza24:
                    dialogRead += 1;

                    if (dialogRead == 1)
                    {
                        PlayerPrefs.SetInt("Plansza24", 1);
                    }
                    else if (dialogRead == 3)
                    {
                        StartCoroutine(NameMarkAppear());
                        nameMarkText.text = "Bohater";
                    }
                    else if (dialogRead == 4)
                    {
                        nameMarkText.text = "Korav’ye";
                    }
                    else if (dialogRead == 5)
                    {
                        nameMarkText.text = "Bohater";
                    }
                    else if (dialogRead == 6)
                    {
                        nameMarkText.text = "Korav’ye";
                    }
                    else if (dialogRead == 7)
                    {
                        StartCoroutine(NameMarkDisappear());
                    }
                    else if (dialogRead == 8)
                    {
                        StartCoroutine(NameMarkAppear());
                        nameMarkText.text = "Bohater";
                    }
                    else if (dialogRead == 9)
                    {
                        StartCoroutine(NameMarkDisappear());
                    }
                    else if (dialogRead == 11)
                    {
                        StartCoroutine(NameMarkAppear());
                        nameMarkText.text = "Korav’ye";
                    }
                    else if (dialogRead == 12)
                    {
                        nameMarkText.text = "Bohater";
                    }
                    else if (dialogRead == 13)
                    {
                        nameMarkText.text = "Korav’ye";
                    }
                    else if (dialogRead == 14)
                    {
                        StartCoroutine(NameMarkDisappear());
                    }
                    else if (dialogRead == 15)
                    {
                        fade.BlackFadeAppear(true, nextLevelName);
                    }
                    break;

                #endregion

                #region Plansza26

                case LevelDialogQueue.Plansza26:
                    dialogRead += 1;

                    if (dialogRead == 1)
                    {
                        PlayerPrefs.SetInt("Plansza26", 1);
                    }
                    else if (dialogRead == 4)
                    {
                        StartCoroutine(NameMarkAppear());
                        nameMarkText.text = "Profesor Antos";
                    }
                    else if (dialogRead == 5)
                    {
                        nameMarkText.text = "Wiktor";
                    }
                    else if (dialogRead == 6)
                    {
                        StartCoroutine(NameMarkDisappear());
                    }
                    else if (dialogRead == 7)
                    {
                        StartCoroutine(NameMarkAppear());
                        nameMarkText.text = "Wiktor";
                    }
                    else if (dialogRead == 8)
                    {
                        StartCoroutine(NameMarkDisappear());
                    }
                    else if (dialogRead == 11)
                    {
                        StartCoroutine(NameMarkAppear());
                        nameMarkText.text = "Profesor Antos";
                    }
                    else if (dialogRead == 12)
                    {
                        nameMarkText.text = "Korav’ye";
                    }
                    else if (dialogRead == 13)
                    {
                        nameMarkText.text = "Profesor Antos";
                    }
                    else if (dialogRead == 14)
                    {
                        StartCoroutine(NameMarkDisappear());
                    }
                    else if (dialogRead == 15)
                    {
                        StartCoroutine(NameMarkAppear());
                        nameMarkText.text = "Profesor Antos";
                    }
                    else if (dialogRead == 16)
                    {
                        StartCoroutine(NameMarkDisappear());
                    }
                    else if (dialogRead == 18)
                    {
                        StartCoroutine(NameMarkAppear());
                        nameMarkText.text = "Profesor Antos";
                    }
                    else if (dialogRead == 19)
                    {
                        StartCoroutine(NameMarkDisappear());
                    }
                    else if (dialogRead == 21)
                    {
                        StartCoroutine(NameMarkAppear());
                        nameMarkText.text = "Nikolai Morozov";
                    }
                    else if (dialogRead == 22)
                    {
                        nameMarkText.text = "Profesor Antos";
                    }
                    else if (dialogRead == 23)
                    {
                        StartCoroutine(NameMarkDisappear());
                    }
                    else if (dialogRead == 24)
                    {
                        StartCoroutine(NameMarkAppear());
                        nameMarkText.text = "Nikolai Morozov";
                    }
                    else if (dialogRead == 25)
                    {
                        nameMarkText.text = "Profesor Antos";
                    }
                    else if (dialogRead == 26)
                    {
                        nameMarkText.text = "Nikolai Morozov";
                    }
                    else if (dialogRead == 27)
                    {
                        StartCoroutine(NameMarkDisappear());
                    }
                    else if (dialogRead == 28)
                    {
                        StartCoroutine(NameMarkAppear());
                        nameMarkText.text = "Profesor Antos";
                    }
                    else if (dialogRead == 29)
                    {
                        StartCoroutine(NameMarkDisappear());
                    }
                    else if (dialogRead == 30)
                    {
                        StartCoroutine(NameMarkAppear());
                        nameMarkText.text = "Nikolai Morozov";
                    }
                    else if (dialogRead == 31)
                    {
                        StartCoroutine(NameMarkDisappear());
                    }
                    else if (dialogRead == 32)
                    {
                        StartCoroutine(NameMarkAppear());
                        nameMarkText.text = "Profesor Antos";
                    }
                    else if (dialogRead == 33)
                    {
                        StartCoroutine(NameMarkDisappear());
                    }
                    else if (dialogRead == 36)
                    {
                        StartCoroutine(NameMarkAppear());
                        nameMarkText.text = "Profesor Antos";
                    }
                    else if (dialogRead == 37)
                    {
                        StartCoroutine(NameMarkDisappear());
                    }
                    else if (dialogRead == 38)
                    {
                        StartCoroutine(NameMarkAppear());
                        nameMarkText.text = "Wiktor";
                    }
                    else if (dialogRead == 39)
                    {
                        nameMarkText.text = "Korav’ye";
                    }
                    else if (dialogRead == 40)
                    {
                        nameMarkText.text = "Wiktor";
                    }
                    else if (dialogRead == 41)
                    {
                        nameMarkText.text = "Korav’ye";
                    }
                    else if (dialogRead == 42)
                    {
                        nameMarkText.text = "Wiktor";
                    }
                    else if (dialogRead == 43)
                    {
                        StartCoroutine(NameMarkAppear());
                        nameMarkText.text = "Korav’ye";
                    }
                    else if (dialogRead == 44)
                    {
                        StartCoroutine(NameMarkDisappear());
                    }
                    else if (dialogRead == 45)
                    {
                        StartCoroutine(NameMarkAppear());
                        nameMarkText.text = "Korav’ye";
                    }
                    else if (dialogRead == 46)
                    {
                        nameMarkText.text = "Profesor Antos";
                    }
                    else if (dialogRead == 47)
                    {
                        nameMarkText.text = "Korav’ye";
                    }
                    else if (dialogRead == 48)
                    {
                        StartCoroutine(NameMarkDisappear());
                    }
                    else if (dialogRead == 49)
                    {
                        StartCoroutine(NameMarkAppear());
                        nameMarkText.text = "Profesor Antos";
                    }
                    else if (dialogRead == 50)
                    {
                        StartCoroutine(NameMarkDisappear());
                    }
                    else if (dialogRead == 51)
                    {
                        StartCoroutine(NameMarkAppear());
                        nameMarkText.text = "Korav’ye";
                    }
                    else if (dialogRead == 52)
                    {
                        fade.BlackFadeAppear(true, nextLevelName);
                    }

                    break;

                #endregion

                #region Plansza27

                case LevelDialogQueue.Plansza27:
                    dialogRead += 1;

                    if (dialogRead == 1)
                    {
                        PlayerPrefs.SetInt("Plansza27", 1);
                    }
                    else if (dialogRead == 2)
                    {
                        StartCoroutine(NameMarkAppear());
                        nameMarkText.text = "Wiktor";
                    }
                    else if (dialogRead == 3)
                    {
                        StartCoroutine(NameMarkDisappear());
                    }
                    else if (dialogRead == 5)
                    {
                        StartCoroutine(NameMarkAppear());
                        nameMarkText.text = "Wiktor";
                    }
                    else if (dialogRead == 6)
                    {
                        nameMarkText.text = "Profesor Antos";
                    }
                    else if (dialogRead == 7)
                    {
                        StartCoroutine(NameMarkDisappear());
                    }
                    else if (dialogRead == 9)
                    {
                        StartCoroutine(NameMarkAppear());
                        nameMarkText.text = "Profesor Antos";
                    }
                    else if (dialogRead == 11)
                    {
                        nameMarkText.text = "Wiktor";
                    }
                    else if (dialogRead == 12)
                    {
                        nameMarkText.text = "Profesor Antos";
                    }
                    else if (dialogRead == 15)
                    {
                        nameMarkText.text = "Wiktor";
                    }
                    else if (dialogRead == 16)
                    {
                        nameMarkText.text = "Profesor Antos";
                    }
                    else if (dialogRead == 17)
                    {
                        StartCoroutine(NameMarkDisappear());
                    }
                    else if (dialogRead == 19)
                    {
                        StartCoroutine(NameMarkAppear());
                        nameMarkText.text = "Wiktor";
                    }
                    else if (dialogRead == 20)
                    {
                        StartCoroutine(NameMarkDisappear());
                    }
                    else if (dialogRead == 21)
                    {
                        StartCoroutine(NameMarkAppear());
                        nameMarkText.text = "Profesor Antos";
                    }
                    else if (dialogRead == 22)
                    {
                        nameMarkText.text = "Wiktor";
                    }
                    else if (dialogRead == 23)
                    {
                        nameMarkText.text = "Profesor Antos";
                    }
                    else if (dialogRead == 24)
                    {
                        nameMarkText.text = "Wiktor";
                    }
                    else if (dialogRead == 25)
                    {
                        nameMarkText.text = "Profesor Antos";
                    }
                    else if (dialogRead == 26)
                    {
                        StartCoroutine(NameMarkDisappear());
                    }
                    else if (dialogRead == 27)
                    {
                        fade.BlackFadeAppear(true, nextLevelName);
                    }

                    break;

                #endregion

                #region Plansza27Incorrect

                case LevelDialogQueue.Plansza27Incorrect:
                    dialogRead += 1;

                    if (dialogRead == 1)
                    {
                        StartCoroutine(NameMarkAppear());
                        nameMarkText.text = "Profesor Antos";
                    }
                    else if (dialogRead == 2)
                    {
                        StartCoroutine(NameMarkDisappear());
                    }
                    else if (dialogRead == 3)
                    {
                        StartCoroutine(NameMarkAppear());
                        nameMarkText.text = "Profesor Antos";
                    }
                    else if (dialogRead == 4)
                    {
                        StartCoroutine(NameMarkDisappear());
                    }
                    else if (dialogRead == 5)
                    {
                        StartCoroutine(NameMarkAppear());
                        nameMarkText.text = "Profesor Antos";
                    }
                    else if (dialogRead == 6)
                    {
                        StartCoroutine(NameMarkDisappear());
                    }
                    else if (dialogRead == 8)
                    {
                        fade.BlackFadeAppear(true, nextLevelName);
                    }

                    break;

                #endregion

                #region Plansza30Incorrect

                case LevelDialogQueue.Plansza30Incorrect:
                    dialogRead += 1;

                    if (dialogRead == 1)
                    {
                        StartCoroutine(NameMarkAppear());
                        nameMarkText.text = "Profesor Antos";
                    }
                    else if (dialogRead == 2)
                    {
                        StartCoroutine(NameMarkDisappear());
                    }
                    else if (dialogRead == 3)
                    {
                        fade.BlackFadeAppear(true, nextLevelName);
                    }

                    break;

                #endregion

                #region Plansza31Incorrect

                case LevelDialogQueue.Plansza31Incorrect:
                    dialogRead += 1;

                    if (dialogRead == 1)
                    {
                        nameMarkText.text = "Profesor Antos";
                    }
                    else if (dialogRead == 2)
                    {
                        nameMarkText.text = "Wiktor";
                    }
                    else if (dialogRead == 3)
                    {
                        StartCoroutine(NameMarkDisappear());
                    }
                    else if (dialogRead == 4)
                    {
                        StartCoroutine(NameMarkAppear());
                        nameMarkText.text = "Profesor Antos";
                    }
                    else if (dialogRead == 5)
                    {
                        StartCoroutine(NameMarkDisappear());
                    }
                    else if (dialogRead == 6)
                    {
                        fade.BlackFadeAppear(true, nextLevelName);
                    }

                    break;

                #endregion

                #region Plansza27Correct

                case LevelDialogQueue.Plansza27Correct:
                    dialogRead += 1;

                    if (dialogRead == 1)
                    {
                        fade.BlackFadeAppear(true, nextLevelName);
                    }

                    break;

                #endregion

                #region Plansza28

                case LevelDialogQueue.Plansza28:
                    dialogRead += 1;

                    if (dialogRead == 1)
                    {
                        fade.BlackFadeAppear(true, nextLevelName);
                    }
                    break;

                #endregion

                #region Plansza29

                case LevelDialogQueue.Plansza29:
                    dialogRead += 1;

                    if (dialogRead == 8)
                    {
                        fade.BlackFadeAppear(true, nextLevelName);
                    }
                    break;

                #endregion

                #region Plansza30Correct

                case LevelDialogQueue.Plansza30Correct:
                    dialogRead += 1;

                    if (dialogRead == 2)
                    {
                        fade.BlackFadeAppear(true, nextLevelName);
                    }
                    break;

                #endregion

                #region Plansza31Correct

                case LevelDialogQueue.Plansza31Correct:
                    dialogRead += 1;

                    if (dialogRead == 1)
                    {
                        fade.BlackFadeAppear(true, nextLevelName);
                    }
                    break;

                #endregion

                #region Plansza32

                case LevelDialogQueue.Plansza32:
                    dialogRead += 1;

                    if (dialogRead == 1)
                    {
                        StartCoroutine(NameMarkDisappear());
                    }
                    else if(dialogRead == 2)
                    {
                        nameMarkText.text = "Korav'ye";
                        StartCoroutine(NameMarkAppear());
                    }
                    else if (dialogRead == 3)
                    {
                        StartCoroutine(NameMarkDisappear());
                    }
                    else if (dialogRead == 4)
                    {
                        nameMarkText.text = "Wiktor";
                        StartCoroutine(NameMarkAppear());
                    }
                    else if (dialogRead == 5)
                    {
                        nameMarkText.text = "Korav'ye";
                    }
                    else if (dialogRead == 6)
                    {
                        StartCoroutine(NameMarkDisappear());
                    }
                    else if (dialogRead == 8)
                    {
                        nameMarkText.text = "Korav'ye";
                        StartCoroutine(NameMarkAppear());
                    }
                    else if (dialogRead == 9)
                    {
                        nameMarkText.text = "Wiktor";
                    }
                    else if (dialogRead == 10)
                    {
                        StartCoroutine(NameMarkDisappear());
                    }
                    else if (dialogRead == 11)
                    {
                        nameMarkText.text = "Korav'ye";
                        StartCoroutine(NameMarkAppear());
                    }
                    else if (dialogRead == 12)
                    {
                        nameMarkText.text = "Wiktor";
                    }
                    else if (dialogRead == 13)
                    {
                        nameMarkText.text = "Korav'ye";
                    }
                    else if (dialogRead == 14)
                    {
                        StartCoroutine(NameMarkDisappear());
                    }
                    else if (dialogRead == 20)
                    {
                        nameMarkText.text = "Korav'ye";
                        StartCoroutine(NameMarkAppear());
                    }
                    else if (dialogRead == 21)
                    {
                        StartCoroutine(NameMarkDisappear());
                        ChooseAppear("Sięgnij po broń.", "Wycofaj się.");
                    }
                    else if (dialogRead == 24)
                    {
                        isDialogNeedBeStopped = true;
                        fade.BlackFadeAppear(true, "Plansza33");
                    }
                    else if (dialogRead == 29)
                    {
                        nameMarkText.text = "Korav'ye";
                        StartCoroutine(NameMarkAppear());
                    }
                    else if (dialogRead == 30)
                    {
                        nameMarkText.text = "Wiktor";
                    }
                    else if (dialogRead == 31)
                    {
                        StartCoroutine(NameMarkDisappear());
                    }
                    else if (dialogRead == 32)
                    {
                        nameMarkText.text = "Korav'ye";
                        StartCoroutine(NameMarkAppear());
                    }
                    else if (dialogRead == 33)
                    {
                        StartCoroutine(NameMarkDisappear());
                    }
                    else if (dialogRead == 35)
                    {
                        fade.BlackFadeAppear(true, "Plansza38");
                    }
                    break;

                #endregion

                #region Plansza33

                case LevelDialogQueue.Plansza33:
                    dialogRead += 1;

                    if (dialogRead == 8)
                    {
                        fade.BlackFadeAppear(true, nextLevelName);
                    }
                    break;

                #endregion

                #region Plansza34

                case LevelDialogQueue.Plansza34:
                    dialogRead += 1;

                    if (dialogRead == 9)
                    {
                        fade.BlackFadeAppear(true, nextLevelName);
                    }
                    break;

                #endregion

                #region Plansza35

                case LevelDialogQueue.Plansza35:
                    dialogRead += 1;

                    if (dialogRead == 7)
                    {
                        nameMarkText.text = "Wiktor";
                        StartCoroutine(NameMarkAppear());
                    }
                    else if (dialogRead == 8)
                    {
                        StartCoroutine(NameMarkDisappear());
                    }
                    else if (dialogRead == 16)
                    {
                        fade.BlackFadeAppear(true, nextLevelName);
                    }
                    break;

                #endregion

                #region Plansza36

                case LevelDialogQueue.Plansza36:
                    dialogRead += 1;

                    if (dialogRead == 5)
                    {
                        nameMarkText.text = "Wiktor";
                        StartCoroutine(NameMarkAppear());
                        backgroundElementFade1.ImageAppear();
                    }
                    else if (dialogRead == 6)
                    {
                        nameMarkText.text = "Karol";
                    }
                    else if (dialogRead == 7)
                    {
                        StartCoroutine(NameMarkDisappear());
                        backgroundElementFade2.ImageAppear();
                    }
                    else if (dialogRead == 8)
                    {
                        nameMarkText.text = "Karol";
                        StartCoroutine(NameMarkAppear());
                    }
                    else if (dialogRead == 9)
                    {
                        fade.BlackFadeAppear(true, nextLevelName);
                    }
                    break;

                #endregion

                #region Plansza37

                case LevelDialogQueue.Plansza37:
                    dialogRead += 1;

                    if (dialogRead == 2)
                    {
                        fade.BlackFadeAppear(true, nextLevelName);
                    }
                    break;

                #endregion

                #region Plansza38

                case LevelDialogQueue.Plansza38:
                    dialogRead += 1;

                    if (dialogRead == 2)
                    {
                        singleFadeComponent.singleFadeActivate = true;
                    }
                    else if (dialogRead == 7)
                    {
                        fade.BlackFadeAppear(true, nextLevelName);
                    }
                    break;

                #endregion

                #region Plansza39

                case LevelDialogQueue.Plansza39:
                    dialogRead += 1;

                    if (dialogRead == 3)
                    {
                        nameMarkText.text = "Korav'ye";
                        StartCoroutine(NameMarkAppear());
                    }
                    else if (dialogRead == 4)
                    {
                        StartCoroutine(NameMarkDisappear());
                    }
                    else if (dialogRead == 6)
                    {
                        fade.BlackFadeAppear(true, nextLevelName);
                    }
                    break;

                #endregion

                #region Plansza40

                case LevelDialogQueue.Plansza40:
                    dialogRead += 1;

                    if (dialogRead == 1)
                    {
                        nameMarkText.text = "Korav'ye";
                        StartCoroutine(NameMarkAppear());
                    }
                    else if (dialogRead == 2)
                    {
                        StartCoroutine(NameMarkDisappear());
                    }
                    else if (dialogRead == 3)
                    {
                        nameMarkText.text = "Wiktor";
                        StartCoroutine(NameMarkAppear());
                    }
                    else if (dialogRead == 4)
                    {
                        nameMarkText.text = "Korav'ye";
                    }
                    else if (dialogRead == 5)
                    {
                        StartCoroutine(NameMarkDisappear());
                    }
                    else if (dialogRead == 7)
                    {
                        nameMarkText.text = "...";
                        StartCoroutine(NameMarkAppear());
                    }
                    else if (dialogRead == 8)
                    {
                        StartCoroutine(NameMarkDisappear());
                    }
                    else if (dialogRead == 9)
                    {
                        fade.BlackFadeAppear(true, nextLevelName);
                    }
                    break;

                #endregion

                #region Plansza41

                case LevelDialogQueue.Plansza41:
                    dialogRead += 1;

                    if (dialogRead == 2)
                    {
                        nameMarkText.text = "Korav'ye";
                        StartCoroutine(NameMarkAppear());
                    }
                    else if (dialogRead == 3)
                    {
                        nameMarkText.text = "Wiktor";
                    }
                    else if (dialogRead == 4)
                    {
                        nameMarkText.text = "Korav'ye";
                    }
                    else if (dialogRead == 6)
                    {
                        nameMarkText.text = "Wiktor";
                    }
                    else if (dialogRead == 7)
                    {
                        nameMarkText.text = "Korav'ye";
                    }
                    else if (dialogRead == 8)
                    {
                        nameMarkText.text = "Wiktor";
                    }
                    else if (dialogRead == 9)
                    {
                        nameMarkText.text = "Korav'ye";
                    }
                    else if (dialogRead == 11)
                    {
                        StartCoroutine(NameMarkDisappear());
                    }
                    else if (dialogRead == 12)
                    {
                        nameMarkText.text = "Wiktor";
                        StartCoroutine(NameMarkAppear());
                    }
                    else if (dialogRead == 13)
                    {
                        nameMarkText.text = "Korav'ye";
                    }
                    else if (dialogRead == 14)
                    {
                        StartCoroutine(NameMarkDisappear());
                    }
                    else if (dialogRead == 15)
                    {
                        nameMarkText.text = "Korav'ye";
                        StartCoroutine(NameMarkAppear());
                    }
                    else if (dialogRead == 18)
                    {
                        StartCoroutine(NameMarkDisappear());
                    }
                    else if (dialogRead == 19)
                    {
                        nameMarkText.text = "Korav'ye";
                        StartCoroutine(NameMarkAppear());
                    }
                    else if (dialogRead == 20)
                    {
                        fade.BlackFadeAppear(true, nextLevelName);
                    }
                    break;

                #endregion

                #region Plansza42

                case LevelDialogQueue.Plansza42:
                    dialogRead += 1;

                    if (dialogRead == 5)
                    {
                        fade.FadeAppear(nextLevelName);
                    }
                    break;

                #endregion

                #region Plansza42.5

                case LevelDialogQueue.Plansza425:
                    dialogRead += 1;

                    if (dialogRead == 7)
                    {
                        nameMarkText.text = "Szaman";
                        StartCoroutine(NameMarkAppear());
                    }
                    if (dialogRead == 8)
                    {
                        nameMarkText.text = "Wiktor";
                    }
                    if (dialogRead == 9)
                    {
                        nameMarkText.text = "Szaman";
                    }
                    if (dialogRead == 10)
                    {
                        nameMarkText.text = "Wiktor";
                    }
                    if (dialogRead == 11)
                    {
                        nameMarkText.text = "Szaman";
                    }
                    if (dialogRead == 12)
                    {
                        StartCoroutine(NameMarkDisappear());
                    }
                    if (dialogRead == 13)
                    {
                        nameMarkText.text = "Szaman";
                        StartCoroutine(NameMarkAppear());
                    }
                    if (dialogRead == 15)
                    {
                        StartCoroutine(NameMarkDisappear());
                    }
                    if (dialogRead == 16)
                    {
                        nameMarkText.text = "Szaman";
                        StartCoroutine(NameMarkAppear());
                    }
                    else if (dialogRead == 18)
                    {
                        fade.FadeAppear(nextLevelName);
                    }
                    break;

                #endregion

                #region Plansza43

                case LevelDialogQueue.Plansza43:
                    dialogRead += 1;

                    if (dialogRead == 3)
                    {
                        nameMarkText.text = "Szaman";
                        StartCoroutine(NameMarkAppear());
                    }
                    else if (dialogRead == 4)
                    {
                        StartCoroutine(NameMarkDisappear());
                    }
                    if (dialogRead == 5)
                    {
                        nameMarkText.text = "Szaman";
                        StartCoroutine(NameMarkAppear());
                    }
                    else if (dialogRead == 6)
                    {
                        StartCoroutine(NameMarkDisappear());
                    }
                    if (dialogRead == 7)
                    {
                        nameMarkText.text = "Wiktor";
                        StartCoroutine(NameMarkAppear());
                    }
                    else if (dialogRead == 8)
                    {
                        StartCoroutine(NameMarkDisappear());
                    }
                    else if (dialogRead == 9)
                    {
                        if (PlayerPrefs.GetInt("IsAlive") == 1)
                            fade.FadeAppear(nextLevelName);
                        else
                            fade.BlackFadeAppear(true, "MainMenu");
                    }
                    break;

                #endregion

                #region Plansza44

                case LevelDialogQueue.Plansza44:
                    dialogRead += 1;

                    if (dialogRead == 2)
                    {
                        nameMarkText.text = "Profesor Antos";
                        StartCoroutine(NameMarkAppear());
                    }
                    else if (dialogRead == 3)
                    {
                        nameMarkText.text = "Wiktor";
                    }
                    else if (dialogRead == 4)
                    {
                        nameMarkText.text = "Profesor Antos";
                    }
                    else if (dialogRead == 5)
                    {
                        nameMarkText.text = "Wiktor";
                    }
                    else if (dialogRead == 7)
                    {
                        nameMarkText.text = "Profesor Antos";
                    }
                    else if (dialogRead == 8)
                    {
                        StartCoroutine(NameMarkDisappear());
                    }
                    else if (dialogRead == 15)
                    {
                        fade.BlackFadeAppear(true, nextLevelName);
                    }
                    break;

                    #endregion
            }
        }
    }

    private GameObject SpawnCharacter(GameObject characterPrefab, Transform parentSpawnPoint, Vector3 endLocation, bool isInverse)
    {
        GameObject character = Instantiate(characterPrefab);
        character.transform.SetParent(parentSpawnPoint);
        character.transform.localPosition = Vector3.zero;
        character.GetComponent<CharactersController>().endLocation = endLocation;

        Vector3 charScale = new Vector3(1.6f, 1.6f, 1f);
        if (isInverse)
            charScale.x *= -1;
        character.transform.localScale = charScale;

        return character;
    }

    private void CameraRotate() // Plansza 6
    {
        if (isCameraRotateStart && cameraRotateAlpha < 1.0f) // p: 300 / 100 / 0
        {
            cameraRotateAlpha += Time.deltaTime / 1.5f;

            Quaternion bgRot = backgroundImage.transform.rotation;
            bgRot.eulerAngles = Vector3.Lerp(bgRotationOnStart, newBgRotation, curve.Evaluate(cameraRotateAlpha));
            backgroundImage.transform.rotation = bgRot;
            backgroundImage.transform.localPosition = Vector3.Lerp(bgPositionOnStart, newBgPosition, curve.Evaluate(cameraRotateAlpha));
        }
        else if(isCameraRotateStart && cameraRotateAlpha >= 1.0f && newBgRotation == new Vector3(0, 0, 10))
        {
            cameraRotateAlpha = 0;
            bgPositionOnStart = backgroundImage.GetComponent<RectTransform>().localPosition;
            bgRotationOnStart = backgroundImage.GetComponent<RectTransform>().localRotation.eulerAngles;
            newBgPosition = new Vector3(0, 0, 0);
            newBgRotation = new Vector3(0, 0, 15);
        }
    }

    private void ChooseAppear(string ch1, string ch2)
    {
        isChoiseAppear = true;
        choise1.GetComponent<Image>().enabled = true;
        choise2.GetComponent<Image>().enabled = true;
        choise1Text.enabled = true;
        choise2Text.enabled = true;
        choise1Text.text = ch1;
        choise2Text.text = ch2;
        choise1.GetComponent<Button>().interactable = true;
        choise2.GetComponent<Button>().interactable = true;
    }

    private void ChooseDisappear()
    {
        isChoiseAppear = false;

        if (!button1Choosen)
        {
            choise1.GetComponent<Image>().enabled = false;
            choise1Text.enabled = false;
            choise1.GetComponent<Button>().interactable = false;
        }

        if (!button2Choosen)
        {
            choise2.GetComponent<Image>().enabled = false;
            choise2Text.enabled = false;
            choise2.GetComponent<Button>().interactable = false;
        }

        if (choise3 != null)
        {
            choise3Text.enabled = false;
            choise3.GetComponent<Image>().enabled = false;
            choise3.GetComponent<Button>().interactable = false;
        }
    }

    private void ChooseAppearTriple(string ch1, string ch2, string ch3)
    {
        isChoiseAppear = true;

        if (!button1Choosen)
        {
            choise1.GetComponent<Image>().enabled = true;
            choise1Text.enabled = true;
            choise1Text.text = ch1;
            choise1.GetComponent<Button>().interactable = true;
        }

        if (!button2Choosen)
        {
            choise2.GetComponent<Image>().enabled = true;
            choise2Text.enabled = true;
            choise2Text.text = ch2;
            choise2.GetComponent<Button>().interactable = true;
        }

        if (!button3Choosen)
        {
            choise3.GetComponent<Image>().enabled = true;
            choise3Text.enabled = true;
            choise3Text.text = ch3;
            choise3.GetComponent<Button>().interactable = true;
        }

        if (button1Choosen && choise1 != null)
            Destroy(choise1);
        if (button2Choosen && choise2 != null)
            Destroy(choise2);
        if (button3Choosen && choise3 != null)
            Destroy(choise3);

        int choosenNumber = 0;
        if (button1Choosen)
            choosenNumber += 1;
        if (button2Choosen)
            choosenNumber += 1;
        if (button3Choosen)
            choosenNumber += 1;

        if (choosenNumber == 3)
        {
            dialogRead = 18;
            isChoiseAppear = false;
            return;
        }
    }

    public void Choose(int numberOfButton)
    {
        if (isTextRead)
        {
            for (int i = 0; i < choosenDialogElements.Length; i++)
            {
                if (choosenDialogElements[i] == 0)
                {
                    choosenDialogElements[i] = numberOfButton;
                    break;
                }
            }

            if (levelPreset == LevelDialogQueue.Plansza3)
            {
                chooseNumber++;
                CheckChooseDialogPL3();
            }
            else if (levelPreset == LevelDialogQueue.Plansza7)
            {
                chooseNumber = numberOfButton;
                CheckChooseDialogPL7();
            }
            else if (levelPreset == LevelDialogQueue.Plansza10)
            {
                isChooseNeedBeDesappeared = true;
                chooseNumber++;
                CheckChooseDialogPL10();
            }
            else if (levelPreset == LevelDialogQueue.Plansza12)
            {
                chooseNumber = numberOfButton;
                CheckChooseDialogPL12();
            }
            else if (levelPreset == LevelDialogQueue.Plansza21)
            {
                chooseNumber = numberOfButton;
                CheckChooseDialogPL21();
            }
            else if (levelPreset == LevelDialogQueue.Plansza32)
            {
                chooseNumber = numberOfButton;
                CheckChooseDialogPL32();
            }

            if (isChooseNeedBeDesappeared)
                ChooseDisappear();


            if (isChooseButtonDestroyed)
            {
                if (numberOfButton == 1)
                    button1Choosen = true;
                else if (numberOfButton == 2)
                    button2Choosen = true;
                else if (numberOfButton == 3)
                    button3Choosen = true;
            }
        }
    }

    private void CheckChooseDialogPL32()
    {
        if (chooseNumber == 1) // Был сделан первый выбор
        {
            AfterChoise(21);
        }
        else if (chooseNumber == 2) // Был сделан второй выбор
        {
            AfterChoise(25);
        }
    }

    private void CheckChooseDialogPL21()
    {
        if (chooseNumber == 1) // Был сделан первый выбор
        {
            AfterChoise(27);
        }
        else if (chooseNumber == 2) // Был сделан второй выбор
        {
            AfterChoise(30);
        }
    }

    private void CheckChooseDialogPL12()
    {
        if (chooseNumber == 1) // Был сделан первый выбор
        {
            AfterChoise(15);
        }
        else if (chooseNumber == 2) // Был сделан второй выбор
        {
            AfterChoise(18);
        }
    }

    private void CheckChooseDialogPL10()
    {
        if (chooseNumber == 1) // Был сделан первый выбор
        {
            if (choosenDialogElements[0] == 1) // Выбрана первая реплика
            {
                AfterChoise(4);
            }
            else if (choosenDialogElements[0] == 2) // Выбрана вторая реплика
            {
                AfterChoise(7);
            }
            else if (choosenDialogElements[0] == 3) // Выбрана третья реплика
            {
                AfterChoise(11);
            }
        }
        else if (chooseNumber == 2)
        {
            if (choosenDialogElements[1] == 1) // Выбрана первая реплика
            {
                AfterChoise(4);
            }
            else if (choosenDialogElements[1] == 2) // Выбрана вторая реплика
            {
                AfterChoise(7);
            }
            else if (choosenDialogElements[1] == 3) // Выбрана третья реплика
            {
                AfterChoise(11);
            }
        }
        else if (chooseNumber == 3)
        {
            if (choosenDialogElements[2] == 1) // Выбрана первая реплика
            {
                AfterChoise(4);
            }
            else if (choosenDialogElements[2] == 2) // Выбрана вторая реплика
            {
                AfterChoise(7);
            }
            else if (choosenDialogElements[2] == 3) // Выбрана третья реплика
            {
                AfterChoise(11);
            }
        }
    }

    private void CheckChooseDialogPL7()
    {
        if (chooseNumber == 1) // Был сделан первый выбор
        {
            AfterChoise(13);
        }
        else if (chooseNumber == 2) // Был сделан второй выбор
        {
            AfterChoise(18);
        }
    }

    private void CheckChooseDialogPL3()
    {
        if(chooseNumber == 1) // Был сделан первый выбор
        {
            if(choosenDialogElements[0] == 1) // Выбрана первая реплика
            {
                AfterChoise(31);
            }
            else if(choosenDialogElements[0] == 2) // Выбрана вторая реплика
            {
                AfterChoise(36);
            }
        }
        else if(chooseNumber == 2) // Был сделан второй выбор
        {
            if (choosenDialogElements[1] == 1) // Выбрана первая реплика
            {
                if (choosenDialogElements[0] == 1)
                    AfterChoise(36);
                else if(choosenDialogElements[0] == 2)
                    AfterChoise(31);
            }
            else if (choosenDialogElements[1] == 2) // Выбрана вторая реплика
            {
                AfterChoise(43);
            }
        }
    }

    private void AfterChoise(int dialogNumber)
    {
        if (isDialogPage && isTextRead && isDialogPanelActive)
        {
            dialogRead = dialogNumber;
            LevelPreset();
            isTextRead = false;

            StartCoroutine(DialogTypingTextTimer());
        }
    }

    private IEnumerator NameMarkAppear()
    {
        StopCoroutine(NameMarkDisappear());
    Reset:
        nameMarkAlpha += 0.05f;
        nameMark.color = new Color(1, 1, 1, nameMarkAlpha);
        nameMarkText.color = new Color(0, 0, 0, nameMarkAlpha);
        yield return new WaitForSeconds(0.01f);
        if (nameMarkAlpha < 1)
            goto Reset;
    }

    private IEnumerator NameMarkDisappear()
    {
        StopCoroutine(NameMarkAppear());
    Reset:
        nameMarkAlpha -= 0.05f;
        nameMark.color = new Color(1, 1, 1, nameMarkAlpha);
        nameMarkText.color = new Color(0, 0, 0, nameMarkAlpha);
        yield return new WaitForSeconds(0.01f);
        if (nameMarkAlpha > 0)
            goto Reset;
    }

    private void SettingsOnAwake()
    {
        fade = GetComponent<Fade>();
        bgPositionOnStart = backgroundImage.GetComponent<RectTransform>().localPosition;
        bgRotationOnStart = backgroundImage.GetComponent<RectTransform>().localRotation.eulerAngles;
        bgScaleOnStart = backgroundImage.GetComponent<RectTransform>().localScale;

        if (isDialogPage)
        {
            dialText = dialogTexts[dialogRead];
            nameMark.color = new Color(1, 1, 1, 0);
            nameMarkText.color = new Color(0, 0, 0, 0);
        }

        GameObject[] allObjects = GameObject.FindGameObjectsWithTag("ActiveObject");
        foreach (var obj in allObjects)
        {
            obj.SetActive(false);
        }
    }

    public void OpenNote()
    {
        GameObject g = Instantiate(note);
        g.transform.SetParent(transform);
        g.transform.localPosition = Vector3.zero;
        g.transform.localScale = new Vector3(1, 1, 1);
    }

    private void PlayDynamicSound(int number)
    {
        dynamicAudio.Stop();
        dynamicAudio.clip = dynamicSounds[number];
        dynamicAudio.Play();
    }

    private IEnumerator AudioOff()
    {
    Return:
        yield return new WaitForSecondsRealtime(0.1f);
        staticAudio.volume -= 0.001f;
        dynamicAudio.volume -= 0.001f;
        if (dynamicAudio.volume > 0 || staticAudio.volume > 0)
            goto Return;
    }

}
