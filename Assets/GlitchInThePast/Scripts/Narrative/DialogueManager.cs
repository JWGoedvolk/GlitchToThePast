using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    public TextMeshPro nameText;
    public TextMeshPro dialogueText;
    private Queue<string> sentences; // Restricted List First in first out

    void Start()
    {
        sentences = new Queue<string>();
    }

    public void StartDialogue(Dialogue dialogue)
    {
      //Debug.Log("Staring dialogue with" + dialogue.name);
       nameText.text = dialogue.name;
        sentences.Clear();

        foreach (string sentence in dialogue.sentences)
        {
            sentences.Enqueue(sentence);
        }

        DisplayNextSentence();
    }

    public void DisplayNextSentence()
    {
        if (sentences.Count == 0)
        {
            EndDialogue();
            return;
        }


        string sentence = sentences.Dequeue();
        dialogueText.text = sentence;
        void EndDialogue()
        {
            Debug.Log("End");
        }


       
        {

        }
    }
}

