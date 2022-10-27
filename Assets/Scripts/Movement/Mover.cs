using RPG.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using RPG.Saving;
using RPG.Attributes;

namespace RPG.Movement
{
    public class Mover : MonoBehaviour, IAction, ISaveable
    {
        [SerializeField] Transform target;
        [SerializeField] float maxSpeed = 6f;

        NavMeshAgent navMeshAgent;
        Health health;

        private void Awake()
        {
            navMeshAgent = GetComponent<NavMeshAgent>();
            health = GetComponent<Health>();
        }
        void Update()
        {
            navMeshAgent.enabled = !health.IsDead();
           
            UpdateAnimator();
            //Debug.DrawRay(lastRay.origin, lastRay.direction * 100);

        }
        public void StartMoveAction(Vector3 destination, float speedFraction)
        {
            GetComponent<ActionScheduler>().StartAction(this);
            MoveTo(destination,speedFraction);
        }

        public void MoveTo(Vector3 destination, float speedFraction)
        {
            navMeshAgent.destination = destination;
            navMeshAgent.speed = maxSpeed * Mathf.Clamp01(speedFraction);
            navMeshAgent.isStopped = false;
        }

        private void UpdateAnimator()
        {
            //Get the global velocity from Nav Mesh Agent
            Vector3 velocity = GetComponent<NavMeshAgent>().velocity;
            //Convert this into a local value relative to the character
            Vector3 localVelocity = transform.InverseTransformDirection(velocity);
            float speed = localVelocity.z;
            //Set the Animator's blend value to be equal to our desired forward speed (on the Z axis )
            GetComponent<Animator>().SetFloat("forwardSpeed", speed);

        }
        public void Cancel()
        {
            navMeshAgent.isStopped = true;
        }

        //Save 
      
        public object CaptureState()
        {
            //------------Capturing multiple parameters part (Using Dictionary)----------------//
            Dictionary<string, object> data = new Dictionary<string, object>();
            data["position"] = new SerializableVector3(transform.position);
            data["rotation"] = new SerializableVector3(transform.eulerAngles);
            return data;
            //--------------------------------------------------------------//

            //Not return multiple parameters part
            //return new SerializableVector3(transform.position);
            
        }
        //Call after Awake but before Start
        public void RestoreState(object state)
        {
            //------------Capturing multiple parameters part (Using Dictionary)----------------//
            Dictionary<string, object> data = (Dictionary<string, object>)state;
            
            //--------------------------------------------------------------//


            //Not return multiple parameters part
            //SerializableVector3 position = (SerializableVector3)state;

            //khi upate transform position thi phai disable NavMeshAgent;
            navMeshAgent.enabled = false;

            //------------Capturing multiple parameters part (Using Dictionary)----------------//
            transform.position = ((SerializableVector3)data["position"]).ToVector();
            transform.eulerAngles = ((SerializableVector3)data["rotation"]).ToVector();
            //--------------------------------------------------------------//

            //Not return multiple parameters part
            //transform.position = position.ToVector();
            navMeshAgent.enabled = true;
        }

        //------------Capturing multiple parameters part (Using truct)----------------//

        //[System.Serializable]
        //struct MoverSaveData
        //{
        //    public SerializableVector3 position;
        //    public SerializableVector3 rotation;
        //}
        //public object CaptureState()
        //{
        //    MoverSaveData data = new MoverSaveData();
        //    data.position = new SerializableVector3(transform.position);
        //    data.rotation = new SerializableVector3(transform.eulerAngles);
        //    return data;
        //}
        //public void RestoreState(object state)
        //{
        //    MoverSaveData data = (MoverSaveData)state;
        //    GetComponent<NavMeshAgent>().enabled = false;
        //    transform.position = data.position.ToVector();
        //    transform.rotation = data.rotation.ToVector();
        //    GetComponent<NavMeshAgent>().enabled = true;

        //}
        //--------------------------------------------------------------//
    }
}
