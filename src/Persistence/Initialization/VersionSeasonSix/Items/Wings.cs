// <copyright file="Wings.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.VersionSeasonSix.Items;

using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.DataModel.Configuration.Items;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.Interfaces;
using MUnique.OpenMU.Persistence.Initialization;
using MUnique.OpenMU.Persistence.Initialization.CharacterClasses;
using MUnique.OpenMU.Persistence.Initialization.Properties;

/// <summary>
/// Initializer for wing items.
/// </summary>
public class Wings : WingsInitializerBase
{
    /// <summary>
    /// The initial damage increase of the fourth wings, in percent.
    /// </summary>
    internal const int FourthWingDamageIncrease = 49;

    /// <summary>
    /// The initial damage absorption of the fourth wings, in percent.
    /// </summary>
    internal const int FourthWingDamageAbsorb = 43;

    private ItemLevelBonusTable? _absorbByLevelTable;
    private ItemLevelBonusTable? _defenseBonusByLevelTable;
    private ItemLevelBonusTable? _damageIncreaseByLevelTable;
    private ItemLevelBonusTable? _damageIncreaseByLevelTableSecond;
    private ItemLevelBonusTable? _defenseBonusByLevelTableThird;

    /// <summary>
    /// Initializes a new instance of the <see cref="Wings"/> class.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="gameConfiguration">The game configuration.</param>
    public Wings(IContext context, GameConfiguration gameConfiguration)
        : base(context, gameConfiguration)
    {
    }

    /// <inheritdoc />
    protected override int MaximumItemLevel => 15;

