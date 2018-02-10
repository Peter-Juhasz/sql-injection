using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace PeterJuhasz.SqlInjection
{
    public class PhpErrorDetector : IErrorDetector
    {
        public async Task<bool> ContainsErrorAsync(HttpResponseMessage response)
        {
            if (response.Content == null)
                return false;

            string content = await response.Content.ReadAsStringAsync();
            return content.Contains("SQLSTATE") || content.Contains("SQL error");
        }
    }
}
