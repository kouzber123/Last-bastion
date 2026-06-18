namespace startup.Scripts.Common.Classes
{
    public interface IStatus
    {
        int Mana { get; set; }
        int Strength { get; set; }
        int Agility { get; set; }
        int Stamina { get; set; }
    }
}