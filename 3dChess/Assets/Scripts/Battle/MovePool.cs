using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[ExecuteInEditMode]
public class MovePool : MonoBehaviour
{
    public static List<Move> Pool = new() {

// ===================== ATTACK =====================
// -------- Common --------
new Move("Strike",            "A simple strike with solid force.",                        MoveType.Attack, MoveRarity.Common, 40f, "basic", "bronze", "stone", "terra", "radiant", "storm"),
new Move("Splash Surge",      "Wave of water crashes into the enemy.",                    MoveType.Attack, MoveRarity.Common, 40f, "aqua", "storm"),
new Move("Riposte",           "Counter-minded strike that punishes aggression.",          MoveType.Attack, MoveRarity.Common, 41f, "basic", "bronze", "stone", "radiant", "storm", "void"),
new Move("Ice Shards",        "Shatter a flurry of shards at the foe.",                   MoveType.Attack, MoveRarity.Common, 44f, "frost", "stone"),
new Move("Rattle Strike",     "A clattering hit that still gets through.",                MoveType.Attack, MoveRarity.Common, 43f, "rust", "bronze", "stone"),
new Move("Earthen Slam",      "A crushing blow from solid ground.",                       MoveType.Attack, MoveRarity.Common, 45f, "terra", "stone", "inferno"),
new Move("Metal Bash",        "Solid strike with blunt force.",                           MoveType.Attack, MoveRarity.Common, 45f, "bronze", "stone", "rust"),
new Move("Seismic Toss",      "Heave the foe with earthy might.",                         MoveType.Attack, MoveRarity.Common, 47f, "terra", "stone", "inferno"),

// -------- Rare --------
new Move("Riptide",           "A sweeping surge that pulls the foe off balance.",         MoveType.Attack, MoveRarity.Rare,   50f, "aqua", "storm"),
new Move("Precision Strike",  "Well-timed hit that lands before slower moves.",           MoveType.Attack, MoveRarity.Rare,   52f, "storm", "radiant", "aqua", "basic", "rust"), 
new Move("Frostbite",         "Cold strike that chills the foe.",                         MoveType.Attack, MoveRarity.Rare,   52f, "frost", "stone"), 
new Move("Quickbolt",         "Fast lightning strike.",                                   MoveType.Attack, MoveRarity.Rare,   52f, "storm", "radiant"),
new Move("Shockwave",         "A wave of force that rarely misses.",                      MoveType.Attack, MoveRarity.Rare,   52f, "radiant", "aqua"),
new Move("Scorch",            "A searing blast with solid power.",                        MoveType.Attack, MoveRarity.Rare,   52f, "inferno", "storm"),

// -------- Epic --------
new Move("Void Echo",         "A nihil strike that resonates painfully.",                 MoveType.Attack, MoveRarity.Epic,   53f, "void", "inferno"),
new Move("Shatter Point",     "Strike brittle spots for bonus effect.",                   MoveType.Attack, MoveRarity.Epic,   54f, "frost", "void"),
new Move("Smite",             "Holy strike that punishes weakened foes.",                 MoveType.Attack, MoveRarity.Epic,   55f, "radiant", "storm"),
new Move("Void Slash",        "Strike infused with null energy.",                         MoveType.Attack, MoveRarity.Epic,   55f, "void", "inferno"),

// -------- Legendary --------
new Move("Flame Burst",       "Explosive fireball with high damage.",                     MoveType.Attack, MoveRarity.Legendary,   60f, "inferno", "radiant"),
new Move("Overcharge",        "Heavy burst of energy but reckless.",                      MoveType.Attack, MoveRarity.Legendary,   70f, "storm", "inferno"),

// ===================== HEAL =====================
// -------- Common --------
new Move("Steady Breath",   "Center yourself and recover a little health.",      MoveType.Heal, MoveRarity.Common,    30f, "basic", "terra", "bronze", "stone", "frost"),
new Move("Mend",            "Patch up wounds with steady resolve.",              MoveType.Heal, MoveRarity.Common,    32f, "terra", "stone", "bronze", "basic"),
new Move("Quench",          "Cool waters restore minor vitality.",               MoveType.Heal, MoveRarity.Common,    34f, "aqua", "frost", "terra"),
new Move("Patchwork",       "Rough but serviceable field healing.",              MoveType.Heal, MoveRarity.Common,    28f, "rust", "bronze", "inferno"),
new Move("First Aid",       "Basic healing technique learned by many.",          MoveType.Heal, MoveRarity.Common,    35f, "basic", "radiant"),
new Move("Soothing Chill",  "A calming frost numbs pain, restoring health.",     MoveType.Heal, MoveRarity.Common,    36f, "frost", "aqua", "storm"),

// -------- Rare --------
new Move("Second Wind",     "Recover stamina mid-fight.",                        MoveType.Heal, MoveRarity.Rare,      40f, "basic", "aqua", "terra", "radiant"),
new Move("Cauterize",       "Seal wounds with heat, restoring health.",          MoveType.Heal, MoveRarity.Rare,      42f, "inferno", "bronze", "basic"),
new Move("Lustrate",        "A focused heal bathed in light.",                   MoveType.Heal, MoveRarity.Rare,      44f, "radiant", "aqua"),
new Move("Purify",          "Cleansing waters wash away injuries.",              MoveType.Heal, MoveRarity.Rare,      46f, "aqua", "radiant"),

// -------- Epic --------
new Move("Entropic Drain",  "Siphon life from the enemy to heal yourself.",      MoveType.Heal, MoveRarity.Epic,      52f, "void", "aqua"),
new Move("Blessing",        "Radiant power heals and shields.",                  MoveType.Heal, MoveRarity.Epic,      55f, "radiant", "aqua"),
new Move("Vital Bloom",     "Earthen vitality surges, mending wounds.",          MoveType.Heal, MoveRarity.Epic,      50f, "terra", "stone", "frost"),
new Move("Infernal Renewal","Heat-forged regeneration restores health quickly.", MoveType.Heal, MoveRarity.Epic,      58f, "inferno", "storm"),

// -------- Legendary --------
new Move("Prismatic Restoration","Blinding light restores immense vitality.",    MoveType.Heal, MoveRarity.Legendary, 70f, "radiant", "storm", "aqua"),
new Move("Void Rebirth",    "Embrace the void to return from near defeat.",      MoveType.Heal, MoveRarity.Legendary, 65f, "void", "terra", "frost"),

// ===================== DEFENSE =====================

// -------- Common --------
new Move("Rally",            "Steady your stance to blunt incoming blows.",        MoveType.Defense, MoveRarity.Common,    12f, "basic", "bronze", "stone", "radiant", "inferno"),
new Move("Glaze",            "A thin ice layer hardens defenses briefly.",         MoveType.Defense, MoveRarity.Common,    15f, "frost", "radiant", "basic"),
new Move("Stonewall Guard",  "Layered guard for reliable protection.",             MoveType.Defense, MoveRarity.Common,    16f, "terra", "stone", "bronze", "basic"),
new Move("Guard",            "Raise your guard to reduce damage.",                 MoveType.Defense, MoveRarity.Common,    18f, "basic", "bronze", "terra"),
new Move("Brace",            "Brace against impact with practiced form.",          MoveType.Defense, MoveRarity.Common,    14f, "frost", "stone", "terra"),
new Move("Water Screen",     "A thin water film softens incoming force.",          MoveType.Defense, MoveRarity.Common,    16f, "aqua", "radiant", "basic"),

// -------- Rare --------
new Move("Fortify",          "Hunker down and reinforce your guard.",              MoveType.Defense, MoveRarity.Rare,      22f, "frost", "stone", "bronze", "terra"),
new Move("Water Veil",       "Protective veil of water dampens strikes.",          MoveType.Defense, MoveRarity.Rare,      22f, "aqua", "radiant"),
new Move("Arc Guard",        "Charged guard disperses part of the impact.",        MoveType.Defense, MoveRarity.Rare,      23f, "storm", "bronze", "basic", "void"),
new Move("Bulwark",          "Adopt an immovable, reinforced stance.",             MoveType.Defense, MoveRarity.Rare,      24f, "terra", "stone", "frost", "inferno"),

// -------- Epic --------
new Move("Glacial Guard",    "Heavy ice plating greatly reduces damage.",          MoveType.Defense, MoveRarity.Epic,      35f, "frost", "terra"),
new Move("Aegis of Terra",   "Earthen shield absorbs powerful blows.",             MoveType.Defense, MoveRarity.Epic,      32f, "terra", "stone"),
new Move("Radiant Ward",     "A brilliant ward that turns aside harm.",            MoveType.Defense, MoveRarity.Epic,      33f, "radiant", "aqua", "void"),
new Move("Storm Barrier",    "Whirling currents deflect incoming force.",          MoveType.Defense, MoveRarity.Epic,      32f, "storm", "bronze", "basic"),

// -------- Legendary --------
new Move("Prismatic Aegis",  "Multispectral shield that negates most damage.",     MoveType.Defense, MoveRarity.Legendary, 42f, "radiant", "storm", "aqua"),
new Move("Null Dome",        "A void field that smothers all impact.",             MoveType.Defense, MoveRarity.Legendary, 45f, "void", "frost", "inferno"),

// ===================== EVASION =====================

// -------- Common --------
new Move("Evasive Stance",   "Adopt a guarded stance to dodge more easily.", MoveType.Evasion, MoveRarity.Common,   12f, "basic", "storm", "radiant", "aqua", "inferno"),
new Move("Patina Veil",      "A dull sheen distracts and deflects strikes.", MoveType.Evasion, MoveRarity.Common,   13f, "bronze", "rust"),
new Move("Light Step",       "A nimble shift makes attacks harder to land.", MoveType.Evasion, MoveRarity.Common,   11f, "radiant", "basic"),
new Move("Mist Cover",       "Veil yourself in water mist to evade blows.",  MoveType.Evasion, MoveRarity.Common,   14f, "aqua", "frost"),
new Move("Dust Cloud",       "Kick up dirt and dust to obscure yourself.",   MoveType.Evasion, MoveRarity.Common,   10f, "terra", "stone"),
new Move("Flicker",          "A sudden sidestep avoids slower attacks.",     MoveType.Evasion, MoveRarity.Common,   12f, "storm", "inferno"),

// -------- Rare --------
new Move("Halo Step",        "Light-guided movement sharpens reflexes.",     MoveType.Evasion, MoveRarity.Rare,     18f, "radiant", "storm", "basic"),
new Move("Kindle",           "Heat distortions make you harder to target.",  MoveType.Evasion, MoveRarity.Rare,     19f, "inferno", "storm"),
new Move("Afterimage",       "Move so quickly you leave a false trail.",     MoveType.Evasion, MoveRarity.Rare,     20f, "storm", "radiant", "aqua"),
new Move("Shadow Step",      "Slip partly into shadow to evade.",            MoveType.Evasion, MoveRarity.Rare,     17f, "void", "rust", "frost"),

// -------- Epic --------
new Move("Eventide Step",    "Twilight footwork hides your true position.",  MoveType.Evasion, MoveRarity.Epic,     22f, "void", "storm"),
new Move("Burning Veil",     "Heat shimmer bends vision around you.",        MoveType.Evasion, MoveRarity.Epic,     23f, "inferno", "radiant"),
new Move("Aurora Dance",     "Radiant steps blur into shifting light.",      MoveType.Evasion, MoveRarity.Epic,     24f, "radiant", "aqua"),
new Move("Glacial Slip",     "Slide across ice with uncanny avoidance.",     MoveType.Evasion, MoveRarity.Epic,     22f, "frost", "aqua", "stone"),

// -------- Legendary --------
new Move("Prismatic Shift",  "Split into illusions of pure light.",          MoveType.Evasion, MoveRarity.Legendary, 28f, "radiant", "storm", "aqua"),
new Move("Void Phase",       "Momentarily phase out of reality.",            MoveType.Evasion, MoveRarity.Legendary, 26f, "void", "inferno", "frost"),

// ===================== SPEED =====================
// -------- Common --------
new Move("Weighted Step",   "Heavy plating slows the foe’s movement.",        MoveType.Slow, MoveRarity.Common,  -12f, "bronze", "stone", "terra", "basic", "inferno"),
new Move("Undercurrent",    "A subtle drag of water slows the target.",       MoveType.Slow, MoveRarity.Common,  -14f, "aqua", "void", "stone"),
new Move("Sticky Mud",      "Bog the enemy down with mud.",                   MoveType.Slow, MoveRarity.Common,  -13f, "terra", "aqua"),
new Move("Frost Chill",     "A numbing cold slows reactions.",                MoveType.Slow, MoveRarity.Common,  -12f, "frost", "stone"),
new Move("Rust Creep",      "Rust spreads, slowing movements.",               MoveType.Slow, MoveRarity.Common,  -11f, "rust", "bronze"),
new Move("Dull Air",        "A heavy stillness makes actions sluggish.",      MoveType.Slow, MoveRarity.Common,  -10f, "basic", "radiant"),

// -------- Rare --------
new Move("Rootbind",        "Entangle the foe, drastically slowing them.",    MoveType.Slow, MoveRarity.Rare,    -18f, "terra", "aqua"),
new Move("Earthen Grasp",   "Roots seize the enemy and drag them down.",      MoveType.Slow, MoveRarity.Rare,    -19f, "terra", "stone", "rust"),
new Move("Icy Shackles",    "Chains of frost slow the enemy sharply.",        MoveType.Slow, MoveRarity.Rare,    -18f, "frost", "void"),
new Move("Storm Drag",      "Wind shear hampers enemy movements.",            MoveType.Slow, MoveRarity.Rare,    -17f, "storm", "inferno"),

// -------- Epic --------
new Move("Permafrost Bind", "Frozen ground locks the foe in place.",          MoveType.Slow, MoveRarity.Epic,    -22f, "frost", "terra", "stone"),
new Move("Entropic Slow",   "Void energies erode quickness.",                 MoveType.Slow, MoveRarity.Epic,    -23f, "void", "rust", "radiant"),
new Move("Lava Shackles",   "Molten chains restrict motion severely.",        MoveType.Slow, MoveRarity.Epic,    -22f, "inferno", "terra"),
new Move("Crashing Wave",   "A powerful undertow slows the target greatly.",  MoveType.Slow, MoveRarity.Epic,    -21f, "aqua", "storm"),

// -------- Legendary --------
new Move("Time Fracture",   "Reality warps and halts the foe’s speed.",       MoveType.Slow, MoveRarity.Legendary,-28f, "void", "radiant"),
new Move("Glacial Prison",  "A tomb of ice nearly stops the foe cold.",       MoveType.Slow, MoveRarity.Legendary,-26f, "frost", "aqua", "terra"),

// ===================== WEAKEN ====================
// -------- Common --------
new Move("Oxidize",        "Spread corrosion that saps enemy strength.",       MoveType.Weaken, MoveRarity.Common,    11f, "rust", "bronze"),
new Move("Corrode",        "Erode plating; slightly reduces enemy offense.",   MoveType.Weaken, MoveRarity.Common,    11f, "rust", "bronze", "stone"),
new Move("Dust Blind",     "Grit clouds reduce the foe's effectiveness.",      MoveType.Weaken, MoveRarity.Common,    12f, "terra", "stone", "rust"),
new Move("Chill Draft",    "A numbing breeze dulls striking power.",           MoveType.Weaken, MoveRarity.Common,    13f, "frost", "stone", "basic"),
new Move("Static Field",   "Crackling air disrupts the foe's power.",          MoveType.Weaken, MoveRarity.Common,    14f, "storm", "bronze"),
new Move("Dampen",         "Saturate and soften, muting force slightly.",      MoveType.Weaken, MoveRarity.Common,    14f, "aqua", "basic", "radiant"),


// -------- Rare --------
new Move("Guard Break",    "Target weak points to soften defenses.",           MoveType.Weaken, MoveRarity.Rare,      18f, "basic", "inferno", "storm", "void"),
new Move("Ember Trap",     "Lingering embers weaken the next attacks.",        MoveType.Weaken, MoveRarity.Rare,      19f, "inferno", "rust"),
new Move("Chilling Miasma","Icy fog seeps in, weakening enemy strikes.",       MoveType.Weaken, MoveRarity.Rare,      18f, "frost", "void", "aqua"),
new Move("Undertow",       "A dragging pull that saps offensive drive.",       MoveType.Weaken, MoveRarity.Rare,      18f, "aqua", "void"),

// -------- Epic --------
new Move("Permafrost",     "Locking cold suppresses the foe's power.",         MoveType.Weaken, MoveRarity.Epic,      22f, "frost", "void"),
new Move("Null Pulse",     "A null shock dampens hostile force.",              MoveType.Weaken, MoveRarity.Epic,      23f, "void", "frost", "radiant"),
new Move("Sunder Armor",   "Crush guard and reduce future damage dealt.",      MoveType.Weaken, MoveRarity.Epic,      22f, "terra", "stone", "bronze"),
new Move("Quell",          "A radiant hush that subdues aggression.",          MoveType.Weaken, MoveRarity.Epic,      24f, "radiant", "aqua"),

// -------- Legendary --------
new Move("Null Field",     "A field of nothingness that smothers power.",      MoveType.Weaken, MoveRarity.Legendary, 28f, "void", "frost"),
new Move("Eclipse Hex",    "Half-light curse that heavily weakens attacks.",   MoveType.Weaken, MoveRarity.Legendary, 26f, "void", "radiant", "inferno"),

// ===================== POSION =====================
// -------- Common --------
new Move("Vile Drip",         "Persistent droplets of toxin keep sapping health.",        MoveType.Poison, MoveRarity.Common, 10f, "basic", "void", "inferno"),
new Move("Bilge Water",       "Foul water carries toxins that linger.",                   MoveType.Poison, MoveRarity.Common, 11f, "aqua", "rust"),
new Move("Venom Needle",      "A quick stab that injects venom.",                         MoveType.Poison, MoveRarity.Common, 12f, "storm", "rust", "bronze"),
new Move("Gunk Spray",        "A filthy spray that sticks and saps health.",              MoveType.Poison, MoveRarity.Common, 13f, "basic", "rust", "aqua"),
new Move("Envenom",           "Inflict a lingering toxin.",                               MoveType.Poison, MoveRarity.Common, 14f, "void", "rust", "aqua", "radiant"),
new Move("Toxic Mist",        "A choking cloud that poisons over time.",                  MoveType.Poison, MoveRarity.Common, 14f, "aqua", "void", "rust", "basic"),

// -------- Rare --------
new Move("Acid Rain",         "Irritating acidic droplets that persist.",                 MoveType.Poison, MoveRarity.Rare,   16f, "aqua", "frost"),
new Move("Blight",            "Wither the target with creeping blight.",                  MoveType.Poison, MoveRarity.Rare,   16f, "terra", "void", "frost"),
new Move("Septic Slash",      "A filthy cut that festers badly.",                         MoveType.Poison, MoveRarity.Rare,   18f, "stone", "rust", "void", "storm", "aqua", "frost"),
new Move("Smoldering Toxin",  "Heat-activated toxin burns from within.",                  MoveType.Poison, MoveRarity.Rare,   19f, "inferno", "void", "storm"),

// -------- Epic --------
new Move("Corrosive Cloud",   "A lingering corrosive haze that eats away at foes.",       MoveType.Poison, MoveRarity.Epic,   22f, "rust", "void", "bronze"),
new Move("Shadow Venin",      "Void-tainted venom that gnaws at vitality.",               MoveType.Poison, MoveRarity.Epic,   22f, "void", "inferno"),
new Move("Ignite",            "Set the foe ablaze, dealing damage over time.",            MoveType.Poison, MoveRarity.Epic,   22f, "inferno", "void"),
new Move("Virulent Surge",    "Highly concentrated toxin for severe DOT.",                MoveType.Poison, MoveRarity.Epic,   23f, "storm", "void", "radiant"),

// -------- Legendary --------
new Move("Neurotoxin",        "A potent nerve agent; heavy damage over time.",            MoveType.Poison, MoveRarity.Legendary, 25f, "void", "storm", "radiant"),
new Move("Miasma Veil",       "A dense, toxic shroud that relentlessly drains.",          MoveType.Poison, MoveRarity.Legendary, 28f, "void", "rust", "terra"),
    };

