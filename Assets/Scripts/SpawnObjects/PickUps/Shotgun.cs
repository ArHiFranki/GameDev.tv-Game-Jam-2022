public class Shotgun : PickUp
{
    protected override void PickUpAction(Player player)
    {
        player.EnableWeapon();
    }
}