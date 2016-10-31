Imports System.Windows.Resources
Imports System.Collections.ObjectModel
Imports System.Globalization

Module modMain
    Public objDataStream As StreamResourceInfo

    Public _Xpokedata As New Dictionary(Of String, PokeData)
    Public _Xmovedata As New Dictionary(Of String, Move)

    Public arrTypes(18) As String
    Public arrAbilities(166) As String
    Public arrNatures(24) As String
    Public colItems As New Collection(Of String)

    Public colSpHitDefMoves(2) As String
    Public colPunchMoves(14) As String
    Public colRecoilMoves(11) As String
    Public colSpeedLowerItems(7) As String

    Public arrTypeItems(,) As String
    Public Eff(,) As Double
    Public arrPokeItemFormes(,) As String
    Public arrImmuneAbilities(,) As Integer

    Public Delegate Sub GetMainPageData()
    Public _pGetMainPageData As GetMainPageData

#Region "Data"
    Public Sub InitData()
        Eff = New Double(18, 18) {{1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1}, _
                                  {1, 1, 2, 1, 1, 1, 1, 1, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1}, _
                                  {1, 1, 1, 2, 1, 1, 0.5, 0.5, 1, 1, 1, 1, 1, 1, 2, 1, 1, 0.5, 1}, _
                                  {1, 1, 0.5, 1, 1, 0, 2, 0.5, 1, 1, 1, 1, 0.5, 2, 1, 2, 1, 1, 1}, _
                                  {1, 1, 0.5, 1, 0.5, 2, 1, 0.5, 1, 1, 1, 1, 0.5, 1, 2, 1, 1, 1, 1}, _
                                  {1, 1, 1, 1, 0.5, 1, 0.5, 1, 1, 1, 1, 2, 2, 0, 1, 2, 1, 1, 1}, _
                                  {1, 0.5, 2, 0.5, 0.5, 2, 1, 1, 1, 2, 0.5, 2, 2, 1, 1, 1, 1, 1, 1}, _
                                  {1, 1, 0.5, 2, 1, 0.5, 2, 1, 1, 1, 2, 1, 0.5, 1, 1, 1, 1, 1, 1}, _
                                  {1, 0, 0, 1, 0.5, 1, 1, 0.5, 2, 1, 1, 1, 1, 1, 1, 1, 1, 2, 1}, _
                                  {1, 0.5, 2, 0.5, 0, 2, 0.5, 0.5, 0.5, 0.5, 2, 1, 0.5, 1, 0.5, 0.5, 0.5, 0.5, 1}, _
                                  {1, 1, 1, 1, 1, 2, 2, 0.5, 1, 0.5, 0.5, 2, 0.5, 1, 1, 0.5, 1, 1, 1}, _
                                  {1, 1, 1, 1, 1, 1, 1, 1, 1, 0.5, 0.5, 0.5, 2, 2, 1, 0.5, 1, 1, 1}, _
                                  {1, 1, 1, 2, 2, 0.5, 1, 2, 1, 1, 2, 0.5, 0.5, 0.5, 1, 2, 1, 1, 1}, _
                                  {1, 1, 1, 0.5, 1, 2, 1, 1, 1, 0.5, 1, 1, 1, 0.5, 1, 1, 1, 1, 1}, _
                                  {1, 1, 0.5, 1, 1, 1, 1, 2, 2, 1, 1, 1, 1, 1, 0.5, 1, 1, 2, 1}, _
                                  {1, 1, 2, 1, 1, 1, 2, 1, 1, 2, 2, 1, 1, 1, 1, 0.5, 1, 1, 1}, _
                                  {1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0.5, 0.5, 0.5, 0.5, 1, 2, 2, 1, 1}, _
                                  {1, 1, 2, 1, 1, 1, 1, 2, 0.5, 1, 1, 1, 1, 1, 0, 1, 1, 0.5, 1}, _
                                  {1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1}}

        arrTypes(0) = " "
        arrTypes(1) = "Normal"
        arrTypes(2) = "Fighting"
        arrTypes(3) = "Flying"
        arrTypes(4) = "Poison"
        arrTypes(5) = "Ground"
        arrTypes(6) = "Rock"
        arrTypes(7) = "Bug"
        arrTypes(8) = "Ghost"
        arrTypes(9) = "Steel"
        arrTypes(10) = "Fire"
        arrTypes(11) = "Water"
        arrTypes(12) = "Grass"
        arrTypes(13) = "Electric"
        arrTypes(14) = "Psychic"
        arrTypes(15) = "Ice"
        arrTypes(16) = "Dragon"
        arrTypes(17) = "Dark"
        arrTypes(18) = "???"

        arrTypeItems = New String(18, 3) _
            {{" ", " ", " ", " "}, _
             {"Chilan Berry", "Silk Scarf", " ", "Normal Gem"}, _
             {"Chople Berry", "Black Belt", "Fist Plate", "Fighting Gem"}, _
             {"Coba Berry", "Sharp Beak", "Sky Plate", "Flying Gem"}, _
             {"Kebia Berry", "Poison Barb", "Toxic Plate", "Poison Gem"}, _
             {"Shuca Berry", "Soft Sand", "Earth Plate", "Ground Gem"}, _
             {"Charti Berry", "Hard Stone", "Stone Plate", "Rock Gem"}, _
             {"Tanga Berry", "SilverPowder", "Insect Plate", "Bug Gem"}, _
             {"Kasib Berry", "Spell Tag", "Spooky Plate", "Ghost Gem"}, _
             {"Babiri Berry", "Metal Coat", "Iron Plate", "Steel Gem"}, _
             {"Occa Berry", "Charcoal", "Flame Plate", "Fire Gem"}, _
             {"Passho Berry", "Mystic Water", "Splash Plate", "Water Gem"}, _
             {"Rindo Berry", "Miracle Seed", "Meadow Plate", "Grass Gem"}, _
             {"Wacan Berry", "Magnet", "Zap Plate", "Electric Gem"}, _
             {"Payapa Berry", "TwistedSpoon", "Mind Plate", "Psychic Gem"}, _
             {"Yache Berry", "NeverMeltIce", "Icicle Plate", "Ice Gem"}, _
             {"Haban Berry", "Dragon Fang", "Draco Plate", "Dragon Gem"}, _
             {"Colbur Berry", "BlackGlasses", "Dread Plate", "Dark Gem"}, _
             {" ", " ", " ", " "}}
        arrPokeItemFormes = New String(16, 1) {{"Giratina-O", "Griseous Orb"}, _
                                               {"Arceus-Fighting", "Fist Plate"}, _
                                               {"Arceus-Flying", "Sky Plate"}, _
                                              {"Arceus-Poison", "Toxic Plate"}, _
                                              {"Arceus-Ground", "Earth Plate"}, _
                                               {"Arceus-Rock", "Stone Plate"}, _
                                               {"Arceus-Bug", "Insect Plate"}, _
                                               {"Arceus-Ghost", "Spooky Plate"}, _
                                               {"Arceus-Steel", "Iron Plate"}, _
                                               {"Arceus-Fire", "Flame Plate"}, _
                                               {"Arceus-Water", "Splash Plate"}, _
                                               {"Arceus-Grass", "Meadow Plate"}, _
                                               {"Arceus-Electric", "Zap Plate"}, _
                                               {"Arceus-Psychic", "Mind Plate"}, _
                                                {"Arceus-Ice", "Icicle Plate"}, _
                                                {"Arceus-Dragon", "Draco Plate"}, _
                                                {"Arceus-Dark", "Dread Plate"}}

        arrAbilities(0) = " "
        arrAbilities(1) = "Stench"
        arrAbilities(2) = "Drizzle"
        arrAbilities(3) = "Speed Boost"
        arrAbilities(4) = "Battle Armor"
        arrAbilities(5) = "Sturdy"
        arrAbilities(6) = "Damp"
        arrAbilities(7) = "Limber"
        arrAbilities(8) = "Sand Veil"
        arrAbilities(9) = "Static"
        arrAbilities(10) = "Volt Absorb"
        arrAbilities(11) = "Water Absorb"
        arrAbilities(12) = "Oblivious"
        arrAbilities(13) = "Cloud Nine"
        arrAbilities(14) = "Compound Eyes"
        arrAbilities(15) = "Insomnia"
        arrAbilities(16) = "Color Change"
        arrAbilities(17) = "Immunity"
        arrAbilities(18) = "Flash Fire"
        arrAbilities(19) = "Shield Dust"
        arrAbilities(20) = "Own Tempo"
        arrAbilities(21) = "Suction Cups"
        arrAbilities(22) = "Intimidate"
        arrAbilities(23) = "Shadow Tag"
        arrAbilities(24) = "Rough Skin"
        arrAbilities(25) = "Wonder Guard"
        arrAbilities(26) = "Levitate"
        arrAbilities(27) = "Effect Spore"
        arrAbilities(28) = "Synchronize"
        arrAbilities(29) = "Clear Body"
        arrAbilities(30) = "Natural Cure"
        arrAbilities(31) = "Lightningrod"
        arrAbilities(32) = "Serene Grace"
        arrAbilities(33) = "Swift Swim"
        arrAbilities(34) = "Chlorophyll"
        arrAbilities(35) = "Illuminate"
        arrAbilities(36) = "Trace"
        arrAbilities(37) = "Huge Power"
        arrAbilities(38) = "Poison Point"
        arrAbilities(39) = "Inner Focus"
        arrAbilities(40) = "Magma Armor"
        arrAbilities(41) = "Water Veil"
        arrAbilities(42) = "Magnet Pull"
        arrAbilities(43) = "Soundproof"
        arrAbilities(44) = "Rain Dish"
        arrAbilities(45) = "Sand Stream"
        arrAbilities(46) = "Pressure"
        arrAbilities(47) = "Thick Fat"
        arrAbilities(48) = "Early Bird"
        arrAbilities(49) = "Flame Body"
        arrAbilities(50) = "Run Away"
        arrAbilities(51) = "Keen Eye"
        arrAbilities(52) = "Hyper Cutter"
        arrAbilities(53) = "Pick Up"
        arrAbilities(54) = "Truant"
        arrAbilities(55) = "Hustle"
        arrAbilities(56) = "Cute Charm"
        arrAbilities(57) = "Plus"
        arrAbilities(58) = "Minus"
        arrAbilities(59) = "Forecast"
        arrAbilities(60) = "Sticky Hold"
        arrAbilities(61) = "Shed Skin"
        arrAbilities(62) = "Guts"
        arrAbilities(63) = "Marvel Scale"
        arrAbilities(64) = "Liquid Ooze"
        arrAbilities(65) = "Overgrow"
        arrAbilities(66) = "Blaze"
        arrAbilities(67) = "Torrent"
        arrAbilities(68) = "Swarm"
        arrAbilities(69) = "Rock Head"
        arrAbilities(70) = "Drought"
        arrAbilities(71) = "Arena Trap"
        arrAbilities(72) = "Vital Spirit"
        arrAbilities(73) = "White Smoke"
        arrAbilities(74) = "Pure Power"
        arrAbilities(75) = "Shell Armor"
        arrAbilities(76) = "Air Lock"
        arrAbilities(77) = "Tangled Feet"
        arrAbilities(78) = "Motor Drive"
        arrAbilities(79) = "Rivalry"
        arrAbilities(80) = "Steadfast"
        arrAbilities(81) = "Snow Cloak"
        arrAbilities(82) = "Gluttony"
        arrAbilities(83) = "Anger Point"
        arrAbilities(84) = "Unburden"
        arrAbilities(85) = "Heatproof"
        arrAbilities(86) = "Simple"
        arrAbilities(87) = "Dry Skin"
        arrAbilities(88) = "Download"
        arrAbilities(89) = "Iron Fist"
        arrAbilities(90) = "Poison Heal"
        arrAbilities(91) = "Adaptability"
        arrAbilities(92) = "Skill Link"
        arrAbilities(93) = "Hydration"
        arrAbilities(94) = "Solar Power"
        arrAbilities(95) = "Quick Feet"
        arrAbilities(96) = "Normalize"
        arrAbilities(97) = "Sniper"
        arrAbilities(98) = "Magic Guard"
        arrAbilities(99) = "No Guard"
        arrAbilities(100) = "Stall"
        arrAbilities(101) = "Technician"
        arrAbilities(102) = "Leaf Guard"
        arrAbilities(103) = "Klutz"
        arrAbilities(104) = "Mold Breaker"
        arrAbilities(105) = "Super Luck"
        arrAbilities(106) = "Aftermath"
        arrAbilities(107) = "Anticipation"
        arrAbilities(108) = "Forewarn"
        arrAbilities(109) = "Unaware"
        arrAbilities(110) = "Tinted Lens"
        arrAbilities(111) = "Filter"
        arrAbilities(112) = "Slow Start"
        arrAbilities(113) = "Scrappy"
        arrAbilities(114) = "Storm Drain"
        arrAbilities(115) = "Ice Body"
        arrAbilities(116) = "Solid Rock"
        arrAbilities(117) = "Snow Warning"
        arrAbilities(118) = "Honey Gather"
        arrAbilities(119) = "Frisk"
        arrAbilities(120) = "Reckless"
        arrAbilities(121) = "Multitype"
        arrAbilities(122) = "Flower Gift"
        arrAbilities(123) = "Bad Dreams"
        arrAbilities(124) = "Pickpocket"
        arrAbilities(125) = "Sheer Force"
        arrAbilities(126) = "Contrary"
        arrAbilities(127) = "Unnerve"
        arrAbilities(128) = "Defiant"
        arrAbilities(129) = "Defeatist"
        arrAbilities(130) = "Cursed Body"
        arrAbilities(131) = "Healer"
        arrAbilities(132) = "Friend Guard"
        arrAbilities(133) = "Weak Armor"
        arrAbilities(134) = "Heavy Metal"
        arrAbilities(135) = "Light Metal"
        arrAbilities(136) = "Multiscale"
        arrAbilities(137) = "Toxic Boost"
        arrAbilities(138) = "Flare Boost"
        arrAbilities(139) = "Harvest"
        arrAbilities(140) = "Telepathy"
        arrAbilities(141) = "Moody"
        arrAbilities(142) = "Overcoat"
        arrAbilities(143) = "Poison Touch"
        arrAbilities(144) = "Regenerator"
        arrAbilities(145) = "Big Pecks"
        arrAbilities(146) = "Sand Rush"
        arrAbilities(147) = "Wonder Skin"
        arrAbilities(149) = "Illusion"
        arrAbilities(150) = "Imposter"
        arrAbilities(151) = "Infiltrator"
        arrAbilities(152) = "Mummy"
        arrAbilities(153) = "Moxie"
        arrAbilities(154) = "Justified"
        arrAbilities(155) = "Rattled"
        arrAbilities(156) = "Magic Bounce"
        arrAbilities(157) = "Sap Sipper"
        arrAbilities(158) = "Prankster"
        arrAbilities(159) = "Sand Force"
        arrAbilities(160) = "Iron Barbs"
        arrAbilities(161) = "Zen Mode"
        arrAbilities(162) = "Victory Star"
        arrAbilities(163) = "Turboblaze"
        arrAbilities(164) = "Teravolt"
        arrAbilities(165) = "Analytic"

        arrNatures(0) = "Hardy"
        arrNatures(1) = "Lonely"
        arrNatures(2) = "Brave"
        arrNatures(3) = "Adamant"
        arrNatures(4) = "Naughty"
        arrNatures(5) = "Bold"
        arrNatures(6) = "Docile"
        arrNatures(7) = "Relaxed"
        arrNatures(8) = "Impish"
        arrNatures(9) = "Lax"
        arrNatures(10) = "Timid"
        arrNatures(11) = "Hasty"
        arrNatures(12) = "Serious"
        arrNatures(13) = "Jolly"
        arrNatures(14) = "Naive"
        arrNatures(15) = "Modest"
        arrNatures(16) = "Mild"
        arrNatures(17) = "Quiet"
        arrNatures(18) = "Bashful"
        arrNatures(19) = "Rash"
        arrNatures(20) = "Calm"
        arrNatures(21) = "Gentle"
        arrNatures(22) = "Sassy"
        arrNatures(23) = "Careful"
        arrNatures(24) = "Quirky"

        With colItems
            .Add(" ")
            .Add("Choice Band")
            .Add("Choice Scarf")
            .Add("Choice Specs")
            .Add("Air Balloon")
            .Add("Big Root")
            .Add("Black Sludge")
            .Add("BrightPowder")
            .Add("Ring Target")
            .Add("Damp Rock")
            .Add("Destiny Knot")
            .Add("Eject Button")
            .Add("Eviolite")
            .Add("Expert Belt")
            .Add("Focus Band")
            .Add("Focus Sash")
            .Add("Grip Claw")
            .Add("Heat Rock")
            .Add("Icy Rock")
            .Add("King's Rock")
            .Add("Lax Incense")
            .Add("Leftovers")
            .Add("Life Orb")
            .Add("Light Clay")
            .Add("Mental Herb")
            .Add("Metronome")
            .Add("Muscle Band")
            .Add("Power Herb")
            .Add("Quick Claw")
            .Add("Binding Band")
            .Add("Razor Claw")
            .Add("Razor Fang")
            .Add("Red Card")
            .Add("Rocky Helmet")
            .Add("Scope Lens")
            .Add("Shed Shell")
            .Add("Shell Bell")
            .Add("Smoke Ball")
            .Add("Smooth Rock")
            .Add("White Herb")
            .Add("Wide Lens")
            .Add("Wise Glasses")
            .Add("Zoom Lens")
            .Add("Draco Plate")
            .Add("Dread Plate")
            .Add("Earth Plate")
            .Add("Fist Plate")
            .Add("Flame Plate")
            .Add("Icicle Plate")
            .Add("Insect Plate")
            .Add("Iron Plate")
            .Add("Meadow Plate")
            .Add("Mind Plate")
            .Add("Sky Plate")
            .Add("Splash Plate")
            .Add("Spooky Plate")
            .Add("Stone Plate")
            .Add("Toxic Plate")
            .Add("Zap Plate")
            .Add("Adamant Orb")
            .Add("DeepSeaScale")
            .Add("DeepSeaTooth")
            .Add("Light Ball")
            .Add("Lucky Punch")
            .Add("Lustrous Orb")
            .Add("Metal Powder")
            .Add("Quick Powder")
            .Add("Soul Dew")
            .Add("Stick")
            .Add("Thick Club")
            .Add("Griseous Orb")
            .Add("Douse Drive")
            .Add("Shock Drive")
            .Add("Burn Drive")
            .Add("Chill Drive")
            .Add("Black Belt")
            .Add("BlackGlasses")
            .Add("Charcoal")
            .Add("Dragon Fang")
            .Add("Hard Stone")
            .Add("Magnet")
            .Add("Metal Coat")
            .Add("Miracle Seed")
            .Add("Mystic Water")
            .Add("NeverMeltIce")
            .Add("Odd Incense")
            .Add("Poison Barb")
            .Add("Rock Incense")
            .Add("Rose Incense")
            .Add("Sea Incense")
            .Add("Sharp Beak")
            .Add("Silk Scarf")
            .Add("SilverPowder")
            .Add("Soft Sand")
            .Add("Spell Tag")
            .Add("TwistedSpoon")
            .Add("Wave Incense")
            .Add("Bug Gem")
            .Add("Dark Gem")
            .Add("Dragon Gem")
            .Add("Ground Gem")
            .Add("Electric Gem")
            .Add("Fighting Gem")
            .Add("Fire Gem")
            .Add("Flying Gem")
            .Add("Ghost Gem")
            .Add("Grass Gem")
            .Add("Ice Gem")
            .Add("Normal Gem")
            .Add("Poison Gem")
            .Add("Psychic Gem")
            .Add("Rock Gem")
            .Add("Steel Gem")
            .Add("Water Gem")
            .Add("Flame Orb")
            .Add("Full Incense")
            .Add("Iron Ball")
            .Add("Lagging Tail")
            .Add("Sticky Barb")
            .Add("Toxic Orb")
            .Add("Apicot Berry")
            .Add("Custap Berry")
            .Add("Ganlon Berry")
            .Add("Lansat Berry")
            .Add("Liechi Berry")
            .Add("Micle Berry")
            .Add("Petaya Berry")
            .Add("Salac Berry")
            .Add("Starf Berry")
            .Add("Aspear Berry")
            .Add("Cheri Berry")
            .Add("Chesto Berry")
            .Add("Leppa Berry")
            .Add("Lum Berry")
            .Add("Oran Berry")
            .Add("Pecha Berry")
            .Add("Persim Berry")
            .Add("Rawst Berry")
            .Add("Sitrus Berry")
            .Add("Occa Berry")
            .Add("Passho Berry")
            .Add("Wacan Berry")
            .Add("Rindo Berry")
            .Add("Yache Berry")
            .Add("Chople Berry")
            .Add("Kebia Berry")
            .Add("Shuca Berry")
            .Add("Coba Berry")
            .Add("Payapa Berry")
            .Add("Tanga Berry")
            .Add("Charti Berry")
            .Add("Kasib Berry")
            .Add("Haban Berry")
            .Add("Colbur Berry")
            .Add("Babiri Berry")
            .Add("Chilan Berry")
            .Add("Aguav Berry")
            .Add("Figy Berry")
            .Add("Iapapa Berry")
            .Add("Mago Berry")
            .Add("Wiki Berry")
            .Add("Amulet Coin")
            .Add("Relic Bracelet")
            .Add("Relic Copper")
            .Add("Relic Crown")
            .Add("Relic Gold")
            .Add("Relic Silver")
            .Add("Relic Statue")
            .Add("Relic Vase")
            .Add("Antidote")
            .Add("Armor Fossil")
            .Add("Awakening")
            .Add("Pretty Wing")
            .Add("Belue Berry")
            .Add("Berry Juice")
            .Add("Big Mushroom")
            .Add("Big Pearl")
            .Add("Black Flute")
            .Add("Blue Flute")
            .Add("Blue Scarf")
            .Add("Blue Shard")
            .Add("Bluk Berry")
            .Add("Burn Heal")
            .Add("Calcium")
            .Add("Carbos")
            .Add("Claw Fossil")
            .Add("Prism Scale")
            .Add("Cleanse Tag")
            .Add("Comet Piece")
            .Add("Cornn Berry")
            .Add("Damp Mulch")
            .Add("Dawn Stone")
            .Add("Deepseascale")
            .Add("Deepseatooth")
            .Add("Pass Orb")
            .Add("Dire Hit")
            .Add("Dome Fossil")
            .Add("Dragon Scale")
            .Add("Dubious Disc")
            .Add("Durin Berry")
            .Add("Dusk Stone")
            .Add("Electrizer")
            .Add("Elixir")
            .Add("Energy Root")
            .Add("EnergyPowder")
            .Add("Engima Berry")
            .Add("Escape Rope")
            .Add("Ether")
            .Add("Everstone")
            .Add("Exp. Share")
            .Add("Fire Stone")
            .Add("Fluffy Tail")
            .Add("BalmMushroom")
            .Add("Fresh Water")
            .Add("Full Heal")
            .Add("Full Restore")
            .Add("Gooey Mulch")
            .Add("Green Scarf")
            .Add("Green Shard")
            .Add("Grepa Berry")
            .Add("Growth Mulch")
            .Add("Guard Spec")
            .Add("Heal Powder")
            .Add("Heart Scale")
            .Add("Sweet Heart")
            .Add("Helix Fossil")
            .Add("Casteliacone")
            .Add("Hondew Berry")
            .Add("Honey")
            .Add("HP Up")
            .Add("Big Nugget")
            .Add("Hyper Potion")
            .Add("Ice Heal")
            .Add("Iron")
            .Add("Jaboca Berry")
            .Add("Kelpsy Berry")
            .Add("Lava Cookie")
            .Add("Leaf Stone")
            .Add("Lemonade")
            .Add("Luck Incense")
            .Add("Lucky Egg")
            .Add("Macho Brace")
            .Add("Magmarizer")
            .Add("Magost Berry")
            .Add("Max Elixir")
            .Add("Max Ether")
            .Add("Max Potion")
            .Add("Max Repel")
            .Add("Max Revive")
            .Add("MooMoo Milk")
            .Add("Moon Stone")
            .Add("Nanab Berry")
            .Add("Nomel Berry")
            .Add("Nugget")
            .Add("Odd KeyStone")
            .Add("Old Amber")
            .Add("Old Gateau")
            .Add("Oval Stone")
            .Add("Pamtre Berry")
            .Add("Paralyze Heal")
            .Add("Pearl")
            .Add("Pinap Berry")
            .Add("Pink Scarf")
            .Add("Pokedoll")
            .Add("Poké Toy")
            .Add("Pomeg Berry")
            .Add("Potion")
            .Add("Power Anklet")
            .Add("Power Band")
            .Add("Power Belt")
            .Add("Power Bracer")
            .Add("Power Lens")
            .Add("Power Weight")
            .Add("PP Max")
            .Add("PP Up")
            .Add("Protector")
            .Add("Protein")
            .Add("Pure Incense")
            .Add("Qualot Berry")
            .Add("Rabuta Berry")
            .Add("Rare Bone")
            .Add("Rare Candy")
            .Add("Razz Berry")
            .Add("Reaper Cloth")
            .Add("Red Flute")
            .Add("Red Scarf")
            .Add("Red Shard")
            .Add("Repel")
            .Add("Revival Herb")
            .Add("Revive")
            .Add("Root Fossil")
            .Add("Pearl String")
            .Add("Rowap Berry")
            .Add("Sacred Ash")
            .Add("Cover Fossil")
            .Add("Shiny Stone")
            .Add("Shoal Salt")
            .Add("Shoal Shell")
            .Add("Skull Fossil")
            .Add("Soda Pop")
            .Add("Soothe Bell")
            .Add("Spelon Berry")
            .Add("Stable Mulch")
            .Add("Star Piece")
            .Add("Stardust")
            .Add("Sun Stone")
            .Add("Super Potion")
            .Add("Super Repel")
            .Add("Tamato Berry")
            .Add("Thunderstone")
            .Add("TinyMushroom")
            .Add("Twistedspoon")
            .Add("Up-Grade")
            .Add("Water Stone")
            .Add("Watmel Berry")
            .Add("Wepear Berry")
            .Add("White Flute")
            .Add("Plume Fossil")
            .Add("X Accuracy")
            .Add("X Attack")
            .Add("X Defend")
            .Add("X Sp. Def")
            .Add("X Special")
            .Add("X Speed")
            .Add("Yellow Flute")
            .Add("Yellow Scarf")
            .Add("Yellow Shard")
            .Add("Zinc")
            .Add("Muscle Wing")
            .Add("Swift Wing")
            .Add("Genius Wing")
            .Add("Resist Wing")
            .Add("Clever Wing")
            .Add("Health Wing")
            .Add("Float Stone")
            .Add("Absorb Bulb")
            .Add("Cell Battery")
        End With

        colPunchMoves(0) = "Bullet Punch"
        colPunchMoves(1) = "Comet Punch"
        colPunchMoves(2) = "Dizzy Punch"
        colPunchMoves(3) = "Drain Punch"
        colPunchMoves(4) = "Dynamic Punch"
        colPunchMoves(5) = "Fire Punch"
        colPunchMoves(6) = "Focus Punch"
        colPunchMoves(7) = "Hammer Arm"
        colPunchMoves(8) = "Ice Punch"
        colPunchMoves(9) = "Mach Punch"
        colPunchMoves(10) = "Mega Punch"
        colPunchMoves(11) = "Meteor Mash"
        colPunchMoves(12) = "Shadow Punch"
        colPunchMoves(13) = "Sky Uppercut"
        colPunchMoves(14) = "ThunderPunch"

        colRecoilMoves(0) = "Brave Bird"
        colRecoilMoves(1) = "Double-Edge"
        colRecoilMoves(2) = "Flare Blitz"
        colRecoilMoves(3) = "Head Smash"
        colRecoilMoves(4) = "Hi Jump Kick"
        colRecoilMoves(5) = "Jump Kick"
        colRecoilMoves(6) = "Submission"
        colRecoilMoves(7) = "Take Down"
        colRecoilMoves(8) = "Volt Tackle"
        colRecoilMoves(9) = "Wood Hammer"
        colRecoilMoves(10) = "Head Charge"
        colRecoilMoves(11) = "Wild Charge"

        colSpHitDefMoves(0) = "Psyshock"
        colSpHitDefMoves(1) = "Psystrike"
        colSpHitDefMoves(2) = "Secret Sword"

        colSpeedLowerItems(0) = "Macho Brace"
        colSpeedLowerItems(1) = "Power Weight"
        colSpeedLowerItems(2) = "Power Bracer"
        colSpeedLowerItems(3) = "Power Belt"
        colSpeedLowerItems(4) = "Power Lens"
        colSpeedLowerItems(5) = "Power Band"
        colSpeedLowerItems(6) = "Power Anklet"
        colSpeedLowerItems(7) = "Iron Ball"

        'Try
        '    Dim objXML As New XmlDocument
        '    objXML.Load("movedata.xml")
        '    For Each objTemp As XmlNode In objXML.GetElementsByTagName("move")
        '        If objTemp.Attributes.Item(0).Name = "name" Then
        '            colAllMoves.Add(objTemp.Attributes.Item(0).Value)
        '            Dim iPower As Integer = 0
        '            For Each objData As XmlNode In objTemp.ChildNodes
        '                If objData.Name = "power" Then iPower = objData.InnerText : Exit For
        '            Next
        '            If iPower > 0 Then
        '                colAtkMoves.Add(objTemp.Attributes.Item(0).Value)
        '            End If
        '        End If
        '    Next

        '    objXML.Load(AppPathSlash() & "pokedata.xml")
        '    For Each objTemp As XmlNode In objXML.GetElementsByTagName("pokemon")
        '        If objTemp.Attributes.Item(0).Name = "name" Then
        '            colAllPoke.Add(objTemp.Attributes.Item(0).Value)
        '        End If
        '    Next



        'Catch ex As Exception

        'End Try
        arrImmuneAbilities = New Integer(,) {{11, 11}, _
                                             {10, 13}, _
                                             {87, 11}, _
                                             {31, 13}, _
                                             {18, 10}, _
                                             {114, 11}, _
                                             {157, 12}, _
                                             {26, 5}}

    End Sub
