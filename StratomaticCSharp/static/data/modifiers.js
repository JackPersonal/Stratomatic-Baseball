export const TRAIT_DEFINITIONS = {
  contactHitter: {
    name: 'Contact Hitter',
    averageBonus: 0.400
  },
  sluggerInstinct: {
    name: 'Slugger Instinct',
    sluggingBonus: 0.150
  },
  eagleEye: {
    name: 'Eagle Eye',
    walkPctBonus: 0.500
  }
};

export const hitterTraits = {
  'Hunter Goodman': ['sluggerInstinct'],
  'TJ Rumfield': ['eagleEye'],
  'Tyler Freeman': ['contactHitter'],
  'Zac Veen': ['contactHitter']
};

export function applyAverageBonus(player, averageBonus, traitNames = []) {
  const modifiedPlayer = { ...player };
  const hitRateBonus = averageBonus / 4;

  modifiedPlayer.avg += averageBonus;
  modifiedPlayer.single += hitRateBonus;
  modifiedPlayer.double += hitRateBonus;
  modifiedPlayer.triple += hitRateBonus;
  modifiedPlayer.hr += hitRateBonus;
  modifiedPlayer.obp = modifiedPlayer.avg + modifiedPlayer.walkPct * (1 - modifiedPlayer.avg);
  modifiedPlayer.traits = [...traitNames];

  return modifiedPlayer;
}

export function applySluggingBonus(player, sluggingBonus, traitNames = []) {
  const modifiedPlayer = { ...player };
  const hitRateBonus = sluggingBonus / 3;

  modifiedPlayer.double += hitRateBonus;
  modifiedPlayer.triple += hitRateBonus;
  modifiedPlayer.hr += hitRateBonus;
  modifiedPlayer.single -= sluggingBonus; // Reduce singles to keep total hit rate constant
  modifiedPlayer.obp = modifiedPlayer.avg + modifiedPlayer.walkPct * (1 - modifiedPlayer.avg); //i think you need this too

  modifiedPlayer.traits = [...traitNames];

  return modifiedPlayer;
}

export function applyWalkPctBonus(player, walkPctBonus, traitNames = []) {
  const modifiedPlayer = { ...player };
  modifiedPlayer.walkPct += walkPctBonus;
  modifiedPlayer.obp = modifiedPlayer.avg + modifiedPlayer.walkPct * (1 - modifiedPlayer.avg);

  modifiedPlayer.traits = [...traitNames];

  return modifiedPlayer;
}

export function applyHitterModifiers(player, traitNames = hitterTraits[player.name] || []) {
  const averageBonus = traitNames.reduce((total, traitName) => {
    return total + (TRAIT_DEFINITIONS[traitName]?.averageBonus || 0);
  }, 0);
  const sluggingBonus = traitNames.reduce((total, traitName) => {
    return total + (TRAIT_DEFINITIONS[traitName]?.sluggingBonus || 0);
  }, 0);
  const walkPctBonus = traitNames.reduce((total, traitName) => {
    return total + (TRAIT_DEFINITIONS[traitName]?.walkPctBonus || 0);
  }, 0);

  let modified = applyAverageBonus(player, averageBonus, traitNames);
  modified = applySluggingBonus(modified, sluggingBonus, traitNames);
  modified = applyWalkPctBonus(modified, walkPctBonus, traitNames);

  return modified;
}