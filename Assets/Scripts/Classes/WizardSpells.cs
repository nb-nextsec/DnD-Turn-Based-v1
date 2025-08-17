using System.Collections.Generic;

public class WizardSpells : ISpellProvider
{
    public ClassId Class => ClassId.Wizard;

    public IEnumerable<KnownSpell> GetLowLevelSpellList() => new[]
    {
        // 0
        new KnownSpell("fire-bolt", 0),
        new KnownSpell("acid-splash", 0),
        // 1
        new KnownSpell("magic-missile", 1),
        new KnownSpell("shield", 1),
        new KnownSpell("burning-hands", 1),
        // 2
        new KnownSpell("scorching-ray", 2),
        new KnownSpell("shatter", 2),
        // 3
        new KnownSpell("fireball", 3),
        new KnownSpell("counterspell", 3),
        new KnownSpell("lightning-bolt", 3),
    };
}
