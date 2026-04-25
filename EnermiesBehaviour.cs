using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using System.Collections.Generic; // for extension adding Array

public class EnermiesBehaviour : MonoBehaviour
{
    public Transform player;
    public float Health = 100;
    public Transform[] waypoint;
    private NavMeshAgent agent;
    
    
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        Shuffle(waypoint);
    }

    public void Detect()
    {
        //call function from DetectRunForPlayer.cs
        StartCoroutine(FollowWaypoints());
    }

    void Update() {
        if (Health <= 0)
        {
            player.GetComponent<DetectRunForPlayer>();
            Destroy(gameObject);
        }

    }
    IEnumerator FollowWaypoints()
    {
        while (true) 
        {
            foreach (var v in waypoint)
            {
                agent.SetDestination(v.position);

                while (agent.pathPending || agent.remainingDistance > 0.1f)
                {
                    yield return null;
                }

                yield return new WaitForSeconds(0.1f);
            }

            // Run Again !!
        }
    }

    void Shuffle<T>(IList<T> list)
    {
        for (int i = 0; i < list.Count; i++) // Array ก็มี .Count (ผ่าน Interface) หรือใช้ .Length
        {
            T temp = list[i];
            int randomIndex = Random.Range(i, list.Count);
            list[i] = list[randomIndex];
            list[randomIndex] = temp;
        }
    }


}

