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
            doAjax('','BuildCountingTable','get').then((data) => fillCountingTable(data));
            doAjax('','BuildLogginTable','get').then((data) => fillLogginTable(data));
            
            nCurrent = 0;
        }
        nCurrent++;
        progressUpdate(nCurrent,nMax);

            
    }, 1000);
}

function fillCountingTable(data)
{
    $('#divCountingTable').html(data);
    //addTblCheckboxListener(".ActiveCheck");
}


function fillLogginTable(data)
{
    $('#divLogginTable').html(data);
    //addTblCheckboxListener(".ActiveCheck");
}



$(function(){
    SetGetViewingStatisticTimer(10000);

});