    /// <summary>
    /// Initializes all wings.
    /// </summary>
    /// <remarks>
    /// Regex: (?m)^\s*(\d+)\s+(-*\d+)\s+(\d+)\s+(\d+)\s+(\d+)\s+(\d+)\s+(\d+)\s+(\d+)\s+\"(.+?)\"\s+(\d+)\s+(\d+)\s+(\d+)\s+(\d+)\s+(\d+)\s+(\d+)\s+(\d+)\s+(\d+)\s+(\d+)\s+(\d+)\s+(\d+)\s+(\d+)\s+(\d+)\s+(\d+)\s+(\d+).*$
    /// Replace by: this.CreateWing($1, $4, $5, "$9", $10, $11, $12, $13, $19, $20, $21, $22, $23, $24, $25);
    /// Sources for stats not provided by the Item.txt:
    /// http://muonlinefanz.com/tools/items/
    /// https://wiki.infinitymu.net/index.php?title=2nd_Level_Wing
    /// https://wiki.infinitymu.net/index.php?title=3rd_Level_Wing
    /// http://www.guiamuonline.com/items-de-mu-online/wings
    /// Item option numbers:
    /// 3rd wings:
    /// 0x11 (3) -> Damage
    /// 0x00 (0) -> Recover
    /// 0x10 (2) -> Defense (for Ruin it's WizDamage)
    /// 2nd wings:
    ///             PDamage Recover WizDamage   CurseDmg
    /// Spirit       0x00    0x10
    /// Soul                 0x00    0x10
    /// Dragon       0x10    0x00
    /// Darkness     0x10            0x00
    /// Warrior Cl   0x10    0x00
    /// Despair              0x00               0x10.
    /// </remarks>
    public override void Initialize()
    {
        this._absorbByLevelTable = this.CreateAbsorbBonusPerLevel();
        this._defenseBonusByLevelTable = this.CreateBonusDefensePerLevel();
        this._defenseBonusByLevelTableThird = this.CreateBonusDefensePerLevelThirdWings();
        this._damageIncreaseByLevelTable = this.CreateDamageIncreaseBonusPerLevelFirstAndThirdWings();
        this._damageIncreaseByLevelTableSecond = this.CreateDamageIncreaseBonusPerLevelSecondWings();

        // First class wings:
        this.CreateWing(0, 3, 2, LocalizedString.FromResource(() => ItemNames.WingsOfElf), 100, 10, 200, 180, CharacterClasses.AllElfs, this.BuildOptions((0, OptionType.HealthRecover)), 12, 12, this._damageIncreaseByLevelTable, null);
        this.CreateWing(1, 5, 3, LocalizedString.FromResource(() => ItemNames.WingsOfHeaven), 100, 10, 200, 180, CharacterClasses.AllMagicians, this.BuildOptions((0, OptionType.WizDamage)), 12, 12, this._damageIncreaseByLevelTable, null);
        this.CreateWing(2, 5, 2, LocalizedString.FromResource(() => ItemNames.WingsOfSatan), 100, 20, 200, 180, CharacterClasses.AllKnights | CharacterClasses.AllMGs, this.BuildOptions((0, OptionType.PhysDamage)), 12, 12, this._damageIncreaseByLevelTable, null);
        this.CreateWing(41, 4, 2, LocalizedString.FromResource(() => ItemNames.WingsOfCurse), 100, 10, 200, 180, CharacterClasses.AllSummoners, this.BuildOptions((0, OptionType.WizDamage)), 12, 12, this._damageIncreaseByLevelTable, null);

        // Second class wings:
        var secondWingOptions = this.CreateSecondClassWingOptions();
        this.CreateWing(3, 5, 3, LocalizedString.FromResource(() => ItemNames.WingsOfSpirits), 150, 30, 200, 215, CharacterClasses.AllElfs, this.BuildOptions((0b10, OptionType.HealthRecover), (0b00, OptionType.PhysDamage)), 32, 25, this._damageIncreaseByLevelTableSecond, secondWingOptions);
        this.CreateWing(4, 5, 3, LocalizedString.FromResource(() => ItemNames.WingsOfSoul), 150, 30, 200, 215, CharacterClasses.AllWizards, this.BuildOptions((0b00, OptionType.HealthRecover), (0b10, OptionType.WizDamage)), 32, 25, this._damageIncreaseByLevelTableSecond, secondWingOptions);
        this.CreateWing(5, 3, 3, LocalizedString.FromResource(() => ItemNames.WingsOfDragon), 150, 45, 200, 215, CharacterClasses.AllKnights, this.BuildOptions((0b00, OptionType.HealthRecover), (0b10, OptionType.PhysDamage)), 32, 25, this._damageIncreaseByLevelTableSecond, secondWingOptions, movementSpeed: MovementSpeedConstants.FastWingMovementSpeed);
        this.CreateWing(6, 4, 2, LocalizedString.FromResource(() => ItemNames.WingsOfDarkness), 150, 40, 200, 215, CharacterClasses.AllMGs, this.BuildOptions((0b00, OptionType.WizDamage), (0b10, OptionType.PhysDamage)), 32, 25, this._damageIncreaseByLevelTableSecond, secondWingOptions);
        this.CreateWing(42, 4, 3, LocalizedString.FromResource(() => ItemNames.WingsOfDespair), 150, 30, 200, 215, CharacterClasses.AllSummoners, this.BuildOptions((0b00, OptionType.CurseDamage), (0b10, OptionType.WizDamage)), 32, 25, this._damageIncreaseByLevelTableSecond, secondWingOptions);
        this.CreateMonsterWings(secondWingOptions);

        // The capes are a bit of a hybrid. Their damage gets increased like first wings, but they start slightly lower than 2nd wings.
        this.CreateWing(49, 2, 3, LocalizedString.FromResource(() => ItemNames.CapeOfFighter), 180, 15, 200, 180, CharacterClasses.AllFighters, this.BuildOptions((0b00, OptionType.HealthRecover), (0b10, OptionType.PhysDamage)), 20, 10, this._damageIncreaseByLevelTable, secondWingOptions);
        var capeOfLord = this.CreateWing(30, 2, 3, LocalizedString.FromResource(() => ItemNames.CapeOfLord), 180, 15, 200, 180, CharacterClasses.AllLords, this.BuildOptions((0b00, OptionType.PhysDamage)), 20, 10, this._damageIncreaseByLevelTable, this.CreateCapeOptions());
        capeOfLord.Group = 13;
        capeOfLord.SetGuid(capeOfLord.Group, capeOfLord.Number);

        this.CreateSmallWings();

        this.CreateFeather();
        this.CreateFeatherOfCondor();
        this.CreateFlameOfCondor();

        // Third class wings:
        var thirdWingOptions = this.CreateThirdClassWingOptions();
        this.CreateWing(36, 4, 3, LocalizedString.FromResource(() => ItemNames.WingOfStorm), 150, 60, 220, 400, CharacterClasses.BladeMaster, this.BuildOptions((0b00, OptionType.HealthRecover), (0b11, OptionType.PhysDamage), (0b10, OptionType.Defense)), 39, 39, this._damageIncreaseByLevelTable, thirdWingOptions, movementSpeed: MovementSpeedConstants.FastWingMovementSpeed);
        this.CreateWing(37, 4, 3, LocalizedString.FromResource(() => ItemNames.WingOfEternal), 150, 45, 220, 400, CharacterClasses.GrandMaster, this.BuildOptions((0b00, OptionType.HealthRecover), (0b11, OptionType.WizDamage), (0b10, OptionType.Defense)), 39, 39, this._damageIncreaseByLevelTable, thirdWingOptions);
        this.CreateWing(38, 4, 3, LocalizedString.FromResource(() => ItemNames.WingOfIllusion), 150, 45, 220, 400, CharacterClasses.HighElf, this.BuildOptions((0b00, OptionType.HealthRecover), (0b11, OptionType.PhysDamage), (0b10, OptionType.Defense)), 39, 39, this._damageIncreaseByLevelTable, thirdWingOptions);
        this.CreateWing(39, 4, 3, LocalizedString.FromResource(() => ItemNames.WingOfRuin), 150, 55, 220, 400, CharacterClasses.DuelMaster, this.BuildOptions((0b00, OptionType.HealthRecover), (0b11, OptionType.PhysDamage), (0b10, OptionType.WizDamage)), 39, 39, this._damageIncreaseByLevelTable, thirdWingOptions);
        this.CreateWing(40, 2, 3, LocalizedString.FromResource(() => ItemNames.CapeOfEmperor), 150, 45, 220, 400, CharacterClasses.LordEmperor, this.BuildOptions((0b00, OptionType.HealthRecover), (0b11, OptionType.PhysDamage), (0b10, OptionType.Defense)), 39, 24, this._damageIncreaseByLevelTable, thirdWingOptions);
        this.CreateWing(43, 4, 3, LocalizedString.FromResource(() => ItemNames.WingOfDimension), 150, 45, 220, 400, CharacterClasses.DimensionMaster, this.BuildOptions((0b00, OptionType.HealthRecover), (0b11, OptionType.WizDamage), (0b10, OptionType.CurseDamage)), 39, 39, this._damageIncreaseByLevelTable, thirdWingOptions);
        this.CreateWing(50, 2, 3, LocalizedString.FromResource(() => ItemNames.CapeOfOverrule), 150, 45, 220, 400, CharacterClasses.FistMaster, this.BuildOptions((0b00, OptionType.HealthRecover), (0b11, OptionType.PhysDamage), (0b10, OptionType.Defense)), 39, 39, this._damageIncreaseByLevelTable, thirdWingOptions);

        // Fourth wings of season 13:
        this.CreateFourthWings(thirdWingOptions);
    }

