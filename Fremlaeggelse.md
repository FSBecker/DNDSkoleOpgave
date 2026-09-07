# DNDSkoleOpgave – kort guide

Denne version bygger på **DNDSkoleOpgave(1).rar** fra din makker. Karakterhierarkier, items, JSON og admin-editor er videreført. Der er tilføjet et spilbart kampforløb med ubegrænsede waves.

## Start her

Pak ZIP-filen ud, og åbn den udpakkede mappe i VS Code. Kør disse kommandoer fra mappen, der indeholder `DNDSkoleOpgave` og `DNDSkoleOpgave.Tests`:

```powershell
dotnet build DNDSkoleOpgave/DNDSkoleOpgave.csproj
dotnet run --project DNDSkoleOpgave.Tests/DNDSkoleOpgave.Tests.csproj
dotnet run --project DNDSkoleOpgave/DNDSkoleOpgave.csproj
```

Projektet bruger .NET 9, Newtonsoft.Json og Spectre.Console. Kamp- og karaktermenuerne bruger piletaster og Enter. Admin-editoren beholder sin eksisterende nummermenu.

**Kontrolstatus:** Kilden, navne, standardindholdets referencer og UML-dækningen er kontrolleret statisk. Miljøet her har ikke .NET SDK, så projektet er ikke kompileret, regressionstests er ikke kørt, og balancen er ikke prøvespillet. Kør kommandoerne ovenfor inden fremlæggelsen.

## De nye regler

| Regel | Standard |
|---|---|
| Waves | Fortsætter, indtil du taber eller vælger at gemme og afslutte mellem waves |
| Fjender | Tilfældigt 1–3 forskellige objekter pr. wave |
| Initiativ | d20 + Dexterity-modifier; højeste først; samme rækkefølge hele kampen |
| Uafgjort initiativ | Beholder deltagernes oprindelige rækkefølge: spiller, derefter fjender |
| AP | Spiller: 2 pr. tur. Fjende: 1 pr. tur |
| Synlig tur | Gul figur, gul ramme, AKTIV-tekst og markering i turordenen |
| Mål | Du vælger blandt levende fjender; tilbage bruger ingen AP |
| Healing-evner | Én anvendelse i alt pr. karakter pr. kamp; nye ture giver ingen ekstra anvendelser |
| Potions | Én ved oprettelse; normale belønninger fylder kun op til to |
| Potion-belønning | Én efter hver tredje wave; eventuelle admin-loottabeller kan også give én pr. wave, inden for grænsen |
| Small Potion | +6 HP, koster 1 AP og fjernes efter brug |
| Hvil efter sejr | Kun +2 HP |
| Level-up | Efter hver anden wave; øger maksimum, men healer ikke |
| Fjendernes styrke | Et ekstra level hver anden wave; +1 til at ramme for hver tre gennemførte waves |
| Angreb | d20 + stat-modifier + angrebsbonus mod AC; naturligt 1 misser og 20 rammer |
| Weak Fireball | Målet slår Dexterity mod 10; ved fejlet slag gives skade og en kort ildeffekt |

Balancereglerne står i `classes/Game/Difficulty.cs`. Karakterernes start-HP og vækst står i `classes/CharacterClasses`. JSON-indhold kan efterfølgende ændres i admin-editoren. Admin kan definere ekstra level-belønninger og stærkere indhold; tabellen beskriver standardspillet.

## Læs koden i denne rækkefølge

| Fil | Hvad den gør |
|---|---|
| `Program.cs` | Sætter tekstkodning, opretter spillet og kalder `Run()` |
| `classes/Game/GameApplication.cs` | Indlæser indhold og starter spil eller admin |
| `classes/Game/WaveGame.cs` | Opretter wave → kæmper → giver belønning → gemmer → spørger om næste wave |
| `classes/Game/WaveCreator.cs` | Opretter 1–3 fjender og justerer deres styrke |
| `classes/Combat/CombatRunner.cs` | Runde → deltagers tur → vælg handling → vælg mål → udfør |
| `classes/Combat/CombatActionExecutor.cs` | Kontrollerer handlingen, bruger ressourcer og beregner virkningen |
| `classes/Combat/CombatParticipant.cs` | Holder styr på én deltagers initiativ, AP og healing i kampen |
| `classes/UI/BattleHud.cs` | Viser turorden, figurer, HP og log |

`GameDefinitions` har nu navne som `AllRaces`, `AllClasses`, `AllActions` og `AllItems`. Andre samlinger hedder fx `AllEquipment` og `AllRaceNames`. JSON-felternes navne er bevaret.

