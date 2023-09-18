namespace TECS.HDLSimulator.Chips.Chips;

internal class Clock
{
    private long ClockCounter { get; set; }
    
    public bool Potential => ClockCounter % 2 != 0;
    
    public string Time => (ClockCounter / 2) + "" + (Potential ? "+" : "");
    
    private Clock()
    {
        ClockCounter = 0;
    }

    public static readonly Clock Instance = new();

    public void Reset()
    {
        ClockCounter = 0;
    }
    
    public void Increment()
    {
        ClockCounter++;
    }
}