using System.Collections.Generic;
using UnityEngine;

public class PlayerParticleController : MonoBehaviour
{
    [SerializeField] private List<ParticleSystem> _paintParticle;

    public void PaintColorSet(Material mat)
    {
        foreach (ParticleSystem p in _paintParticle)
        {
            if (p.TryGetComponent(out ParticlesController particlesController))
            {
                particlesController.paintColor = mat.color;

                // material �ٲ��� �ʿ� ����
                continue;
            }

            ParticleSystemRenderer rend =
                p.GetComponent<ParticleSystemRenderer>();
            rend.material = mat;
            rend.trailMaterial = mat;
        }
    }
}
