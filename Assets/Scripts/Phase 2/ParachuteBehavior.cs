using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParachuteBehavior : EchelonBehavior
{
    private int cooldown = 0;

    public void UsePara()
    {
        cooldown = 5;
    }

    public int GetCooldown()
    {
        return cooldown;
    }

    public void ReduceCooldown()
    {
        cooldown--;
        if (cooldown < 0)
            cooldown = 0;
    }
}
