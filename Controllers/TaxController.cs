using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace ConsumerTaxes.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TaxController : ControllerBase
    {
        protected readonly IConfiguration Configuration;
        private DateTime datetax;
        private string urlApi;
        private dynamic apiResponse;
        public TaxController(IConfiguration configuration)
        {
            Configuration = configuration;
            urlApi = Configuration.GetConnectionString("API");
        }

        [HttpGet("FindTax")]
        public async Task<IActionResult> FindTax(string Municipality, string DateTax)
        {
            try
            {
                if (!DateTime.TryParse(DateTax, out datetax))
                    return BadRequest();

                using (var httpClient = new HttpClient())
                {
                    using (var response = await httpClient.GetAsync($"{urlApi}Tax/FindTax?Municipality={Municipality}&DateTax={DateTax}"))
                    {
                        if (!response.IsSuccessStatusCode)
                            return BadRequest();

                        apiResponse = JsonConvert.DeserializeObject(await response.Content.ReadAsStringAsync());
                    }
                }

                return Ok($"Tax is:{apiResponse.value}, at {Municipality} during {DateTax}");
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new JsonResult(ex));
            }
        }
    }
}
