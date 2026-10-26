/*
upgrades player's attributes, adds traits as a result of a trainer card being applied on them
*/

public class UpgradePlayer
{
    public void ApplyTrainerCard(Player player, TrainerCard card)
    {
        // Example: If the trainer card is a "Power Hitter" card, increase the player's power attribute
        if (card.Type == TrainerCardType)
        {
            player.Attributes += card.AttributeBoost;
            player.Traits.Add(Traits.name);
        }
    }
}