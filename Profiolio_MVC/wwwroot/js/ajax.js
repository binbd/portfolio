async function doAjax(args,ajaxurl,method,err) {
    let result;

    try {
        result = await $.ajax({
            url: ajaxurl,
            type: method,
            data: args
        });

        return result;
    } catch (error) {
        if(err)
            err(error)
    }
}
