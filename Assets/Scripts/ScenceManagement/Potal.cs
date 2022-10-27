using RPG.Control;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.UIElements.GraphView;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

namespace RPG.SceneManagement
{
    public class Potal : MonoBehaviour
    {
        enum DestinationIdentifier
        {
            A, B, C, D, E
        }
        [SerializeField] int sceneToLoad = -1;
        [SerializeField] Transform spawnPoint;
        [SerializeField] DestinationIdentifier destination;
        [SerializeField] float fadeOutTime = 1f;
        [SerializeField] float fadeInTime = 2f;
        [SerializeField] float fadeWaitTime = 0f;


        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag("Player")){
                StartCoroutine(Transition());
;
            }
        }
        private IEnumerator Transition()
        {
            if (sceneToLoad < 0)
            {
                Debug.LogError("Scene to load not set.");
                yield break;
            }

            DontDestroyOnLoad(gameObject);

            Fader fader = FindObjectOfType<Fader>();
            //Save Current Level
            SavingWrapper wrapper = FindObjectOfType<SavingWrapper>();

            //171 Remove Control
            PlayerController playerController = GameObject.FindWithTag("Player").GetComponent<PlayerController>();
            playerController.enabled = false;
            yield return fader.FadeOut(fadeOutTime);

            
            wrapper.Save();

            //remove control
            yield return SceneManager.LoadSceneAsync(sceneToLoad);
            PlayerController newPlayerController = GameObject.FindWithTag("Player").GetComponent<PlayerController>();
            newPlayerController.enabled = false;


            //GameObject player = GameObject.FindWithTag("Player");
            //player.GetComponent<PlayerController>().enabled = false;

            //Load Current Level
            wrapper.Load();

            Potal otherPortal = GetOtherPortal();
            UpdatePlayer(otherPortal);

            //Save Again
            wrapper.Save();
            //player.GetComponent<PlayerController>().enabled = true;
          

            yield return new WaitForSeconds(fadeWaitTime);
            fader.FadeIn(fadeInTime);

            newPlayerController.enabled = true;
            // Restore control
            Destroy(gameObject);
        }


        private void UpdatePlayer(Potal otherPortal)
        {
            GameObject player = GameObject.FindWithTag("Player");
            //Phan Saving
            player.GetComponent<NavMeshAgent>().enabled = false;


            //-------------Saving---------------//
            //Navmesh Agent di chuyen den vi tri do
            player.GetComponent<NavMeshAgent>().Warp(otherPortal.spawnPoint.position);
            //player.GetComponent<NavMeshAgent>().enabled = false;
            //player.transform.position = otherPortal.spawnPoint.position;
            player.transform.rotation = otherPortal.spawnPoint.rotation;
            player.GetComponent<NavMeshAgent>().enabled = true;
        }

        private Potal GetOtherPortal()
        {
            foreach(Potal portal in FindObjectsOfType<Potal>())
            {
                //neu la portal nay thi skip
                if (portal == this) continue;
                // neu la portal cung ten thi cung skip
                if (portal.destination != destination) continue;

                return portal;
            }
            //Neu khong tim duoc Portal nao thi tra ve null
            return null;
        }
    }

}
