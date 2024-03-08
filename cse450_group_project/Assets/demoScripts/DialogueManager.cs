using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Ink.Runtime;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class DialogueManager : MonoBehaviour
{
    [Header("Scene to Go To")]
    [SerializeField] string scene;
    [Header("Choices UI")]
    [SerializeField] private GameObject[] choices;
    private TextMeshProUGUI[] choicesText;

    [Header("Dilaogue UI")]
    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private TextMeshProUGUI dialogueText;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private Image pictureBox;

    [Header("Finnicky Timing Things")]
    [SerializeField] private float bufferSpeedPostDialogue;

    [Header("Character Art: Match name element indices to cooresponding pictures ")]
    [SerializeField] string[] char_names;
    [SerializeField] Sprite[] char_pictures;
 
    private Story currentStory;
    private Dictionary<string, Sprite> characterArtMap = new Dictionary<string, Sprite>();
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

        int i = 0;
        foreach (string name in char_names)
        {
            characterArtMap.Add(name, char_pictures[i]);
            i++;
        }
        
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
        // HAD THIS TOO: || Input.GetMouseButtonDown(0)
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetMouseButtonDown(0))
        {
            ContinueStory();
        }
    }

    public void EnterDilaogueMode(TextAsset inkJSON)
    {
        Debug.Log("dialogue mode entered from dialogue manager");
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
        nameText.text = "";
        SceneManager.LoadScene(scene);
    }
    private void ContinueStory()
    {
        //built in method that determines if the story has anything left to show
        if (currentStory.canContinue)
        {
            string nextDialogueToDisplay = currentStory.Continue();
            string name = "";
            int firstColonPosition = -1;
            //extract the first word from the dialogue. This will always be the character name.
            for(int i = 0; i < nextDialogueToDisplay.Length; i++)
            {
                if (nextDialogueToDisplay[i] == ':')
                {
                    //adding two so that the colon and spaces aren't included on the second line as dialogue runs
                    firstColonPosition = i+2;
                    if(i == 1)
                    {
                        Debug.LogError("Line of Dialogue with space in front given");
                    }
                    break;
                }
                else
                {
                    name += nextDialogueToDisplay[i];
                }
            }
            if(firstColonPosition == -1)
            {
                Debug.LogError("No colon in text");
            }
            nameText.text = name + ":";
            //set the speaking text
            dialogueText.text = nextDialogueToDisplay.Substring(firstColonPosition);
            Sprite result;
            if(characterArtMap.TryGetValue(name, out result))
            {
                pictureBox.sprite = result;
            }
            else
            {
                Debug.LogWarning("no picture found under the name: " + name);
            }
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
            //added this space in front of the choice text so that it looks nicer
            choicesText[index].text = " " + choice.text;
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
        Debug.Log("choice made was: " + choiceIndex);
        currentStory.ChooseChoiceIndex(choiceIndex);
    }
}
