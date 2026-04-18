using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IShootable
{
    void OnHit(bool hitByPlayer, Transform transform);

}