    /// <summary>
    /// Adds the small wings and capes to an existing configuration, which was created before they were part of it.
    /// </summary>
    /// <remarks>
    /// The small wings use the same item level bonus tables as the first wings, so the existing tables of the
    /// Wings of Elf are used. Only when they can't be found, new tables are created.
    /// </remarks>
    /// <returns>The created items. Small wings which already exist are not created again.</returns>
    internal IReadOnlyList<ItemDefinition> AddSmallWings()
    {
        var wingsOfElf = this.GameConfiguration.Items.FirstOrDefault(item => item is { Group: 12, Number: 0 });
        this._absorbByLevelTable = GetBonusTable(wingsOfElf, Stats.DamageReceiveDecrement) ?? this.CreateAbsorbBonusPerLevel();
        this._damageIncreaseByLevelTable = GetBonusTable(wingsOfElf, Stats.AttackDamageIncrease) ?? this.CreateDamageIncreaseBonusPerLevelFirstAndThirdWings();
        this._defenseBonusByLevelTable = GetBonusTable(wingsOfElf, Stats.DefenseBase) ?? this.CreateBonusDefensePerLevel();
        return this.CreateSmallWings();
    }

    /// <summary>
    /// Adds the monster wings (Cloak of Death, Wings of Chaos, Wings of Magic and Wings of Life) to an existing
    /// configuration, which was created before they were part of it.
    /// </summary>
    /// <remarks>
    /// The monster wings use the same item level bonus tables and excellent options as the second wings, so the existing
    /// ones of the Wings of Spirits are used. Only when they can't be found, new ones are created.
    /// </remarks>
    /// <returns>The created items. Monster wings which already exist are not created again.</returns>
    internal IReadOnlyList<ItemDefinition> AddMonsterWings()
    {
        var wingsOfSpirits = this.GameConfiguration.Items.FirstOrDefault(item => item is { Group: 12, Number: 3 });
        this._absorbByLevelTable = GetBonusTable(wingsOfSpirits, Stats.DamageReceiveDecrement) ?? this.CreateAbsorbBonusPerLevel();
        this._damageIncreaseByLevelTableSecond = GetBonusTable(wingsOfSpirits, Stats.AttackDamageIncrease) ?? this.CreateDamageIncreaseBonusPerLevelSecondWings();
        this._defenseBonusByLevelTable = GetBonusTable(wingsOfSpirits, Stats.DefenseBase) ?? this.CreateBonusDefensePerLevel();
        var secondWingOptionsId = GuidHelper.CreateGuid<ItemOptionDefinition>(ItemOptionDefinitionNumbers.Wing2nd);
        var secondWingOptions = this.GameConfiguration.ItemOptions.FirstOrDefault(option => option.GetId() == secondWingOptionsId)
                                ?? this.CreateSecondClassWingOptions();
        return this.CreateMonsterWings(secondWingOptions);
    }

    /// <summary>
    /// Adds the fourth wings of season 13 (Wings of Elysium, Wings of Flame God, Wing of Celestial Body, Wings of Annihilation,
    /// Cloak of Dominator, Wings of Other World, Cloak of Judgment and Cloak of Isolation) to an existing configuration,
    /// which was created before they were part of it.
    /// </summary>
    /// <remarks>
    /// The fourth wings use the same item level bonus tables and excellent options as the third wings, so the existing ones
    /// of the Wing of Storm are used. Only when they can't be found, new ones are created.
    /// </remarks>
    /// <returns>The created items. Fourth wings which already exist are not created again.</returns>
    internal IReadOnlyList<ItemDefinition> AddFourthWings()
    {
        var wingOfStorm = this.GameConfiguration.Items.FirstOrDefault(item => item is { Group: 12, Number: 36 });
        this._absorbByLevelTable = GetBonusTable(wingOfStorm, Stats.DamageReceiveDecrement) ?? this.CreateAbsorbBonusPerLevel();
        this._damageIncreaseByLevelTable = GetBonusTable(wingOfStorm, Stats.AttackDamageIncrease) ?? this.CreateDamageIncreaseBonusPerLevelFirstAndThirdWings();
        this._defenseBonusByLevelTableThird = GetBonusTable(wingOfStorm, Stats.DefenseBase) ?? this.CreateBonusDefensePerLevelThirdWings();
        var thirdWingOptionsId = GuidHelper.CreateGuid<ItemOptionDefinition>(ItemOptionDefinitionNumbers.Wing3rd);
        var thirdWingOptions = this.GameConfiguration.ItemOptions.FirstOrDefault(option => option.GetId() == thirdWingOptionsId)
                               ?? this.CreateThirdClassWingOptions();
        return this.CreateFourthWings(thirdWingOptions);
    }

