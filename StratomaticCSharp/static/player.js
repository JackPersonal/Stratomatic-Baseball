export const ABResult = {
    Out: 'Out',
    Single: 'Single',
    Double: 'Double',
    Triple: 'Triple',
    HomeRun: 'HomeRun',
    Walk: 'Walk'
};

export class OutcomeWeights {
    constructor(weights) {
        this.weights = new Map(weights.map(({ outcome, weight }) => [outcome, Math.max(0, weight)]));
    }

    roll() {
        const total = [...this.weights.values()].reduce((sum, value) => sum + value, 0);
        if (total <= 0) {
            return ABResult.Out;
        }

        let roll = Math.random() * total;
        for (const [outcome, weight] of this.weights.entries()) {
            if (weight <= 0) continue;
            roll -= weight;
            if (roll <= 0) {
                return outcome;
            }
        }

        return [...this.weights.keys()].pop();
    }
}

export class Player {
    constructor(name, baseWeights) {
        this.name = name;
        this.baseWeights = baseWeights;
    }

    //123: instead of obp as a param, walkPct is
    static buildBaseWeights(avg, walkPct, singlePct, doublePct, triplePct, hrPct) {
        avg = Math.min(Math.max(avg, 0), 1);
        walkPct = Math.min(Math.max(walkPct || 0, 0), 1);

        const outWeight = 1000;

        // singlePct, doublePct, triplePct, hrPct are AB-based fractions that should sum to avg
        const derivedAvg = singlePct + doublePct + triplePct + hrPct;
        if (Math.abs(derivedAvg - avg) > 0.001) {
            console.warn(`Hit type rates sum to ${derivedAvg.toFixed(3)}, expected ${avg}`);
        }

        //123: compleetly reworked weight calculations:
        // Convert AB-based hit rates to PA-based probabilities
        // PA = AB + BB (we ignore HBP/sf in this model)
        // fraction of PAs that are ABs = 1 - walkPct
        const abToPa = Math.max(0, 1 - walkPct);

        const pSingle = singlePct * abToPa;
        const pDouble = doublePct * abToPa;
        const pTriple = triplePct * abToPa;
        const pHr = hrPct * abToPa;
        const pWalk = walkPct;

        const pHits = pSingle + pDouble + pTriple + pHr;
        let pOut = 1 - (pHits + pWalk);
        if (pOut <= 0) {
            console.warn(`Computed non-positive out probability (${pOut}). Clamping to small positive value.`);
            pOut = 1e-6;
        }

        // Convert PA-based probabilities to outcome weights that are scaled relative to outs
        const singleWeight  = outWeight * (pSingle / pOut);
        const doubleWeight  = outWeight * (pDouble / pOut);
        const tripleWeight  = outWeight * (pTriple / pOut);
        const homeRunWeight = outWeight * (pHr / pOut);
        const walkWeight    = outWeight * (pWalk / pOut);

        return new OutcomeWeights([
            { outcome: ABResult.Single, weight: singleWeight },
            { outcome: ABResult.Double, weight: doubleWeight },
            { outcome: ABResult.Triple, weight: tripleWeight },
            { outcome: ABResult.HomeRun, weight: homeRunWeight },
            { outcome: ABResult.Walk, weight: walkWeight },
            { outcome: ABResult.Out, weight: outWeight }
        ]);
    }
}
