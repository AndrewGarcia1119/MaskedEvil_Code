using System;
using System.Collections;
using System.Collections.Generic;
using AI;
using UnityEngine;

public class KillOnTouch : MonoBehaviour
{
    public event Action onKillAssassin;
    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.TryGetComponent<AIController>(out AIController character))
        {
            character.OnHit(false, character.transform);
            if (character.IsAssassin())
            {
                onKillAssassin?.Invoke();
            }
        }
    }
}
