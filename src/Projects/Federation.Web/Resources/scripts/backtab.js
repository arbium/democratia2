$(function () {
    var back = $(".lighttabs .back");
    if (back && (document.referrer || back.attr('href') != undefined)) {
        if (back.attr('href') == undefined) {
            back.click(function (e) {
                e.preventDefault();
                history.back();
            });
        }
        
        back.addClass("display-block");
    }
});