## Seks steder at vise underviseren

1. **Abstraktion og arv:** `CoreCharacter`, `PlayerCharacter`, `EnemyCharacter`; samt `CharacterClass` og `CharacterRace` med deres underklasser.
2. **Race + klasse:** `CoreCharacter` har én reference til hver. De kombineres ved oprettelse i `CharacterCreator`.
3. **Polymorfi og overload:** `SpellAction` har virtuelle egenskaber, som `WeakFireball` og `DefinedSpellAction` overrider. `ActionEffect.Apply(...)` og `DiceRoller.Roll(...)` har overloads med forskellige parametre.
4. **Indkapsling og collections:** HP har private setters; `TakeDamage` og `Heal` holder grænserne. Inventory er en privat `List<CoreItem>` med en offentlig læsevisning. `EquipmentSlots` bruger en `Dictionary` til opslag via slot.
5. **Interfaces og løs kobling:** `ActionEffect.Apply(IDamageable)` kan behandle noget, der har HP. Kampkoden modtager `IDiceRoller`, så `RandomDiceRoller` kan udskiftes med `FixedDiceRoller`.
6. **Exceptions:** `CharacterIsDefeatedException` og `InsufficientActionPointsException` kastes fra `CombatParticipant`. `CombatRunner` fanger dem og viser en besked. Regressionstestene fremkalder fejlene bevidst.

Tre access modifiers at forklare: `private set` beskytter HP; `protected` på CoreCharacters konstruktør lader underklasser bruge den; `private` på CombatRunners hjælpemetoder holder kampens interne trin inde i klassen.

## UML og kardinalitet

Åbn **DNDSkoleOpgave_UML.drawio** i diagrams.net/draw.io med **File → Open From → Device**. Bokse, tekst, pile og tal kan redigeres individuelt.

Diagrammet har 14 faner. De første to viser karaktermodellen og spilforløbet. Resten indeholder alle typerne fordelt efter ansvar. Alle **100 spilklasser, 2 interfaces og 8 enums**, plus de to klasser i testprojektet, er med. Der vises udvalgte medlemmer; `Klasseoversigt.md` henviser til alle kodefilerne.

| Forbindelse | Betydning |
|---|---|
| CoreCharacter → CharacterRace / CharacterClass: `1` | Hver karakter har præcis én race og én klasse |
| Ved CoreCharacter-enden: `0..*` | Samme race-/klasseobjekt kan refereres af flere karakterer |
| CombatInstance → PlayerCharacter: `1` | En kamp har én spiller |
| CombatInstance → EnemyCharacter: `1..3` | En kamp oprettes med 1–3 forskellige fjender |
| CombatInstance → CombatParticipant: `2..4` | Spilleren plus 1–3 fjender giver 2–4 deltagere |
| EquipmentSlots → EquipmentItem: `0..6` | Højst ét item i hver af de seks slots |

Kardinalitet beskriver antallet af forbindelser. Den lægges ikke på arvepile eller på stiplede pile, der blot viser et metodekald.

## Gemning og en kort demonstration

Indhold og karakterer gemmes i `data` under programmets outputmappe, normalt `DNDSkoleOpgave/bin/Debug/net9.0/data`. Mappen oprettes ved første start. JSON-definitionerne bruges også ved oprettelse af nye karakterer og indlæsning af eksisterende karakterer.

Efter en wave gemmes HP, level, inventory, udstyr, actions og næste wave. Indlæsning giver ikke gratis healing. Aktive effekter afsluttes efter kampen. En besejret karakter forbliver besejret, når den indlæses.

Vis en kamp med flere fjender, vælg et mål, brug en potion og fortsæt til næste wave med reduceret HP. Vis derefter `WaveGame.Run()` og `CombatRunner.PlayTurn()`.

For en deterministisk demonstration ændres terningen i `Program.cs` til `new FixedDiceRoller(20)`. Det giver tre fjender, og normale angreb rammer. Weak Fireballs mål vil dog også få høje redningsslag. Sæt `RandomDiceRoller` tilbage bagefter.

## Forholdet til opgaveteksten

Designet følger jeres valg om at kombinere race og klasse som referencer. Det er derfor ikke en ordret implementering af kravene om tre underklasser af Character med hver sin abstrakte angrebsmetode. `Party` og et særligt `ISpellcaster`/mana-system er heller ikke med. Polymorfien ligger især i spell-hierarkiet. Dette bør forklares som jeres designvalg; alle opgavens bogstavelige krav er ikke opfyldt.

Din makkers oprindelige arkiv ligger uændret i `Original/DNDSkoleOpgave.rar`.
