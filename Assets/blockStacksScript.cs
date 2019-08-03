using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using KModkit;

public class blockStacksScript : MonoBehaviour {

    public KMBombInfo Bomb;
    public KMNeedyModule needyModule;
    public KMAudio Audio;

    public TextMesh topScreenText;
    public KMSelectable[] stacks;
    public KMHighlightable[] highlights;
    public TextMesh[] stackNumbers;

    //Logging
    static int moduleIdCounter = 1;
    int moduleId;
    private bool moduleSolved = true;

    int mainNumber;
    int randomNumber1;
    int randomNumber2;
    int randomNumber3;
    public List<int> stackHeights = new List<int> { };
    public List<int> answerStacks = new List<int> { };
    string stackCheck = "";

    void Awake () {
        moduleId = moduleIdCounter++;
        needyModule = GetComponent<KMNeedyModule>();
        needyModule.OnNeedyActivation += OnNeedyActivation;
        needyModule.OnNeedyDeactivation += OnNeedyDeactivation;
        needyModule.OnTimerExpired += OnTimerExpired;

        foreach (KMSelectable stack in stacks) {
            KMSelectable pressedStack = stack;
            stack.OnInteract += delegate () { StackPress(pressedStack); return false; };
        }
    }

    // Use this for initialization
    void Start () {
        moduleSolved = true;
	}

    void OnNeedyActivation()
    {
        moduleSolved = false;
        mainNumber = UnityEngine.Random.Range(4, 8);
        topScreenText.text = mainNumber.ToString();
        randomNumber1 = UnityEngine.Random.Range(1, mainNumber);
        stackHeights.Add(randomNumber1);
        stackHeights.Add(mainNumber - randomNumber1);
        randomNumber2 = UnityEngine.Random.Range(1, mainNumber);
        stackHeights.Add(randomNumber2);
        stackHeights.Add(mainNumber - randomNumber2);
        randomNumber3 = UnityEngine.Random.Range(1, mainNumber);
        stackHeights.Add(randomNumber3);
        stackHeights.Add(mainNumber - randomNumber3);
        stackHeights.Shuffle();
        Debug.Log(stackHeights[0] + ", " + stackHeights[1] + ", " + stackHeights[2] + ", " + stackHeights[3] + ", " + stackHeights[4] + ", " + stackHeights[5]);

        for (int i = 0; i < 6; i++) {
            stacks[i].transform.localScale = new Vector3(0.01f, 0.01f, (0.01f * stackHeights[i]));
            highlights[i].transform.localScale = new Vector3(0.015f, 0.001f, (0.01f * stackHeights[i] + 0.005f));
            stackNumbers[i].text = stackHeights[i].ToString();
        }

        Debug.LogFormat("[Block Stacks #{0}] The chosen number is {1}.", moduleId, mainNumber);
        Debug.LogFormat("[Block Stacks #{0}] The heights of each stack are: {1}, {2}, {3}, {4}, {5}, {6}", moduleId, stackHeights[0], stackHeights[1], stackHeights[2], stackHeights[3], stackHeights[4], stackHeights[5]);
    }

    void OnNeedyDeactivation ()
    {
        moduleSolved = true;
        topScreenText.text = "_";
        stackHeights.Clear();
        answerStacks.Clear();
        stackCheck = "";
    }

    void OnTimerExpired()
    {
        moduleSolved = true;
        topScreenText.text = "_";
        stackHeights.Clear();
        answerStacks.Clear();
        stackCheck = "";
    }
	
	void StackPress (KMSelectable stack){
        stack.AddInteractionPunch();
        GetComponent<KMAudio>().PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, transform);
        if (moduleSolved == false)
        {
            if (stack == stacks[0])
            {
                answerStacks.Add(stackHeights[0]);
                stackCheck += "A";
            }
            else if (stack == stacks[1])
            {
                answerStacks.Add(stackHeights[1]);
                stackCheck += "B";
            }
            else if (stack == stacks[2])
            {
                answerStacks.Add(stackHeights[2]);
                stackCheck += "C";
            }
            else if (stack == stacks[3])
            {
                answerStacks.Add(stackHeights[3]);
                stackCheck += "D";
            }
            else if (stack == stacks[4])
            {
                answerStacks.Add(stackHeights[4]);
                stackCheck += "E";
            }
            else if (stack == stacks[5])
            {
                answerStacks.Add(stackHeights[5]);
                stackCheck += "F";
            }
            else
            {
                Debug.LogFormat("[Block Stacks #{0}] You have just encountered a bug, please tell Blananas2 immediately. (CHECK STACK PRESS)", moduleId);
            }
        }

        if (stackCheck.Length == 6)
        {
            if ((stackCheck.Contains("A") && stackCheck.Contains("B") && stackCheck.Contains("C") && stackCheck.Contains("D") && stackCheck.Contains("E") && stackCheck.Contains("F")) && (answerStacks[0] + answerStacks[1] == mainNumber && answerStacks[2] + answerStacks[3] == mainNumber && answerStacks[4] + answerStacks[5] == mainNumber))
            {
                Debug.LogFormat("[Block Stacks #{0}] You selected stacks correctly, instance solved. ({1})", moduleId, stackCheck);
            }
            else
            {
                Debug.LogFormat("[Block Stacks #{0}] You selected stacks incorrectly, instance striked. ({1})", moduleId, stackCheck);
                needyModule.HandleStrike();
            }
            needyModule.HandlePass();
            topScreenText.text = "_";
            stackHeights.Clear();
            answerStacks.Clear();
            stackCheck = "";
        }

	}

}
