using UnityEngine;

using Debug = UnityEngine.Debug;

namespace CE6127.Tanks.AI
{
    internal class PursuitState : BaseState
    {
        private TankSM m_TankSM;
        private float nextFireTime; // To control the rate of fire
        public PursuitState(TankSM tankStateMachine) : base("Pursuit", tankStateMachine) => m_TankSM = (TankSM)m_StateMachine;

        public override void Enter()
        {
            base.Enter();
            // Set a suitable stopping distance for pursuit
            m_TankSM.SetStopDistanceTo(10f);
            nextFireTime = Time.time; // Initialize fire time
        }

        public override void Update()
        {
            base.Update();
            
            // Calculate the direction vector from the tank to the target
            Vector3 directionToTarget = m_TankSM.Target.position - m_TankSM.transform.position;

            // Create a rotation that points the tank in the direction of the target
            Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);

            // Smoothly rotate the tank towards the target using spherical interpolation (slerp)
            m_TankSM.transform.rotation = Quaternion.Slerp(m_TankSM.transform.rotation, targetRotation, Time.deltaTime * 5f);

            // Get the current position of the target
            Vector3 targetPosition = m_TankSM.Target.position;

            // Calculate the distance between the tank and the target
            float distanceToTarget = Vector3.Distance(m_TankSM.transform.position, targetPosition);

            // Follow the target
            if (m_TankSM.Target != null)
            {
                m_TankSM.NavMeshAgent.SetDestination(m_TankSM.Target.position);

                // Shoot with a delay
                if (Time.time > nextFireTime)
                {
                    // Adjust the projectile's launch parameters based on the distance to the target
                    float projectilePower = Mathf.Lerp(m_TankSM.LaunchForceMinMax.x, m_TankSM.LaunchForceMinMax.y , distanceToTarget / (m_TankSM.LaunchForceMinMax.y - m_TankSM.LaunchForceMinMax.x)); 
                    m_TankSM.LaunchProjectile(projectilePower);

                    nextFireTime = Time.time + m_TankSM.FireInterval.x; // Control fire rate
                }
            }
            else
            {
                // Target is lost, return to patrolling
                m_StateMachine.ChangeState(m_TankSM.m_States.Patrolling);
            }
        }
    }
}