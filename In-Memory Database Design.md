# In-Memory Database Design

This is a work in progress document to track the requirements needed to replace the current internal model with either a temporary physical database or an in-memory database.

## Current State

Right now, the core model is tracked across 3 separate files: `SabreTools.DatFiles\DatFile.cs`, `SabreTools.DatFiles\DatHeader.cs`, and `SabreTools.DatFiles\ItemDictionary.cs`. There is a work-in-progress file at `SabreTools.DatFiles\ItemDictionaryDB.cs`, but for now, that should be ignored for the purposes of this document.

### `DatFile.cs`

`DatFile.cs` mainly contains the DAT-level commonalities between all of the formats, which actually doesn't include as much as you may think. Most of the methods in there would be considered utilities and not much more.

In all likelihood, `DatFile.cs` would not change in any appreciable way as a result of model updates elsewhere.

### `DatHeader.cs`

Similar to `DatFile.cs`, `DatHeader.cs` is probably not going to change in any appreciable way. That being said, depending on how far into a database model we get, the header for each read and written DAT may actually be good to store in an in-memory database just to make it easier to track, especially since basically everything else will be in that database as well.

Porting this to a database model is far more straightforward, as the set of fields is much smaller than any single item.

### `ItemDictionary.cs`

This is where basically all of the changes will need to be made. And they have to be made in such a way that they're "invisible" to the rest of the code. The current code relies VERY heavily on the fact that this is a concurrently readable and writable dictionary. Normally, that would make this easier, but it definitely does not.

There are at least 2 database tables needed in order to track this data, as far as I can tell: one table to hold the item data (possibly based on the combined model that the generic XML and JSON outputs employ) and one table to hold the item-to-source mappings. It's highly unclear to me if another table is needed for keys as well, or if it will actually be easier to determine the keys with the database model (just select a different column).

There's another hard part here: should there be separate tables for each item type? This would severely reduce the model for any individual table BUT it would increase the complexity when it comes to writing data since all of the tables would have to be queried.

## Design

This section is reserved for database design ideas.