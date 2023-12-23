health-change-display =
    { $deltasign ->
        [-1] [[DMG|{$kind}|+|{NATURALFIXED($amount, 2)}]]
        *[1] [[DMG|{$kind}|-|{NATURALFIXED($amount, 2)}]]
    }

health-change-display-soitdisplaystemporary =
    { $deltasign ->
        [-1] [color=green]{NATURALFIXED($amount, 2)}[/color] {$kind}
        *[1] [color=red]{NATURALFIXED($amount, 2)}[/color] {$kind}
    }
