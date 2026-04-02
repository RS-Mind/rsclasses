using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RSClasses
{
    public class HeaterShield : MonoBehaviour
    {
        private Player player;
        private float rotVel;
        private float currentRot;
        private Transform position;
        internal GameObject shieldCollider;

        void Start()
        {
            player = GetComponentInParent<Player>();
            GetComponentInChildren<Image>().color = player.GetTeamColors().color;
            position = transform.GetChild(0);
            shieldCollider = position.GetChild(0).gameObject;
            shieldCollider.transform.SetParent(null);
        }

        void OnDisable()
        {
            shieldCollider.SetActive(false);
        }

        void OnEnable()
        {
            shieldCollider.SetActive(true);
        }

        void OnDestroy()
        {
            Destroy(shieldCollider);
        }

        void Update()
        {
            float targetRot = Vector2.SignedAngle(Vector2.up, player.data.aimDirection);
            if (targetRot - currentRot > 180)
                currentRot += 360;
            if (currentRot - targetRot > 180)
                currentRot -= 360;

            rotVel = FRILerp.Lerp(rotVel, (targetRot - currentRot) * 2.5f, 10f);
            currentRot += rotVel * TimeHandler.deltaTime;
            transform.localEulerAngles = new Vector3(0f, 0f, currentRot);
            shieldCollider.transform.SetPositionAndRotation(position.position, position.rotation);
        }
    }
}