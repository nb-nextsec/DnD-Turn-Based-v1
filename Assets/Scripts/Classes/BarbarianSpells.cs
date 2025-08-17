using System.Collections.Generic;

public class BarbarianSpells : ISpellProvider
{
    public ClassId Class => ClassId.Barbarian;
    public IEnumerable<KnownSpell> GetLowLevelSpellList() => System.Array.Empty<KnownSpell>();
}