    private static ItemLevelBonusTable? GetBonusTable(ItemDefinition? item, AttributeDefinition targetAttribute)
    {
        return item?.BasePowerUpAttributes.FirstOrDefault(powerUp => powerUp.TargetAttribute == targetAttribute)?.BonusPerLevelTable;
    }

    /// <summary>
    /// Creates the small wings and capes, which are sold in the in-game shop.
    /// </summary>
    /// <remarks>
    /// The values are the ones of the season 6 client: size, defense, classes, drop level, durability and level requirement
    /// of the item data, damage increase and absorption of the item tooltip. Like with the first wings, damage increase
    /// and absorption grow by 2 % and the defense by 3 per item level.
    /// The client knows no item options for them, so they don't get any.
    /// </remarks>
    /// <returns>The created items. Small wings which already exist are not created again.</returns>
    private IReadOnlyList<ItemDefinition> CreateSmallWings()
    {
        var created = new List<ItemDefinition?>
        {
            this.CreateSmallWing(130, 2, 2, LocalizedString.FromResource(() => ItemNames.SmallCapeOfLord), 15, CharacterClasses.AllLords, 20),
            this.CreateSmallWing(131, 3, 2, LocalizedString.FromResource(() => ItemNames.SmallWingOfCurse), 10, CharacterClasses.AllSummoners, 12),
            this.CreateSmallWing(132, 3, 2, LocalizedString.FromResource(() => ItemNames.SmallWingsOfElf), 10, CharacterClasses.AllElfs, 12),
            this.CreateSmallWing(133, 3, 2, LocalizedString.FromResource(() => ItemNames.SmallWingsOfHeaven), 10, CharacterClasses.AllMagicians, 12),
            this.CreateSmallWing(134, 3, 2, LocalizedString.FromResource(() => ItemNames.SmallWingsOfSatan), 20, CharacterClasses.AllKnights | CharacterClasses.AllMGs, 12),
            this.CreateSmallWing(135, 2, 2, LocalizedString.FromResource(() => ItemNames.LittleWarriorsCloak), 15, CharacterClasses.AllFighters, 20),
        };

        return created.OfType<ItemDefinition>().ToList();
    }

    /// <summary>
    /// Creates the monster wings ("2.5 wings"): Cloak of Death, Wings of Chaos, Wings of Magic and Wings of Life.
    /// </summary>
    /// <remarks>
    /// Size, defense, classes, drop level, durability and level requirement are the ones of the game client's item data.
    /// Damage increase, absorption, their growth per level and the excellent options are the ones of the second wings,
    /// as the game client's tooltip shows them.
    /// </remarks>
    /// <param name="secondWingOptions">The excellent options of the second wings.</param>
    /// <returns>The created items. Monster wings which already exist are not created again.</returns>
    private IReadOnlyList<ItemDefinition> CreateMonsterWings(ItemOptionDefinition secondWingOptions)
    {
        var created = new List<ItemDefinition?>
        {
            this.CreateMonsterWing(262, 2, 3, LocalizedString.FromResource(() => ItemNames.CloakOfDeath), 27, CharacterClasses.AllLords | CharacterClasses.AllFighters, this.BuildOptions((0b00, OptionType.HealthRecover), (0b10, OptionType.PhysDamage)), secondWingOptions),
            this.CreateMonsterWing(263, 4, 3, LocalizedString.FromResource(() => ItemNames.WingsOfChaos), 46, CharacterClasses.BladeKnightAndBladeMaster | CharacterClasses.AllMGs, this.BuildOptions((0b00, OptionType.HealthRecover), (0b10, OptionType.PhysDamage)), secondWingOptions),
            this.CreateMonsterWing(264, 4, 3, LocalizedString.FromResource(() => ItemNames.WingsOfMagic), 37, CharacterClasses.SoulMasterAndGrandMaster | CharacterClasses.AllMGs | CharacterClasses.BloodySummonerAndDimensionMaster, this.BuildOptions((0b00, OptionType.HealthRecover), (0b10, OptionType.WizDamage)), secondWingOptions),
            this.CreateMonsterWing(265, 4, 3, LocalizedString.FromResource(() => ItemNames.WingsOfLife), 37, CharacterClasses.MuseElfAndHighElf, this.BuildOptions((0b10, OptionType.HealthRecover), (0b00, OptionType.PhysDamage)), secondWingOptions),
        };

        return created.OfType<ItemDefinition>().ToList();
    }

