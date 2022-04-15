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
        public void AddSetting(AddSettingReq req)
        {
            int id = 1;
            if (Context.Set<LineSeting>().Count() > 0)
            {
                id = Context.Set<LineSeting>().ToList().Max(t => t.Id) + 1;
            }
            Context.Add<LineSeting>(new LineSeting() { 
                Id = id,
                Line = req.Line,
                GoalCount = req.GoalCount,
                ShiftType = req.ShiftType
            });
            Context.SaveChanges();
        }
        public void DeletSetting(DeletSettingReq req)
        {
            var item = Context.Set<LineSeting>().FirstOrDefault(t => t.Id == req.Id);
            if (item != null)
            {
                Context.Set<LineSeting>().Remove(item);
                Context.SaveChanges();
            }
        }
        public LineSeting? GetSetting(GetSettingReq req)
        {
            return Context.Set<LineSeting>().FirstOrDefault(t => t.Id == req.Id);
        }
        public void UpdateSetting(UpdateSettingReq req)
        {
            var setting = Context.Set<LineSeting>().FirstOrDefault(t => t.Id == req.Id);
            if (setting != null)
            {
                setting.GoalCount = req.GoalCount;
                Context.SaveChanges();
            }
        }
        public FStarMESController(MyDbContext myDbContext)
        {
            Context = myDbContext;
        }
    }    
}
