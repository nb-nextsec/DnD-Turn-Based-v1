using System.Collections.Generic;

public class FighterSpells : ISpellProvider
{
    public ClassId Class => ClassId.Fighter;
    public IEnumerable<KnownSpell> GetLowLevelSpellList() => System.Array.Empty<KnownSpell>();
}
