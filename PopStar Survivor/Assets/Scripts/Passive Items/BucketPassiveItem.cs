using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BucketPassiveItem : PassiveItem
{
    protected override void ApplyModifier()
    {
        player.CurrentMagnet *= 1 + passiveItemData.Multiplier / 100f;
    }

}
