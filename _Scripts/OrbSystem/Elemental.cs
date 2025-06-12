/// <summary>
/// Represents different types of elemantals using for magic orbs, each with unique properties and abilities.
/// </summary>
public enum ElementalType
{
    None,
    /// <summary>
    /// Electric Orb:
    /// - Sticks to an enemy and jumps to nearby enemies as an electric current.
    /// - Remains attached to the enemy during its effect.
    /// Upgradable Stats:
    /// - Bounce Count
    /// - Damage
    /// - Bounce Speed
    /// - Enemy Slowdown Speed
    /// </summary>
    Electric,

    /// <summary>
    /// Ice Orb:
    /// - Sticks to an enemy and freezes a specific area.
    /// - Frozen enemies are immobilized and cannot deal damage.
    /// Upgradable Stats:
    /// - Damage
    /// - Freeze Duration
    /// - Damage Taken While Frozen
    /// - Freeze Radius
    /// </summary>
    Ice,

    /// <summary>
    /// Fire Orb:
    /// - Deals burn damage over time after hitting an enemy.
    /// Upgradable Stats:
    /// - Initial Damage
    /// - Burn Damage
    /// - Burn Duration
    /// - Explosion Radius
    /// </summary>
    Fire,

    /// <summary>
    /// Soul Binder Orb:
    /// - Sticks to an enemy and seals it.
    /// - If multiple seals exist, damage taken by one seal is shared among all sealed enemies.
    /// Upgradable Stats:
    /// - Damage
    /// - Number of Seals per Orb
    /// - Seal Damage Multiplier
    /// - Stun Duration on Sealed Enemies
    /// </summary>
    Soul,

    /// <summary>
    /// Air Orb:
    /// - Pushes characters within a specific area after contact.
    /// Upgradable Stats:
    /// - Damage
    /// - Push Distance
    /// </summary>
    Air,

    /// <summary>
    /// Health Orb:
    /// - Sticks to an enemy and slowly transfers its health to the player.
    /// - Note: The orb does not deal damage while transferring health.
    /// Upgradable Stats:
    /// - Maximum Health Gain
    /// - Health Transfer Speed
    /// </summary>
    Lifesteal,

    /// <summary>
    /// Flash Orb:
    /// - Creates an explosion upon contact with an enemy.
    /// - Stuns enemies within the explosion radius.
    /// Upgradable Stats:
    /// - Damage
    /// - Explosion Radius
    /// - Stun Duration
    /// </summary>
    Flash
}