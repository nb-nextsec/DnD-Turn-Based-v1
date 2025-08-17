using System.Collections.Generic;

public class BardSpells : ISpellProvider
{
    public ClassId Class => ClassId.Bard;

    public IEnumerable<KnownSpell> GetLowLevelSpellList() => new[]
    {
        // 0
        new KnownSpell("vicious-mockery", 0),
        new KnownSpell("ray-of-frost", 0),
        // 1
        new KnownSpell("healing-word", 1),
        new KnownSpell("bane", 1),
        new KnownSpell("thunderwave", 1),
        // 2
        new KnownSpell("shatter", 2),
        // 3
        new KnownSpell("fireball", 3), // via Magical Secrets feel

    };
}
