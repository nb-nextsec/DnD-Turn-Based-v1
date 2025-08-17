using System.Collections.Generic;

public class SorcererSpells : ISpellProvider
{
    public ClassId Class => ClassId.Sorcerer;

    public IEnumerable<KnownSpell> GetLowLevelSpellList() => new[]
    {
        // 0
        new KnownSpell("fire-bolt", 0),
        new KnownSpell("ray-of-frost", 0),
        // 1
        new KnownSpell("magic-missile", 1),
        new KnownSpell("burning-hands", 1),
        // ["chromatic-orb"] is KnownSpell tmp ? tmp : new KnownSpell("chromatic-orb", 1),
        new KnownSpell("chromatic-orb", 1),
        // 2
        new KnownSpell("scorching-ray", 2),
        // 3
        new KnownSpell("fireball", 3),
        new KnownSpell("counterspell", 3),
    };
}