    private ItemDefinition? CreateMonsterWing(short number, byte width, byte height, LocalizedString name, int defense, CharacterClasses characterClasses, IEnumerable<IncreasableItemOption> possibleOptions, ItemOptionDefinition secondWingOptions)
    {
        if (this.GameConfiguration.Items.Any(item => item.Group == 12 && item.Number == number))
        {
            return null;
        }

        return this.CreateWing(number, width, height, name, 150, defense, 210, 290, characterClasses, possibleOptions, 32, 25, this._damageIncreaseByLevelTableSecond!, secondWingOptions);
    }

    /// <summary>
    /// Creates the fourth wings of season 13.
    /// </summary>
    /// <remarks>
    /// Size, defense, drop level and durability are the ones of the season 20 client's item data. As this server has no
    /// fourth classes and no level 800, they need level 400 and the third class, like the third wings, whose excellent
    /// options, additional options and growth per level they share. The Cloak of Isolation belongs to the Grow Lancer,
    /// which doesn't exist here, so it's a second cloak of the Fist Master.
    /// </remarks>
    /// <param name="thirdWingOptions">The excellent options of the third wings.</param>
    /// <returns>The created items. Fourth wings which already exist are not created again.</returns>
    private IReadOnlyList<ItemDefinition> CreateFourthWings(ItemOptionDefinition thirdWingOptions)
    {
        var created = new List<ItemDefinition?>
        {
            this.CreateFourthWing(414, 4, LocalizedString.FromResource(() => ItemNames.WingsOfElysium), 155, CharacterClasses.GrandMaster, this.BuildOptions((0b00, OptionType.HealthRecover), (0b11, OptionType.WizDamage), (0b10, OptionType.Defense)), thirdWingOptions),
            this.CreateFourthWing(415, 4, LocalizedString.FromResource(() => ItemNames.WingsOfFlameGod), 170, CharacterClasses.BladeMaster, this.BuildOptions((0b00, OptionType.HealthRecover), (0b11, OptionType.PhysDamage), (0b10, OptionType.Defense)), thirdWingOptions, MovementSpeedConstants.FastWingMovementSpeed),
            this.CreateFourthWing(416, 4, LocalizedString.FromResource(() => ItemNames.WingOfCelestialBody), 170, CharacterClasses.HighElf, this.BuildOptions((0b00, OptionType.HealthRecover), (0b11, OptionType.PhysDamage), (0b10, OptionType.Defense)), thirdWingOptions),
            this.CreateFourthWing(417, 4, LocalizedString.FromResource(() => ItemNames.WingsOfAnnihilation), 170, CharacterClasses.DuelMaster, this.BuildOptions((0b00, OptionType.HealthRecover), (0b11, OptionType.PhysDamage), (0b10, OptionType.WizDamage)), thirdWingOptions),
            this.CreateFourthWing(418, 2, LocalizedString.FromResource(() => ItemNames.CloakOfDominator), 155, CharacterClasses.LordEmperor, this.BuildOptions((0b00, OptionType.HealthRecover), (0b11, OptionType.PhysDamage), (0b10, OptionType.Defense)), thirdWingOptions),
            this.CreateFourthWing(419, 4, LocalizedString.FromResource(() => ItemNames.WingsOfOtherWorld), 170, CharacterClasses.DimensionMaster, this.BuildOptions((0b00, OptionType.HealthRecover), (0b11, OptionType.WizDamage), (0b10, OptionType.CurseDamage)), thirdWingOptions),
            this.CreateFourthWing(420, 2, LocalizedString.FromResource(() => ItemNames.CloakOfJudgment), 170, CharacterClasses.FistMaster, this.BuildOptions((0b00, OptionType.HealthRecover), (0b11, OptionType.PhysDamage), (0b10, OptionType.Defense)), thirdWingOptions),
            this.CreateFourthWing(421, 2, LocalizedString.FromResource(() => ItemNames.CloakOfIsolation), 170, CharacterClasses.FistMaster, this.BuildOptions((0b00, OptionType.HealthRecover), (0b11, OptionType.PhysDamage), (0b10, OptionType.Defense)), thirdWingOptions),
        };

        return created.OfType<ItemDefinition>().ToList();
    }

    private ItemDefinition? CreateFourthWing(short number, byte width, LocalizedString name, int defense, CharacterClasses characterClasses, IEnumerable<IncreasableItemOption> possibleOptions, ItemOptionDefinition thirdWingOptions, float movementSpeed = MovementSpeedConstants.DefaultWingMovementSpeed)
    {
        if (this.GameConfiguration.Items.Any(item => item.Group == 12 && item.Number == number))
        {
            return null;
        }

        return this.CreateWing(number, width, 3, name, 153, defense, 220, 400, characterClasses, possibleOptions, FourthWingDamageIncrease, FourthWingDamageAbsorb, this._damageIncreaseByLevelTable!, thirdWingOptions, movementSpeed);
    }

    private ItemDefinition? CreateSmallWing(byte number, byte width, byte height, LocalizedString name, int defense, CharacterClasses characterClasses, int damageIncreaseAndAbsorbInitial)
    {
        if (this.GameConfiguration.Items.Any(item => item.Group == 12 && item.Number == number))
        {
            return null;
        }

        var wing = this.CreateWing(number, width, height, name, 1, defense, 200, 1, characterClasses);
        this.AddDamagePowerUps(wing, damageIncreaseAndAbsorbInitial, damageIncreaseAndAbsorbInitial, this._damageIncreaseByLevelTable);
        return wing;
    }

