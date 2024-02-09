using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Ink.Runtime;
using UnityEngine.EventSystems;

public class DialogueManager : MonoBehaviour
{
    [Header("Choices UI")]
    [SerializeField] private GameObject[] choices;
    private TextMeshProUGUI[] choicesText;

    [Header("Dilaogue UI")]
    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private TextMeshProUGUI dialogueText;

    [Header("Finnicky Timing Things")]
    [SerializeField] private float bufferSpeedPostDialogue;

    private Story currentStory;
    //the weird notation here means that this is read only for external files
    public bool dialogueIsPlaying { get; private set; }

    private static DialogueManager instance;

    private void Awake()
    {
        //this will make it get mad if there are more than one instance of the dialogue manager
        if (instance != null)
        {
            Debug.LogWarning("Found more than one Dialogue Manager in the Scene");
        }
        instance = this;
    }

    public static DialogueManager GetInstance()
    {
        return instance;
    }
    // Start is called before the first frame update
    void Start()
    {
        dialogueIsPlaying = false;
        dialoguePanel.SetActive(false);


        // this part is the code that sets up the ability for us to have dialogue with multiple choices
        choicesText = new TextMeshProUGUI[choices.Length];
        int index = 0;
        foreach(GameObject choice in choices)
        {
            //uses the index to assign the choices correctly based on the Ink story that is passed in
            choicesText[index] = choice.GetComponentInChildren<TextMeshProUGUI>();
            index++;
        }
    }

    private void Update()
    {
        if (!dialogueIsPlaying)
        {
            //no need to do anything else
            return;
        }
        //CHANGE LATER: use an input manager to standardize input throughout project
        //currently checks for return or left click
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetMouseButtonDown(0))
        {
            ContinueStory();
        }
    }

    public void EnterDilaogueMode(TextAsset inkJSON)
    {
        currentStory = new Story(inkJSON.text);
        dialogueIsPlaying = true;
        dialoguePanel.SetActive(true);

        ContinueStory();
    }
    //Made an IE numerator so we could utilize wait for seconds
    private IEnumerator ExitDialogueMode()
    {
        //Stops potential conflict from controls that may both move dialogue along
        //      AND do something different when dialogue is not playing.
        //      Otherwise, we may get some buttons having mutliple effects.
        yield return new WaitForSeconds(bufferSpeedPostDialogue);
        dialogueIsPlaying = false;
        dialoguePanel.SetActive(false);
        dialogueText.text = "";
    }
    private void ContinueStory()
    {
        //built in method that determines if the story has anything left to show
        if (currentStory.canContinue)
        {
            dialogueText.text = currentStory.Continue();

            // will present the choices if needed
            DisplayChoices();
        }
        else
        {
            //I wrote ExitDialogueMode as a coroutine so that we could use the wait for seconds method
            // SO now it has to be called like this every time
            StartCoroutine(ExitDialogueMode());
        }
    }
    private void DisplayChoices()
    {
        List<Choice> currentChoices = currentStory.currentChoices;

        //pre check to make sure the UI has the infrastructure to display the number of choice that this part of the story needs
        if(currentChoices.Count > choices.Length)
        {
            Debug.LogError("Too Many Choices compared to what the current UI can support. Given: " + currentChoices.Count + " choices.");
        }

        int index = 0;
        //enable the display for all choice object in the array
        foreach(Choice choice in currentChoices)
        {
            choices[index].gameObject.SetActive(true);
            choicesText[index].text = choice.text;
            index++;
        }
        //ensure that any choices that the UI supports, but the story doesn't, stay hidden.
        for(int i = index; i < choices.Length; i++)
        {
            choices[i].gameObject.SetActive(false);
        }
        StartCoroutine(SelectFirstchoice());
    }
    private IEnumerator SelectFirstchoice()
    {
        //event system requires being cleared first
        EventSystem.current.SetSelectedGameObject(null);
        yield return new WaitForEndOfFrame();
        EventSystem.current.SetSelectedGameObject(choices[0].gameObject);
    }
    public void MakeChoice(int choiceIndex)
    {
        currentStory.ChooseChoiceIndex(choiceIndex);
    }
}
