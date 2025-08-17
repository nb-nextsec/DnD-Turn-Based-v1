using System.Collections.Generic;

public class ArtificerSpells : ISpellProvider
{
    public ClassId Class => ClassId.Artificer;

    public IEnumerable<KnownSpell> GetLowLevelSpellList() => new[]
    {
        // 0
        new KnownSpell("ray-of-frost", 0),
        // 1
        new KnownSpell("cure-wounds", 1),
        new KnownSpell("shield", 1),
        // 2
        new KnownSpell("shatter", 2),
    };
}
