# C2 F5 0F - JewelBankStateExtended (by server)

## Is sent when

The open source client requested the state of the jewel bank, the player deposited or withdrew jewels, or the player opened the jewel bank with its chat command.

## Causes the following actions on the client side

The client shows the stored amounts in its jewel bank window, and opens the window when requested.

## Structure

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 |   Byte   | 0xC2  | [Packet type](PacketTypes.md) |
| 1 | 2 |    Short   |      | Packet header - length of the packet |
| 3 | 1 |    Byte   | 0xF5  | Packet header - packet type identifier |
| 4 | 1 |    Byte   | 0x0F  | Packet header - sub packet type identifier |
| 5 | 1 | Boolean |  | OpenWindow; If true, the client opens the jewel bank window. |
| 6 | 1 | Byte |  | Count |
| 7 | JewelBankEntry.Length * Count | Array of JewelBankEntry |  | Entries |

### JewelBankEntry Structure

A jewel which the jewel bank accepts, with its stored amount. Every accepted jewel is listed, also when none is stored.

Length: 8 Bytes

| Index | Length | Data Type | Value | Description |
|-------|--------|-----------|-------|-------------|
| 0 | 1 | Byte |  | ItemGroup; The group of the single jewel's item definition. |
| 2 | 2 | ShortLittleEndian |  | ItemNumber; The number of the single jewel's item definition. |
| 4 | 4 | IntegerLittleEndian |  | Amount; The number of stored single jewels. |