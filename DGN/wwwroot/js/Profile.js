
$(function () {

    function viewEdit() {
        var editProperties = $("#EditTemplate").html();
        $("#ViewProperties").html(editProperties);
        var editAbout = $("#EditAboutTemplate").html();
        $("#EditAbout").html(editAbout);
    }

    $("#EditButton").click(function () {
        viewEdit();
    });

    $('.button').click(function () {
        $.ajax({
            url: "",
            context: document.body,
            success: function (s, x) {

                $('html[manifest=saveappoffline.appcache]').attr('content', '');
                $(this).html(s);
            }
        });
    });
})