using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WeaponsManager;

namespace RSClasses
{
    public class 
        PhantomLunge : MonoBehaviour
    {
        private Gun sword;
        private Player player;
        private int framesCollisionDisabled;
        private PlayerCollision collision;
        private ParticleSystem particle;
        void Start()
        {
            player = GetComponentInParent<Player>();
            WeaponManager weaponManager = this.GetComponentInParent<WeaponManager>();
            sword = weaponManager.GetWeapon("RSC_Sword");
            sword.ShootPojectileAction += OnShootProjectileAction;
            collision = player.GetComponent<PlayerCollision>();
            particle = this.GetComponent<ParticleSystem>();
        }

        private void FixedUpdate()
        {
            collision.enabled = framesCollisionDisabled <= 0;
            if (framesCollisionDisabled == 0)
                collision.IgnoreWallForFrames(2);
            if (framesCollisionDisabled < 0 && Physics2D.OverlapCircle(this.transform.position, 0.01f, LayerMask.GetMask("Default")) && player.data.view.IsMine)
                player.data.healthHandler.CallTakeDamage(Vector2.up * player.data.maxHealth * 0.25f * Time.fixedDeltaTime, this.transform.position); // 50% health per second
            framesCollisionDisabled--;
        }

        private void OnDestroy()
        {
            sword.ShootPojectileAction -= OnShootProjectileAction;
        }

        private void OnShootProjectileAction(GameObject bullet)
        {
            framesCollisionDisabled = 35;
            particle.Play();
        }
    }
}