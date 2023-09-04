namespace TECS.HDLSimulator.Chips.Chips;

internal class Clock
{
    public long ClockCounter { get; private set; }
    
    public bool Potential => ClockCounter % 2 == 0;
    
    public string Time => (ClockCounter / 2) + "" + (Potential ? "" : "+");
    
    private Clock()
    {
        ClockCounter = 0;
    }

    public static Clock Instance = new();

    public void Reset()
    {
        ClockCounter = 0;
    }
    
    public void Increment()
    {
        ClockCounter++;
    }
}