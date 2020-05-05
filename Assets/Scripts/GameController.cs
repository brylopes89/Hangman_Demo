using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Security;

public class GameController : MonoBehaviour
{    
    [Header("Levels")]
    public HangmanLevel[] levels;
    private int currentLevel = 0;

    [Header("Game Buttons")]
    [SerializeField]
    private Button[] letterButtons;
    [SerializeField]
    private Button hintButton;
    [SerializeField]
    private Button nextButton;
    [SerializeField]
    private Button resetButton;

    [Header("Text Displays")]
    [SerializeField]
    private TextMeshProUGUI hintText;
    [SerializeField]
    private TextMeshProUGUI levelText;
    [SerializeField]
    private TextMeshProUGUI scoreIndicator;
    [SerializeField]
    private Text UISolutionText;
    [SerializeField]
    private Text UISolveText;

    [Header("Sound Effects")]
    [SerializeField]
    private AudioSource correctSFX;
    [SerializeField]
    private AudioSource incorrectSFX;
    [SerializeField]
    private AudioSource winSFX;
    [SerializeField]
    private AudioSource loseSFX;

    private int lettersFound;    
    private string secretWord;

    [HideInInspector]
    public bool solved;							
    [HideInInspector]
    public bool failed;
    [HideInInspector]
    public string allButtonLetters;    

    // Start is called before the first frame update
    void Start()
    {
        currentLevel = 0;
        AddListeners();
        
        Retry();
    }

    private void AddListeners()
    {
        for (int i = 0; i < letterButtons.Length; i++)
        {
            int index = i;
            letterButtons[index].onClick.AddListener(() => OnLetterPicked(index, letterButtons[index].GetComponent<Text>().text));
            allButtonLetters += letterButtons[index].GetComponent<Text>().text;
        }
    }

    public void LateStart()
    {
        	// Count how many letters the word is using        
    }


    private void SetRandomSecretWord(HangmanLevel level)
    {        
        secretWord = level.secretWords[UnityEngine.Random.Range(0, level.secretWords.Length)];
        hintText.text = "Hint: " + level.hint;      
    }

    public void OnLetterPicked(int index, string letterText)
    {
        Debug.Log("You have clicked the letter " + letterText, letterButtons[index]);
        if (!solved && !failed)
        {
            string t = letterText.ToLower();
            if (!CheckLetter(t))
                letterButtons[index].gameObject.SetActive(!letterButtons[index].gameObject.activeSelf);
        }       
    }   

    public bool CheckLetter(string l)
    {                   
        bool foundLetter = false;
        string n = UISolveText.text;                        
        for (int i = 0; i < secretWord.Length; i++)
        {       
            if (secretWord[i].ToString() == l)
            {                       
                n = n.Remove(i, 1);                            
                n = n.Insert(i, l);                            
                foundLetter = true;                             					
                lettersFound++;               
            }
        }
        if (foundLetter)
        {
            if (!correctSFX.isPlaying)
                correctSFX.Play();
            UISolveText.text = n;                               
            if (lettersFound >= secretWord.Length)
            {               
                SolvedWord();                                   
            }
        }
        else
        {
            WrongLetter();            
        }
        return foundLetter;										
    }

    public void SolvedWord()
    {                                       
        solved = true;
        if (correctSFX.isPlaying)
        {
            correctSFX.Stop();
            winSFX.Play();
        }

        nextButton.gameObject.SetActive(true);
        Debug.Log("You win!");        
    }

    public void WrongLetter()
    {        
        HangmanController.hangman.Punish();
        if (!incorrectSFX.isPlaying)
            incorrectSFX.Play();

        if (HangmanController.hangman.IsDead)
        {
            if (!loseSFX.isPlaying)
                loseSFX.Play();
            
            UISolutionText.text = secretWord;
            failed = true;
            resetButton.gameObject.SetActive(true);
        }                                                    
        
    }

    public void ProcessLettersInWord()
    {
        UISolveText.text = "";

        for (int i = 0; i < secretWord.Length; i++)
        {            
             UISolveText.text = UISolveText.text + "-";            
        }
    }

    public void ShowHint()
    {
        hintText.enabled = true;
        hintButton.gameObject.SetActive(false);
    }

    public void Next()
    {
        currentLevel++;

        if (currentLevel == levels.Length-1)
        {
            nextButton.GetComponentInChildren<Text>().text = "Play Again";
        } else if (currentLevel > levels.Length-1)
        {
            currentLevel = 0;
            nextButton.GetComponentInChildren<Text>().text = "Next Level";
        }

        Retry();
    }

    public void Retry()
    {        
        HangmanController.hangman.ResetHangman();
        levelText.text = "Level " + (currentLevel+1) + "/" + levels.Length + ":\n" + levels[currentLevel].levelName;        

        UISolutionText.text = "";
        UISolveText.text = "_";
        secretWord = "";

        solved = false;
        failed = false;        

        lettersFound = 0;

        SetRandomSecretWord(levels[currentLevel]);
        ProcessLettersInWord();

        foreach (Button button in letterButtons)
            button.gameObject.SetActive(true);

        hintButton.gameObject.SetActive(true);
        hintText.enabled = false;
        nextButton.gameObject.SetActive(false);
        resetButton.gameObject.SetActive(false);        
    }
}
