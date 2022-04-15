using FStarMES.Data;
using FStarMES.Shared;
using Microsoft.AspNetCore.Mvc;

namespace FStarMES.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class FStarMESController : ControllerBase
    {
        MyDbContext Context;
        [HttpGet]
        public IActionResult Connect()
        {
            return new OkObjectResult("OK");
        }
        [HttpPost]
        public async Task<IActionResult> Add(MachineStatePost req)
        {
            var r = await Task.Run(() => {
                try
                {
                    Context.Add<MachineState>(new MachineState()
                    {
                        Line = req.Line,
                        Station = req.Station,
                        Count = req.Count,
                        TotalCount = req.TotalCount,
                        StateStatus = req.StateStatus,
                        StateDetail = req.StateDetail,
                        ProductType = req.ProductType,
                        CreationTime = DateTime.Now
                    });
                    Context.SaveChanges();
                    return "Success";
                }
                catch (Exception ex)
                {
                    return ex.Message;
                }
            });
            if (r == "Success")
            {
                return new OkObjectResult("OK");
            }
            else
            {
                return new ObjectResult(r);
            }
            
        }
        public List<LineSeting> GetAllSettings()
        {
            return Context.Set<LineSeting>().ToList();
        }
        public FStarMESController(MyDbContext myDbContext)
        {
            Context = myDbContext;
        }
    }    
}
