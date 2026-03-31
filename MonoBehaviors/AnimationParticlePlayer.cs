using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RSClasses
{
    public class AnimationParticlePlayer : MonoBehaviour
    {
        private ParticleSystem[] particles;
        void Start()
        {
            particles = GetComponentsInChildren<ParticleSystem>();
        }

        void PlayParticles()
        {
            foreach (ParticleSystem particle in particles)
                particle.Play();
        }
    }
}