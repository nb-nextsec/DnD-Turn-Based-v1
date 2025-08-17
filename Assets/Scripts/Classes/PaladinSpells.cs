using System.Collections.Generic;

public class PaladinSpells : ISpellProvider
{
    public ClassId Class => ClassId.Paladin;

    public IEnumerable<KnownSpell> GetLowLevelSpellList() => new[]
    {
        // 1
        new KnownSpell("cure-wounds", 1),
        new KnownSpell("bless", 1),
        // 2
        new KnownSpell("spiritual-weapon", 2), // not strictly paladin; stand-in for smite-y strike
    };
}
