function toggleNavMenu (clas){
  $('#navbar .scrollto').each(function(){
       if($(this).attr('id')===clas && !$(this).hasClass('active')){
         console.log('Id:' + clas);
         $(this).toggleClass('active');
       }
       else
           $(this).removeClass('active');
  });

}

SendPings = ()=>{
    var val = $('#visitorId').val();
        var data = {
            visitorId:val
        }
        var pathname = window.location.pathname;
        console.log(pathname);
        var url = pathname ? '' : 'Home/';
        if(pathname=="/")
            url = 'Home/';
        
        doAjax(data,url + 'ViewerPing','GET');
        doAjax('',url + 'ViewerStatusUpdate','GET',function(error){
            console.log(error);
        });
}

function SetViewingPingTimer(timer)
{
    setInterval(SendPings, timer);
}


$(function () {
    SendPings();
    SetViewingPingTimer(10000);
 });