#End Region

#Region "Process"
    Public Function FindArray(ByVal arrSource As Array, ByVal Item As Object) As Integer
        If arrSource.Length = 0 Then Return -1
        For i As Integer = 0 To arrSource.Length - 1
            If LCase(arrSource(i)) = LCase(Item) Then Return i
        Next
        Return -1
    End Function

    'Public Function GetPokeData(ByVal PokeName As String) As PokeData
    '    On Error Resume Next
    '    Dim objXML As New XmlDocument
    '    Dim objResult As New PokeData

    '    objXML.Load("pokedata.xml")
    '    For Each objNode As XmlNode In objXML.GetElementsByTagName("pokemon")
    '        If objNode.Attributes.Count <> 0 Then

    '            If LCase(objNode.Attributes.Item(0).Value) = LCase(PokeName) Then

    '                With objResult
    '                    .Name = PokeName

    '                    For Each objTemp As XmlNode In objNode.ChildNodes
    '                        Select Case objTemp.Name
    '                            Case "number"
    '                                .Number = Val(objTemp.InnerText)
    '                            Case "type1"
    '                                .Type1 = IIf(Val(objTemp.InnerText) = -1, 0, Val(objTemp.InnerText))
    '                            Case "type2"
    '                                .Type2 = IIf(Val(objTemp.InnerText) = -1, 0, Val(objTemp.InnerText))
    '                            Case "Ability1"
    '                                .Ability1 = IIf(Val(objTemp.InnerText) = -1, 0, Val(objTemp.InnerText))
    '                            Case "Ability2"
    '                                .Ability2 = IIf(Val(objTemp.InnerText) = -1, 0, Val(objTemp.InnerText))
    '                            Case "Ability3"
    '                                .Ability3 = IIf(Val(objTemp.InnerText) = -1, 0, Val(objTemp.InnerText))
    '                            Case "weight"
    '                                .Weight = Val(objTemp.InnerText)
    '                            Case "baseHP"
    '                                .BaseStats.HP = Val(objTemp.InnerText)
    '                            Case "baseAtk"
    '                                .BaseStats.Atk = Val(objTemp.InnerText)
    '                            Case "baseDef"
    '                                .BaseStats.Def = Val(objTemp.InnerText)
    '                            Case "baseSpAtk"
    '                                .BaseStats.SpAtk = Val(objTemp.InnerText)
    '                            Case "baseSpDef"
    '                                .BaseStats.SpDef = Val(objTemp.InnerText)
    '                            Case "baseSpe"
    '                                .BaseStats.Speed = Val(objTemp.InnerText)
    '                            Case Else
    '                        End Select

    '                    Next

    '                End With

    '                Return objResult
    '            End If
    '        End If
    '    Next
    '    Return objResult
    'End Function

    'Public Function GetMoveData(ByVal MoveName As String) As Move
    '    On Error Resume Next
    '    Dim objXML As New XmlDocument
    '    Dim objResult As New Move
    '    objXML.Load("movedata.xml")

    '    For Each objNode As XmlNode In objXML.GetElementsByTagName("move")
    '        If objNode.Attributes.Count <> 0 Then

    '            If LCase(objNode.Attributes.Item(0).Value) = LCase(MoveName) Then

    '                With objResult
    '                    .Name = MoveName

    '                    For Each objTemp As XmlNode In objNode.ChildNodes
    '                        Select Case objTemp.Name
    '                            Case "type"
    '                                .Type = Val(objTemp.InnerText)
    '                            Case "dmgtype"
    '                                .DmgType = CType(objTemp.InnerText, _DmgType)
    '                            Case "power"
    '                                .Power = Val(objTemp.InnerText)
    '                            Case "accuracy"
    '                                .Accuracy = Val(objTemp.InnerText)
    '                            Case "pp"
    '                                .PP = Val(objTemp.InnerText)
    '                            Case "priority"
    '                                .Priority = Val(objTemp.InnerText)
    '                            Case "eff"
    '                                .EffPercent = Val(objTemp.InnerText)
    '                            Case "target"
    '                                .Target = Val(objTemp.InnerText)
    '                            Case Else

    '                        End Select

    '                    Next

    '                End With

    '                Return objResult
    '            End If
    '        End If
    '    Next
    '    Return objResult
    'End Function

    Public Function GetNature(ByVal Nature As String) As Stats
        Dim dNature As New Stats
        With dNature
            Select Case Nature
                Case "Lonely"
                    .Atk = 2
                    .Def = 1
                Case "Brave"
                    .Atk = 2
                    .Speed = 1
                Case "Adamant"
                    .Atk = 2
                    .SpAtk = 1
                Case "Naughty"
                    .Atk = 2
                    .SpDef = 1

                Case "Bold"
                    .Def = 2
                    .Atk = 1
                Case "Relaxed"
                    .Def = 2
                    .Speed = 1
                Case "Impish"
                    .Def = 2
                    .SpAtk = 1
                Case "Lax"
                    .Def = 2
                    .SpDef = 1

                Case "Timid"
                    .Speed = 2
                    .Atk = 1
                Case "Hasty"
                    .Speed = 2
                    .Def = 1
                Case "Jolly"
                    .Speed = 2
                    .SpAtk = 1
                Case "Naive"
                    .Speed = 2
                    .SpDef = 1

                Case "Modest"
                    .SpAtk = 2
                    .Atk = 1
                Case "Mild"
                    .SpAtk = 2
                    .Def = 1
                Case "Quiet"
                    .SpAtk = 2
                    .Speed = 1
                Case "Rash"
                    .SpAtk = 2
                    .SpDef = 1

                Case "Calm"
                    .SpDef = 2
                    .Atk = 1
                Case "Gentle"
                    .SpDef = 2
                    .Def = 1
                Case "Sassy"
                    .SpDef = 2
                    .Speed = 1
                Case "Careful"
                    .SpDef = 2
                    .SpAtk = 1

                Case Else
            End Select
        End With
        Return dNature
    End Function

    'Public Function GetMoveset(ByVal Pokemon As String, ByVal Moveset As String) As MovesetData
    '    Dim objMoveset As New MovesetData(Pokemon)
    '    Dim strPath As String = AppPathSlash() & "movesets\" & Pokemon & ".xml"

    '    If Not IO.File.Exists(strPath) Then Return objMoveset

    '    Dim objXML As New XmlDocument
    '    objXML.Load(strPath)

    '    For Each objSetNode As XmlNode In objXML.GetElementsByTagName("moveset")
    '        If objSetNode.Attributes(0).Name = "name" And objSetNode.Attributes(0).Value = Moveset Then
    '            With objMoveset
    '                .MovesetName = objSetNode.Attributes(0).Value
    '                For Each objChild As XmlNode In objSetNode.ChildNodes
    '                    Select Case objChild.Name
    '                        Case "nature"
    '                            .Nature = objChild.InnerText
    '                        Case "ability"
    '                            .AbilityIndex = Val(objChild.InnerText)
    '                        Case "item"
    '                            .Item = objChild.InnerText
    '                        Case "ev_hp"
    '                            .EVs.HP = Val(objChild.InnerText)
    '                        Case "ev_atk"
    '                            .EVs.Atk = Val(objChild.InnerText)
    '                        Case "ev_def"
    '                            .EVs.Def = Val(objChild.InnerText)
    '                        Case "ev_spatk"
    '                            .EVs.SpAtk = Val(objChild.InnerText)
    '                        Case "ev_spdef"
    '                            .EVs.SpDef = Val(objChild.InnerText)
    '                        Case "ev_speed"
    '                            .EVs.Speed = Val(objChild.InnerText)
    '                        Case "iv_hp"
    '                            .IVs.HP = Val(objChild.InnerText)
    '                        Case "iv_atk"
    '                            .IVs.Atk = Val(objChild.InnerText)
    '                        Case "iv_def"
    '                            .IVs.Def = Val(objChild.InnerText)
    '                        Case "iv_spatk"
    '                            .IVs.SpAtk = Val(objChild.InnerText)
    '                        Case "iv_spdef"
    '                            .IVs.SpDef = Val(objChild.InnerText)
    '                        Case "iv_speed"
    '                            .IVs.Speed = Val(objChild.InnerText)
    '                        Case "level"
    '                            .Level = Val(objChild.InnerText)
    '                        Case "happiness"
    '                            .Happiness = Val(objChild.InnerText)
    '                        Case "gender"
    '                            .Gender = Val(objChild.InnerText)
    '                        Case "move"
    '                            .Moves.Add(objChild.InnerText)
    '                    End Select
    '                Next
    '            End With
    '            Return objMoveset
    '        End If
    '    Next
    '    Return objMoveset
    'End Function
