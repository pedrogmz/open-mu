# C1 F5 14 - JewelBankDepositAllRequest (by client)

## Is sent when

The player deposits all jewels in the jewel bank window.

## Causes the following actions on the server side

The server moves every jewel which the jewel bank accepts from the inventory into the jewel bank of the account, and answers with the JewelBankStateExtended.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |   4   | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0xF5  | Packet header - packet type identifier |
| 3 | 1 |    Byte   | 0x14  | Packet header - sub packet type identifier |