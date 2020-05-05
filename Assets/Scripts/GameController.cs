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

    [Header("Animator")]
    [SerializeField]
    private Animator anim;

    private int lettersFound;    
    private string secretWord;
    private bool isWon;

    [HideInInspector]
    public bool solved;							
    [HideInInspector]
    public bool failed;       

    // Start is called before the first frame update
    void Start()
    {
        currentLevel = 0;
        AddListeners();
        
        Retry();
    }

    private void Update()
    {
        foreach (KeyCode vKey in Enum.GetValues(typeof(KeyCode)))
        {
            if (Input.GetKeyDown(vKey)) 
            {                
                foreach(Button button in letterButtons)
                {
                    if (button.GetComponent<Text>().text == vKey.ToString())
                        button.onClick.Invoke();
                }                  
            }
        }
    }

    private void AddListeners()
    {
        for (int i = 0; i < letterButtons.Length; i++)
        {
            int index = i;           
            letterButtons[index].onClick.AddListener(() => OnLetterPicked(index, letterButtons[index].GetComponent<Text>().text));            
        }
    }

    private void SetRandomSecretWord(HangmanLevel level)
    {        
        secretWord = level.secretWords[UnityEngine.Random.Range(0, level.secretWords.Length)];
        hintText.text = "Hint: " + level.hint;      
    }

    public void OnLetterPicked(int index, string letterText)
    {       
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

        foreach(GameObject part in HangmanController.hangman.parts)
        {
            part.SetActive(true);
        }
        anim.SetBool("isVictory", true);
        if(isWon)
            UISolutionText.text = "You Win! Play Again?";  
        else
            UISolutionText.text = "You got it!";

        nextButton.gameObject.SetActive(true);              
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
            isWon = true;
        } else if (currentLevel > levels.Length-1)
        {
            currentLevel = 0;
            isWon = false;
            nextButton.GetComponentInChildren<Text>().text = "Next Level";
        }
        
        Retry();
    }

    public void Retry()
    {        
        HangmanController.hangman.ResetHangman();
        anim.SetBool("isVictory", false);
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
