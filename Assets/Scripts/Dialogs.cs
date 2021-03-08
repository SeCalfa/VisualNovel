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
        Plansza9,
        Plansza10,
        Plansza11,
        Plansza12,
        Plansza13
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
    [SerializeField] private Text choise1Text = null;
    [SerializeField] private Text choise2Text = null;
    [SerializeField] private GameObject note = null;

    [Header("Parametrs")]
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
    private Vector3 endLocLeft2 = new Vector3(480, 0, 0);
    private Vector3 endLocLeft15 = new Vector3(600, 0, 0);
    private Vector3 endLocRight2 = new Vector3(-480, 0, 0);
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
    private int[] choosenDialogElements = new int[2] { 0, 0 };
    private int chooseNumber = 0;
    private bool isChooseNeedBeDesappeared = true;

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

        if (Input.GetKeyDown(KeyCode.Space))
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
            }
            else if (isDialogPage && isTextRead && isDialogPanelActive && !isChoiseAppear)
            {
                LevelPreset();
                isTextRead = false;

                StartCoroutine(DialogTypingTextTimer());
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
        else if (levelPreset == LevelDialogQueue.Plansza13)
        {
            nameMarkText.text = "Bohater";
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
                case LevelDialogQueue.Plansza3: // Czukcz Przewodnik, Nikolai Morozov, Korav'ye
                    dialogRead += 1;

                    if (dialogRead == 1)
                    {
                        PlayerPrefs.SetInt("Plansza3", 1);
                    }
                    if (dialogRead == 3)
                    {
                        nameMarkText.text = "Bohater";
                        StartCoroutine(NameMarkAppear());
                        FindObjectOfType<SpriteElementFade>().ImageAppear();
                    }
                    else if (dialogRead == 4)
                    {
                        nameMarkText.text = "Profesor Antos";
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
                    else if (dialogRead == 6)
                    {
                        nameMarkText.text = "Profesor Antos";
                        StartCoroutine(NameMarkAppear());
                        allCharactersInScene["Profesor"].GetComponent<CharactersController>().isTalking = true;
                        allCharactersInScene["Profesor"].GetComponent<CharactersController>().SetEmotion(0);
                    }
                    else if (dialogRead == 7)
                    {
                        nameMarkText.text = "Nikolai Morozov";
                        allCharactersInScene.Add("Nikolai", SpawnCharacter(characters[1], rightSpawnPoint, endLocRight15, false));
                        allCharactersInScene["Nikolai"].GetComponent<CharactersController>().isTalking = true;
                        allCharactersInScene["Nikolai"].GetComponent<CharactersController>().isLeftSide = false;
                        allCharactersInScene["Profesor"].GetComponent<CharactersController>().isTalking = false;
                    }
                    else if (dialogRead == 8)
                    {
                        nameMarkText.text = "Bohater";
                        allCharactersInScene["Nikolai"].GetComponent<CharactersController>().isTalking = false;
                    }
                    else if (dialogRead == 9)
                    {
                        nameMarkText.text = "Profesor Antos";
                        allCharactersInScene["Profesor"].GetComponent<CharactersController>().isTalking = true;
                        allCharactersInScene["Profesor"].GetComponent<CharactersController>().SetEmotion(0);
                    }
                    else if (dialogRead == 10)
                    {
                        nameMarkText.text = "Nikolai Morozov";
                        allCharactersInScene["Nikolai"].GetComponent<CharactersController>().isTalking = true;
                        allCharactersInScene["Profesor"].GetComponent<CharactersController>().isTalking = false;
                        allCharactersInScene["Nikolai"].GetComponent<CharactersController>().SetEmotion(0);
                    }
                    else if (dialogRead == 11)
                    {
                        StartCoroutine(NameMarkDisappear());
                        allCharactersInScene["Nikolai"].GetComponent<CharactersController>().isTalking = false;
                    }
                    else if (dialogRead == 15)
                    {
                        nameMarkText.text = "Czukcz Przewodnik";
                        StartCoroutine(NameMarkAppear());
                        allCharactersInScene.Add("Czukcz", SpawnCharacter(characters[2], leftSpawnPoint, endLocLeft2, true));
                        allCharactersInScene["Czukcz"].GetComponent<CharactersController>().isTalking = true;
                        allCharactersInScene["Profesor"].GetComponent<CharactersController>().isNewLocationActive = true;
                        allCharactersInScene["Czukcz"].GetComponent<CharactersController>().SetEmotion(0);
                    }
                    else if (dialogRead == 16)
                    {
                        StartCoroutine(NameMarkDisappear());
                        allCharactersInScene["Czukcz"].GetComponent<CharactersController>().isTalking = false;
                    }
                    else if (dialogRead == 17)
                    {
                        nameMarkText.text = "Korav'ye";
                        StartCoroutine(NameMarkAppear());
                        allCharactersInScene["Czukcz"].GetComponent<CharactersController>().isTalking = true;
                        allCharactersInScene["Czukcz"].GetComponent<CharactersController>().SetEmotion(0);
                    }
                    else if (dialogRead == 18)
                    {
                        nameMarkText.text = "Profesor Antos";
                        allCharactersInScene["Czukcz"].GetComponent<CharactersController>().isTalking = false;
                        allCharactersInScene["Profesor"].GetComponent<CharactersController>().isTalking = true;
                        allCharactersInScene["Profesor"].GetComponent<CharactersController>().SetEmotion(3);
                    }
                    else if (dialogRead == 19)
                    {
                        nameMarkText.text = "Korav'ye";
                        allCharactersInScene["Profesor"].GetComponent<CharactersController>().isTalking = false;
                        allCharactersInScene["Czukcz"].GetComponent<CharactersController>().isTalking = true;
                        allCharactersInScene["Czukcz"].GetComponent<CharactersController>().SetEmotion(0);
                    }
                    else if (dialogRead == 20)
                    {
                        nameMarkText.text = "Nikolai Morozov";
                        allCharactersInScene["Czukcz"].GetComponent<CharactersController>().isTalking = false;
                        allCharactersInScene["Nikolai"].GetComponent<CharactersController>().isTalking = true;
                        allCharactersInScene["Nikolai"].GetComponent<CharactersController>().SetEmotion(0);
                    }
                    else if (dialogRead == 21)
                    {
                        StartCoroutine(NameMarkDisappear());
                        allCharactersInScene["Nikolai"].GetComponent<CharactersController>().isTalking = false;
                        allCharactersInScene["Profesor"].GetComponent<CharactersController>().SetEmotion(6);
                    }
                    else if (dialogRead == 22)
                    {
                        nameMarkText.text = "Profesor Antos";
                        StartCoroutine(NameMarkAppear());
                        allCharactersInScene["Profesor"].GetComponent<CharactersController>().isTalking = true;
                        allCharactersInScene["Profesor"].GetComponent<CharactersController>().SetEmotion(5);
                    }
                    else if (dialogRead == 23)
                    {
                        nameMarkText.text = "Korav'ye";
                        allCharactersInScene["Profesor"].GetComponent<CharactersController>().isTalking = false;
                        allCharactersInScene["Czukcz"].GetComponent<CharactersController>().isTalking = true;
                        allCharactersInScene["Czukcz"].GetComponent<CharactersController>().SetEmotion(0);
                    }
                    else if (dialogRead == 24)
                    {
                        nameMarkText.text = "Nikolai Morozov";
                        allCharactersInScene["Czukcz"].GetComponent<CharactersController>().isTalking = false;
                        allCharactersInScene["Nikolai"].GetComponent<CharactersController>().isTalking = true;
                        allCharactersInScene["Nikolai"].GetComponent<CharactersController>().SetEmotion(1);
                    }
                    else if (dialogRead == 25)
                    {
                        nameMarkText.text = "Korav'ye";
                        allCharactersInScene["Nikolai"].GetComponent<CharactersController>().isTalking = false;
                        allCharactersInScene["Czukcz"].GetComponent<CharactersController>().isTalking = true;
                        allCharactersInScene["Czukcz"].GetComponent<CharactersController>().SetEmotion(0);
                    }
                    else if (dialogRead == 26)
                    {
                        nameMarkText.text = "Nikolai Morozov";
                        allCharactersInScene["Czukcz"].GetComponent<CharactersController>().isTalking = false;
                        allCharactersInScene["Nikolai"].GetComponent<CharactersController>().isTalking = true;
                        allCharactersInScene["Nikolai"].GetComponent<CharactersController>().SetEmotion(1);
                    }
                    else if (dialogRead == 27)
                    {
                        StartCoroutine(NameMarkDisappear());
                        allCharactersInScene["Nikolai"].GetComponent<CharactersController>().isTalking = false;
                    }
                    else if (dialogRead == 28)
                    {
                        nameMarkText.text = "Profesor Antos";
                        StartCoroutine(NameMarkAppear());
                        allCharactersInScene["Profesor"].GetComponent<CharactersController>().isTalking = true;
                        allCharactersInScene["Profesor"].GetComponent<CharactersController>().SetEmotion(6);
                    }
                    else if (dialogRead == 29)
                    {
                        StartCoroutine(NameMarkDisappear());
                        allCharactersInScene["Profesor"].GetComponent<CharactersController>().isTalking = false;
                        allCharactersInScene["Nikolai"].GetComponent<CharactersController>().isLeave = true;
                        allCharactersInScene.Remove("Nikolai");
                    }
                    else if (dialogRead == 30)
                    {
                        nameMarkText.text = "Profesor Antos";
                        StartCoroutine(NameMarkAppear());
                        allCharactersInScene["Profesor"].GetComponent<CharactersController>().isTalking = true;
                        allCharactersInScene["Profesor"].GetComponent<CharactersController>().SetEmotion(5);
                    }
                    else if (dialogRead == 31)
                    {
                        nameMarkText.text = "Korav'ye";
                        allCharactersInScene["Czukcz"].GetComponent<CharactersController>().isTalking = true;
                        allCharactersInScene["Profesor"].GetComponent<CharactersController>().isTalking = false;
                        allCharactersInScene["Czukcz"].GetComponent<CharactersController>().SetEmotion(0);
                    }
                    else if (dialogRead == 32)
                    {
                        StartCoroutine(NameMarkDisappear());
                        allCharactersInScene["Czukcz"].GetComponent<CharactersController>().isTalking = false;
                        allCharactersInScene["Czukcz"].GetComponent<CharactersController>().isLeave = true;
                        allCharactersInScene.Remove("Czukcz");
                        allCharactersInScene["Profesor"].GetComponent<CharactersController>().isMiddleReturn = true;

                        ChooseAppear("O jakim konflikcie była mowa?", "Co właściwie robi rosyjski żołnierzna naszej wyprawie?");
                    }
                    else if (dialogRead == 33) // A // 33-45 = Диалоговая дорожка связанная с выбором игрока.........................................................................
                    {
                        nameMarkText.text = "Profesor Antos";
                        StartCoroutine(NameMarkAppear());
                        allCharactersInScene["Profesor"].GetComponent<CharactersController>().isTalking = true;
                        allCharactersInScene["Profesor"].GetComponent<CharactersController>().SetEmotion(0);
                    }
                    else if (dialogRead == 34) // A
                    {
                        nameMarkText.text = "Bohater";
                        allCharactersInScene["Profesor"].GetComponent<CharactersController>().isTalking = false;
                    }
                    else if (dialogRead == 35) // A
                    {
                        nameMarkText.text = "Profesor Antos";
                        allCharactersInScene["Profesor"].GetComponent<CharactersController>().isTalking = true;
                        allCharactersInScene["Profesor"].GetComponent<CharactersController>().SetEmotion(0);
                    }
                    else if (dialogRead == 36) // A
                    {
                        nameMarkText.text = "Bohater";
                        allCharactersInScene["Profesor"].GetComponent<CharactersController>().isTalking = false;

                        if (choosenDialogElements[1] == 0) // Второй выбор не был сделан
                        {
                            dialogRead = 40;
                        }
                        else
                        {
                            dialogRead = 42;
                        }
                    }
                    else if (dialogRead == 37) // B
                    {
                        nameMarkText.text = "Profesor Antos";
                        StartCoroutine(NameMarkAppear());
                        allCharactersInScene["Profesor"].GetComponent<CharactersController>().isTalking = true;
                        allCharactersInScene["Profesor"].GetComponent<CharactersController>().SetEmotion(0);
                    }
                    else if (dialogRead == 38) // B
                    {
                        nameMarkText.text = "Bohater";
                        allCharactersInScene["Profesor"].GetComponent<CharactersController>().isTalking = false;
                    }
                    else if (dialogRead == 39) // B
                    {
                        nameMarkText.text = "Profesor Antos";
                        allCharactersInScene["Profesor"].GetComponent<CharactersController>().isTalking = true;
                        allCharactersInScene["Profesor"].GetComponent<CharactersController>().SetEmotion(0);
                    }
                    else if (dialogRead == 40) // B
                    {
                        nameMarkText.text = "Bohater";
                        allCharactersInScene["Profesor"].GetComponent<CharactersController>().isTalking = false;

                        if (choosenDialogElements[1] == 0) // Второй выбор не был сделан
                        {
                            dialogRead = 40;
                        }
                        else
                        {
                            dialogRead = 42;
                        }
                    }
                    else if (dialogRead == 41)  // L
                    {
                        StartCoroutine(NameMarkDisappear());
                        allCharactersInScene["Profesor"].GetComponent<CharactersController>().isTalking = false;
                    }
                    else if (dialogRead == 42) // L
                    {
                        StartCoroutine(NameMarkAppear());
                        nameMarkText.text = "Profesor Antos";
                        allCharactersInScene["Profesor"].GetComponent<CharactersController>().isTalking = true;

                        if (choosenDialogElements[0] == 1) // Певый выбор был первой репликой
                        {
                            ChooseAppear("Co właściwie robi rosyjski żołnierz na naszej wyprawie?", "Nie, to nic takiego");
                        }
                        else if (choosenDialogElements[0] == 2) // Певый выбор был второй репликой
                        {
                            ChooseAppear("O jakim konflikcie była mowa?", "Nie, to nic takiego");
                        }

                        allCharactersInScene["Profesor"].GetComponent<CharactersController>().SetEmotion(3);
                    }
                    else if (dialogRead == 43) // 33-43 = Диалоговая дорожка связанная с выбором игрока.........................................................................
                    {
                        StartCoroutine(NameMarkDisappear());
                        allCharactersInScene["Profesor"].GetComponent<CharactersController>().isTalking = false;
                        allCharactersInScene["Profesor"].GetComponent<CharactersController>().isLeave = true;
                        allCharactersInScene.Remove("Profesor");
                    }
                    else if (dialogRead == 44)
                    {
                        StartCoroutine(NameMarkAppear());
                        nameMarkText.text = "Reporter";
                        allCharactersInScene.Add("Reporter", SpawnCharacter(characters[4], rightSpawnPoint, endLocRight15, false));
                        allCharactersInScene["Reporter"].GetComponent<CharactersController>().isLeftSide = false;
                        allCharactersInScene["Reporter"].GetComponent<CharactersController>().isTalking = true;
                        allCharactersInScene["Reporter"].GetComponent<CharactersController>().SetEmotion(3);
                    }
                    else if (dialogRead == 45)
                    {
                        nameMarkText.text = "Bohater";
                        allCharactersInScene["Reporter"].GetComponent<CharactersController>().isTalking = false;
                    }
                    else if (dialogRead == 46)
                    {
                        nameMarkText.text = "Czesław";
                        allCharactersInScene["Reporter"].GetComponent<CharactersController>().isTalking = true;
                        allCharactersInScene["Reporter"].GetComponent<CharactersController>().SetEmotion(3);
                    }
                    else if (dialogRead == 47)
                    {
                        StartCoroutine(NameMarkDisappear());
                        allCharactersInScene["Reporter"].GetComponent<CharactersController>().isTalking = false;
                    }
                    else if (dialogRead == 48)
                    {
                        StartCoroutine(NameMarkAppear());
                        nameMarkText.text = "Bohater";
                    }
                    else if (dialogRead == 49)
                    {
                        nameMarkText.text = "Czesław";
                        allCharactersInScene["Reporter"].GetComponent<CharactersController>().isTalking = true;
                        allCharactersInScene["Reporter"].GetComponent<CharactersController>().SetEmotion(3);
                    }
                    else if (dialogRead == 50)
                    {
                        nameMarkText.text = "Bohater";
                        allCharactersInScene["Reporter"].GetComponent<CharactersController>().isTalking = false;
                    }
                    else if (dialogRead == 51)
                    {
                        nameMarkText.text = "Czesław";
                        allCharactersInScene["Reporter"].GetComponent<CharactersController>().isTalking = true;
                        allCharactersInScene["Reporter"].GetComponent<CharactersController>().SetEmotion(2);
                    }
                    else if (dialogRead == 52)
                    {
                        nameMarkText.text = "Bohater";
                        allCharactersInScene["Reporter"].GetComponent<CharactersController>().isTalking = false;
                    }
                    else if (dialogRead == 53)
                    {
                        StartCoroutine(NameMarkDisappear());
                    }
                    else if (dialogRead == 54)
                    {
                        StartCoroutine(NameMarkAppear());
                        nameMarkText.text = "Czesław";
                        allCharactersInScene["Reporter"].GetComponent<CharactersController>().isTalking = true;
                        allCharactersInScene["Reporter"].GetComponent<CharactersController>().SetEmotion(1);
                    }
                    else if (dialogRead == 55)
                    {
                        nameMarkText.text = "Bohater";
                        allCharactersInScene["Reporter"].GetComponent<CharactersController>().isTalking = false;
                    }
                    else if (dialogRead == 56)
                    {
                        nameMarkText.text = "Inżynier";
                        allCharactersInScene.Add("Manya", SpawnCharacter(characters[3], leftSpawnPoint, endLocLeft15, false));
                        allCharactersInScene["Manya"].GetComponent<CharactersController>().isTalking = true;
                        allCharactersInScene["Manya"].GetComponent<CharactersController>().SetEmotion(0);
                    }
                    else if (dialogRead == 57)
                    {
                        nameMarkText.text = "Czesław";
                        allCharactersInScene["Manya"].GetComponent<CharactersController>().isTalking = false;
                        allCharactersInScene["Reporter"].GetComponent<CharactersController>().isTalking = true;
                        allCharactersInScene["Reporter"].GetComponent<CharactersController>().SetEmotion(3);
                    }
                    else if (dialogRead == 58)
                    {
                        StartCoroutine(NameMarkDisappear());
                        allCharactersInScene["Reporter"].GetComponent<CharactersController>().isTalking = false;
                    }
                    else if (dialogRead == 59)
                    {

                    }
                    else if (dialogRead == 60)
                    {
                        StartCoroutine(NameMarkAppear());
                        nameMarkText.text = "Czesław";
                        allCharactersInScene["Reporter"].GetComponent<CharactersController>().isTalking = true;
                        allCharactersInScene["Reporter"].GetComponent<CharactersController>().SetEmotion(0);
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
                        nameMarkText.text = "Bohater";
                        allCharactersInScene["Manya"].GetComponent<CharactersController>().isTalking = false;
                    }
                    else if (dialogRead == 63)
                    {
                        nameMarkText.text = "Manya";
                        allCharactersInScene["Manya"].GetComponent<CharactersController>().isTalking = true;
                        allCharactersInScene["Manya"].GetComponent<CharactersController>().SetEmotion(0);
                    }
                    else if (dialogRead == 64)
                    {
                        StartCoroutine(NameMarkDisappear());
                        allCharactersInScene["Manya"].GetComponent<CharactersController>().isTalking = false;
                    }
                    else if (dialogRead == 65)
                    {

                    }
                    else if (dialogRead == 66)
                    {
                        StartCoroutine(NameMarkAppear());
                        nameMarkText.text = "Nikolai";
                        allCharactersInScene.Add("Nikolai", SpawnCharacter(characters[1], rightSpawnPoint, endLocRight2, false));
                        allCharactersInScene["Nikolai"].GetComponent<CharactersController>().isTalking = true;
                        allCharactersInScene["Reporter"].GetComponent<CharactersController>().isNewLocationActive = true;
                        allCharactersInScene["Nikolai"].GetComponent<CharactersController>().SetEmotion(4);
                    }
                    else if (dialogRead == 67)
                    {
                        nameMarkText.text = "Manya";
                        allCharactersInScene["Nikolai"].GetComponent<CharactersController>().isTalking = false;
                        allCharactersInScene["Manya"].GetComponent<CharactersController>().isTalking = true;
                        allCharactersInScene["Manya"].GetComponent<CharactersController>().SetEmotion(1);
                    }
                    else if (dialogRead == 68)
                    {
                        nameMarkText.text = "Nikolai";
                        allCharactersInScene["Manya"].GetComponent<CharactersController>().isTalking = false;
                        allCharactersInScene["Nikolai"].GetComponent<CharactersController>().isTalking = true;
                        allCharactersInScene["Nikolai"].GetComponent<CharactersController>().SetEmotion(4);
                    }
                    else if (dialogRead == 69)
                    {
                        nameMarkText.text = "Manya";
                        allCharactersInScene["Nikolai"].GetComponent<CharactersController>().isTalking = false;
                        allCharactersInScene["Manya"].GetComponent<CharactersController>().isTalking = true;
                        allCharactersInScene["Manya"].GetComponent<CharactersController>().SetEmotion(1);
                    }
                    else if (dialogRead == 70)
                    {
                        StartCoroutine(NameMarkDisappear());
                        allCharactersInScene["Manya"].GetComponent<CharactersController>().isTalking = false;
                    }
                    else if (dialogRead == 71)
                    {
                        StartCoroutine(NameMarkAppear());
                        nameMarkText.text = "Czesław";
                        allCharactersInScene["Reporter"].GetComponent<CharactersController>().isTalking = true;
                        allCharactersInScene["Reporter"].GetComponent<CharactersController>().SetEmotion(3);
                    }
                    else if (dialogRead == 72)
                    {
                        StartCoroutine(NameMarkDisappear());
                        allCharactersInScene["Reporter"].GetComponent<CharactersController>().isTalking = false;
                        allCharactersInScene.Add("Profesor", SpawnCharacter(characters[0], leftSpawnPoint, endLocLeft2, false));
                        allCharactersInScene["Manya"].GetComponent<CharactersController>().isLeftSide = true;
                        allCharactersInScene["Manya"].GetComponent<CharactersController>().isNewLocationActive = true;
                    }
                    else if (dialogRead == 73)
                    {
                        StartCoroutine(NameMarkAppear());
                        nameMarkText.text = "Profesor Antos";
                        allCharactersInScene["Profesor"].GetComponent<CharactersController>().isTalking = true;
                        allCharactersInScene["Profesor"].GetComponent<CharactersController>().SetEmotion(0);
                    }
                    else if (dialogRead == 74)
                    {
                        StartCoroutine(NameMarkDisappear());
                        allCharactersInScene["Profesor"].GetComponent<CharactersController>().isTalking = false;
                    }
                    else if (dialogRead == 75)
                    {
                        StartCoroutine(NameMarkAppear());
                        nameMarkText.text = "Profesor Antos";
                        allCharactersInScene["Profesor"].GetComponent<CharactersController>().isTalking = true;
                        allCharactersInScene["Profesor"].GetComponent<CharactersController>().SetEmotion(0);
                    }
                    else if (dialogRead == 76) // End Level
                    {
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
                    if (dialogRead == 7)
                    {
                        nameMarkText.text = "Profesor Antos";
                        StartCoroutine(NameMarkAppear());
                    }
                    else if (dialogRead == 8)
                    {
                        StartCoroutine(NameMarkDisappear());
                    }
                    else if (dialogRead == 20)
                    {
                        fade.BlackFadeAppear(false, "");
                    }
                    else if (dialogRead == 26)
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
                    else if (dialogRead == 12)
                    {
                        nameMarkText.text = "Bohater";
                        StartCoroutine(NameMarkAppear());
                    }
                    else if (dialogRead == 13)
                    {
                        StartCoroutine(NameMarkDisappear());
                    }
                    else if (dialogRead == 22)
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
                    else if (dialogRead == 3)
                    {
                        nameMarkText.text = "Bohater";
                        StartCoroutine(NameMarkAppear());
                    }
                    else if (dialogRead == 4)
                    {
                        StartCoroutine(NameMarkDisappear());
                    }
                    else if (dialogRead == 7)
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
                        nameMarkText.text = "Bohater";
                        allCharactersInScene["Korav'ye"].GetComponent<CharactersController>().isTalking = false;
                    }
                    else if (dialogRead == 2)
                    {
                        nameMarkText.text = "Korav'ye";
                        allCharactersInScene["Korav'ye"].GetComponent<CharactersController>().isTalking = true;
                    }
                    else if (dialogRead == 3)
                    {
                        StartCoroutine(NameMarkDisappear());
                        allCharactersInScene["Korav'ye"].GetComponent<CharactersController>().isTalking = false;
                        allCharactersInScene["Korav'ye"].GetComponent<CharactersController>().isNewLocationActive = true;
                        allCharactersInScene.Add("Profesor", SpawnCharacter(characters[0], leftSpawnPoint, endLocLeft15, false));
                        allCharactersInScene["Profesor"].GetComponent<CharactersController>().isLeftSide = true;
                        allCharactersInScene.Add("Czesław", SpawnCharacter(characters[2], rightSpawnPoint, endLocRight2, false));
                        allCharactersInScene["Czesław"].GetComponent<CharactersController>().isLeftSide = false;
                    }
                    else if (dialogRead == 4)
                    {
                        nameMarkText.text = "Profesor Antos";
                        StartCoroutine(NameMarkAppear());
                        allCharactersInScene["Profesor"].GetComponent<CharactersController>().isTalking = true;
                    }
                    else if (dialogRead == 5)
                    {
                        StartCoroutine(NameMarkDisappear());
                        allCharactersInScene["Profesor"].GetComponent<CharactersController>().isTalking = false;
                    }
                    else if (dialogRead == 6)
                    {
                        nameMarkText.text = "Czesław";
                        StartCoroutine(NameMarkAppear());
                        allCharactersInScene["Czesław"].GetComponent<CharactersController>().isTalking = true;
                    }
                    else if (dialogRead == 7)
                    {
                        StartCoroutine(NameMarkDisappear());
                        allCharactersInScene["Czesław"].GetComponent<CharactersController>().isTalking = false;
                    }
                    else if (dialogRead == 8)
                    {
                        nameMarkText.text = "Profesor Antos";
                        StartCoroutine(NameMarkAppear());
                        allCharactersInScene["Profesor"].GetComponent<CharactersController>().isTalking = true;
                    }
                    else if (dialogRead == 9)
                    {
                        StartCoroutine(NameMarkDisappear());
                        allCharactersInScene["Profesor"].GetComponent<CharactersController>().isTalking = false;
                        allCharactersInScene["Profesor"].GetComponent<CharactersController>().isLeave = true;
                        allCharactersInScene.Remove("Profesor");
                    }
                    else if (dialogRead == 11)
                    {
                        nameMarkText.text = "Bohater";
                        StartCoroutine(NameMarkAppear());
                    }
                    else if (dialogRead == 12)
                    {
                        nameMarkText.text = "Czesław";
                        allCharactersInScene["Czesław"].GetComponent<CharactersController>().isTalking = true;

                        ChooseAppear("Czy tylko na mnie zadziałały zioła?", "Czyli sporo mnie ominęło.");
                    }
                    else if (dialogRead == 13) // 13 - 18 = Диалоговая дорожка связанная с выбором игрока.........................................................................
                    {
                        nameMarkText.text = "Bohater";
                        allCharactersInScene["Czesław"].GetComponent<CharactersController>().isTalking = false;
                    }
                    else if (dialogRead == 14) // A
                    {
                        nameMarkText.text = "Czesław";
                        allCharactersInScene["Czesław"].GetComponent<CharactersController>().isTalking = true;
                    }
                    else if (dialogRead == 15) // A
                    {
                        nameMarkText.text = "Bohater";
                        allCharactersInScene["Czesław"].GetComponent<CharactersController>().isTalking = false;
                    }
                    else if (dialogRead == 16) // A
                    {
                        nameMarkText.text = "Czesław";
                        allCharactersInScene["Czesław"].GetComponent<CharactersController>().isTalking = true;
                    }
                    else if (dialogRead == 17) // A
                    {
                        nameMarkText.text = "Bohater";
                        allCharactersInScene["Czesław"].GetComponent<CharactersController>().isTalking = false;
                        dialogRead++;
                    }
                    else if (dialogRead == 18) // A | 13 - 18 = Диалоговая дорожка связанная с выбором игрока.........................................................................
                    {
                        nameMarkText.text = "Bohater";
                        allCharactersInScene["Czesław"].GetComponent<CharactersController>().isTalking = false;
                    }
                    else if (dialogRead == 19)
                    {
                        StartCoroutine(NameMarkDisappear());
                        allCharactersInScene["Czesław"].GetComponent<CharactersController>().isLeave = true;
                        allCharactersInScene.Remove("Czesław");
                    }
                    else if (dialogRead == 20)
                    {
                        nameMarkText.text = "Nikolai";
                        StartCoroutine(NameMarkAppear());
                        allCharactersInScene.Add("Nikolai", SpawnCharacter(characters[3], rightSpawnPoint, endLocRight2, false));
                        allCharactersInScene["Nikolai"].GetComponent<CharactersController>().isLeftSide = false;
                        allCharactersInScene["Nikolai"].GetComponent<CharactersController>().isTalking = true;
                    }
                    else if (dialogRead == 21)
                    {
                        nameMarkText.text = "Bohater";
                        allCharactersInScene["Nikolai"].GetComponent<CharactersController>().isTalking = false;
                    }
                    else if (dialogRead == 22)
                    {
                        nameMarkText.text = "Nikolai";
                        allCharactersInScene["Nikolai"].GetComponent<CharactersController>().isTalking = true;
                    }
                    else if (dialogRead == 23)
                    {
                        nameMarkText.text = "Profesor";
                        allCharactersInScene["Nikolai"].GetComponent<CharactersController>().isTalking = false;
                        allCharactersInScene.Add("Profesor", SpawnCharacter(characters[0], leftSpawnPoint, endLocLeft15, false));
                        allCharactersInScene["Profesor"].GetComponent<CharactersController>().isLeftSide = true;
                        allCharactersInScene["Profesor"].GetComponent<CharactersController>().isTalking = true;
                    }
                    else if (dialogRead == 24)
                    {
                        StartCoroutine(NameMarkDisappear());
                        allCharactersInScene["Profesor"].GetComponent<CharactersController>().isTalking = false;
                    }
                    else if (dialogRead == 25)
                    {
                        nameMarkText.text = "Profesor";
                        StartCoroutine(NameMarkAppear());
                        allCharactersInScene["Profesor"].GetComponent<CharactersController>().isTalking = true;
                    }
                    else if (dialogRead == 26)
                    {
                        nameMarkText.text = "Nikolai";
                        allCharactersInScene["Profesor"].GetComponent<CharactersController>().isTalking = false;
                        allCharactersInScene["Nikolai"].GetComponent<CharactersController>().isTalking = true;
                    }
                    else if (dialogRead == 27)
                    {
                        nameMarkText.text = "Profesor";
                        allCharactersInScene["Nikolai"].GetComponent<CharactersController>().isTalking = false;
                        allCharactersInScene["Profesor"].GetComponent<CharactersController>().isTalking = true;
                    }
                    else if (dialogRead == 28)
                    {
                        nameMarkText.text = "Nikolai";
                        allCharactersInScene["Profesor"].GetComponent<CharactersController>().isTalking = false;
                        allCharactersInScene["Nikolai"].GetComponent<CharactersController>().isTalking = true;
                    }
                    else if (dialogRead == 29)
                    {
                        StartCoroutine(NameMarkDisappear());
                        allCharactersInScene["Nikolai"].GetComponent<CharactersController>().isTalking = false;
                    }
                    else if (dialogRead == 30)
                    {
                        nameMarkText.text = "Profesor";
                        StartCoroutine(NameMarkAppear());
                        allCharactersInScene["Profesor"].GetComponent<CharactersController>().isTalking = true;
                    }
                    else if (dialogRead == 31)
                    {
                        StartCoroutine(NameMarkDisappear());
                        allCharactersInScene["Profesor"].GetComponent<CharactersController>().isTalking = false;
                        allCharactersInScene.Add("Manya", SpawnCharacter(characters[4], leftSpawnPoint, endLocLeft2, false));
                        allCharactersInScene["Manya"].GetComponent<CharactersController>().isLeftSide = true;
                        allCharactersInScene["Profesor"].GetComponent<CharactersController>().isNewLocationActive = true;

                    }
                    else if (dialogRead == 32)
                    {
                        nameMarkText.text = "Manya";
                        StartCoroutine(NameMarkAppear());
                        allCharactersInScene["Manya"].GetComponent<CharactersController>().isTalking = true;
                    }
                    else if (dialogRead == 33)
                    {
                        nameMarkText.text = "Profesor";
                        allCharactersInScene["Manya"].GetComponent<CharactersController>().isTalking = false;
                        allCharactersInScene["Profesor"].GetComponent<CharactersController>().isTalking = true;
                    }
                    else if (dialogRead == 34)
                    {
                        StartCoroutine(NameMarkDisappear());
                        allCharactersInScene["Profesor"].GetComponent<CharactersController>().isTalking = false;
                    }
                    else if (dialogRead == 37)
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
                    }
                    else if (dialogRead == 5)
                    {
                        nameMarkText.text = "Profesor";
                        StartCoroutine(NameMarkAppear());
                        allCharactersInScene.Add("Profesor", SpawnCharacter(characters[0], leftSpawnPoint, endLocLeft15, false));
                        allCharactersInScene["Profesor"].GetComponent<CharactersController>().isTalking = true;
                        allCharactersInScene["Profesor"].GetComponent<CharactersController>().isLeftSide = true;
                    }
                    else if (dialogRead == 6)
                    {
                        nameMarkText.text = "Czesław";
                        allCharactersInScene["Profesor"].GetComponent<CharactersController>().isTalking = false;
                        allCharactersInScene.Add("Czesław", SpawnCharacter(characters[1], rightSpawnPoint, endLocRight15, false));
                        allCharactersInScene["Czesław"].GetComponent<CharactersController>().isTalking = true;
                        allCharactersInScene["Czesław"].GetComponent<CharactersController>().isLeftSide = false;
                    }
                    else if (dialogRead == 7)
                    {
                        StartCoroutine(NameMarkDisappear());
                        allCharactersInScene["Czesław"].GetComponent<CharactersController>().isTalking = false;
                        allCharactersInScene["Czesław"].GetComponent<CharactersController>().isLeave = true;
                        allCharactersInScene.Remove("Czesław");
                    }
                    else if(dialogRead == 8)
                    {
                        FindObjectOfType<SpriteElementFade>().ImageAppear();
                    }
                    else if (dialogRead == 14)
                    {
                        nameMarkText.text = "Nikolai";
                        StartCoroutine(NameMarkAppear());
                        allCharactersInScene.Add("Nikolai", SpawnCharacter(characters[2], rightSpawnPoint, endLocRight15, false));
                        allCharactersInScene["Nikolai"].GetComponent<CharactersController>().isTalking = true;
                        allCharactersInScene["Nikolai"].GetComponent<CharactersController>().isLeftSide = false;
                    }
                    else if (dialogRead == 15)
                    {
                        nameMarkText.text = "Manya";
                        allCharactersInScene["Nikolai"].GetComponent<CharactersController>().isTalking = false;
                        allCharactersInScene.Add("Manya", SpawnCharacter(characters[3], leftSpawnPoint, endLocLeft2, false));
                        allCharactersInScene["Manya"].GetComponent<CharactersController>().isTalking = true;
                        allCharactersInScene["Manya"].GetComponent<CharactersController>().isLeftSide = true;
                        allCharactersInScene["Profesor"].GetComponent<CharactersController>().isNewLocationActive = true;
                    }
                    else if (dialogRead == 16)
                    {
                        nameMarkText.text = "Profesor";
                        allCharactersInScene["Manya"].GetComponent<CharactersController>().isTalking = false;
                        allCharactersInScene["Profesor"].GetComponent<CharactersController>().isTalking = true;
                    }
                    else if (dialogRead == 17)
                    {
                        StartCoroutine(NameMarkDisappear());
                        allCharactersInScene["Profesor"].GetComponent<CharactersController>().isTalking = false;
                    }
                    else if(dialogRead == 19)
                    {
                        FindObjectOfType<SpriteElementFade>().ImageDisappear();
                    }
                    else if (dialogRead == 21)
                    {
                        nameMarkText.text = "Korav'ye";
                        StartCoroutine(NameMarkAppear());
                        allCharactersInScene.Add("Korav'ye", SpawnCharacter(characters[4], rightSpawnPoint, endLocRight2, false));
                        allCharactersInScene["Korav'ye"].GetComponent<CharactersController>().isTalking = true;
                        allCharactersInScene["Korav'ye"].GetComponent<CharactersController>().isLeftSide = false;
                        allCharactersInScene["Nikolai"].GetComponent<CharactersController>().isNewLocationActive = true;
                    }
                    else if (dialogRead == 22)
                    {
                        nameMarkText.text = "Nikolai";
                        allCharactersInScene["Korav'ye"].GetComponent<CharactersController>().isTalking = false;
                        allCharactersInScene["Nikolai"].GetComponent<CharactersController>().isTalking = true;
                    }
                    else if (dialogRead == 23)
                    {
                        StartCoroutine(NameMarkDisappear());
                        allCharactersInScene["Nikolai"].GetComponent<CharactersController>().isTalking = false;
                    }
                    else if (dialogRead == 24)
                    {
                        nameMarkText.text = "Profesor";
                        StartCoroutine(NameMarkAppear());
                        allCharactersInScene["Profesor"].GetComponent<CharactersController>().isTalking = true;
                    }
                    else if (dialogRead == 25)
                    {
                        StartCoroutine(NameMarkDisappear());
                        allCharactersInScene["Profesor"].GetComponent<CharactersController>().isTalking = false;
                    }
                    else if (dialogRead == 26)
                    {
                        nameMarkText.text = "Korav'ye";
                        StartCoroutine(NameMarkAppear());
                        allCharactersInScene["Korav'ye"].GetComponent<CharactersController>().isTalking = true;
                    }
                    else if (dialogRead == 27)
                    {
                        StartCoroutine(NameMarkDisappear());
                        allCharactersInScene["Korav'ye"].GetComponent<CharactersController>().isTalking = false;
                    }
                    else if (dialogRead == 28)
                    {
                        nameMarkText.text = "Bohater";
                        StartCoroutine(NameMarkAppear());
                    }
                    else if (dialogRead == 29)
                    {
                        StartCoroutine(NameMarkDisappear());
                    }
                    else if (dialogRead == 30)
                    {
                        nameMarkText.text = "Profesor";
                        StartCoroutine(NameMarkAppear());
                        allCharactersInScene["Profesor"].GetComponent<CharactersController>().isTalking = true;
                    }
                    else if (dialogRead == 31)
                    {
                        StartCoroutine(NameMarkDisappear());
                        allCharactersInScene["Profesor"].GetComponent<CharactersController>().isTalking = false;
                    }
                    else if (dialogRead == 32)
                    {
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
                    else if (dialogRead == 4)
                    {
                        nameMarkText.text = "Bohater";
                        StartCoroutine(NameMarkAppear());
                    }
                    else if (dialogRead == 5)
                    {
                        StartCoroutine(NameMarkDisappear());

                        allCharactersInScene.Add("Korav'ye", SpawnCharacter(characters[0], leftSpawnPoint, endLocLeft15, true));
                        allCharactersInScene["Korav'ye"].GetComponent<CharactersController>().isTalking = false;
                        allCharactersInScene["Korav'ye"].GetComponent<CharactersController>().isLeftSide = true;

                        ChooseAppear("Byłeś niezadowolony kiedy wróciłeś sprzed drzwi. Co się stało?", "Byłeś tu wcześniej?");
                    }
                    else if (dialogRead == 6)
                    {
                        nameMarkText.text = "Korav'ye";
                        StartCoroutine(NameMarkAppear());
                        allCharactersInScene["Korav'ye"].GetComponent<CharactersController>().isTalking = true;

                        if (choosenDialogElements[1] == 0)
                        {
                            isChooseNeedBeDesappeared = false;
                        }
                        dialogRead += 1;
                    }
                    else if (dialogRead == 7)
                    {
                        nameMarkText.text = "Korav'ye";
                        StartCoroutine(NameMarkAppear());
                        allCharactersInScene["Korav'ye"].GetComponent<CharactersController>().isTalking = true;

                        if (choosenDialogElements[1] == 0)
                        {
                            isChooseNeedBeDesappeared = false;
                            dialogRead += 1;
                        }
                    }
                    else if (dialogRead == 8)
                    {
                        StartCoroutine(NameMarkDisappear());
                        allCharactersInScene["Korav'ye"].GetComponent<CharactersController>().isTalking = false;
                    }
                    else if (dialogRead == 9)
                    {
                        nameMarkText.text = "Korav'ye";
                        StartCoroutine(NameMarkAppear());
                        allCharactersInScene["Korav'ye"].GetComponent<CharactersController>().isTalking = true;
                    }
                    else if (dialogRead == 10)
                    {
                        nameMarkText.text = "Bohater";
                        allCharactersInScene["Korav'ye"].GetComponent<CharactersController>().isTalking = false;
                    }
                    else if (dialogRead == 11)
                    {
                        StartCoroutine(NameMarkDisappear());
                    }
                    else if (dialogRead == 12)
                    {
                        nameMarkText.text = "Korav'ye";
                        StartCoroutine(NameMarkAppear());
                        allCharactersInScene["Korav'ye"].GetComponent<CharactersController>().isTalking = true;
                    }
                    else if (dialogRead == 13)
                    {
                        nameMarkText.text = "Bohater";
                        allCharactersInScene["Korav'ye"].GetComponent<CharactersController>().isTalking = false;
                    }
                    else if (dialogRead == 14)
                    {
                        nameMarkText.text = "Korav'ye";
                        allCharactersInScene["Korav'ye"].GetComponent<CharactersController>().isTalking = true;
                    }
                    else if (dialogRead == 15)
                    {
                        StartCoroutine(NameMarkDisappear());
                        allCharactersInScene["Korav'ye"].GetComponent<CharactersController>().isTalking = false;
                    }
                    else if (dialogRead == 16)
                    {
                        nameMarkText.text = "Bohater";
                        StartCoroutine(NameMarkAppear());
                    }
                    else if (dialogRead == 17)
                    {
                        nameMarkText.text = "Korav'ye";
                        allCharactersInScene["Korav'ye"].GetComponent<CharactersController>().isTalking = true;
                    }
                    else if (dialogRead == 18)
                    {
                        StartCoroutine(NameMarkDisappear());
                        allCharactersInScene["Korav'ye"].GetComponent<CharactersController>().isTalking = false;
                    }
                    else if (dialogRead == 19)
                    {
                        nameMarkText.text = "Korav'ye";
                        StartCoroutine(NameMarkAppear());
                        allCharactersInScene["Korav'ye"].GetComponent<CharactersController>().isTalking = true;
                    }
                    else if (dialogRead == 20)
                    {
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
                    }
                    else if (dialogRead == 4)
                    {
                        nameMarkText.text = "Nikolai";
                        StartCoroutine(NameMarkAppear());
                        allCharactersInScene.Add("Nikolai", SpawnCharacter(characters[0], rightSpawnPoint, endLocRight15, false));
                        allCharactersInScene["Nikolai"].GetComponent<CharactersController>().isTalking = true;
                        allCharactersInScene["Nikolai"].GetComponent<CharactersController>().isLeftSide = false;
                    }
                    else if (dialogRead == 5)
                    {
                        nameMarkText.text = "Profesor Antos";
                        allCharactersInScene["Nikolai"].GetComponent<CharactersController>().isTalking = false;
                        allCharactersInScene.Add("Profesor", SpawnCharacter(characters[1], leftSpawnPoint, endLocLeft15, false));
                        allCharactersInScene["Profesor"].GetComponent<CharactersController>().isTalking = true;
                        allCharactersInScene["Profesor"].GetComponent<CharactersController>().isLeftSide = true;
                    }
                    else if (dialogRead == 6)
                    {
                        nameMarkText.text = "Nikolai";
                        allCharactersInScene["Profesor"].GetComponent<CharactersController>().isTalking = false;
                        allCharactersInScene["Nikolai"].GetComponent<CharactersController>().isTalking = true;
                    }
                    else if (dialogRead == 7)
                    {
                        nameMarkText.text = "Profesor Antos";
                        allCharactersInScene["Profesor"].GetComponent<CharactersController>().isTalking = true;
                        allCharactersInScene["Nikolai"].GetComponent<CharactersController>().isTalking = false;
                    }
                    else if (dialogRead == 8)
                    {
                        nameMarkText.text = "Nikolai";
                        allCharactersInScene["Profesor"].GetComponent<CharactersController>().isTalking = false;
                        allCharactersInScene["Nikolai"].GetComponent<CharactersController>().isTalking = true;
                    }
                    else if (dialogRead == 9)
                    {
                        nameMarkText.text = "Profesor Antos";
                        allCharactersInScene["Profesor"].GetComponent<CharactersController>().isTalking = true;
                        allCharactersInScene["Nikolai"].GetComponent<CharactersController>().isTalking = false;
                    }
                    else if (dialogRead == 10)
                    {
                        nameMarkText.text = "Nikolai";
                        allCharactersInScene["Profesor"].GetComponent<CharactersController>().isTalking = false;
                        allCharactersInScene["Nikolai"].GetComponent<CharactersController>().isTalking = true;
                    }
                    else if (dialogRead == 11)
                    {
                        StartCoroutine(NameMarkDisappear());
                        allCharactersInScene["Nikolai"].GetComponent<CharactersController>().isTalking = false;
                    }
                    else if (dialogRead == 14)
                    {
                        nameMarkText.text = "Korav'ye";
                        StartCoroutine(NameMarkAppear());
                        allCharactersInScene.Add("Korav'ye", SpawnCharacter(characters[2], rightSpawnPoint, endLocRight2, false));
                        allCharactersInScene["Korav'ye"].GetComponent<CharactersController>().isTalking = true;
                        allCharactersInScene["Korav'ye"].GetComponent<CharactersController>().isLeftSide = false;
                        allCharactersInScene["Nikolai"].GetComponent<CharactersController>().isNewLocationActive = true;
                    }
                    else if (dialogRead == 15)
                    {
                        nameMarkText.text = "Manya";
                        allCharactersInScene["Korav'ye"].GetComponent<CharactersController>().isTalking = false;
                        allCharactersInScene.Add("Manya", SpawnCharacter(characters[3], leftSpawnPoint, endLocLeft2, false));
                        allCharactersInScene["Manya"].GetComponent<CharactersController>().isTalking = true;
                        allCharactersInScene["Manya"].GetComponent<CharactersController>().isLeftSide = true;
                        allCharactersInScene["Profesor"].GetComponent<CharactersController>().isNewLocationActive = true;
                    }
                    else if (dialogRead == 16)
                    {
                        nameMarkText.text = "Profesor Antos";
                        allCharactersInScene["Manya"].GetComponent<CharactersController>().isTalking = false;
                        allCharactersInScene["Profesor"].GetComponent<CharactersController>().isTalking = true;
                    }
                    else if (dialogRead == 17)
                    {
                        StartCoroutine(NameMarkDisappear());
                        allCharactersInScene["Profesor"].GetComponent<CharactersController>().isTalking = false;
                    }
                    else if (dialogRead == 23)
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
                    else if (dialogRead == 6)
                    {
                        nameMarkText.text = "Profesor Antos";
                        StartCoroutine(NameMarkAppear());
                        allCharactersInScene.Add("Profesor", SpawnCharacter(characters[0], leftSpawnPoint, endLocLeft15, false));
                        allCharactersInScene["Profesor"].GetComponent<CharactersController>().isTalking = true;
                        allCharactersInScene["Profesor"].GetComponent<CharactersController>().isLeftSide = true;
                    }
                    else if (dialogRead == 7)
                    {
                        StartCoroutine(NameMarkDisappear());
                        allCharactersInScene["Profesor"].GetComponent<CharactersController>().isTalking = false;
                    }
                    else if (dialogRead == 10)
                    {
                        nameMarkText.text = "Bohater";
                        StartCoroutine(NameMarkAppear());
                    }
                    else if (dialogRead == 11)
                    {
                        nameMarkText.text = "Profesor Antos";
                        allCharactersInScene["Profesor"].GetComponent<CharactersController>().isTalking = true;
                    }
                    else if (dialogRead == 12)
                    {
                        StartCoroutine(NameMarkDisappear());
                        allCharactersInScene["Profesor"].GetComponent<CharactersController>().isTalking = false;
                    }
                    else if (dialogRead == 13)
                    {
                        nameMarkText.text = "Manya";
                        StartCoroutine(NameMarkAppear());
                        allCharactersInScene.Add("Manya", SpawnCharacter(characters[3], leftSpawnPoint, endLocLeft2, false));
                        allCharactersInScene["Manya"].GetComponent<CharactersController>().isTalking = true;
                        allCharactersInScene["Manya"].GetComponent<CharactersController>().isLeftSide = true;
                        allCharactersInScene["Profesor"].GetComponent<CharactersController>().isNewLocationActive = true;
                    }
                    else if (dialogRead == 14)
                    {
                        nameMarkText.text = "Nikolai";
                        allCharactersInScene["Manya"].GetComponent<CharactersController>().isTalking = false;
                        allCharactersInScene.Add("Nikolai", SpawnCharacter(characters[1], rightSpawnPoint, endLocRight15, false));
                        allCharactersInScene["Nikolai"].GetComponent<CharactersController>().isTalking = true;
                        allCharactersInScene["Nikolai"].GetComponent<CharactersController>().isLeftSide = false;
                    }
                    else if (dialogRead == 15)
                    {
                        StartCoroutine(NameMarkDisappear());
                        allCharactersInScene["Nikolai"].GetComponent<CharactersController>().isTalking = false;
                    }
                    else if (dialogRead == 17)
                    {
                        nameMarkText.text = "Bohater";
                        StartCoroutine(NameMarkAppear());
                    }
                    else if (dialogRead == 18)
                    {
                        StartCoroutine(NameMarkDisappear());
                    }
                    else if (dialogRead == 19)
                    {
                        nameMarkText.text = "Nikolai";
                        StartCoroutine(NameMarkAppear());
                        allCharactersInScene["Nikolai"].GetComponent<CharactersController>().isTalking = true;
                    }
                    else if (dialogRead == 20)
                    {
                        StartCoroutine(NameMarkDisappear());
                        allCharactersInScene["Nikolai"].GetComponent<CharactersController>().isTalking = false;
                    }
                    else if (dialogRead == 22)
                    {
                        nameMarkText.text = "Nikolai";
                        StartCoroutine(NameMarkAppear());
                        allCharactersInScene["Nikolai"].GetComponent<CharactersController>().isTalking = true;

                        ChooseAppear("Dlaczego tak nie znosisz Korav’ye?", "Nie jesteś religijny?");
                    }
                    else if (dialogRead == 23)
                    {
                        nameMarkText.text = "Nikolai";

                        if (choosenDialogElements[0] == 1)
                        {
                            dialogRead += 1;
                        }
                        else
                        {
                            dialogRead = 25;
                            StartCoroutine(NameMarkAppear());
                            allCharactersInScene["Nikolai"].GetComponent<CharactersController>().isTalking = true;
                        }
                    }
                    else if (dialogRead == 24)
                    {
                        nameMarkText.text = "Nikolai";

                        if (chooseNumber == 2)
                        {
                            dialogRead += 1;
                            StartCoroutine(NameMarkAppear());
                            allCharactersInScene["Nikolai"].GetComponent<CharactersController>().isTalking = true;
                        }
                    }
                    else if (dialogRead == 25) // Opis
                    {
                        StartCoroutine(NameMarkDisappear());
                        allCharactersInScene["Nikolai"].GetComponent<CharactersController>().isTalking = false;

                        if (choosenDialogElements[0] == 1)
                        {
                            ChooseAppear("Nie jesteś religijny?", "Przepraszam, ale Sabr nadal stoi przed wejściem i nie mam serca patrzeć jak tam marznie. Pójdę po niego.");
                        }
                        else if (choosenDialogElements[0] == 2)
                        {
                            ChooseAppear("Dlaczego tak nie znosisz Korav’ye?", "Przepraszam, ale Sabr nadal stoi przed wejściem i nie mam serca patrzeć jak tam marznie. Pójdę po niego.");
                        }
                    }
                    else if (dialogRead == 26)
                    {
                        nameMarkText.text = "Bohater";
                        allCharactersInScene["Nikolai"].GetComponent<CharactersController>().isTalking = false;
                    }
                    else if (dialogRead == 27)
                    {
                        StartCoroutine(NameMarkDisappear());
                    }
                    else if (dialogRead == 29)
                    {
                        nameMarkText.text = "Bohater";
                        StartCoroutine(NameMarkAppear());
                    }
                    else if (dialogRead == 30)
                    {
                        StartCoroutine(NameMarkDisappear());
                    }
                    else if (dialogRead == 31)
                    {
                        nameMarkText.text = "Profesor Antos";
                        StartCoroutine(NameMarkAppear());
                        allCharactersInScene["Profesor"].GetComponent<CharactersController>().isTalking = true;
                    }
                    else if (dialogRead == 32)
                    {
                        nameMarkText.text = "Czesław";
                        allCharactersInScene["Profesor"].GetComponent<CharactersController>().isTalking = false;
                        allCharactersInScene.Add("Czesław", SpawnCharacter(characters[4], rightSpawnPoint, endLocRight2, false));
                        allCharactersInScene["Czesław"].GetComponent<CharactersController>().isTalking = true;
                        allCharactersInScene["Czesław"].GetComponent<CharactersController>().isLeftSide = false;
                        allCharactersInScene["Nikolai"].GetComponent<CharactersController>().isNewLocationActive = true;
                    }
                    else if (dialogRead == 33)
                    {
                        nameMarkText.text = "Korav'ye";
                        allCharactersInScene["Czesław"].GetComponent<CharactersController>().isTalking = false;
                    }
                    else if (dialogRead == 34)
                    {
                        nameMarkText.text = "Profesor Antos";
                        allCharactersInScene["Profesor"].GetComponent<CharactersController>().isTalking = true;
                    }
                    else if (dialogRead == 35)
                    {
                        nameMarkText.text = "Czesław";
                        allCharactersInScene["Profesor"].GetComponent<CharactersController>().isTalking = false;
                        allCharactersInScene["Czesław"].GetComponent<CharactersController>().isTalking = true;
                    }
                    else if (dialogRead == 36)
                    {
                        nameMarkText.text = "Profesor Antos";
                        allCharactersInScene["Profesor"].GetComponent<CharactersController>().isTalking = true;
                        allCharactersInScene["Czesław"].GetComponent<CharactersController>().isTalking = false;
                    }
                    else if (dialogRead == 37)
                    {
                        StartCoroutine(NameMarkDisappear());
                        allCharactersInScene["Profesor"].GetComponent<CharactersController>().isTalking = false;
                    }
                    else if (dialogRead == 38)
                    {
                        nameMarkText.text = "Czesław";
                        StartCoroutine(NameMarkAppear());
                        allCharactersInScene["Czesław"].GetComponent<CharactersController>().isTalking = true;
                    }
                    else if (dialogRead == 39)
                    {
                        nameMarkText.text = "Bohater";
                        allCharactersInScene["Czesław"].GetComponent<CharactersController>().isTalking = false;
                    }
                    else if (dialogRead == 40)
                    {
                        nameMarkText.text = "Czesław";
                        allCharactersInScene["Czesław"].GetComponent<CharactersController>().isTalking = true;
                    }
                    else if (dialogRead == 41)
                    {
                        StartCoroutine(NameMarkDisappear());
                        allCharactersInScene["Czesław"].GetComponent<CharactersController>().isTalking = false;
                    }
                    else if (dialogRead == 42)
                    {
                        nameMarkText.text = "Czesław";
                        StartCoroutine(NameMarkAppear());
                        allCharactersInScene["Czesław"].GetComponent<CharactersController>().isTalking = true;
                    }
                    else if (dialogRead == 43)
                    {
                        StartCoroutine(NameMarkDisappear());
                        allCharactersInScene["Czesław"].GetComponent<CharactersController>().isTalking = false;
                    }
                    else if (dialogRead == 45)
                    {
                        nameMarkText.text = "Czesław";
                        StartCoroutine(NameMarkAppear());
                        allCharactersInScene["Czesław"].GetComponent<CharactersController>().isTalking = true;
                    }
                    else if (dialogRead == 46)
                    {
                        StartCoroutine(NameMarkDisappear());
                        allCharactersInScene["Czesław"].GetComponent<CharactersController>().isTalking = false;
                    }
                    else if (dialogRead == 48)
                    {
                        nameMarkText.text = "Czesław";
                        StartCoroutine(NameMarkAppear());
                        allCharactersInScene["Czesław"].GetComponent<CharactersController>().isTalking = true;
                    }
                    else if (dialogRead == 49)
                    {
                        StartCoroutine(NameMarkDisappear());
                        allCharactersInScene["Czesław"].GetComponent<CharactersController>().isTalking = false;
                    }
                    else if (dialogRead == 52)
                    {
                        nameMarkText.text = "Manya";
                        StartCoroutine(NameMarkAppear());
                        allCharactersInScene["Manya"].GetComponent<CharactersController>().isTalking = true;
                    }
                    else if (dialogRead == 53)
                    {
                        nameMarkText.text = "Czesław";
                        allCharactersInScene["Manya"].GetComponent<CharactersController>().isTalking = false;
                        allCharactersInScene["Czesław"].GetComponent<CharactersController>().isTalking = true;
                    }
                    else if (dialogRead == 54)
                    {
                        StartCoroutine(NameMarkDisappear());
                        allCharactersInScene["Nikolai"].GetComponent<CharactersController>().isMiddleReturn = true;
                        allCharactersInScene["Czesław"].GetComponent<CharactersController>().isLeave = true;
                        allCharactersInScene["Czesław"].GetComponent<CharactersController>().isTalking = false;
                    }
                    else if (dialogRead == 58)
                    {
                        nameMarkText.text = "Profesor Antos";
                        StartCoroutine(NameMarkAppear());
                        allCharactersInScene["Profesor"].GetComponent<CharactersController>().isTalking = true;
                        allCharactersInScene.Remove("Czesław");
                    }
                    else if (dialogRead == 59)
                    {
                        StartCoroutine(NameMarkDisappear());
                        allCharactersInScene["Profesor"].GetComponent<CharactersController>().isTalking = false;
                    }
                    else if (dialogRead == 61)
                    {
                        nameMarkText.text = "Bohater";
                        StartCoroutine(NameMarkAppear());
                    }
                    else if (dialogRead == 62)
                    {
                        nameMarkText.text = "Profesor Antos";
                        allCharactersInScene["Profesor"].GetComponent<CharactersController>().isTalking = true;
                    }
                    else if (dialogRead == 63)
                    {
                        StartCoroutine(NameMarkDisappear());
                        allCharactersInScene["Profesor"].GetComponent<CharactersController>().isTalking = false;
                    }
                    else if (dialogRead == 65)
                    {
                        nameMarkText.text = "Profesor Antos";
                        StartCoroutine(NameMarkAppear());
                        allCharactersInScene["Profesor"].GetComponent<CharactersController>().isTalking = true;
                    }
                    else if (dialogRead == 66)
                    {
                        StartCoroutine(NameMarkDisappear());
                        allCharactersInScene["Profesor"].GetComponent<CharactersController>().isTalking = false;
                    }
                    else if (dialogRead == 67)
                    {
                        nameMarkText.text = "Manya";
                        StartCoroutine(NameMarkAppear());
                        allCharactersInScene["Manya"].GetComponent<CharactersController>().isTalking = true;
                    }
                    else if (dialogRead == 68)
                    {
                        StartCoroutine(NameMarkDisappear());
                        allCharactersInScene["Manya"].GetComponent<CharactersController>().isTalking = false;
                    }
                    else if (dialogRead == 69)
                    {
                        nameMarkText.text = "Bohater";
                        StartCoroutine(NameMarkAppear());
                    }
                    else if (dialogRead == 70)
                    {
                        nameMarkText.text = "Nikolai";
                        allCharactersInScene["Nikolai"].GetComponent<CharactersController>().isTalking = true;
                    }
                    else if (dialogRead == 71)
                    {
                        StartCoroutine(NameMarkDisappear());
                        allCharactersInScene["Nikolai"].GetComponent<CharactersController>().isTalking = false;
                    }
                    else if (dialogRead == 72)
                    {
                        nameMarkText.text = "Profesor Antos";
                        StartCoroutine(NameMarkAppear());
                        allCharactersInScene["Profesor"].GetComponent<CharactersController>().isTalking = true;
                    }
                    else if (dialogRead == 73)
                    {
                        StartCoroutine(NameMarkDisappear());
                        allCharactersInScene["Profesor"].GetComponent<CharactersController>().isTalking = false;
                    }
                    else if (dialogRead == 75)
                    {
                        nameMarkText.text = "Bohater";
                        StartCoroutine(NameMarkAppear());
                    }
                    else if (dialogRead == 76)
                    {
                        StartCoroutine(NameMarkDisappear());
                    }
                    else if (dialogRead == 80)
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
                        StartCoroutine(NameMarkDisappear());
                        PlayerPrefs.SetInt("Plansza13", 1);
                    }
                    else if (dialogRead == 5)
                    {
                        nameMarkText.text = "Korav'ye";
                        StartCoroutine(NameMarkAppear());
                    }
                    else if (dialogRead == 6)
                    {
                        nameMarkText.text = "Bohater";
                    }
                    else if (dialogRead == 7)
                    {
                        StartCoroutine(NameMarkDisappear());
                    }
                    else if (dialogRead == 11)
                    {
                        fade.FadeAppear(nextLevelName);
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
        choise1.GetComponent<Image>().enabled = false;
        choise2.GetComponent<Image>().enabled = false;
        choise1Text.enabled = false;
        choise2Text.enabled = false;
        choise1.GetComponent<Button>().interactable = false;
        choise2.GetComponent<Button>().interactable = false;
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
                chooseNumber++;
                CheckChooseDialogPL12();
            }

            if (isChooseNeedBeDesappeared)
                ChooseDisappear();
        }
    }

    private void CheckChooseDialogPL12()
    {
        if (chooseNumber == 1) // Был сделан первый выбор
        {
            if (choosenDialogElements[0] == 1) // Выбрана первая реплика
            {
                AfterChoise(22);
            }
            else if (choosenDialogElements[0] == 2) // Выбрана вторая реплика
            {
                AfterChoise(23);
            }
        }
        else if (chooseNumber == 2) // Был сделан второй выбор
        {
            if (choosenDialogElements[1] == 1) // Выбрана первая реплика
            {
                if (choosenDialogElements[0] == 1)
                    AfterChoise(23);
                else if (choosenDialogElements[0] == 2)
                    AfterChoise(22);
            }
            else if (choosenDialogElements[1] == 2) // Выбрана вторая реплика
            {
                AfterChoise(26);
            }
        }
    }

    private void CheckChooseDialogPL10()
    {
        if (chooseNumber == 1) // Был сделан первый выбор
        {
            if (choosenDialogElements[0] == 1) // Выбрана первая реплика
            {
                AfterChoise(5);
                ChooseAppear("Byłeś tu wcześniej?", "Chciałbym zapytać o symbol znad wrót. Wiem, że taki sam widnieje na twoim amulecie. Widziałem go też podczas wizji.");
            }
            else if (choosenDialogElements[0] == 2) // Выбрана вторая реплика
            {
                AfterChoise(6);
                ChooseAppear("Byłeś niezadowolony kiedy wróciłeś sprzed drzwi. Co się stało?", "Chciałbym zapytać o symbol znad wrót. Wiem, że taki sam widnieje na twoim amulecie. Widziałem go też podczas wizji.");
            }
        }
        else if (chooseNumber == 2) // Был сделан второй выбор
        {
            if (choosenDialogElements[1] == 1) // Выбрана первая реплика
            {
                if (choosenDialogElements[0] == 1)
                    AfterChoise(6);
                else if (choosenDialogElements[0] == 2)
                    AfterChoise(5);
            }
            else if (choosenDialogElements[1] == 2) // Выбрана вторая реплика
            {
                AfterChoise(7);
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
                AfterChoise(32);
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
                    AfterChoise(32);
            }
            else if (choosenDialogElements[1] == 2) // Выбрана вторая реплика
            {
                AfterChoise(42);
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
}
