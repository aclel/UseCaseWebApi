# Web api built around use cases and statically typed SQL

This is an example web api that does not use an ORM. The object relational mapping problem is hard. Let's avoid it completely and use the database as the model.

* ASP .NET Core targeting NET Framework 4.6.1
* [Giraffe](https://github.com/dustinmoris/Giraffe)
* [FSharp.Data.SqlClient](http://fsprojects.github.io/FSharp.Data.SqlClient/)
* [Railway oriented programming](https://fsharpforfunandprofit.com/posts/recipe-part2/) for validation

## Getting Started

* Set up the database
	* Run schema.sql against a SQL server or Azure SQL database 
* Open the project in VS 2017 Preview (2)
* Add the connection string for the database to BasinUseCases.fs
* Build

## Explanation

### Types for each feature

Instead of creating lots of types that are used throughout the app, types are defined within the use case or feature.

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

### Raw SQL

All of the database interaction is done through raw SQL. There's more code to write and maintain, but the SQL is statically typed. If the schema changes or a bad query is written, the code will break at compile time. If we update the schema, all of the broken SQL statements are flagged as an error by intellisense so we can go through and fix them all.