    private void CreateFeather()
    {
        var feather = this.Context.CreateNew<ItemDefinition>();
        feather.Name = LocalizedString.FromResource(() => ItemNames.LochSFeather);
        feather.MaximumItemLevel = 1;
        feather.Number = 14;
        feather.Group = 13;
        feather.DropLevel = 78;
        feather.Width = 1;
        feather.Height = 2;
        feather.Durability = 1;
        feather.SetGuid(feather.Group, feather.Number);
        this.GameConfiguration.Items.Add(feather);
    }

    private void CreateFeatherOfCondor()
    {
        var feather = this.Context.CreateNew<ItemDefinition>();
        feather.Name = LocalizedString.FromResource(() => ItemNames.FeatherOfCondor);
        feather.MaximumItemLevel = 1;
        feather.Number = 53;
        feather.Group = 13;
        feather.DropLevel = 120;
        feather.Width = 1;
        feather.Height = 2;
        feather.Durability = 1;
        feather.SetGuid(feather.Group, feather.Number);
        this.GameConfiguration.Items.Add(feather);
    }

    private void CreateFlameOfCondor()
    {
        var feather = this.Context.CreateNew<ItemDefinition>();
        feather.Name = LocalizedString.FromResource(() => ItemNames.FlameOfCondor);
        feather.MaximumItemLevel = 1;
        feather.Number = 52;
        feather.Group = 13;
        feather.DropLevel = 0;
        feather.Width = 1;
        feather.Height = 2;
        feather.Durability = 1;
        feather.SetGuid(feather.Group, feather.Number);
        this.GameConfiguration.Items.Add(feather);
    }

    private void AddDamagePowerUps(ItemDefinition wing, int damageIncreaseInitial, int damageAbsorbInitial, ItemLevelBonusTable? damageIncreasePerLevel)
    {
        if (damageAbsorbInitial > 0)
        {
            var powerUp = this.CreateItemBasePowerUpDefinition(Stats.DamageReceiveDecrement, 1f - (damageAbsorbInitial / 100f), AggregateType.Multiplicate);
            powerUp.BonusPerLevelTable = this._absorbByLevelTable;
            wing.BasePowerUpAttributes.Add(powerUp);
        }

        if (damageIncreaseInitial > 0)
        {
            var powerUp = this.CreateItemBasePowerUpDefinition(Stats.AttackDamageIncrease, 1f + (damageIncreaseInitial / 100f), AggregateType.Multiplicate);
            powerUp.BonusPerLevelTable = damageIncreasePerLevel;
            wing.BasePowerUpAttributes.Add(powerUp);
        }
    }

