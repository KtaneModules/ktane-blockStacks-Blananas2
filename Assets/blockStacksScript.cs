using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

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
        mainNumber = Random.Range(4, 8);
        topScreenText.text = mainNumber.ToString();
        randomNumber1 = Random.Range(1, mainNumber);
        stackHeights.Add(randomNumber1);
        stackHeights.Add(mainNumber - randomNumber1);
        randomNumber2 = Random.Range(1, mainNumber);
        stackHeights.Add(randomNumber2);
        stackHeights.Add(mainNumber - randomNumber2);
        randomNumber3 = Random.Range(1, mainNumber);
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
        stackHeights.Clear();
        answerStacks.Clear();
        stackCheck = "";
    }

    void OnTimerExpired()
    {
        needyModule.HandleStrike();
        moduleSolved = true;
        topScreenText.text = "Time";
        stackHeights.Clear();
        answerStacks.Clear();
        stackCheck = "";
    }

    void StackPress (KMSelectable stack){
        stack.AddInteractionPunch();
        Audio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, stack.transform);
        if (moduleSolved == false)
        {
            if (stack == stacks[0])
            {
                answerStacks.Add(stackHeights[0]);
                stackCheck += "1";
            }
            else if (stack == stacks[1])
            {
                answerStacks.Add(stackHeights[1]);
                stackCheck += "2";
            }
            else if (stack == stacks[2])
            {
                answerStacks.Add(stackHeights[2]);
                stackCheck += "3";
            }
            else if (stack == stacks[3])
            {
                answerStacks.Add(stackHeights[3]);
                stackCheck += "4";
            }
            else if (stack == stacks[4])
            {
                answerStacks.Add(stackHeights[4]);
                stackCheck += "5";
            }
            else if (stack == stacks[5])
            {
                answerStacks.Add(stackHeights[5]);
                stackCheck += "6";
            }
            else
            {
                Debug.LogFormat("[Block Stacks #{0}] You have just encountered a bug, please tell Blananas2 immediately. (CHECK STACK PRESS)", moduleId);
            }
        }

        if (stackCheck.Length == 6)
        {
            if ((stackCheck.Contains("1") && stackCheck.Contains("2") && stackCheck.Contains("3") && stackCheck.Contains("4") && stackCheck.Contains("5") && stackCheck.Contains("6")) && (answerStacks[0] + answerStacks[1] == mainNumber && answerStacks[2] + answerStacks[3] == mainNumber && answerStacks[4] + answerStacks[5] == mainNumber))
            {
                Debug.LogFormat("[Block Stacks #{0}] You selected stacks correctly, instance solved. (Order: {1})", moduleId, stackCheck);
                topScreenText.text = ":)";
            }
            else
            {
                Debug.LogFormat("[Block Stacks #{0}] You selected stacks incorrectly, instance striked. (Order: {1})", moduleId, stackCheck);
                needyModule.HandleStrike();
                topScreenText.text = "X";
            }
            needyModule.HandlePass();
            stackHeights.Clear();
            answerStacks.Clear();
            stackCheck = "";
        }

	}

    //twitch plays
    private bool isInputVal(string s)
    {
        string[] valids = { "1", "2", "3", "4", "5", "6" };
        string[] numbs = s.Split(';',',');
        for(int i = 0; i < numbs.Length; i++)
        {
            if (!valids.Contains(numbs[i]))
            {
                return false;
            }
        }
        return true;
    }

    #pragma warning disable 414
    private readonly string TwitchHelpMessage = @"!{0} 1;4;5;2 [Selects the block pairs '1 and 4' and '5 and 2' with 1-6 being the block's positions on the module from left to right]";
    #pragma warning restore 414
    IEnumerator ProcessTwitchCommand(string command)
    {
        string[] parameters = command.Split(' ');
        string fixedparam = "";
        for (int i = 0; i < parameters.Length; i++)
        {
            fixedparam += parameters[i];
        }
        if (isInputVal(fixedparam))
        {
            yield return null;
            if (moduleSolved)
            {
                yield return "sendtochaterror You can't interact with the module right now.";
                yield break;
            }
            string[] numbs = fixedparam.Split(';', ',');
            for (int i = 0; i < numbs.Length; i++)
            {
                if (numbs[i].Equals("1"))
                {
                    stacks[0].OnInteract();
                }
                else if (numbs[i].Equals("2"))
                {
                    stacks[1].OnInteract();
                }
                else if (numbs[i].Equals("3"))
                {
                    stacks[2].OnInteract();
                }
                else if (numbs[i].Equals("4"))
                {
                    stacks[3].OnInteract();
                }
                else if (numbs[i].Equals("5"))
                {
                    stacks[4].OnInteract();
                }
                else if (numbs[i].Equals("6"))
                {
                    stacks[5].OnInteract();
                }
                yield return new WaitForSeconds(0.1f);
            }
            yield break;
        }
    }

    void TwitchHandleForcedSolve()
    {
        //The code is done in a coroutine instead of here so that if the solvebomb command was executed this will just input the number right when it activates and it wont wait for its turn in the queue
        StartCoroutine(DealWithNeedy());
    }

    private IEnumerator DealWithNeedy()
    {
        if (!moduleSolved)
        {
            List<char> dupes = new List<char>();
            for (int i = 0; i < stackCheck.Length; i++)
            {
                if (dupes.Contains(stackCheck[i]) || ((i % 2 == 1) && (stackHeights[int.Parse(stackCheck[i - 1].ToString()) - 1] + stackHeights[int.Parse(stackCheck[i].ToString()) - 1] != mainNumber)))
                {
                    OnNeedyDeactivation();
                    needyModule.HandlePass();
                    topScreenText.text = ":)";
                    break;
                }
                else
                    dupes.Add(stackCheck[i]);
            }
        }
        while (true)
        {
            while (moduleSolved) { yield return null; }
            List<int> used = new List<int>();
            for (int i = 0; i < stackCheck.Length; i++)
                used.Add(int.Parse(stackCheck[i].ToString()) - 1);
            int start = stackCheck.Length / 2;
            for (int i = start; i < 3; i++)
            {
                if (stackCheck.Length % 2 == 0)
                {
                    int rando = Random.Range(0, 6);
                    while (used.Contains(rando)) rando = Random.Range(0, 6);
                    used.Add(rando);
                    stacks[rando].OnInteract();
                    yield return new WaitForSeconds(0.1f);
                }
                int rando2 = Random.Range(0, 6);
                while (used.Contains(rando2) || (stackHeights[int.Parse(stackCheck[stackCheck.Length - 1].ToString()) - 1] + stackHeights[rando2]) != mainNumber) rando2 = Random.Range(0, 6);
                used.Add(rando2);
                stacks[rando2].OnInteract();
                yield return new WaitForSeconds(0.1f);
            }
        }
    }
}
