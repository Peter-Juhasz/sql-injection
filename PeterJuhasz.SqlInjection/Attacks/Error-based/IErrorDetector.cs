using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace PeterJuhasz.SqlInjection
{
    public interface IErrorDetector
    {
        Task<bool> ContainsErrorAsync(HttpResponseMessage response);
    }
}
