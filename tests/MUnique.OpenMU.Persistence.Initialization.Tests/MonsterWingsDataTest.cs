// <copyright file="MonsterWingsDataTest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Tests;

using Microsoft.Extensions.Logging.Abstractions;
using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.DataModel.Configuration.Items;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.Persistence.Initialization.CharacterClasses;
using MUnique.OpenMU.Persistence.Initialization.Items;
using MUnique.OpenMU.Persistence.Initialization.Updates;
using MUnique.OpenMU.Persistence.InMemory;

/// <summary>
/// Tests the monster wings (12,262–265) of the season 6 data.
/// </summary>
[TestFixture]
internal class MonsterWingsDataTest
{
    private static readonly short[] MonsterWingNumbers = [262, 263, 264, 265];

    /// <summary>
    /// Tests that a new season 6 database contains the monster wings with the values of the client.
    /// </summary>
    [Test]
    public async Task NewDatabaseContainsMonsterWingsAsync()
    {
        var (context, gameConfiguration) = await CreateSeason6ConfigurationAsync().ConfigureAwait(false);
        using var _ = context;

        AssertMonsterWings(gameConfiguration);
    }

    /// <summary>
    /// Tests that the update adds the monster wings to a database which was created before they existed,
    /// reusing the level bonus tables and excellent options of the second wings, and that applying it twice doesn't add them twice.
    /// </summary>
    [Test]
    public async Task UpdateAddsMonsterWingsToExistingDatabaseAsync()
    {
        var (context, gameConfiguration) = await CreateSeason6ConfigurationAsync().ConfigureAwait(false);
        using var _ = context;
        foreach (var wing in gameConfiguration.Items.Where(IsMonsterWing).ToList())
        {
            gameConfiguration.Items.Remove(wing);
            foreach (var option in gameConfiguration.ItemOptions.Where(o => o.GetId() == GuidHelper.CreateGuid<ItemOptionDefinition>(wing.GetItemId())).ToList())
            {
                gameConfiguration.ItemOptions.Remove(option);
            }
        }

        var bonusTableCount = gameConfiguration.ItemLevelBonusTables.Count;
        var itemOptionCount = gameConfiguration.ItemOptions.Count;

        var update = new AddMonsterWingsUpdatePlugIn();
        await update.ApplyUpdateAsync(context, gameConfiguration).ConfigureAwait(false);
        await update.ApplyUpdateAsync(context, gameConfiguration).ConfigureAwait(false);

        Assert.That(gameConfiguration.Items.Count(IsMonsterWing), Is.EqualTo(MonsterWingNumbers.Length));
        Assert.That(gameConfiguration.ItemLevelBonusTables, Has.Count.EqualTo(bonusTableCount));

        // Each wing gets its own option definition (its additional option), the excellent options are shared.
        Assert.That(gameConfiguration.ItemOptions, Has.Count.EqualTo(itemOptionCount + MonsterWingNumbers.Length));
        AssertMonsterWings(gameConfiguration);
    }

    private static void AssertMonsterWings(GameConfiguration gameConfiguration)
    {
        AssertMonsterWing(gameConfiguration, 262, "Cloak of Death", 2, 27, BuiltInItemStatProfiles.SecondTierCape, CharacterClassNumber.DarkLord, CharacterClassNumber.RageFighter);
        AssertMonsterWing(gameConfiguration, 263, "Wings of Chaos", 4, 46, BuiltInItemStatProfiles.SecondTierWing, CharacterClassNumber.BladeKnight, CharacterClassNumber.MagicGladiator);
        AssertMonsterWing(gameConfiguration, 264, "Wings of Magic", 4, 37, BuiltInItemStatProfiles.SecondTierWing, CharacterClassNumber.SoulMaster, CharacterClassNumber.MagicGladiator, CharacterClassNumber.BloodySummoner);
        AssertMonsterWing(gameConfiguration, 265, "Wings of Life", 4, 37, BuiltInItemStatProfiles.SecondTierWing, CharacterClassNumber.MuseElf);
    }

    private static bool IsMonsterWing(ItemDefinition item)
    {
        return item.Group == 12 && MonsterWingNumbers.Contains(item.Number);
    }

    private static void AssertMonsterWing(GameConfiguration gameConfiguration, short number, string name, byte width, float defense, string statProfile, params CharacterClassNumber[] classes)
    {
        var item = gameConfiguration.Items.Single(i => i.Group == 12 && i.Number == number);
        var wingsOfSpirits = gameConfiguration.Items.Single(i => i is { Group: 12, Number: 3 });
        var secondWingOptions = wingsOfSpirits.PossibleItemOptions.Single(o => o.GetId() == GuidHelper.CreateGuid<ItemOptionDefinition>(ItemOptionDefinitionNumbers.Wing2nd));

        Assert.Multiple(() =>
        {
            Assert.That(item.Name.ValueInNeutralLanguage, Is.EqualTo(name));
            Assert.That((item.Width, item.Height), Is.EqualTo((width, (byte)3)));
            Assert.That(item.DropLevel, Is.EqualTo(150));
            Assert.That(item.Durability, Is.EqualTo(210));
            Assert.That(item.ItemSlot, Is.SameAs(wingsOfSpirits.ItemSlot));
            Assert.That(item.Requirements.Single(r => r.Attribute == Stats.Level).MinimumValue, Is.EqualTo(290));
            Assert.That(item.QualifiedCharacters.Select(c => (CharacterClassNumber)c.Number), Is.SupersetOf(classes));
            Assert.That(item.PossibleItemOptions, Does.Contain(secondWingOptions));
            Assert.That(item.StatProfile?.Name, Is.EqualTo(statProfile));

            Assert.That(GetPowerUp(item, Stats.DefenseBase).BaseValue, Is.EqualTo(defense));
            Assert.That(GetPowerUp(item, Stats.AttackDamageIncrease).BaseValue, Is.EqualTo(1f + (32 / 100f)));
            Assert.That(GetPowerUp(item, Stats.DamageReceiveDecrement).BaseValue, Is.EqualTo(1f - (25 / 100f)));
            Assert.That(GetPowerUp(item, Stats.CanFly).BaseValue, Is.EqualTo(1));

            // The bonus per level is the same as for the second wings.
            foreach (var attribute in new[] { Stats.DefenseBase, Stats.AttackDamageIncrease, Stats.DamageReceiveDecrement })
            {
                Assert.That(GetPowerUp(item, attribute).BonusPerLevelTable, Is.Not.Null.And.SameAs(GetPowerUp(wingsOfSpirits, attribute).BonusPerLevelTable), attribute.Designation);
            }
        });
    }

    private static ItemBasePowerUpDefinition GetPowerUp(ItemDefinition item, AttributeDefinition attribute)
    {
        return item.BasePowerUpAttributes.Single(p => p.TargetAttribute == attribute);
    }

    private static async Task<(IContext Context, GameConfiguration GameConfiguration)> CreateSeason6ConfigurationAsync()
    {
        var contextProvider = new InMemoryPersistenceContextProvider();
        var dataInitialization = new VersionSeasonSix.DataInitialization(contextProvider, new NullLoggerFactory());
        await dataInitialization.CreateInitialDataAsync(1, true).ConfigureAwait(false);

        var context = contextProvider.CreateNewContext();
        var gameConfiguration = (await context.GetAsync<GameConfiguration>().ConfigureAwait(false)).First();
        return (context, gameConfiguration);
    }
}