    private ItemDefinition CreateWing(short number, byte width, byte height, LocalizedString name, byte dropLevel, int defense, byte durability, int levelRequirement, CharacterClasses characterClasses, IEnumerable<IncreasableItemOption> possibleOptions, int damageIncreaseInitial, int damageAbsorbInitial, ItemLevelBonusTable damageIncreasePerLevel, ItemOptionDefinition? wingOptionDefinition, float movementSpeed = MovementSpeedConstants.DefaultWingMovementSpeed)
    {
        var wing = this.CreateWing(number, width, height, name, dropLevel, defense, durability, levelRequirement, characterClasses, movementSpeed);
        if (wingOptionDefinition != null)
        {
            wing.PossibleItemOptions.Add(wingOptionDefinition);
        }

        this.AddDamagePowerUps(wing, damageIncreaseInitial, damageAbsorbInitial, damageIncreasePerLevel);

        var optionDefinition = this.Context.CreateNew<ItemOptionDefinition>();
        optionDefinition.SetGuid(wing.GetItemId());
        this.GameConfiguration.ItemOptions.Add(optionDefinition);

        optionDefinition.Name = number switch
        {
            0 => LocalizedString.FromResource(() => ItemOptionNames.WingsOfElfOptions),
            1 => LocalizedString.FromResource(() => ItemOptionNames.WingsOfHeavenOptions),
            2 => LocalizedString.FromResource(() => ItemOptionNames.WingsOfSatanOptions),
            41 => LocalizedString.FromResource(() => ItemOptionNames.WingsOfCurseOptions),
            3 => LocalizedString.FromResource(() => ItemOptionNames.WingsOfSpiritsOptions),
            4 => LocalizedString.FromResource(() => ItemOptionNames.WingsOfSoulOptions),
            5 => LocalizedString.FromResource(() => ItemOptionNames.WingsOfDragonOptions),
            6 => LocalizedString.FromResource(() => ItemOptionNames.WingsOfDarknessOptions),
            42 => LocalizedString.FromResource(() => ItemOptionNames.WingsOfDespairOptions),
            49 => LocalizedString.FromResource(() => ItemOptionNames.CapeOfFighterOptions),
            30 => LocalizedString.FromResource(() => ItemOptionNames.CapeOfLordOptions),
            36 => LocalizedString.FromResource(() => ItemOptionNames.WingOfStormOptions),
            37 => LocalizedString.FromResource(() => ItemOptionNames.WingOfEternalOptions),
            38 => LocalizedString.FromResource(() => ItemOptionNames.WingOfIllusionOptions),
            39 => LocalizedString.FromResource(() => ItemOptionNames.WingOfRuinOptions),
            40 => LocalizedString.FromResource(() => ItemOptionNames.CapeOfEmperorOptions),
            43 => LocalizedString.FromResource(() => ItemOptionNames.WingOfDimensionOptions),
            50 => LocalizedString.FromResource(() => ItemOptionNames.CapeOfOverruleOptions),
            262 => LocalizedString.FromResource(() => ItemOptionNames.CloakOfDeathOptions),
            263 => LocalizedString.FromResource(() => ItemOptionNames.WingsOfChaosOptions),
            264 => LocalizedString.FromResource(() => ItemOptionNames.WingsOfMagicOptions),
            265 => LocalizedString.FromResource(() => ItemOptionNames.WingsOfLifeOptions),
            414 => LocalizedString.FromResource(() => ItemOptionNames.WingsOfElysiumOptions),
            415 => LocalizedString.FromResource(() => ItemOptionNames.WingsOfFlameGodOptions),
            416 => LocalizedString.FromResource(() => ItemOptionNames.WingOfCelestialBodyOptions),
            417 => LocalizedString.FromResource(() => ItemOptionNames.WingsOfAnnihilationOptions),
            418 => LocalizedString.FromResource(() => ItemOptionNames.CloakOfDominatorOptions),
            419 => LocalizedString.FromResource(() => ItemOptionNames.WingsOfOtherWorldOptions),
            420 => LocalizedString.FromResource(() => ItemOptionNames.CloakOfJudgmentOptions),
            421 => LocalizedString.FromResource(() => ItemOptionNames.CloakOfIsolationOptions),
            _ => new LocalizedString($"{name.ValueInNeutralLanguage} Options"),
        };
        optionDefinition.AddChance = 0.25f;
        optionDefinition.AddsRandomly = true;
        optionDefinition.MaximumOptionsPerItem = 1;
        wing.PossibleItemOptions.Add(optionDefinition);
        byte i = 0;
        foreach (var option in possibleOptions)
        {
            i++;
            option.SetGuid(ItemOptionDefinitionNumbers.WingDefense, wing.GetItemId(), i);
            optionDefinition.PossibleOptions.Add(option);
        }

        wing.PossibleItemOptions.Add(this.GameConfiguration.ItemOptions.First(iod => iod.PossibleOptions.Any(o => o?.OptionType == ItemOptionTypes.Luck)));
        return wing;
    }

    private ItemDefinition CreateWing(short number, byte width, byte height, LocalizedString name, byte dropLevel, int defense, byte durability, int levelRequirement, CharacterClasses characterClasses, float movementSpeed = MovementSpeedConstants.DefaultWingMovementSpeed)
    {
        var wing = this.Context.CreateNew<ItemDefinition>();
        this.GameConfiguration.Items.Add(wing);
        wing.Group = 12;
        wing.Number = number;
        wing.Width = width;
        wing.Height = height;
        wing.Name = name;
        wing.DropLevel = dropLevel;
        wing.MaximumItemLevel = 15;
        wing.DropsFromMonsters = false;
        wing.Durability = durability;
        wing.ItemSlot = this.GameConfiguration.ItemSlotTypes.First(st => st.ItemSlots.Contains(7));
        wing.SetGuid(wing.Group, wing.Number);

        //// TODO: each level increases the requirement by 5 Levels
        this.CreateItemRequirementIfNeeded(wing, Stats.Level, levelRequirement);

        if (defense > 0)
        {
            var powerUp = this.CreateItemBasePowerUpDefinition(Stats.DefenseBase, defense, AggregateType.AddRaw);
            wing.BasePowerUpAttributes.Add(powerUp);
            powerUp.BonusPerLevelTable = levelRequirement == 400 ? this._defenseBonusByLevelTableThird : this._defenseBonusByLevelTable;
        }

        var classes = this.GameConfiguration.DetermineCharacterClasses(characterClasses, ignoreMissing: true);
        foreach (var characterClass in classes)
        {
            wing.QualifiedCharacters.Add(characterClass);
        }

        // add CanFly Attribute to all wings
        var canFlyPowerUp = this.Context.CreateNew<ItemBasePowerUpDefinition>();
        canFlyPowerUp.TargetAttribute = Stats.CanFly.GetPersistent(this.GameConfiguration);
        canFlyPowerUp.BaseValue = 1;
        wing.BasePowerUpAttributes.Add(canFlyPowerUp);

        wing.BasePowerUpAttributes.Add(this.CreateItemBasePowerUpDefinition(Stats.MovementSpeed, movementSpeed, AggregateType.Maximum));
        wing.BasePowerUpAttributes.Add(this.CreateItemBasePowerUpDefinition(Stats.MovementSpeedUnderwater, movementSpeed, AggregateType.Maximum));

        return wing;
    }

