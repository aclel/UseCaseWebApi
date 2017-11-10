# Web api built around use cases and statically typed SQL

The object relational mapping problem is hard. Let's avoid it completely and use the database as the model.

Instead of creating lots of types that are used throughout the app, types are defined within the use case or feature.

Instead of creating a Basin type and using it throughout the app we can define types that are used for each use case:

```F#
type BasinId = int
type Name = string
type Code = string

// Instead of this
type Basin = {
	BasinId: BasinId;
	Code: Code;
	Name: Name;
}


// we do this:
type CreateBasinRequest = {
	Code: Code;
	Name: Name;
}

type UpdateBasinRequest = {
	BasinId: BasinId;
	Code: Code;
	Name: Name;
}

type GetBasinResponse = {
	BasinId: BasinId;
	Code: Code;
	Name: Name;
}
```

All of the database interaction is done through raw SQL. There's more code to write and maintain, but the SQL is statically typed. If the schema changes or a bad query is written, the code will break at compile time. If we update the schema, all of the broken SQL statements are flagged as an error by intellisense so we can go through and fix them all.