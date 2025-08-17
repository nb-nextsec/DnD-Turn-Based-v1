using System.Collections.Generic;

public class WarlockSpells : ISpellProvider
{
    public ClassId Class => ClassId.Warlock;

    public IEnumerable<KnownSpell> GetLowLevelSpellList() => new[]
    {
        // 0
        new KnownSpell("eldritch-blast", 0),
        // 1
        new KnownSpell("hex", 1),              // add to SpellDB when you want the debuff mechanic
        new KnownSpell("witch-bolt", 1),
        // 2
        new KnownSpell("shatter", 2),
        // 3
        new KnownSpell("counterspell", 3),
    };
}
