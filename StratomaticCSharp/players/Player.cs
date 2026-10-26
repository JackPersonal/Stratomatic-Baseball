using System;

public class Player
{
    public string Name { get; set; }
    public double Era { get; set; } = 4.00;
    public OutcomeWeights BaseWeights { get; set; }

    public Player(string name, OutcomeWeights baseWeights)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
        BaseWeights = baseWeights ?? throw new ArgumentNullException(nameof(baseWeights));
    }

    public Player(string name, double era)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Era = era;
        BaseWeights = new OutcomeWeights(new[]
        {
            new OutcomeWeight(ABResult.Out, 1.0)
        });
    }

    public static OutcomeWeights BuildBaseWeights(double avg, double obp)
    {
        avg = Math.Clamp(avg, 0.0, 1.0);
        obp = Math.Clamp(obp, avg, 1.0);

        const double outWeight = 1000.0;
        double hitWeight = avg <= 0.0 ? 0.0 : outWeight * avg / (1.0 - avg);
        double totalOnBaseWeight = obp <= 0.0 ? 0.0 : outWeight * obp / (1.0 - obp);
        double walkWeight = Math.Max(0.0, totalOnBaseWeight - hitWeight);

        double singleWeight = hitWeight * 0.55;
        double doubleWeight = hitWeight * 0.18;
        double tripleWeight = hitWeight * 0.02;
        double homeRunWeight = Math.Max(0.0, hitWeight - singleWeight - doubleWeight - tripleWeight);

        return new OutcomeWeights(new[]
        {
            new OutcomeWeight(ABResult.Single, singleWeight),
            new OutcomeWeight(ABResult.Double, doubleWeight),
            new OutcomeWeight(ABResult.Triple, tripleWeight),
            new OutcomeWeight(ABResult.HomeRun, homeRunWeight),
            new OutcomeWeight(ABResult.Walk, walkWeight),
            new OutcomeWeight(ABResult.Out, outWeight),
        });
    }
}

public class Batter : Player
{
    public Batter(string name, double avg, double obp)
        : base(name, BuildBaseWeights(avg, obp))
    {
    }
}

public class Pitcher : Player
{
    public Pitcher(string name, double era)
        : base(name, era)
    {
    }
}
