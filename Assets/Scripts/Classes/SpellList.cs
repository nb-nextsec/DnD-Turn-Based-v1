using System.Collections.Generic;

public struct KnownSpell
{
    public string id;   // matches key in SpellDB.All
    public int level;   // 0 = cantrip, 1..9 = level
    public KnownSpell(string id, int level) { this.id = id; this.level = level; }
}

public interface ISpellProvider
{
    ClassId Class { get; }
    // Return "known/prepared" low-level combat-relevant spells (0–3) for this class
    IEnumerable<KnownSpell> GetLowLevelSpellList();
}

// Single entry point to fetch a class' list without reflection
public static class Spellbooks
{
    public static IEnumerable<KnownSpell> For(ClassId cls)
    {
        switch (cls)
        {
            case ClassId.Bard: return new BardSpells().GetLowLevelSpellList();
            case ClassId.Cleric: return new ClericSpells().GetLowLevelSpellList();
            case ClassId.Druid: return new DruidSpells().GetLowLevelSpellList();
            case ClassId.Fighter: return new FighterSpells().GetLowLevelSpellList();   // Eldritch Knight archetype focus
            case ClassId.Paladin: return new PaladinSpells().GetLowLevelSpellList();
            case ClassId.Ranger: return new RangerSpells().GetLowLevelSpellList();
            case ClassId.Rogue: return new RogueSpells().GetLowLevelSpellList();     // Arcane Trickster archetype focus
            case ClassId.Sorcerer: return new SorcererSpells().GetLowLevelSpellList();
            case ClassId.Warlock: return new WarlockSpells().GetLowLevelSpellList();
            case ClassId.Wizard: return new WizardSpells().GetLowLevelSpellList();
            case ClassId.Artificer: return new ArtificerSpells().GetLowLevelSpellList();
            case ClassId.Barbarian: return new BarbarianSpells().GetLowLevelSpellList(); // empty
            case ClassId.Monk: return new MonkSpells().GetLowLevelSpellList();      // empty
            default: return System.Array.Empty<KnownSpell>();
        }
    }
}
