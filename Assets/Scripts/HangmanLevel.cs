using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "HangmanLevel", menuName = "Hangman/AddLevel", order = 1)]
public class HangmanLevel : ScriptableObject
{
    public string levelName;
    public string hint;
    public string[] secretWords;
}
