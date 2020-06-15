using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using TMPro;

public class TextManager : MonoBehaviour
{
    MoveII Move;
    public GameObject textObject;
    GameObject objectText;
    TextMeshPro myTextMesh;
    Multiplayer multiplayer;
    bool uppercase, active;
    string myName = "EnterName";
    public Text PlayerName;
    public Color32 inWrighting;
    public Text numberOfPlayer;

    // Start is called before the first frame update
    void Start()
    {
        myTextMesh = textObject.GetComponent<TextMeshPro>();
        multiplayer = FindObjectOfType<Multiplayer>();
        Move = FindObjectOfType<MoveII>();
        myTextMesh.text = myName;
    }
    private void Update()
    {
        numberOfPlayer.text = NetworkPlayerManager.infoPl.Count.ToString();
        if (active)
        {
            if (Input.GetKey(KeyCode.LeftShift) | Input.GetKey(KeyCode.RightShift))
                uppercase = true;
            else
                uppercase = false;

            if (myTextMesh.text == "EnterName")
                myTextMesh.color = Color.red;
            else
                myTextMesh.color = Color.magenta;
            if (Input.anyKeyDown)
            {
                if (myTextMesh.text == "EnterName")
                    myTextMesh.text = "";
                foreach (char c in Input.inputString)
                {
                    if (c == '\b') // has backspace/delete been pressed?
                    {
                        if (myTextMesh.text.Length != myName.Length + 2)
                        {
                            if(myTextMesh.text.Length>0)
                            myTextMesh.text = myTextMesh.text.Substring(0, myTextMesh.text.Length - 1);
                        }
                    }
                    else if ((c == '\n') || (c == '\r')) // enter/return
                    {
                        if (myName != "EnterName")
                        {
                            myTextMesh.color =   Color.magenta;
                            if (objectText == null)
                            {

                                objectText = Instantiate(textObject, transform.position - (transform.up * -1), transform.rotation);
                            }
                            else
                            {
                                objectText.transform.SetPositionAndRotation(transform.position - (transform.up * -1), this.transform.rotation);
                                objectText.GetComponent<TextMeshPro>().text = myTextMesh.text;
                            }
                            multiplayer.w.SendString(multiplayer.FormatMessege(objectText.transform, myTextMesh.text, "TextMessage"));
                        }
                        else
                        {
                            if (myTextMesh.text != "")
                            {
                                myName = myTextMesh.text;
                                myTextMesh.text = myName + ":" + "\n";
                            }else
                                myTextMesh.text = myName;
                        }
                    }
                    else
                    {
                        if (uppercase)
                            myTextMesh.text += c.ToString().ToUpper();
                        else
                            myTextMesh.text += c.ToString();
                    }
                }
            }
        }
        if (Input.GetKeyDown(KeyCode.Return))
        {
            active = !active;
            Move.enabled = !active;
            if (myName != "EnterName" & myName!="") PlayerName.text = myName;
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            active = false;
            Move.enabled = true;
            myTextMesh.text = myName + ":" + "\n";
        }

        if (Input.GetKeyDown(KeyCode.Tab))
        {
            myName = "EnterName";
            myTextMesh.text = myName;
        }

        myTextMesh.enabled = active;
    }
    
}

