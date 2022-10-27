using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Nhin vao Intro Sequence se co Playable Director
using UnityEngine.Playables;

namespace RPG.Cinematic
{
    
    public class CinematicsTrigger : MonoBehaviour
    {
        bool alreadyTriggered = false;
        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag("Player") && alreadyTriggered == false)
            {
                GetComponent<PlayableDirector>().Play();
                alreadyTriggered = true;
            } 
        }
    }
    //1
}

