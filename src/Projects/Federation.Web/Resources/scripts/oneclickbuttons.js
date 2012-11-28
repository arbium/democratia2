$(function () {
    $("input:submit").click(function () {
        var target = $(this);

        target.attr("disabled", "disabled");
        target.parents("form").submit();

        setTimeout(function () {
            target.removeAttr("disabled");
        }, 777);
    });
});