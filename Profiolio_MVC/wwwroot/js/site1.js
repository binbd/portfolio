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

function SetViewingPingTimer(timer)
{
    setInterval(function() {
        var val = $('#visitorId').val();
        var data = {
            visitorId:val
        }
        doAjax(data,'Home/ViewerPing','GET');
        doAjax('','Home/ViewerStatusUpdate','GET',function(error){
            console.log(error);
        });
    }, timer);
}


$(function () {
    
    SetViewingPingTimer(60000);
 });