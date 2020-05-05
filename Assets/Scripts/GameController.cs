using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class GameController : MonoBehaviour
{        
    private string[] level1Passwords = { "fiction", "author", "read", "biography" };    
    private string[] level2Passwords = { "melody", "dance", "listen", "guitar" };    
    private string[] level3Passwords = { "comedy", "horror", "romance", "action" };    

    enum Level { Level1 = 0, Level2 = 1, Level3 = 2 };
    Level currentLevel;

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

    private int currentWordLength;
    private int lettersFound;
    private int indexEnum;
    private string secretWord;
    private bool foundLetter;

    [HideInInspector]
    public bool solved;							
    [HideInInspector]
    public bool failed;
    [HideInInspector]
    public string allButtonLetters;    

    // Start is called before the first frame update
    void Start()
    {
        currentLevel = Level.Level1;
        nextButton.gameObject.SetActive(false);
        resetButton.gameObject.SetActive(false);

        levelText.text = currentLevel.ToString();
        UISolutionText.text = "";
        UISolveText.text = "_";

        SetRandomSecretWord(currentLevel);
        AddListeners();
        Invoke("LateStart", 0.01f);
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
        currentWordLength += CountLettersInWord();	// Count how many letters the word is using        
    }


    private void SetRandomSecretWord(Level currentLevel)
    {
        switch (currentLevel)
        {
            case Level.Level1:
                secretWord = level1Passwords[UnityEngine.Random.Range(0, level1Passwords.Length)];
                hintText.text = "Hint: The current theme is books";
                print(secretWord);
                break;
            case Level.Level2:
                secretWord = level2Passwords[UnityEngine.Random.Range(0, level2Passwords.Length)];
                hintText.text = "Hint: The current theme is music";
                break;
            case Level.Level3:
                secretWord = level3Passwords[UnityEngine.Random.Range(0, level3Passwords.Length)];
                hintText.text = "Hint: The current theme is movies";
                break;
        }        
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
        foundLetter = false;
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
            if (lettersFound >= currentWordLength)
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
        HangmanController.hangman.punish();
        if (!incorrectSFX.isPlaying)
            incorrectSFX.Play();

        if (HangmanController.hangman.isDead)
        {
            if (!loseSFX.isPlaying)
                loseSFX.Play();
            
            UISolutionText.text = secretWord;
            failed = true;
            resetButton.gameObject.SetActive(true);
        }                                                    
        
    }

    public int CountLettersInWord()
    {
        int c = secretWord.Length;        
        UISolveText.text = "";

        for (int i = 0; i < secretWord.Length; i++)
        {            
             UISolveText.text = UISolveText.text + "-";            
        }

        return c;
    }

    public bool CheckLetterButtons(string s)
    {
        for (int i = 0; i < allButtonLetters.Length; i++)
        {
            if (s == allButtonLetters[i].ToString())
            {
                return true;
            }
        }
        return false;
    }

    public void HintButtonPressed()
    {
        hintText.enabled = !hintText.enabled;
        hintButton.gameObject.SetActive(!hintButton.gameObject.activeSelf);
    }

    public void next()
    {
        HangmanController.hangman.reset();
        
        if (currentLevel == Level.Level1)
            currentLevel = Level.Level2;
        else if (currentLevel == Level.Level2)
            currentLevel = Level.Level3;        

        Reset();
    }

    public void Reset()
    {        
        HangmanController.hangman.reset();
        levelText.text = currentLevel.ToString();

        UISolutionText.text = "";
        UISolveText.text = "_";
        secretWord = "";

        solved = false;
        failed = false;
        foundLetter = false;

        SetRandomSecretWord(currentLevel);
        Invoke("LateStart", 0.01f);

        foreach (Button button in letterButtons)
            button.gameObject.SetActive(true);
        nextButton.gameObject.SetActive(false);
        resetButton.gameObject.SetActive(false);        
    }
}
