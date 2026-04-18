using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AI;
using System.Threading;
using Unity.VisualScripting;
using System;
using GameManagement;

public class Mine : MonoBehaviour, IShootable
{
    [SerializeField]
    private bool mineEnabled = false;

    [SerializeField]
    private const int explosionRadius = 2;

    [SerializeField]
    private Material material;

    [SerializeField]
    private Material activeMineMaterial;

    [SerializeField]
    private AudioSource explosionSFX;

    private GameObject explosionVisual;

    private GameVariables gv;

    private bool enableVisualSpawn = true;

    void Start()
    {
        gv = FindObjectOfType<GameVariables>();
        Invoke(nameof(EnableMine), 5f);
    }

    void OnCollisionEnter(Collision collision)
    {
        if (!mineEnabled) return;
        if (collision.gameObject.GetComponent<AIController>())
        {
            AIController triggerAI = collision.gameObject.GetComponent<AIController>();
            if (!triggerAI.IsAssassin() && triggerAI != gv.designatedSurvivor)
            {
                if (enableVisualSpawn)
                {
                    explosionSFX.mute = false;
                    explosionSFX.time = 0.5f;
                    explosionSFX.Play();
                    explosionVisual = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                    explosionVisual.transform.position = transform.position;
                    explosionVisual.transform.localScale = new Vector3(explosionRadius, explosionRadius, explosionRadius);
                    explosionVisual.GetComponent<Renderer>().material = material;
                    gameObject.GetComponent<Renderer>().enabled = false;
                    enableVisualSpawn = false;
                }
                Collider[] hitColliders = Physics.OverlapSphere(transform.position, explosionRadius);
                foreach (var hitCollider in hitColliders)
                {
                    if (hitCollider.gameObject.GetComponent<AIController>())
                    {
                        AIController loopAI = hitCollider.gameObject.GetComponent<AIController>();
                        if (!loopAI.IsAssassin() && loopAI != gv.designatedSurvivor)
                        {
                            loopAI.OnHit(false, gameObject.transform);
                        }
                    }
                }
                Invoke(nameof(ClearAll), 1.5f);
            }
        }
    }

    void EnableMine()
    {
        gameObject.GetComponent<Renderer>().material = activeMineMaterial;
        mineEnabled = true;
    }

    public void OnHit(bool hitByPlayer, Transform transform)
    {
        Destroy(gameObject);
    }
    public void ClearAll()
    {
        Destroy(explosionVisual);
        transform.GetComponent<BoxCollider>().enabled = false;
        Invoke(nameof(ClearAllP2), 1f);
    }

    public void ClearAllP2()
    {
        Destroy(gameObject);
    }

}
