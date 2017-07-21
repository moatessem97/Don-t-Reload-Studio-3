using System;
using UnityEngine;

namespace UnityStandardAssets._2D
{
    public class Camera2DFollow : Photon.PunBehaviour
    {
        public Transform target;
        public float damping = 1;
        public float lookAheadFactor = 3;
        public float lookAheadReturnSpeed = 0.5f;
        public float lookAheadMoveThreshold = 0.1f;

        private float m_OffsetZ;
        private Vector3 m_LastTargetPosition;
        private Vector3 m_CurrentVelocity;
        private Vector3 m_LookAheadPos;

        private GameObject SceneCamera;

        // Use this for initialization
        private void Start()
        {
            target = gameObject.transform;
            SceneCamera = GameObject.FindGameObjectWithTag("MainCamera");
            m_LastTargetPosition = target.position;
            m_OffsetZ = (SceneCamera.transform.position - target.position).z;
            transform.parent = null;
        }


        // Update is called once per frame
        private void Update()
        {
            //if (!photonView.isMine)
            //{
            //    return;
            //}
            // only update lookahead pos if accelerating or changed direction
            float xMoveDelta = (target.position - m_LastTargetPosition).x;

            bool updateLookAheadTarget = Mathf.Abs(xMoveDelta) > lookAheadMoveThreshold;

            if (updateLookAheadTarget)
            {
                m_LookAheadPos = lookAheadFactor*Vector3.right*Mathf.Sign(xMoveDelta);
            }
            else
            {
                m_LookAheadPos = Vector3.MoveTowards(m_LookAheadPos, Vector3.zero, Time.deltaTime*lookAheadReturnSpeed);
            }

            Vector3 aheadTargetPos = target.position + m_LookAheadPos + Vector3.forward*m_OffsetZ;
            Vector3 newPos = Vector3.SmoothDamp(SceneCamera.transform.position, aheadTargetPos, ref m_CurrentVelocity, damping);

            SceneCamera.transform.position = newPos;

            m_LastTargetPosition = target.position;
        }
    }
}
