public class AtBatSimulatorTests
{
    private const int NumABs = 10000;

    // Tolerance for statistical assertions — 10000 ABs reduces variance,
    // so the result is much more stable while still matching the expected value.
    private const double Tolerance = 0.05;

    [Fact]
    public void SimulatedStats_ShouldApproximateInputStats()
    {
        double expectedAvg = 0.254;
        double expectedObp = 0.295;

        OutcomeWeights weights = BuildBaseWeights(expectedAvg, expectedObp);
        Player hg = new Player(hunterGoodman, weights);
        Player kf = new Player(kyleFreeland, era: 4.00);

        int numSingles = 0, numDoubles = 0, numTriples = 0, numHRs = 0;
        int numWalks = 0, numOuts = 0;

        for (int i = 0; i < NumABs; i++)
        {
            ABResult result = AtBatSimulator.SimulateAtBat(hg, kf);

            switch (result)
            {
                case ABResult.Single:         numSingles++; break;
                case ABResult.Double:         numDoubles++; break;
                case ABResult.Triple:         numTriples++; break;
                case ABResult.HomeRun:        numHRs++;     break;
                case ABResult.Walk:           numWalks++;   break;
                case ABResult.Out:            numOuts++;    break;
            }
        }

        double experimentalAvg = (double)(numSingles + numDoubles + numTriples + numHRs) / (NumABs - numWalks);
        double experimentalObp = (double)(numSingles + numDoubles + numTriples + numHRs + numWalks) / NumABs;

        Assert.InRange(experimentalAvg, expectedAvg - Tolerance, expectedAvg + Tolerance);
        Assert.InRange(experimentalObp, expectedObp - Tolerance, expectedObp + Tolerance);
    }
}