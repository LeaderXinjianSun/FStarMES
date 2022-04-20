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
        public Task<List<Report>> CheckAsync(DateSelectReq req)
        {
            List<Report> reports = new List<Report>();
            var lineSettings = Context.Set<LineSeting>().Where(t => t.ShiftType == ShiftTypeEnum.Day).ToList();
            foreach (var item1 in lineSettings)
            {
                DateTime startTime = req.DatePicker.Date.AddHours(8);
                if (DateTime.Now < startTime)
                {
                    break;
                }
                DateTime endTime = req.DatePicker.Date.AddHours(20);
                if (DateTime.Now < endTime)
                {
                    endTime = DateTime.Now;
                }
                List<MyState> states = new List<MyState>();
                states.Add(new MyState()
                {
                    Type = 1,
                    State = "运行",
                    Detail = "",
                    Create = startTime
                });
                int addminute = 10;
                for (int i = 1; startTime.AddMinutes(i * addminute) < endTime; i++)
                {
                    DateTime end1 = startTime.AddMinutes(i * addminute);
                    var items = Context.Set<MachineState>().Where(t => t.Line == item1.Line && t.CreationTime > startTime && t.CreationTime <= end1).ToList();
                    if (items.Count == 0)
                    {
                        states.Add(new MyState() { 
                            Type = 4,
                            State = "未开机",
                            Detail = "",
                            Create = startTime.AddMinutes(i * addminute)
                        });
                    }
                    else
                    {
                        var state = items.FirstOrDefault(t => t.StateStatus != StateStatusEnum.Run);
                        if (state != null)
                        {
                            states.Add(new MyState()
                            {
                                Type = (int)state.StateStatus,
                                State = state.StateStatus.ToString(),
                                Detail = state.StateDetail,
                                Create = startTime.AddMinutes(i * addminute)
                            });
                        }
                        else
                        {
                            states.Add(new MyState()
                            {
                                Type = 1,
                                State = "运行",
                                Detail = "",
                                Create = startTime.AddMinutes(i * addminute)
                            });
                        }
                    }
                }
                states.Add(new MyState()
                {
                    Type = 1,
                    State = "运行",
                    Detail = "",
                    Create = endTime
                });
                List<MyStateElapse> stateElapses = new List<MyStateElapse>();
                int[] diff_v = new int[states.Count - 1];
                for (int i = 0; i < states.Count - 1; i++)
                {
                    if (states[i].Type == 1 && states[i + 1].Type > 1)
                    {
                        diff_v[i] = 1;
                    }
                    else if (states[i].Type > 1 && states[i + 1].Type == 1)
                    {
                        diff_v[i] = -1;
                    }
                    else
                    {
                        diff_v[i] = 0;
                    }
                }
                for (int i = 0; i < diff_v.Length; i++)
                {
                    if (diff_v[i] == 1)
                    {
                        for (int j = i; j < diff_v.Length; j++)
                        {
                            if (diff_v[j] == -1)
                            {
                                stateElapses.Add(new MyStateElapse() {
                                    State = states[i + 1].State,
                                    Detail = states[i + 1].Detail,
                                    Start = states[i + 1].Create,
                                    End = states[j].Create
                                });
                                i = j;
                            }
                        }
                    }
                }
                //添加
                var endmachine = Context.Set<MachineState>().Where(t => t.Line == item1.Line && t.Station == "下料机" && t.CreationTime > startTime && t.CreationTime <= endTime).OrderBy(s => s.Count).ToList();
                string productType = "";
                int count1 = 0;
                string haltRange = "";
                string haltType = "";
                string haltReason = "";
                string haltElapse = "";
                TimeSpan ts1 = new TimeSpan(0);
                if (endmachine.Count != 0)
                {
                    productType = endmachine[0].ProductType;
                    count1 = endmachine[0].Count;
                }
                if (stateElapses.Count > 0)
                {
                    foreach (var item in stateElapses)
                    {
                        haltRange += item.Start.ToString("HH:ss") + "~" + item.End.ToString("HH:ss") + ";";
                        haltType += item.State + ";";
                        haltReason += item.Detail + ";";
                        ts1 += item.End - item.Start;
                    }
                    haltElapse = ts1.TotalMinutes.ToString();
                }
                reports.Add(new Report() {
                    Line = item1.Line,
                    Date = req.DatePicker.Date,
                    Month = req.DatePicker.Month,
                    ShiftType = ShiftTypeEnum.Day,
                    ProductType = productType,
                    Count = count1,
                    GoalCount = item1.GoalCount,
                    FillRate = Math.Round((double)count1 / item1.GoalCount * 100, 0),
                    NotReachRate = 100 - Math.Round((double)count1 / item1.GoalCount * 100, 0),
                    HaltRange = haltRange,
                    HaltType = haltType,
                    HaltReason = haltReason,
                    HaltElapse = haltElapse
                });

            }
            lineSettings = Context.Set<LineSeting>().Where(t => t.ShiftType == ShiftTypeEnum.Night).ToList();
            foreach (var item1 in lineSettings)
            {
                DateTime startTime = req.DatePicker.Date.AddHours(20);
                if (DateTime.Now < startTime)
                {
                    break;
                }
                DateTime endTime = req.DatePicker.Date.AddHours(32);
                if (DateTime.Now < endTime)
                {
                    endTime = DateTime.Now;
                }
                List<MyState> states = new List<MyState>();
                states.Add(new MyState()
                {
                    Type = 1,
                    State = "运行",
                    Detail = "",
                    Create = startTime
                });
                int addminute = 10;
                for (int i = 1; startTime.AddMinutes(i * addminute) < endTime; i++)
                {
                    DateTime end1 = startTime.AddMinutes(i * addminute);
                    var items = Context.Set<MachineState>().Where(t => t.Line == item1.Line && t.CreationTime > startTime && t.CreationTime <= end1).ToList();
                    if (items.Count == 0)
                    {
                        states.Add(new MyState()
                        {
                            Type = 4,
                            State = "未开机",
                            Detail = "",
                            Create = startTime.AddMinutes(i * addminute)
                        });
                    }
                    else
                    {
                        var state = items.FirstOrDefault(t => t.StateStatus != StateStatusEnum.Run);
                        if (state != null)
                        {
                            states.Add(new MyState()
                            {
                                Type = (int)state.StateStatus,
                                State = state.StateStatus.ToString(),
                                Detail = state.StateDetail,
                                Create = startTime.AddMinutes(i * addminute)
                            });
                        }
                        else
                        {
                            states.Add(new MyState()
                            {
                                Type = 1,
                                State = "运行",
                                Detail = "",
                                Create = startTime.AddMinutes(i * addminute)
                            });
                        }
                    }
                }
                states.Add(new MyState()
                {
                    Type = 1,
                    State = "运行",
                    Detail = "",
                    Create = endTime
                });
                List<MyStateElapse> stateElapses = new List<MyStateElapse>();
                int[] diff_v = new int[states.Count - 1];
                for (int i = 0; i < states.Count - 1; i++)
                {
                    if (states[i].Type == 1 && states[i + 1].Type > 1)
                    {
                        diff_v[i] = 1;
                    }
                    else if (states[i].Type > 1 && states[i + 1].Type == 1)
                    {
                        diff_v[i] = -1;
                    }
                    else
                    {
                        diff_v[i] = 0;
                    }
                }
                for (int i = 0; i < diff_v.Length; i++)
                {
                    if (diff_v[i] == 1)
                    {
                        for (int j = i; j < diff_v.Length; j++)
                        {
                            if (diff_v[j] == -1)
                            {
                                stateElapses.Add(new MyStateElapse()
                                {
                                    State = states[i + 1].State,
                                    Detail = states[i + 1].Detail,
                                    Start = states[i + 1].Create,
                                    End = states[j].Create
                                });
                                i = j;
                            }
                        }
                    }
                }
                //添加
                var endmachine = Context.Set<MachineState>().Where(t => t.Line == item1.Line && t.Station == "下料机" && t.CreationTime > startTime && t.CreationTime <= endTime).OrderBy(s => s.Count).ToList();
                string productType = "";
                int count1 = 0;
                string haltRange = "";
                string haltType = "";
                string haltReason = "";
                string haltElapse = "";
                TimeSpan ts1 = new TimeSpan(0);
                if (endmachine.Count != 0)
                {
                    productType = endmachine[0].ProductType;
                    count1 = endmachine[0].Count;
                }
                if (stateElapses.Count > 0)
                {
                    foreach (var item in stateElapses)
                    {
                        haltRange += item.Start.ToString("HH:ss") + "~" + item.End.ToString("HH:ss") + ";";
                        haltType += item.State + ";";
                        haltReason += item.Detail + ";";
                        ts1 += item.End - item.Start;
                    }
                    haltElapse = ts1.TotalMinutes.ToString();
                }
                reports.Add(new Report()
                {
                    Line = item1.Line,
                    Date = req.DatePicker.Date,
                    Month = req.DatePicker.Month,
                    ShiftType = ShiftTypeEnum.Night,
                    ProductType = productType,
                    Count = count1,
                    GoalCount = item1.GoalCount,
                    FillRate = Math.Round((double)count1 / item1.GoalCount * 100, 0),
                    NotReachRate = 100 - Math.Round((double)count1 / item1.GoalCount * 100, 0),
                    HaltRange = haltRange,
                    HaltType = haltType,
                    HaltReason = haltReason,
                    HaltElapse = haltElapse
                });

            }
            return Task.FromResult(reports);
        }
        public FStarMESController(MyDbContext myDbContext)
        {
            Context = myDbContext;
        }
    }
    class MyState
    {
        public int Type { get; set; }
        public string State { get; set; }
        public string Detail { get; set; }
        public DateTime Create { get; set; }
    }
    class MyStateElapse
    {
        public string State { get; set; }
        public string Detail { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
    }
}
