// <copyright file="FourthWingsDataTest.cs" company="MUnique">
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
using MUnique.OpenMU.Persistence.Initialization.VersionSeasonSix.Items;
using MUnique.OpenMU.Persistence.InMemory;

/// <summary>
/// Tests the fourth wings of season 13 (12,414–421) of the season 6 data.
/// </summary>
[TestFixture]
internal class FourthWingsDataTest
{
    private static readonly short[] FourthWingNumbers = [414, 415, 416, 417, 418, 419, 420, 421];

    /// <summary>
    /// Tests that a new season 6 database contains the fourth wings.
    /// </summary>
    [Test]
    public async Task NewDatabaseContainsFourthWingsAsync()
    {
        var (context, gameConfiguration) = await CreateSeason6ConfigurationAsync().ConfigureAwait(false);
        using var _ = context;

        AssertFourthWings(gameConfiguration);
    }

    /// <summary>
    /// Tests that the update adds the fourth wings to a database which was created before they existed,
    /// reusing the level bonus tables and excellent options of the third wings, and that applying it twice doesn't add them twice.
    /// </summary>
    [Test]
    public async Task UpdateAddsFourthWingsToExistingDatabaseAsync()
    {
        var (context, gameConfiguration) = await CreateSeason6ConfigurationAsync().ConfigureAwait(false);
        using var _ = context;
        foreach (var wing in gameConfiguration.Items.Where(IsFourthWing).ToList())
        {
            gameConfiguration.Items.Remove(wing);
            foreach (var option in gameConfiguration.ItemOptions.Where(o => o.GetId() == GuidHelper.CreateGuid<ItemOptionDefinition>(wing.GetItemId())).ToList())
            {
                gameConfiguration.ItemOptions.Remove(option);
            }
        }

        var bonusTableCount = gameConfiguration.ItemLevelBonusTables.Count;
        var itemOptionCount = gameConfiguration.ItemOptions.Count;

        var update = new AddFourthWingsUpdatePlugIn();
        await update.ApplyUpdateAsync(context, gameConfiguration).ConfigureAwait(false);
        await update.ApplyUpdateAsync(context, gameConfiguration).ConfigureAwait(false);

        Assert.That(gameConfiguration.Items.Count(IsFourthWing), Is.EqualTo(FourthWingNumbers.Length));
        Assert.That(gameConfiguration.ItemLevelBonusTables, Has.Count.EqualTo(bonusTableCount));

        // Each wing gets its own option definition (its additional option), the excellent options are shared.
        Assert.That(gameConfiguration.ItemOptions, Has.Count.EqualTo(itemOptionCount + FourthWingNumbers.Length));
        AssertFourthWings(gameConfiguration);
    }

    private static void AssertFourthWings(GameConfiguration gameConfiguration)
    {
        AssertFourthWing(gameConfiguration, 414, "Wings of Elysium", 4, 155, CharacterClassNumber.GrandMaster);
        AssertFourthWing(gameConfiguration, 415, "Wings of Flame God", 4, 170, CharacterClassNumber.BladeMaster);
        AssertFourthWing(gameConfiguration, 416, "Wing of Celestial Body", 4, 170, CharacterClassNumber.HighElf);
        AssertFourthWing(gameConfiguration, 417, "Wings of Annihilation", 4, 170, CharacterClassNumber.DuelMaster);
        AssertFourthWing(gameConfiguration, 418, "Cloak of Dominator", 2, 155, CharacterClassNumber.LordEmperor);
        AssertFourthWing(gameConfiguration, 419, "Wings of Other World", 4, 170, CharacterClassNumber.DimensionMaster);
        AssertFourthWing(gameConfiguration, 420, "Cloak of Judgment", 2, 170, CharacterClassNumber.FistMaster);
        AssertFourthWing(gameConfiguration, 421, "Cloak of Isolation", 2, 170, CharacterClassNumber.FistMaster);
    }

    private static bool IsFourthWing(ItemDefinition item)
    {
        return item.Group == 12 && FourthWingNumbers.Contains(item.Number);
    }

    private static void AssertFourthWing(GameConfiguration gameConfiguration, short number, string name, byte width, float defense, CharacterClassNumber characterClass)
    {
        var item = gameConfiguration.Items.Single(i => i.Group == 12 && i.Number == number);
        var wingOfStorm = gameConfiguration.Items.Single(i => i is { Group: 12, Number: 36 });
        var thirdWingOptions = wingOfStorm.PossibleItemOptions.Single(o => o.GetId() == GuidHelper.CreateGuid<ItemOptionDefinition>(ItemOptionDefinitionNumbers.Wing3rd));

        Assert.Multiple(() =>
        {
            Assert.That(item.Name.ValueInNeutralLanguage, Is.EqualTo(name));
            Assert.That((item.Width, item.Height), Is.EqualTo((width, (byte)3)));
            Assert.That(item.DropLevel, Is.EqualTo(153));
            Assert.That(item.Durability, Is.EqualTo(220));
            Assert.That(item.ItemSlot, Is.SameAs(wingOfStorm.ItemSlot));
            Assert.That(item.Requirements.Single(r => r.Attribute == Stats.Level).MinimumValue, Is.EqualTo(400));
            Assert.That(item.QualifiedCharacters.Select(c => (CharacterClassNumber)c.Number), Is.EquivalentTo(new[] { characterClass }));
            Assert.That(item.PossibleItemOptions, Does.Contain(thirdWingOptions));
            Assert.That(item.PossibleItemOptions.Single(o => o.GetId() == GuidHelper.CreateGuid<ItemOptionDefinition>(item.GetItemId())).PossibleOptions, Has.Count.EqualTo(3));
            Assert.That(item.StatProfile?.Name, Is.EqualTo(BuiltInItemStatProfiles.ThirdTierWing));

            Assert.That(GetPowerUp(item, Stats.DefenseBase).BaseValue, Is.EqualTo(defense));
            Assert.That(GetPowerUp(item, Stats.AttackDamageIncrease).BaseValue, Is.EqualTo(1f + (Wings.FourthWingDamageIncrease / 100f)));
            Assert.That(GetPowerUp(item, Stats.DamageReceiveDecrement).BaseValue, Is.EqualTo(1f - (Wings.FourthWingDamageAbsorb / 100f)));
            Assert.That(GetPowerUp(item, Stats.CanFly).BaseValue, Is.EqualTo(1));

            // The bonus per level is the same as for the third wings.
            foreach (var attribute in new[] { Stats.DefenseBase, Stats.AttackDamageIncrease, Stats.DamageReceiveDecrement })
            {
                Assert.That(GetPowerUp(item, attribute).BonusPerLevelTable, Is.Not.Null.And.SameAs(GetPowerUp(wingOfStorm, attribute).BonusPerLevelTable), attribute.Designation);
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