    private ItemOptionDefinition CreateCapeOptions()
    {
        var definition = this.CreateSecondClassWingOptions();
        definition.SetGuid(ItemOptionDefinitionNumbers.Cape);
        definition.Name = LocalizedString.FromResource(() => ItemOptionNames.CapeOfLordOptions);
        definition.PossibleOptions.Add(this.CreateWingOption(4, Stats.TotalLeadership, 10f, AggregateType.AddRaw, 5f)); // Increase Command +10~85. Increases your Command by 10 plus 5 for each level. Only Cape of Lord can have it (PvM, PvP)
        this.GameConfiguration.ItemOptions.Add(definition);
        foreach (var option in definition.PossibleOptions)
        {
            option.SetGuid(ItemOptionDefinitionNumbers.Cape, option.PowerUpDefinition!.TargetAttribute!.Id.ExtractFirstTwoBytes());
        }

        return definition;
    }

    private ItemOptionDefinition CreateSecondClassWingOptions()
    {
        var definition = this.Context.CreateNew<ItemOptionDefinition>();
        definition.SetGuid(ItemOptionDefinitionNumbers.Wing2nd);
        this.GameConfiguration.ItemOptions.Add(definition);
        definition.Name = LocalizedString.FromResource(() => ItemOptionNames.Level2ndWingOptions);
        definition.AddChance = 0.1f;
        definition.AddsRandomly = true;
        definition.MaximumOptionsPerItem = 1;

        // "Excellent" 2nd wing options:
        definition.PossibleOptions.Add(this.CreateWingOption(1, Stats.MaximumHealth, 50f, AggregateType.AddRaw, 5f)); // Increase max HP +50~125. Increases your maximum amount of life by 50 plus 5 for each level (PvM, PvP)
        definition.PossibleOptions.Add(this.CreateWingOption(2, Stats.MaximumMana, 50f, AggregateType.AddRaw, 5f)); // Increase max mana +50~125. Increases your maximum amount of mana by 50 plus 5 for each level (PvM, PvP)
        definition.PossibleOptions.Add(this.CreateWingOption(3, Stats.DefenseIgnoreChance, 0.03f, AggregateType.AddRaw)); // Ignore opponent's defensive power by 3%. Gives you 3% chance to bypass your opponent's defense for a strike. This strike is shown with cyan colour (PvM, PvP)

        return definition;
    }

    private ItemOptionDefinition CreateThirdClassWingOptions()
    {
        var definition = this.Context.CreateNew<ItemOptionDefinition>();
        definition.SetGuid(ItemOptionDefinitionNumbers.Wing3rd);
        this.GameConfiguration.ItemOptions.Add(definition);
        definition.Name = LocalizedString.FromResource(() => ItemOptionNames.Level3rdWingOptions);
        definition.AddChance = 0.1f;
        definition.AddsRandomly = true;
        definition.MaximumOptionsPerItem = 1;

        definition.PossibleOptions.Add(this.CreateWingOption(1, Stats.DefenseIgnoreChance, 0.05f, AggregateType.AddRaw)); // Ignore opponent's defensive power by 5%
        definition.PossibleOptions.Add(this.CreateWingOption(2, Stats.FullyReflectDamageAfterHitChance, 0.05f, AggregateType.AddRaw)); // Fully reflect the damage when hit by 5%
        definition.PossibleOptions.Add(this.CreateWingOption(3, Stats.FullyRecoverHealthAfterHitChance, 0.05f, AggregateType.AddRaw)); // Fully restore health when hit by 5%
        definition.PossibleOptions.Add(this.CreateWingOption(4, Stats.FullyRecoverManaAfterHitChance, 0.05f, AggregateType.AddRaw)); // Fully recover mana when hit by 5%

        return definition;
    }

    private IncreasableItemOption CreateWingOption(byte number, AttributeDefinition attributeDefinition, float value, AggregateType aggregateType, float? valueIncrementPerLevel = null)
    {
        var itemOption = this.Context.CreateNew<IncreasableItemOption>();
        itemOption.SetGuid(ItemOptionDefinitionNumbers.Wing2nd, attributeDefinition.Id.ExtractFirstTwoBytes(), number);
        itemOption.OptionType = this.GameConfiguration.ItemOptionTypes.First(t => t == ItemOptionTypes.Wing);
        itemOption.Number = number;
        var targetAttribute = attributeDefinition.GetPersistent(this.GameConfiguration);
        itemOption.PowerUpDefinition = this.CreatePowerUpDefinition(targetAttribute, value, aggregateType);

        if (valueIncrementPerLevel.HasValue)
        {
            itemOption.LevelType = LevelType.ItemLevel;
            for (int level = 1; level <= this.MaximumItemLevel; level++)
            {
                var optionOfLevel = this.Context.CreateNew<ItemOptionOfLevel>();
                optionOfLevel.Level = level;
                optionOfLevel.PowerUpDefinition = this.CreatePowerUpDefinition(targetAttribute, value + (level * valueIncrementPerLevel.Value), aggregateType);
                itemOption.LevelDependentOptions.Add(optionOfLevel);
            }
        }

        return itemOption;
    }
}
