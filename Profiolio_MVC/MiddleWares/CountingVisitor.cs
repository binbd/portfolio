using System.Collections.Concurrent;
using Microsoft.AspNetCore.Mvc;
using Profiolio_MVC.Models;


public class VisitorCounterMiddleware
{
    private readonly RequestDelegate _requestDelegate;

    public VisitorCounterMiddleware(RequestDelegate requestDelegate)
    {
        _requestDelegate = requestDelegate;
    }

    public async Task Invoke(HttpContext context)
    {
      if(context!= null)
      {
        string? visitorId = context.Request.Cookies["VisitorId"];
          System.Net.IPAddress? iPAddress = context.Connection.RemoteIpAddress;
          string remoteAddress = "";
            if(iPAddress!= null)
                 remoteAddress = iPAddress.MapToIPv4().ToString();
        if (visitorId == null)
        {
            //don the necessary staffs here to save the count by one
          
            visitorId = Guid.NewGuid().ToString();
            context.Response.Cookies.Append("VisitorId",visitorId, new CookieOptions()
                {
                        Path = "/",
                        HttpOnly = true,
                        Secure = false,
                });
          
        }
       
        if(!Config.Application.Keys.Contains("Visitor")){
         
          Config.Application["Visitor"] = new ConcurrentDictionary<string, object>();
        }
        
        ConcurrentDictionary<string,object> visitorObj = (ConcurrentDictionary<string,object>)Config.Application["Visitor"];
       
        if(!visitorObj.Keys.Contains(visitorId)){
          //context.Items["visitorId"] = visitorId;
          ViewerCounting viewerCounting = new ViewerCounting();
          viewerCounting.ClientId = visitorId;
          viewerCounting.IsCurrentViewing = true;
          viewerCounting.FirstViewing = DateTime.UtcNow;
          viewerCounting.LastViewing = DateTime.UtcNow;
          ViewerLoggin viewerLoggin = new ViewerLoggin();
          viewerLoggin.ClientIp = remoteAddress;
          viewerLoggin.LogginTime = DateTime.UtcNow;
          viewerCounting.ViewerLoggins.Add(viewerLoggin);
          //Config.Application["VisitorId"] = viewerCounting;
          visitorObj[visitorId]   = viewerCounting;

        }

        context.Items["visitorId"] = visitorId;
        // if(!Config.Application.Keys.Contains("VisitorId")){
         
        
        // }

        await _requestDelegate(context);

      }

    }
}