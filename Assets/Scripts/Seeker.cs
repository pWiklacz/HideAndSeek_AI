using System;
using System.Collections.Generic;
using Unity.Barracuda;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using Unity.MLAgentsExamples;
using Unity.VisualScripting;
using UnityEngine;
using static Unity.MLAgents.Sensors.RayPerceptionOutput;
using Random = System.Random;

namespace Assets.Scripts
{
    public class Seeker : Agent
    {
        public float MoveSpeed = 1.5f;
        public float TurnSpeed = 300f; 
        public bool SeeHider { get; set; }

        public bool Freeze { get; private set; }

        private Rigidbody m_AgentRb;
  

        [SerializeField] private Transform m_HiderTransform;

        public List<MyRay> Rays;

        //private float smoothRotate = 0f;

        public void Unfreeze()
        {
            Freeze = false;
        }

        public override void Initialize()
        {
            HideAndSeekEnvController envController = GetComponentInParent<HideAndSeekEnvController>();
            m_AgentRb = GetComponent<Rigidbody>();
            Rays = new List<MyRay>();
        }

        public override void OnEpisodeBegin()   
        {
            transform.localPosition = new Vector3(UnityEngine.Random.Range(-13.0f, 13.0f), 1.0f, 12.0f);
            transform.localRotation = Quaternion.Euler(0, -180f, 0);
            SeeHider = false;
            Freeze = true;
        }

        public override void CollectObservations(VectorSensor sensor)
        {
            if (!Freeze)
            {
                sensor.AddObservation(transform.localPosition);
                sensor.AddObservation(transform.localRotation);
                sensor.AddObservation(m_HiderTransform.localPosition);
                sensor.AddObservation(SeeHider);
            }
        }

        public void MoveAgent(ActionBuffers actionBuffers)
        {
            var continuousActions = actionBuffers.ContinuousActions;

            var forward = Mathf.Clamp(continuousActions[0], -1f, 1f);
            var right = Mathf.Clamp(continuousActions[1], -1f, 1f);
            var rotate = Mathf.Clamp(continuousActions[2], -1f, 1f);

            var dirToGo = transform.forward * forward;
            dirToGo += transform.right * right;
            var rotateDir = -transform.up * rotate;

            m_AgentRb.AddForce(dirToGo * MoveSpeed, ForceMode.VelocityChange);
            transform.Rotate(rotateDir, Time.fixedDeltaTime * TurnSpeed);

            if (m_AgentRb.velocity.sqrMagnitude > 25f) 
            {
                m_AgentRb.velocity *= 0.95f;
            }
        }

        public override void OnActionReceived(ActionBuffers actionBuffers)
        {
            if (!Freeze)
            {
                MoveAgent(actionBuffers);
            }
        }

        public override void Heuristic(in ActionBuffers actionsOut)
        {
            var continuousActionsOut = actionsOut.ContinuousActions;
            if (Input.GetKey(KeyCode.D))
            {
                continuousActionsOut[1] = 1;
            }

            if (Input.GetKey(KeyCode.W))
            {
                continuousActionsOut[0] = 1;
            }

            if (Input.GetKey(KeyCode.A))
            {
                continuousActionsOut[1] = -1;
            }

            if (Input.GetKey(KeyCode.S))
            {
                continuousActionsOut[0] = -1;
            }

            if (Input.GetKey(KeyCode.E))
            {
                continuousActionsOut[2] = -1;
            }

            if (Input.GetKey(KeyCode.Q))
            {
                continuousActionsOut[2] = 1;
            }
        }

        private void Start()
        {
            m_HiderTransform = transform.parent.Find("Hider");
            var viewField = transform.Find("View");

            foreach (var child in viewField.GetComponentsInChildren<Transform>())
            {
                if (!child.CompareTag("MyRay")) continue;
                var myRay = child.GetComponent<MyRay>();
                Rays.Add(myRay);
            }
        }
        private void FixedUpdate()
        {
        }
    }
}
