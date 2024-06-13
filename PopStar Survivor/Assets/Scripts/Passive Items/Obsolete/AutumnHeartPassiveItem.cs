using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutumnHeartPassiveItem : PassiveItem
{

    protected override void ApplyModifier()
    {
        player.CurrentHealth += passiveItemData.Multiplier;
    }

}
