$(function () {
    $(".badge img").mousemove(function (event) {
        var key = $(this).attr("key");
        $("#" + key).addClass("visible");
        var x = event.pageX;
        var y = event.pageY + 20;
        $("#" + key).offset({ top: y, left: x });
    });

    $(".badge img").mouseleave(function() {
        var key = $(this).attr("key");
        $("#" + key).removeClass("visible");
    });
});