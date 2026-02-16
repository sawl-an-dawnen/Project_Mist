using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PillarTrap : MonoBehaviour, ITrap
{
    private new Animation animation;

    // Start is called before the first frame update
    void Start()
    {
        animation = GetComponent<Animation>();
    }

    public void ActivateTrap()
    {
        animation.Play();
    }

    public void DeactivateTrap()
    {
        throw new System.NotImplementedException();
    }

    public void ResetTrap()
    {
        throw new System.NotImplementedException();
    }



}
