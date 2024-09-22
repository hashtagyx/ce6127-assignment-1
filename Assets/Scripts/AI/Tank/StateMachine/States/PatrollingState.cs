using System.Collections;
using UnityEngine;

using Random = UnityEngine.Random;
using Debug = UnityEngine.Debug;

namespace CE6127.Tanks.AI
{
    /// <summary>
    /// Class <c>PatrollingState</c> represents the state of the tank when it is patrolling.
    /// </summary>
    internal class PatrollingState : BaseState
    {
        private TankSM m_TankSM;        // Reference to the tank state machine.
        private Vector3 m_Destination;  // Destination for the tank to move to.
        private float m_RotationSpeed = 8f; // Rotation speed of the tank

        private float m_RaycastDist = 9999999999f; // Raycast max distance from the tank
        /// <summary>
        /// Constructor <c>PatrollingState</c> constructor.
        /// </summary>
        public PatrollingState(TankSM tankStateMachine) : base("Patrolling", tankStateMachine) => m_TankSM = (TankSM)m_StateMachine;

        /// <summary>
        /// Method <c>Enter</c> on enter.
        /// </summary>
        public override void Enter()
        {
            base.Enter();

            m_TankSM.SetStopDistanceToZero();

            m_TankSM.StartCoroutine(Patrolling());
        }


        public override void Update()
        {
            base.Update();
            
            m_TankSM.transform.Rotate(0f, m_RotationSpeed, 0f);
            // Raycast for the human tank
            if (Physics.Raycast(m_TankSM.transform.position, m_TankSM.transform.forward, out RaycastHit hit, m_RaycastDist, LayerMask.GetMask("Players")))
            // if (Physics.Raycast(m_TankSM.transform.position, m_TankSM.transform.forward, out RaycastHit hit, m_RaycastDist))
            {
                if (hit.collider.CompareTag("Player")) // Assuming the human tank has the "Player" tag
                {
                    // Target spotted, switch to pursuit state
                    m_TankSM.Target = hit.collider.gameObject.transform;
                    m_StateMachine.ChangeState(m_TankSM.m_States.Pursuit);
                }
            }

            if (Time.time >= m_TankSM.NavMeshUpdateDeadline)
            {
                m_TankSM.NavMeshUpdateDeadline = Time.time + m_TankSM.PatrolNavMeshUpdate;
                m_TankSM.NavMeshAgent.SetDestination(m_Destination);
            }
        }

        /// <summary>
        /// Method <c>Exit</c> on exiting PatrollingState.
        /// </summary>
        public override void Exit()
        {
            base.Exit();

            m_TankSM.StopCoroutine(Patrolling());
        }

        /// <summary>
        /// Coroutine <c>Patrolling</c> patrolling coroutine.
        /// </summary>
        IEnumerator Patrolling()
        {
            while (true)
            {
                var destination = Random.insideUnitCircle * Random.Range(m_TankSM.PatrolMaxDist.x, m_TankSM.PatrolMaxDist.y);
                m_Destination = m_TankSM.transform.position + new Vector3(destination.x, 0f, destination.y);

                float waitInSec = Random.Range(m_TankSM.PatrolWaitTime.x, m_TankSM.PatrolWaitTime.y);
                yield return new WaitForSeconds(waitInSec);
            }
        }
    }
}