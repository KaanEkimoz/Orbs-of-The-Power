using UnityEngine;

namespace com.game.abilitysystem
{
    public enum AbilityUseType
    {
        [InspectorName("Press (One Shot)")] Press,
        [InspectorName("Hold (Durable)")] Hold,
        [InspectorName("Toggle (Durable)")] Toggle,
        [InspectorName("Custom (Define in Behaviour)")] Custom,
    }
}
