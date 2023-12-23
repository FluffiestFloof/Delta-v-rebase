wikidump-reagent-name = {CAPITALIZE($name)}
wikidump-reagent-recipes-reagent = {$ratio} part {CAPITALIZE($reagent)}
wikidump-reagent-recipes-reagent-catalyst = {$ratio} part {CAPITALIZE($reagent)}<sup>(catalyst)</sup>
wikidump-reagent-recipes-product = {$ratio} part {CAPITALIZE($reagent)}
wikidump-reagent-effects-metabolism-rate = METABOLISMRATE: {$rate}
wikidump-reagent-effects-metabolism-group = {$group}
wikidump-reagent-recipes-mix = Mix
wikidump-reagent-recipes-mix-info = {$minTemp ->
    [0] {$hasMax ->
            [true] {$verb} below {$maxTemp}K
            *[false] {$verb}
        }
    *[other] {$verb} {$hasMax ->
            [true] between {$minTemp}K and {$maxTemp}K
            *[false] above {$minTemp}K
        }
}

# Missing reagent effects
reagent-effect-status-effect-PsionicallyInsulated = psionic insulation
reagent-effect-status-effect-PsionicsDisabled = inability to use psionic powers
reagent-effect-status-effect-SlurredSpeech = slurred speech

# Missing reagent names
reagent-name-nausium = nausium
reagent-name-prometheum = prometheum
