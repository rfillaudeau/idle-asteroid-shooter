using System;

[Serializable]
public class StatUpgrade
{
    public StatType type;
    public int cost;
    public int incrementalCost;
    public int value;
    public int maxValue;
    public bool hasMaxValue;
}
