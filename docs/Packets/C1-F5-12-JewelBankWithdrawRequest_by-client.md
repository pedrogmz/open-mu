# C1 F5 12 - JewelBankWithdrawRequest (by client)

## Is sent when

The player withdraws an amount of one kind of jewel in the jewel bank window.

## Causes the following actions on the server side

The server puts the amount into the inventory, as few items as possible (bundles of the largest size first, then single jewels), when the inventory has the space for them, and answers with the JewelBankStateExtended.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |   12   | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0xF5  | Packet header - packet type identifier |
| 3 | 1 |    Byte   | 0x12  | Packet header - sub packet type identifier |
| 4 | 1 | Byte |  | ItemGroup; The group of the single jewel's item definition. |
| 6 | 2 | ShortLittleEndian |  | ItemNumber; The number of the single jewel's item definition. |
| 8 | 4 | IntegerLittleEndian |  | Amount; The number of single jewels to withdraw; a bundle counts as the number of jewels it holds. |