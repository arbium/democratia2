$(function () {
    $(".like").click(function (e) {
        e.preventDefault();
        if ($(this).attr("href")) {
            $.ajax({ url: $(this).attr("href") });
            var parent = $(this).parent();
            parent.children(".like").removeAttr("href");
            var val = $(this).children(".val");
            val.html(parseInt(val.html()) + 1);
            val.css("font-weight", "bold");
            parent.children(".like").children(".arrow").css("display", "none");
            parent.children(".plus").children(".val").prepend("+");
            parent.children(".minus").children(".val").prepend("&minus;");
        }
    });
});