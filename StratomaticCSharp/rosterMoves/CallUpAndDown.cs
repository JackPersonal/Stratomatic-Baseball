/*
allows user to call up or down players from the minor leagues, updates roster and player cards accordingly
*/

public class CallUpAndDown
{
    // method to call up a player from the minor leagues
    public void CallUpPlayer(User user, Player player)
    {
        // check if player is in minor league roster
        if (user.MinorLeagueRoster.Contains(player))
        {
            // remove player from minor league roster and add to major league roster
            user.MinorLeagueRoster.Remove(player);
            user.MajorLeagueRoster.Add(player);
            // update player card to reflect major league status
            player.IsInMinorLeagues = false;
        }
    }

    // method to send a player down to the minor leagues
    public void SendDownPlayer(User user, Player player)
    {
        // check if player is in major league roster
        if (user.MajorLeagueRoster.Contains(player))
        {
            // remove player from major league roster and add to minor league roster
            user.MajorLeagueRoster.Remove(player);
            user.MinorLeagueRoster.Add(player);
            // update player card to reflect minor league status
            player.IsInMinorLeagues = true;
        }
    }
}