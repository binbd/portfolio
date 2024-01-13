using System.Collections.Concurrent;
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using Profiolio_MVC.Data;
using Profiolio_MVC.Models;

namespace Profiolio_MVC.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private myDbContext _dbContext;
    public HomeController(ILogger<HomeController> logger,myDbContext dbContext)
    {
        _logger = logger;
        _dbContext = dbContext;
    }

    private void CheckingViewing()
    {
            // var user = await _userManager.GetUserAsync(User);
        string? visitorId = (string?)HttpContext.Items["visitorId"];
        
        if(string.IsNullOrEmpty(visitorId))
        return ;

        ViewBag.visitorId = visitorId;
        TempData["visitorId"] = visitorId;
        ConcurrentDictionary<string,object> visitorObj = (ConcurrentDictionary<string,object>)Config.Application["Visitor"];
        ViewerCounting viewerCounting =  (ViewerCounting)visitorObj[visitorId];
        ViewerCounting? tmpVC =_dbContext.viewerCountings.Where(b => b.ClientId == viewerCounting.ClientId).FirstOrDefault();
        if(tmpVC == null){

            tmpVC = viewerCounting;//new ViewerCounting();
            //tmpVC.IsCurrentViewing  = false;
            //tmpVC.ClientId          = viewerCounting.ClientId;
            //tmpVC.FirstViewing      = viewerCounting.FirstViewing;
            //tmpVC.LastViewing       = viewerCounting.LastViewing;
            _dbContext.viewerCountings.Add(tmpVC);
            _dbContext.SaveChanges();
        }

        if(!tmpVC.IsCurrentViewing){

            ViewerLoggin viewerLoggin   = (ViewerLoggin)viewerCounting.ViewerLoggins.ElementAt(0);
            tmpVC.IsCurrentViewing      = true;
            tmpVC.ViewerLoggins.Add(viewerLoggin);
            tmpVC.LastViewing = viewerCounting.LastViewing;
            _dbContext.Entry(tmpVC).State = EntityState.Modified;
            _dbContext.SaveChanges();
            
        }
        //update with database
        visitorObj[visitorId] = tmpVC;
        //HttpContext.Items["visitorId"] = visitorId;
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
            
            ConcurrentDictionary<string,object> visitorObj = (ConcurrentDictionary<string,object>)Config.Application["Visitor"];
            ViewerCounting? viewerCounting =  visitorObj.Keys.Contains(visitorId) ? (ViewerCounting)visitorObj[visitorId] : null;
            ViewBag.visitorId = visitorId;
            if(viewerCounting== null)
                return StatusCode(StatusCodes.Status404NotFound);

            if(!viewerCounting.IsCurrentViewing)
                viewerCounting.IsCurrentViewing = true;

            viewerCounting.LastViewing = DateTime.UtcNow;
            _dbContext.Entry(viewerCounting).State = EntityState.Modified;
            _dbContext.SaveChanges();
        }

        return StatusCode(StatusCodes.Status200OK);

    }
    
    public int UpdateUnViewedList(int nTimeOut)
    {
        var ViewerCountings = _dbContext.viewerCountings.Where(x=>x.IsCurrentViewing == true).ToList();
        int nOnlineCounting = ViewerCountings.Count();
        
        if (ViewerCountings == null)
            throw new InvalidOperationException("No viewing data is found!");

        int nchangedCount = 0;
        foreach(ViewerCounting vc in ViewerCountings )
        {
            var dt = DateTime.UtcNow - vc.LastViewing;
            if( dt.TotalMinutes > nTimeOut)
            {
                vc.IsCurrentViewing = false;
                _dbContext.viewerCountings.Entry(vc).State = EntityState.Modified;
                nchangedCount++;
            }

        }

        int nTotalView    = 0;
        foreach(ViewerCounting vc in _dbContext.viewerCountings.Include(x => x.ViewerLoggins))
        {
            nTotalView += vc.ViewerLoggins.Count();
        }

        var settings = _dbContext.settingsNumbers.Where(x => x.Key == "TOTALVIEWONLINE").ToList();

        if( settings == null || settings.Count()==0)
            _dbContext.settingsNumbers.Add(new SettingsNumber(){ Key = "TOTALVIEWONLINE", Value = nOnlineCounting });
        else
        {
            settings.ForEach(x => { 
                if(x.Key=="TOTALVIEWONLINE") 
                    x.Value = nOnlineCounting;  
                _dbContext.settingsNumbers.Entry(x).State = EntityState.Modified; 
            });
        }
        
        settings = _dbContext.settingsNumbers.Where(x => x.Key == "TOTALVIEW").ToList();

        if( settings == null || settings.Count()==0)
            _dbContext.settingsNumbers.Add(new SettingsNumber(){ Key = "TOTALVIEW", Value = nTotalView });
        else
        {
            settings.ForEach(x => { 
                if(x.Key=="TOTALVIEW") 
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
            return StatusCode(StatusCodes.Status400BadRequest,ex.Message);
             // TODO
        }
       
        return StatusCode(StatusCodes.Status200OK);
    }

    [HttpGet]
    public IActionResult GetViewStatistic()
    {
        
        var totalView        =  _dbContext.settingsNumbers.Where(x => x.Key == "TOTALVIEWONLINE").ToList();
        var ntotalViewOnline = totalView.Count() > 0 ? totalView.ElementAt(0).Value : 0;
        totalView  =_dbContext.settingsNumbers.Where(x => x.Key == "TOTALVIEW").ToList();
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
