using System.Collections.Generic;

public class ClericSpells : ISpellProvider
{
    public ClassId Class => ClassId.Cleric;

    public IEnumerable<KnownSpell> GetLowLevelSpellList() => new[]
    {
        // 0
        new KnownSpell("sacred-flame", 0),
        new KnownSpell("thaumaturgy", 0),
        new KnownSpell("guidance", 0),
        // 1
        new KnownSpell("cure-wounds", 1),
        new KnownSpell("guiding-bolt", 1),
        new KnownSpell("bless", 1),
        // 2
        new KnownSpell("spiritual-weapon", 2),
        new KnownSpell("prayer-of-healing", 2),
        // 3
        new KnownSpell("revivify", 3),
    };
}
