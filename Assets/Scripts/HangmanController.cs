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

    private GameObject[] parts;
    private int tries;

    private void Awake()
    {
        if (hangman == null)
            hangman = this;
    }
    public bool isDead
    {
        get { return tries < 0; }
    }

    void Start()
    {
        parts = new GameObject[] { rightLeg, leftLeg, leftArm, rightArm, torso, head };
        reset();
    }

    public void punish()
    {        
        if (tries >= 0)        
            parts[tries--].SetActive(true);           
    }

    public void reset()
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
