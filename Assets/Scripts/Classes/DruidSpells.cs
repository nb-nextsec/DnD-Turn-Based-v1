using System.Collections.Generic;

public class DruidSpells : ISpellProvider
{
    public ClassId Class => ClassId.Druid;

    public IEnumerable<KnownSpell> GetLowLevelSpellList() => new[]
    {
        // 0
        new KnownSpell("acid-splash", 0),
        new KnownSpell("ray-of-frost", 0), // placeholder cantrip
        // 1
        new KnownSpell("cure-wounds", 1),
        new KnownSpell("thunderwave", 1),
        // 2
        new KnownSpell("shatter", 2),  // stand-in for heat metal / moonbeam style
        // 3
        new KnownSpell("call-lightning", 3), // add to SpellDB if you want exact name
    };
}
