using System.Collections.Concurrent;
using System.Diagnostics;
using System.Net.Sockets;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Profiolio_MVC.Data;
using Profiolio_MVC.Models;

namespace Profiolio_MVC.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private myDbContext _dbContext;
    public HomeController(ILogger<HomeController> logger, myDbContext dbContext)
    {
        _logger = logger;
        _dbContext = dbContext;
    }

    public System.Net.IPAddress? GetRemoteHostIpAddressUsingXForwardedFor(HttpContext httpContext)
    {
        System.Net.IPAddress? remoteIpAddress = null;
        var forwardedFor = httpContext.Request.Headers["X-Forwarded-For"].FirstOrDefault();

        if (!string.IsNullOrEmpty(forwardedFor))
        {
            var ips = forwardedFor.Split(',', StringSplitOptions.RemoveEmptyEntries)
                                .Select(s => s.Trim());

            foreach (var ip in ips)
            {
                if (System.Net.IPAddress.TryParse(ip, out var address) &&
                    (address.AddressFamily is AddressFamily.InterNetwork
                    or AddressFamily.InterNetworkV6))
                {
                    remoteIpAddress = address;
                    break;
                }
            }
        }

        return remoteIpAddress;
    }

    public System.Net.IPAddress? GetRemoteHostIpAddressUsingXRealIp(HttpContext httpContext)
    {
        System.Net.IPAddress? remoteIpAddress = null;
        var xRealIpExists = httpContext.Request.Headers.TryGetValue("X-Real-IP", out var xRealIp);

        if(xRealIpExists)
        {
            if (!System.Net.IPAddress.TryParse(xRealIp, out System.Net.IPAddress? address))
            {
                return remoteIpAddress;
            }

            var isValidIP = address.AddressFamily is AddressFamily.InterNetwork
                            or AddressFamily.InterNetworkV6;
            
            if (isValidIP)
            {
                remoteIpAddress = address;
            }

            return remoteIpAddress;
        }

        return remoteIpAddress;
    }

    private void CheckingViewing(HttpContext context,string? svisitorId)
    {
        // var user = await _userManager.GetUserAsync(User);

        UpdateUnViewedList(30);

        if (context != null)
        {
            string? visitorId = context.Request.Cookies["VisitorId"];
            System.Net.IPAddress? iPAddress = context.Connection.RemoteIpAddress;
            System.Net.IPAddress? iPAddressXForwardedFor = GetRemoteHostIpAddressUsingXForwardedFor(context);
            System.Net.IPAddress? iPAddressXRealIp = GetRemoteHostIpAddressUsingXRealIp(context);
            string remoteAddress = "";
            if (iPAddress != null)
                remoteAddress = iPAddress.MapToIPv4().ToString();

            if(iPAddressXForwardedFor !=null)
                remoteAddress += "|" + iPAddressXForwardedFor.MapToIPv4().ToString();

            if(iPAddressXRealIp !=null)
                remoteAddress += "|" + iPAddressXRealIp.MapToIPv4().ToString();
            
            if (string.IsNullOrEmpty(visitorId) && string.IsNullOrEmpty(svisitorId))
            {
                //don the necessary staffs here to save the count by one

                visitorId = Guid.NewGuid().ToString();
                context.Response.Cookies.Append("VisitorId", visitorId, new CookieOptions()
                {
                    Path = "/",
                    HttpOnly = true,
                    Secure = false,
                    //Expires = DateTime.UtcNow.AddDays(30),
                });
                

            }

            if(string.IsNullOrEmpty(visitorId))
                visitorId = svisitorId;

            if (!Config.Application.Keys.Contains("Visitor"))
            {

                Config.Application["Visitor"] = new ConcurrentDictionary<string, object>();
            }

            ConcurrentDictionary<string, object> visitorObj = (ConcurrentDictionary<string, object>)Config.Application["Visitor"];

            //if (!visitorObj.Keys.Contains(visitorId))
            {
                //context.Items["visitorId"] = visitorId;
                //loading from DB
                var ss = _dbContext.viewerCountings.Include(x => x.ViewerLoggins);
              
                ViewerCounting? viewerCounting = ss.FirstOrDefault(b => b.ClientId == visitorId);//.FirstOrDefault();

                if (viewerCounting == null) //new
                {
                    _logger.LogInformation("Add new viewer ClientId = {clientid}",visitorId);
                    viewerCounting = new ViewerCounting
                    {
                        ClientId = visitorId,
                        IsCurrentViewing = true,
                        FirstViewing = DateTime.UtcNow,
                        LastViewing = DateTime.UtcNow
                    };
                    ViewerLoggin viewerLoggin = new ViewerLoggin
                    {
                        ClientIp = remoteAddress,
                        LogginTime = DateTime.UtcNow
                    };
                    viewerCounting.ViewerLoggins.Add(viewerLoggin);
                    _dbContext.viewerCountings.Add(viewerCounting);
                    _dbContext.SaveChanges();
                    //Config.Application["VisitorId"] = viewerCounting;
                }
                else
                {
                    if (!viewerCounting.IsCurrentViewing)
                    {

                        ViewerLoggin viewerLoggin = new()
                        {
                            ClientIp = remoteAddress,
                            LogginTime = DateTime.UtcNow,
                        };
                         _logger.LogInformation("Add new loggin ClientId = {clientid}",visitorId);
                        viewerCounting.IsCurrentViewing = true;
                        viewerCounting.ViewerLoggins.Add(viewerLoggin);
                        viewerCounting.LastViewing = DateTime.UtcNow;
                        _dbContext.Entry(viewerCounting).State = EntityState.Modified;
                        _dbContext.SaveChanges();

                    }
                    else
                    {
                         viewerCounting.LastViewing = DateTime.UtcNow;
                        _dbContext.Entry(viewerCounting).State = EntityState.Modified;
                        _dbContext.SaveChanges();
                    }
                }

                visitorObj[visitorId] = viewerCounting;
            }
           

            ViewBag.visitorId = visitorId;
            TempData["visitorId"] = visitorId;
            //HttpContext.Items["visitorId"] = visitorId;
        }
    }
    public IActionResult Index()
    {
        //CheckingViewing();
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    public IActionResult About()
    {
        //CheckingViewing();
        return View();
    }


    public IActionResult Resume()
    {
        //CheckingViewing();
        return View();
    }

    public IActionResult Project()
    {
        //CheckingViewing();
        return View();
    }

    public IActionResult Contact()
    {
        //CheckingViewing();
        return View();
    }

    public IActionResult ViewStatistic()
    {
        //CheckingViewing();
        return View();
    }

    [HttpGet]
    public IActionResult ViewerPing(string? visitorId)
    {
        //string? svisitorId = (string?)TempData.Peek("visitorId");
        {

            try
            {
                CheckingViewing(HttpContext,visitorId);
            }
            catch (System.Exception ex)
            {
                return StatusCode(StatusCodes.Status400BadRequest);
            }    
            // ViewerCounting? viewerCounting = _dbContext.viewerCountings.Where(x => x.ClientId==visitorId).FirstOrDefault();
            // ViewBag.visitorId = visitorId;
            // if (viewerCounting == null)
            //     return StatusCode(StatusCodes.Status404NotFound);

            // if (!viewerCounting.IsCurrentViewing)
            //     viewerCounting.IsCurrentViewing = true;

            // viewerCounting.LastViewing = DateTime.UtcNow;
            // _dbContext.Entry(viewerCounting).State = EntityState.Modified;
            // _dbContext.SaveChanges();
        }

        return StatusCode(StatusCodes.Status200OK);

    }

    //  public int solution(int[] A) {
    //     Dictionary <int,int> mk = new Dictionary<int,int>();
    //     for(int i=0;i<A.Length;i++)
    //         mk[A[i]]=1;
    //     return mk.Count;
    //     // Implement your solution here
    // }
    public int UpdateUnViewedList(int nTimeOut)
    {
        var ViewerCountings = _dbContext.viewerCountings.Where(x => x.IsCurrentViewing == true).ToList();
        int nOnlineCounting = ViewerCountings.Count();

        if (ViewerCountings == null)
            throw new InvalidOperationException("No viewing data is found!");

        int nchangedCount = 0;
        foreach (ViewerCounting vc in ViewerCountings)
        {
            var dt = DateTime.UtcNow - vc.LastViewing;
            if (dt.TotalMinutes > nTimeOut)
            {
                vc.IsCurrentViewing = false;
                _dbContext.viewerCountings.Entry(vc).State = EntityState.Modified;
                nchangedCount++;
            }

        }

        int nTotalView = 0;
        foreach (ViewerCounting vc in _dbContext.viewerCountings.Include(x => x.ViewerLoggins))
        {
            nTotalView += vc.ViewerLoggins.Count();
        }

        var settings = _dbContext.settingsNumbers.Where(x => x.Key == "TOTALVIEWONLINE").ToList();

        if (settings == null || settings.Count() == 0)
            _dbContext.settingsNumbers.Add(new SettingsNumber() { Key = "TOTALVIEWONLINE", Value = nOnlineCounting });
        else
        {
            settings.ForEach(x =>
            {
                if (x.Key == "TOTALVIEWONLINE")
                    x.Value = nOnlineCounting;
                _dbContext.settingsNumbers.Entry(x).State = EntityState.Modified;
            });
        }

        settings = _dbContext.settingsNumbers.Where(x => x.Key == "TOTALVIEW").ToList();

        if (settings == null || settings.Count() == 0)
            _dbContext.settingsNumbers.Add(new SettingsNumber() { Key = "TOTALVIEW", Value = nTotalView });
        else
        {
            settings.ForEach(x =>
            {
                if (x.Key == "TOTALVIEW")
                    x.Value = nTotalView;
                _dbContext.settingsNumbers.Entry(x).State = EntityState.Modified;
            });
        }


        //if(nchangedCount > 0)
        _dbContext.SaveChanges();

        return nchangedCount;
    }

    [HttpGet]
    public IActionResult ViewerStatusUpdate()
    {
        //string? svisitorId = (string?)TempData.Peek("visitorId");
        return StatusCode(StatusCodes.Status200OK);
        try
        {
            UpdateUnViewedList(30);
        }
        catch (System.Exception ex)
        {
            return StatusCode(StatusCodes.Status400BadRequest, ex.Message);
            // TODO
        }

        return StatusCode(StatusCodes.Status200OK);
    }

    [HttpGet]
    public IActionResult GetViewStatistic()
    {

        var totalView = _dbContext.settingsNumbers.Where(x => x.Key == "TOTALVIEWONLINE").ToList();
        var ntotalViewOnline = totalView.Count() > 0 ? totalView.ElementAt(0).Value : 0;
        totalView = _dbContext.settingsNumbers.Where(x => x.Key == "TOTALVIEW").ToList();
        var ntotalView = totalView.Count() > 0 ? totalView.ElementAt(0).Value : 0;
        //_dbcontext.SaveChanges();
        ViewingStatistic viewing = new()
        {
            TotalView = ntotalView,
            TotalViewOnline = ntotalViewOnline
        };
        //return StatusCode(StatusCodes.Status200OK);
        return Json(viewing);

    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

    [HttpGet]
    public IActionResult BuildCountingTable()
    {
        //return PartialView("_ToDoTable",_dbcontext.ToDos.Where(x=>x.userIdentityTest.Id == currentUserId).ToList());
        return PartialView("_TableViewerCounting",GetCountingTable());
    }

     [HttpGet]
    public IActionResult BuildLogginTable()
    {
        //return PartialView("_ToDoTable",_dbcontext.ToDos.Where(x=>x.userIdentityTest.Id == currentUserId).ToList());
        return PartialView("_TableViewerLoggin",GetViewerLoginTable());
    }

    private IEnumerable<ViewerCounting> GetCountingTable()
    {
        //  var user = await _userManager.GetUserAsync(User);
        IEnumerable<ViewerCounting> myToDos = _dbContext.viewerCountings.OrderByDescending(x => x.Id).Take(100).ToList();

        return myToDos;
    }

    private IEnumerable<ViewerLoggin> GetViewerLoginTable()
    {
        //  var user = await _userManager.GetUserAsync(User);
        IEnumerable<ViewerLoggin> myToDos = _dbContext.ViewerLoggin.OrderByDescending(x => x.Id).Take(100).ToList();

        return myToDos;
    }


}
