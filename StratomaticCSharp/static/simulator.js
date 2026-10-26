import { ABResult, Player } from './player.js';

export function simulateAtBat(batter) {
    return batter.baseWeights.roll();
}

export function runSimulation({ avg, obp, walkPct, plateAppearances, singlePct, doublePct, triplePct, hrPct }) {
    const expectedWalkPct = Number.isFinite(walkPct)
        ? walkPct
        : (Number.isFinite(obp) && Number.isFinite(avg) && (1 - avg) !== 0)
            ? (obp - avg) / (1 - avg)
            : NaN;

    const batter = {
        name: 'Hitter',
        baseWeights: Player.buildBaseWeights(avg, expectedWalkPct, singlePct, doublePct, triplePct, hrPct) //123: added expectedWalkPct
    };

    const counts = {
        [ABResult.Single]: 0,
        [ABResult.Double]: 0,
        [ABResult.Triple]: 0,
        [ABResult.HomeRun]: 0,
        [ABResult.Walk]: 0,
        [ABResult.Out]: 0
    };

    for (let i = 0; i < plateAppearances; i++) {
        const result = simulateAtBat(batter);
        counts[result] = (counts[result] || 0) + 1;
    }

    const hits = counts[ABResult.Single] + counts[ABResult.Double] + counts[ABResult.Triple] + counts[ABResult.HomeRun];
    const atBats = plateAppearances - counts[ABResult.Walk];
    const experimentalAvg = atBats > 0 ? hits / atBats : 0; //mlb uses ABs
    const experimentalObp = (hits + counts[ABResult.Walk]) / plateAppearances; //mlb uses PAs
    const experimentalSinglePct = counts[ABResult.Single] / atBats; //mlb uses ABs to calculate slg
    const experimentalDoublePct = counts[ABResult.Double] / atBats;
    const experimentalTriplePct = counts[ABResult.Triple] / atBats;
    const experimentalHrPct = counts[ABResult.HomeRun] / atBats;
    const experimentalWalkPct = counts[ABResult.Walk] / plateAppearances;
    

    const tolerance = 0.05;
    const avgPass = experimentalAvg >= avg - tolerance && experimentalAvg <= avg + tolerance;
    const obpPass = experimentalObp >= obp - tolerance && experimentalObp <= obp + tolerance;
    const singlePass = experimentalSinglePct >= singlePct - tolerance && experimentalSinglePct <= singlePct + tolerance;
    const doublePass = experimentalDoublePct >= doublePct - tolerance && experimentalDoublePct <= doublePct + tolerance;
    const triplePass = experimentalTriplePct >= triplePct - tolerance && experimentalTriplePct <= triplePct + tolerance;
    const hrPass = experimentalHrPct >= hrPct - tolerance && experimentalHrPct <= hrPct + tolerance;
    const walkPass = experimentalWalkPct >= expectedWalkPct - tolerance && experimentalWalkPct <= expectedWalkPct + tolerance; //123

    return {
        counts,
        experimentalAvg,
        experimentalObp,
        experimentalWalkPct,
        experimentalSinglePct,
        experimentalDoublePct,
        experimentalTriplePct,
        experimentalHrPct,
        expectedAvg: avg,
        expectedObp: avg + expectedWalkPct * (1 - avg),
        expectedWalkPct: expectedWalkPct, //123
        atBats,
        plateAppearances,
        avgPass,
        obpPass,
        singlePass,
        doublePass,
        triplePass,
        hrPass,
        walkPass,
        tolerance
    };
}
