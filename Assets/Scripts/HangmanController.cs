using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class HangmanController : MonoBehaviour
{
    public static HangmanController hangman;

    [SerializeField]
    private GameObject head;
    [SerializeField]
    private GameObject torso;
    [SerializeField]
    private GameObject rightArm;
    [SerializeField]
    private GameObject leftArm;
    [SerializeField]
    private GameObject leftLeg;
    [SerializeField]
    private GameObject rightLeg;

    [HideInInspector]
    public GameObject[] parts;
    private int tries;

    private void Awake()
    {
        if (hangman == null)
            hangman = this;
    }
    public bool IsDead
    {
        get { return tries < 0; }
    }

    void Start()
    {
        parts = new GameObject[] { rightLeg, leftLeg, leftArm, rightArm, torso, head };
        ResetHangman();
    }

    public void Punish()
    {        
        if (tries >= 0)        
            parts[tries--].SetActive(true);           
    }

    public void ResetHangman()
    {
        if (parts == null)
            return;

        tries = parts.Length - 1;
        foreach (GameObject g in parts)
        {
            g.SetActive(false);
        }
    }
}