    public static Move GetRandomMove(string variant, EntityData.Type type, int moveInd)
    {
        var rng = new System.Random();

        var allMovesForVariant = Pool
            .Where(m => m.Variants.Contains(variant) || m.Variants.Contains("basic"))
            .ToList();

        var attacks = allMovesForVariant.Where(m => m.Type == MoveType.Attack).ToList();
        var specifics = allMovesForVariant.Where(m => m.Type == GetPreferredType(type)).ToList();
        var randoms = allMovesForVariant;

        int roll = Random.Range(0, 100);

        List<Move> candidates;
        if (moveInd == 0)
        {
            candidates = attacks;
        }
        else if (roll < 80 && specifics.Count > 0)
        {
            candidates = specifics;
        }
        else
        {
            candidates = randoms;
        }

        if (candidates.Count == 0)
        {
            if (attacks.Count > 0) candidates = attacks;
            else candidates = allMovesForVariant;
        }

        var rarity = PickRarityWithAvailability(candidates, rng, moveInd);

        var sameRarity = candidates.Where(m => m.Rarity == rarity).ToList();
        if (sameRarity.Count == 0) sameRarity = candidates;

        return sameRarity[rng.Next(0, sameRarity.Count)];
    }

    private static MoveType GetPreferredType(EntityData.Type type)
    {
        return type switch
        {
            EntityData.Type.Pawn => MoveType.Evasion,
            EntityData.Type.Knight => MoveType.Defense,
            EntityData.Type.Bishop => MoveType.Poison,
            EntityData.Type.Rook => MoveType.Heal,
            EntityData.Type.Queen => MoveType.Slow,
            EntityData.Type.King => MoveType.Weaken,
            _ => MoveType.Attack
        };
    }
    public static List<Dictionary<MoveRarity, double>> baseWeights = new() {
    new()
    {
        { MoveRarity.Common,     0.70 },
        { MoveRarity.Rare,       0.20 },
        { MoveRarity.Epic,       0.07 },
        { MoveRarity.Legendary,  0.03 }
    },
    new()
    {
        { MoveRarity.Common,     0.55 },
        { MoveRarity.Rare,       0.25 },
        { MoveRarity.Epic,       0.13 },
        { MoveRarity.Legendary,  0.07 }
    },
    new()
    {
        { MoveRarity.Common,     0.40 },
        { MoveRarity.Rare,       0.30 },
        { MoveRarity.Epic,       0.20 },
        { MoveRarity.Legendary,  0.10 }
    },
    new()
    {
        { MoveRarity.Common,     0.25 },
        { MoveRarity.Rare,       0.30 },
        { MoveRarity.Epic,       0.27 },
        { MoveRarity.Legendary,  0.18 }
    },
    };

    private static MoveRarity PickRarityWithAvailability(List<Move> pool, System.Random rng, int moveInd = 0)
    {       
        var available = baseWeights[moveInd]
            .Where(kv => pool.Any(m => m.Rarity == kv.Key))
            .ToDictionary(kv => kv.Key, kv => kv.Value);

        double sum = available.Values.Sum();
        var norm = available.ToDictionary(kv => kv.Key, kv => kv.Value / sum);

        double roll = rng.NextDouble();
        double acc = 0;
        foreach (var kv in norm)
        {
            acc += kv.Value;
            if (roll <= acc) return kv.Key;
        }

        return norm.OrderByDescending(x => x.Value).First().Key;
    }
    public List<Move> VisualizeMoves = new();

    private void Awake()
    {
        VisualizeMoves = Pool;
    }
}
