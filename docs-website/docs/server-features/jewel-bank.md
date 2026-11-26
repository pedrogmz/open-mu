---
title: Jewel bank
sidebar_label: Jewel bank
sidebar_position: 7
description: Every account can store jewels by their number, so they take no space in the vault or the inventory.
---

The jewel bank stores the jewels of an account by their number instead of as
items. Any amount of them takes no space in the vault or the inventory, and all
characters of the account share it. The original game doesn't have it, so it's
off by default.

The open source client ([MuMain](https://github.com/sven-n/MuMain)) shows the
bank in its own window beside the inventory. Other clients can't use it.

## Activation

Activate these plugins in the admin panel (*Plugins*):

* **Jewel bank**, the feature itself, with its settings;
* **Chat command: /banco (jewel bank)**, if players should open the window with
  the chat command.

The packet handlers and the view of the jewel bank are active by default, and
they do nothing while the feature is off.

### Settings

| Setting | Default | Meaning |
|---|---|---|
| Jewels | Bless, Soul, Chaos, Life, Creation, Guardian, Harmony, Gemstone, lower and higher refining stone | The jewels the bank accepts, by the group and number of the single jewel |
| Maximum amount per jewel | 2,147,483,647 | The most single jewels of one kind an account stores; 0 means no limit |

A configured jewel which the game configuration doesn't have is left out.

## Deposit and withdrawal

* **Deposit** moves every jewel of one kind from the inventory into the bank:
  the single jewels and their bundles. **Deposit all** does that for every
  accepted jewel. The equipped items and the personal store aren't touched.
* A bundle is stored as the number of jewels it holds: 10 for level 0, 20 for
  level 1 and 30 for level 2, like the jewel mix of Lahap combines them. The
  bundle of a jewel is the one of its jewel mix in the game configuration.
* **Withdraw** asks for an amount and puts it into the inventory as few items as
  possible: bundles of 30, 20 and 10 first, then single jewels. A full inventory
  stops the withdrawal; what didn't fit stays in the bank.

Each item is taken from the inventory or the bank only once it was added to the
other side, so a disconnect in between neither loses nor duplicates jewels:
both are saved together with the character.

Deposits and withdrawals are only possible while the character is in the game
world without a dialog, e.g. no trade, vault or merchant.

## Messages

| Message | When |
|---|---|
| You have no jewels to deposit. | The inventory has no jewel the deposit asked for |
| The jewel bank can't store more of this jewel. | A deposit would exceed the maximum amount; the rest stays in the inventory |
| The jewel bank doesn't have that many jewels. | A withdrawal asked for more than is stored |
| Inventory got not enough space. | A withdrawal stopped because the inventory is full |

## Protocol

The messages are OpenMU's own, sent only to the open source client:
`JewelBankStateRequest` (C1 F5 0E), `JewelBankDepositRequest` (C1 F5 10),
`JewelBankWithdrawRequest` (C1 F5 12), `JewelBankDepositAllRequest` (C1 F5 14)
and the answer `JewelBankStateExtended` (C2 F5 0F). See the
[packet documentation](../reference/packets.md).
