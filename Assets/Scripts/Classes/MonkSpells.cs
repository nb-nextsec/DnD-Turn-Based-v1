using System.Collections.Generic;

public class MonkSpells : ISpellProvider
{
    public ClassId Class => ClassId.Monk;
    public IEnumerable<KnownSpell> GetLowLevelSpellList() => System.Array.Empty<KnownSpell>();
}
