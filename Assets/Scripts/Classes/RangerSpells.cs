using System.Collections.Generic;

public class RangerSpells : ISpellProvider
{
    public ClassId Class => ClassId.Ranger;

    public IEnumerable<KnownSpell> GetLowLevelSpellList() => new[]
    {
        // 1
        new KnownSpell("hail-of-thorns", 1),
        new KnownSpell("cure-wounds", 1),
        // 2
        new KnownSpell("scorching-ray", 2), // stand-in for spike growth / pass without trace vibe
    };
}
