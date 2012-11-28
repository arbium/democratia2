$(function () {
    $(".paginator").removeAttr("title");
    $(".paginator").removeAttr("style");
    $(".paginator").click(function (e) {
        e.preventDefault();
        var feed = $(this).parent();
        feed.children(".paginator").remove();
        feed.append("<img class='loading' src='/Resources/themes/base/images/indicator.gif' alt='…' />");
        $.ajax({
            url: $(this).attr("url"),
            success: function (data) {
                feed.children(".loading").remove();
                feed.append(data);
            }
        });
    });
});