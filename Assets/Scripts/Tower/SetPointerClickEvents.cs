using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

//Custom pointer clicks because serialized scene information doesn't stay on prefabs
public class SetPointerClickEvents : EventTrigger {
    public CastSpell CastSpell;
    public PlaceTrap PlaceTrap;

    public override void OnPointerClick(PointerEventData eventData)
    {
        base.OnPointerClick(eventData);
        PlaceTrap.OnClickTower();
    }
}
