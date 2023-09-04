using UnityEngine;
using UnityEngine.InputSystem;

namespace Assets.Scripts
{
    public class HideAndSeekEnvController : MonoBehaviour
    {
        public Hider Hider;
        public Seeker Seeker;

        [Tooltip("Max Environment Steps")] public int MaxEnvironmentSteps = 25000;

        private int m_ResetTimer;
        private float m_SeeHiderDuration;
        private const float RequiredDuration = 2.0f;

        private void Start()
        {
        }

        private void FixedUpdate()
        {
            m_ResetTimer += 1;
            if (m_ResetTimer >= MaxEnvironmentSteps && MaxEnvironmentSteps > 0)
            {
                Hider.AddReward(1f);
                Seeker.AddReward(-1f);
                print("HiderWin");
                ResetScene(); 
            }
            if (m_ResetTimer  == 500) Seeker.Unfreeze();
            
            if(!Seeker.Freeze)
                CheckIfSeeTheHider();
        }

        private void ResetScene()
        {
            m_ResetTimer = 0;
            m_SeeHiderDuration = 0;
            Hider.EndEpisode();
            Seeker.EndEpisode();
        }

        private void CheckIfSeeTheHider()
        {
            var seeHider = false;

            foreach (var ray in Seeker.Rays)
            {
                if (ray.FoundHider)
                {
                    seeHider = true;
                    break;
                }
            }

            if (!seeHider && Seeker.SeeHider)
            {
                Seeker.AddReward(-.5f);
                Hider.AddReward(.5f);
                print("stracil z oczu");
            }

            Seeker.SeeHider = seeHider;
            Hider.IfSeekerSee = Seeker.SeeHider;

            if (Seeker.SeeHider)
            {
                if (m_SeeHiderDuration < RequiredDuration)
                {
                    m_SeeHiderDuration += 0.02f;
                    float bonus = .02f * Mathf.Clamp01(Vector3.Dot(Seeker.transform.forward.normalized,
                        -Hider.transform.localPosition.normalized));
                    Seeker.AddReward(.01f + bonus);
                    Hider.AddReward(-.01f);
                    print("see");
                }
                else
                {
                    print("seekerWin");
                    Seeker.AddReward(1f);
                    Hider.AddReward(-1f);
                    ResetScene();
                }
            }
            else
            {
                m_SeeHiderDuration = 0;
                Seeker.AddReward(-.001f);
                Hider.AddReward(.001f);
            }
        }
    }
}
