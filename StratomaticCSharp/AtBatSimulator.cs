using System;
using System.Collections.Generic;
using System.Linq;

public readonly record struct OutcomeWeight(ABResult Outcome, double Weight);
public readonly record struct WeightModifier(ABResult Outcome, double Multiplier);

public class OutcomeWeights
{
    private readonly Dictionary<ABResult, double> _weights;

    public OutcomeWeights(IEnumerable<OutcomeWeight> outcomes)
    {
        _weights = outcomes.ToDictionary(o => o.Outcome, o => Math.Max(0.0, o.Weight));
    }

    public OutcomeWeights Apply(IEnumerable<WeightModifier> modifiers)
    {
        var modified = new Dictionary<ABResult, double>(_weights);

        foreach (var mod in modifiers)
        {
            if (modified.ContainsKey(mod.Outcome))
            {
                modified[mod.Outcome] = Math.Max(0.0, modified[mod.Outcome] * mod.Multiplier);
            }
        }

        return new OutcomeWeights(modified.Select(kv => new OutcomeWeight(kv.Key, kv.Value)));
    }

    public ABResult Roll()
    {
        double total = _weights.Values.Sum();
        if (total <= 0)
            return ABResult.Out;

        double roll = Random.Shared.NextDouble() * total;

        foreach (var (outcome, weight) in _weights)
        {
            if (weight <= 0)
                continue;

            roll -= weight;
            if (roll <= 0)
                return outcome;
        }

        return _weights.Keys.Last();
    }
}

public static class AtBatSimulator
{
    public static ABResult SimulateAtBat(Player batter, Player pitcher)
    {
        if (batter is null) throw new ArgumentNullException(nameof(batter));
        if (pitcher is null) throw new ArgumentNullException(nameof(pitcher));

        return batter.BaseWeights
            .Apply(PitcherModifiers.PitcherQuality(4.0 - pitcher.Era))
            .Roll();
    }

    public static ABResult SimulateAtBat(Player batter, Player pitcher, Strategy battingStrategy, Strategy pitchingStrategy)
    {
        return SimulateAtBat(batter, pitcher);
    }
}

public static class PitcherModifiers
{
    public static IEnumerable<WeightModifier> PitcherQuality(double eraDelta)
    {
        double outBoost = 1.0 + (eraDelta * 0.05);
        return new[]
        {
            new WeightModifier(ABResult.Out, outBoost),
            new WeightModifier(ABResult.Walk, 1.0 / Math.Max(0.01, outBoost)),
        };
    }
}
