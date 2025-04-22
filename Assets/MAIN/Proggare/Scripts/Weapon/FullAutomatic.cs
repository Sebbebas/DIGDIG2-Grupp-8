using System.Collections;
using UnityEngine;

public class FullAutomatic : Weapon
{
    public new void Start()
    {
        base.Start();
    }

    public override bool Fire()
    {
        if (!base.Fire())
        {
            return false;
        }

        return true;
    }
}
