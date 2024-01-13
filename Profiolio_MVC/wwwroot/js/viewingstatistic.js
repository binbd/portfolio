function fillData(data)
{
    console.log(data);
    $('#totalPageViews').text(data.totalView);
    $('#currentOnlineUsers').text(data.totalViewOnline);
}

function progressUpdate(nCurrent, nMax)
{
    //nWidth = 
    nWidth = Math.round(100  * (nCurrent / nMax))
    $('.progress-bar').css("width", nWidth + "%");
}


function SetGetViewingStatisticTimer(timer)
{
    nMax = timer/1000;
    nCurrent = 0;
    setInterval(function() {
        if(nCurrent>=nMax)
        {
            var val = $('#visitorId').val();
            var data = {visitorId:val}

            //doAjax(data,'ViewerPing','GET');
            doAjax('','GetViewStatistic','GET').then((data) => fillData(data));
            nCurrent = 0;
        }
        nCurrent++;
        progressUpdate(nCurrent,nMax);

            
    }, 1000);
}

$(function(){
    SetGetViewingStatisticTimer(60000);
});
