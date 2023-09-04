using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;

namespace Assets.Scripts
{
    public class Hider : Agent
    {
        public float MoveSpeed = 1.5f;
        public float TurnSpeed = 300f;
        public bool IfSeekerSee { get; set; }

        private Rigidbody m_AgentRb;
        private int positionNum;

        private Seeker m_Seeker;

        [SerializeField] private Transform m_SeekerTransform;

        public override void Initialize()
        {
            HideAndSeekEnvController envController = GetComponentInParent<HideAndSeekEnvController>();
            m_AgentRb = GetComponent<Rigidbody>();
        }

        public override void OnEpisodeBegin()
        {
            positionNum++;
            IfSeekerSee = false;

            switch (positionNum)
            {
                case 1:
                    transform.localPosition = new Vector3(UnityEngine.Random.Range(0f, -11.0f), 1.0f, -16.5f);
                    transform.localRotation = Quaternion.Euler(0, UnityEngine.Random.Range(0, 359), 0);
                    break;
                case 2:
                    transform.localPosition = new Vector3(UnityEngine.Random.Range(4.5f, 11.5f), 1.0f, -16.5f);
                    transform.localRotation = Quaternion.Euler(0, UnityEngine.Random.Range(0, 359), 0);
                    break;
                case 3:
                    transform.localPosition = new Vector3(UnityEngine.Random.Range(4.5f, 11.5f), 1.0f, -4f);
                    transform.localRotation = Quaternion.Euler(0, UnityEngine.Random.Range(0, 359), 0);
                    positionNum = 0;
                    break;
            }
        }

        public override void CollectObservations(VectorSensor sensor)
        {
            sensor.AddObservation(transform.localPosition);
            sensor.AddObservation(transform.localRotation);
            sensor.AddObservation(m_SeekerTransform.localPosition);
            sensor.AddObservation(IfSeekerSee);
        }

        public override void Heuristic(in ActionBuffers actionsOut)
        {
            var continuousActionsOut = actionsOut.ContinuousActions;
            if (Input.GetKey(KeyCode.Keypad6))
            {
                continuousActionsOut[1] = 1;
            }

            if (Input.GetKey(KeyCode.Keypad8))
            {
                continuousActionsOut[0] = 1;
            }

            if (Input.GetKey(KeyCode.Keypad4))
            {
                continuousActionsOut[1] = -1;
            }

            if (Input.GetKey(KeyCode.Keypad5))
            {
                continuousActionsOut[0] = -1;
            }

            if (Input.GetKey(KeyCode.Keypad7))
            {
                continuousActionsOut[2] = 1;
            }

            if (Input.GetKey(KeyCode.Keypad9))
            {
                continuousActionsOut[2] = -1;
            }
        }

        public override void OnActionReceived(ActionBuffers actionBuffers)
        {
            MoveAgent(actionBuffers);
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

        private void Start()
        {
            m_SeekerTransform = transform.parent.Find("Seeker");
            m_Seeker = m_SeekerTransform.GetComponent<Seeker>();
        }

    }
}
