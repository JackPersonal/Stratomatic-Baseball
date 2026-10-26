/*
executes a game simulation with the given strategies

game flow:
1. users select lineup and starting pitcher, then click "play game"
2. game simulates, showing play-by-play and box score, as well as live diamond visualization
3. users can adjust lineup and strategy during the game, which will affect the simulation results
4. users choose strategies before each AB, but the opposing user can't see it
*/

public class Gameplay
{
    public int inning { get; set; } = 1;
    public int outs { get; set; } = 0;
    public int homeScore { get; set; } = 0;
    public int awayScore { get; set; } = 0;
    public bool isTopOfInning { get; set; } = true;
    public static bool isOver = false;

    public void SimulateGame(User home, User away)
    {
        // simulate game using lineups of both teams
        Lineup homeLineup = home.SetLineup.SelectLineup();
        Lineup awayLineup = away.SetLineup.SelectLineup();
        // allow users to adjust lineup and strategy during the game
        // implement strategy selection before each at-bat
        // update the game state based on the result of each at-bat

        while (!Gameplay.isOver)
        {
            // for each AB, get strategy from each user, then simulate the AB using those strategies and the players involved
            // between ABs, allow users to make substitutions and change defensive positioning
            if(!isTopOfInning) // bottom of the inning
            {
                //home team is batting, away team is pitching
                Strategy homeStrategy = SelectStrategy(home);
                Strategy awayStrategy = SelectStrategy(away);
                SimulateAtBat(homeLineup.GetCurrentBatter(), awayLineup.GetCurrentPitcher(), homeStrategy, awayStrategy);

                //if home team hits the button to make a substitution, allow them to do so before the next at-bat
                Substitution.SubstitutePlayer(home, homeLineup.GetCurrentBatter(), homeLineup.GetNewBatter()); // pinch hit
                Substitution.SubstitutePlayer(home, homeLineup.GetSelectedRunner(), homeLineup.GetNewRunner()); // pinch run
                // if the away team hits the button to make a substitution, allow them to do so before the next at-bat
                Substitution.SubstitutePlayer(away, awayLineup.GetCurrentPitcher(), awayLineup.GetNewPitcher()); // pitching change
                Substitution.SubstitutePlayer(away, awayLineup.GetSelectedDefender(), awayLineup.GetNewDefender()); // defensive substitution
            }
            else //top of the inning
            {
                //away team is batting, home team is pitching
                Strategy awayStrategy = SelectStrategy(away);
                Strategy homeStrategy = SelectStrategy(home);
                SimulateAtBat(awayLineup.GetCurrentBatter(), homeLineup.GetCurrentPitcher(), awayStrategy, homeStrategy);

                //if the away team hits the button to make a substitution, allow them to do so before the next at-bat
                Substitution.SubstitutePlayer(away, awayLineup.GetCurrentBatter(), awayLineup.GetNewBatter()); // pinch hit
                Substitution.SubstitutePlayer(away, awayLineup.GetSelectedRunner(), awayLineup.GetNewRunner()); // pinch run
                // if the home team hits the button to make a substitution, allow them to do so before the next at-bat
                Substitution.SubstitutePlayer(home, homeLineup.GetCurrentPitcher(), homeLineup.GetNewPitcher()); // pitching change
                Substitution.SubstitutePlayer(home, homeLineup.GetSelectedDefender(), homeLineup.GetNewDefender()); // defensive substitution
            }

            if (IsGameOver())
            {
                //display final box score+stats
                GameDisplay.ShowFinalBoxScore(home, away);
                GameDisplay.ShowPlayerStats(home, away);
                break;
            }
        }
    }

    public Strategy SelectStrategy(User user)
    {
        // allow user to select strategy for the next at-bat
        return Strategy.None;
    }

    public void SimulateAtBat(Player batter, Player pitcher, Strategy battingStrategy, Strategy pitchingStrategy)
    {
        //simulate the AB using players and modifiers involved by generating a random outcome
        ABResult result = AtBatSimulator.SimulateAtBat(batter, pitcher);
        //update game state based on result of the AB
        UpdateGameState(result);
    }

    public bool IsGameOver()
    {
        // check if the game is over based on the current score and inning
        if(inning > 9 && homeScore != awayScore)
        {
            return true;
        }
        return false;
    }

    public void UpdateGameState(ABResult result)
    {
        // update the game state based on the result of each at-bat, including the score, inning, and player stats
        if(result == ABResult.Out)
        {
            outs++;
        }
        if(outs == 3)
        {
            //may not work w/ while loop 
            EndHalfInning();
        }
    }

    private void EndHalfInning()
    {
        outs = 0;

        if (!isTopOfInning)
        {
            inning++;
        }

        isTopOfInning = !isTopOfInning;
    }
}


