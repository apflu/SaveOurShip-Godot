using Godot;
using System;

public partial class ComboSet : Node
{
    [Export] public Godot.Collections.Array<BaseCombo> Combos { get; set; }

    public BaseCombo GetComboAtIndex(int index)
    {
        if (index >= 0 && index < Combos.Count)
        {
            return Combos[index];
        }
        return null; // 或者抛出错误
    }

    public bool HasNextCombo(int currentIndex)
    {
        return currentIndex + 1 < Combos.Count;
    }
}
