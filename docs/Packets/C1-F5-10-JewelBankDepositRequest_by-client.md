# C1 F5 10 - JewelBankDepositRequest (by client)

## Is sent when

The player deposits one kind of jewel in the jewel bank window.

## Causes the following actions on the server side

The server moves every jewel of this kind from the inventory into the jewel bank of the account, the single jewels and their bundles, and answers with the JewelBankStateExtended.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |   8   | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0xF5  | Packet header - packet type identifier |
| 3 | 1 |    Byte   | 0x10  | Packet header - sub packet type identifier |
| 4 | 1 | Byte |  | ItemGroup; The group of the single jewel's item definition. |
| 6 | 2 | ShortLittleEndian |  | ItemNumber; The number of the single jewel's item definition. |