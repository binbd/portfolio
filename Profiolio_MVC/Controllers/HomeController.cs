using System.Collections.Concurrent;
using System.Diagnostics;
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

    private void CheckingViewing()
    {
        // var user = await _userManager.GetUserAsync(User);

        UpdateUnViewedList(30);

        if (HttpContext != null)
        {
            string? visitorId = HttpContext.Request.Cookies["VisitorId"];
            System.Net.IPAddress? iPAddress = HttpContext.Connection.RemoteIpAddress;
            string remoteAddress = "";
            if (iPAddress != null)
                remoteAddress = iPAddress.MapToIPv4().ToString();
            if (visitorId == null)
            {
                //don the necessary staffs here to save the count by one

                visitorId = Guid.NewGuid().ToString();
                HttpContext.Response.Cookies.Append("VisitorId", visitorId, new CookieOptions()
                {
                    Path = "/",
                    HttpOnly = true,
                    Secure = false,
                });

            }

            if (!Config.Application.Keys.Contains("Visitor"))
            {

                Config.Application["Visitor"] = new ConcurrentDictionary<string, object>();
            }

            ConcurrentDictionary<string, object> visitorObj = (ConcurrentDictionary<string, object>)Config.Application["Visitor"];

            if (!visitorObj.Keys.Contains(visitorId))
            {
                //context.Items["visitorId"] = visitorId;
                //loading from DB
                ViewerCounting? viewerCounting = _dbContext.viewerCountings.Include(x => x.ViewerLoggins).Where(b => b.ClientId == visitorId). First();

                if (viewerCounting == null) //new
                {
                    viewerCounting = new ViewerCounting();
                    viewerCounting.ClientId = visitorId;
                    viewerCounting.IsCurrentViewing = true;
                    viewerCounting.FirstViewing = DateTime.UtcNow;
                    viewerCounting.LastViewing = DateTime.UtcNow;
                    ViewerLoggin viewerLoggin = new ViewerLoggin();
                    viewerLoggin.ClientIp = remoteAddress;
                    viewerLoggin.LogginTime = DateTime.UtcNow;
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
                        viewerCounting.IsCurrentViewing = true;
                        viewerCounting.ViewerLoggins.Add(viewerLoggin);
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
        CheckingViewing();
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    public IActionResult About()
    {
        CheckingViewing();
        return View();
    }


    public IActionResult Resume()
    {
        CheckingViewing();
        return View();
    }

    public IActionResult Project()
    {
        CheckingViewing();
        return View();
    }

    public IActionResult Contact()
    {
        CheckingViewing();
        return View();
    }

    public IActionResult ViewStatistic()
    {
        CheckingViewing();
        return View();
    }

    [HttpGet]
    public IActionResult ViewerPing(string? visitorId)
    {
        //string? svisitorId = (string?)TempData.Peek("visitorId");

        //if(visitorId == svisitorId)
        {

            ConcurrentDictionary<string, object> visitorObj = (ConcurrentDictionary<string, object>)Config.Application["Visitor"];
            ViewerCounting? viewerCounting = visitorObj.Keys.Contains(visitorId) ? (ViewerCounting)visitorObj[visitorId] : null;
            //ViewerCounting? viewerCounting = _dbContext.viewerCountings.Where()
            ViewBag.visitorId = visitorId;
            if (viewerCounting == null)
                return StatusCode(StatusCodes.Status404NotFound);

            if (!viewerCounting.IsCurrentViewing)
                viewerCounting.IsCurrentViewing = true;

            viewerCounting.LastViewing = DateTime.UtcNow;
            _dbContext.Entry(viewerCounting).State = EntityState.Modified;
            _dbContext.SaveChanges();
        }

        return StatusCode(StatusCodes.Status200OK);

    }

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
}
