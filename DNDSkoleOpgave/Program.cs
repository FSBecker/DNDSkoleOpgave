using DNDSkoleOpgave.Characters;

ConsoleCharacterCreator characterCreator = new(Console.In, Console.Out);
PlayerCharacter player = characterCreator.Create();