#End Region

    Public Function GetDouble(ByVal value As String) As Double
        Dim culture As CultureInfo = Nothing
        Dim number As Double

        ' Throw exception if string is empty.
        If String.IsNullOrEmpty(value) Then _
           Throw New ArgumentNullException("The input string is invalid.")

        ' Determine if value can be parsed using current culture.
        Try
            culture = CultureInfo.CurrentCulture
            number = Double.Parse(value, culture)
            Return number
        Catch
        End Try
        ' If Parse operation fails, see if there's a neutral culture.
        Try
            culture = culture.Parent
            number = Double.Parse(value, culture)
            Return number
        Catch
        End Try
        ' If there is no neutral culture or if parse operation fails, use
        ' the invariant culture.
        culture = CultureInfo.InvariantCulture
        Try
            number = Double.Parse(value, culture)
            Return number
            ' All attempts to parse the string have failed. Rethrow the exception.
        Catch e As FormatException
            Throw New FormatException(String.Format("Unable to parse '{0}'.", value), _
                                      e)
        End Try
    End Function

    Public Function ExLTrim(ByVal strSource As String) As String
        If strSource = vbNullString Then Return vbNullString
        If strSource.Length <= 2 Then Return strSource
        Dim strTest As String = "abcdefghijklmnopqrstuvwxyz"
        For i As Integer = 0 To strSource.Length - 1
            If InStr(strTest, LCase(strSource.Substring(i, 1))) <> 0 Then
                Return strSource.Substring(i, strSource.Length - i)
                Exit For
            End If
        Next
        Return strSource
    End Function
End Module

