# C1 F5 0E - JewelBankStateRequest (by client)

## Is sent when

The open source client opens the jewel bank window, e.g. through its button in the inventory.

## Causes the following actions on the server side

The server sends the JewelBankStateExtended with the stored amount of each jewel which the jewel bank accepts.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC1  | [Packet type](PacketTypes.md) |
| 1 | 1 |    Byte   |   4   | Packet header - length of the packet |
| 2 | 1 |    Byte   | 0xF5  | Packet header - packet type identifier |
| 3 | 1 |    Byte   | 0x0E  | Packet header - sub packet type identifier |