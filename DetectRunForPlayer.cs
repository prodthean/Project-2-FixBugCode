using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DetectRunForPlayer : MonoBehaviour
{
    public Transform[] NPC_Choppers;
    public float distance = 15;
    public float HaveCaught = 0;
    private bool stillRun = false;
    public TextMeshProUGUI TextQuest;
    // (For player script) when closing to NPC_Choppers
    void Start()
    {
        StartCoroutine(CheckDistanceNPC());
    }

    void CaughtCount()
    {
        HaveCaught += 1;
        TextQuest.text = "Have Caught" + HaveCaught + "/5";
    }
    IEnumerator CheckDistanceNPC()
    {
        while (true)
        {
            foreach(var v in NPC_Choppers)
            {
                if(Vector3.Distance(v.position,gameObject.transform.position) < distance && stillRun == false)
                {
                    stillRun = true;
                    EnermiesBehaviour script = v.GetComponent<EnermiesBehaviour>();
                    script.Detect();
                    break;
                }
            }
            yield return new WaitForSeconds(2.0f);
        }
    }
}
