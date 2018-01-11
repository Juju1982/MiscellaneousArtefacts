using System.Net.Http;
using System.Threading.Tasks;

namespace GregWebServices
{
    public interface IWebServiceHelper
    {
        //T Call<T>(HttpMethod verb, string url, object obj);
        Task<T> CallAsync<T>(HttpMethod verb, string url, object obj);
        //T Call<T>(HttpMethod verb, string url, string body);
        //Task<T> CallAsync<T>(HttpMethod verb, string url, string body);
        ////T Call<T>(HttpMethod verb, string url);
        //Task<T> CallAsync<T>(HttpMethod verb, string url);
        //string Call(HttpMethod verb, string url);
        //Task<string> CallAsync(HttpMethod verb, string url);
        //string Call(HttpMethod verb, string url, object obj);
        //string Call(HttpMethod verb, string url, string body);
        //Task<string> CallAsync(HttpMethod verb, string url, object obj);
        //Task<string> CallAsync(HttpMethod verb, string url, string body);
    }
}