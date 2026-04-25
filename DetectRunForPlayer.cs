using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectRunForPlayer : MonoBehaviour
{
    public Transform[] NPC_Choppers;
    public float distance = 15;
    private bool stillRun = false;
    // (For player script) when closing to NPC_Choppers
    void Start()
    {
        StartCoroutine(CheckDistanceNPC());
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
