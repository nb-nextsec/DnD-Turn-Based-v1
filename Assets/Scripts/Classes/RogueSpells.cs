using System.Collections.Generic;

public class RogueSpells : ISpellProvider
{
    public ClassId Class => ClassId.Rogue;

    public IEnumerable<KnownSpell> GetLowLevelSpellList() => new[]
    {
        // 0
        new KnownSpell("shocking-grasp", 0),
        // 1
        new KnownSpell("shield", 1),
        new KnownSpell("magic-missile", 1),
        // 2
        new KnownSpell("shatter", 2),
    };
}
