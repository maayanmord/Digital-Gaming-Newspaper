$(function () {
    $("#searchSubmitButton").click(function (e) {
        e.preventDefault();
        let fullname = $('#fullnameInput').val();
        let username = $('#usernameInput').val();
        let commentBody = $('#commentBodyInput').val();

        $.ajax({
            type: "POST",
            url: "/Comments/Search",
            data: {
                fullname: fullname,
                username: username,
                body: commentBody,
                __RequestVerificationToken: gettoken()
            },
            success: function (data) {
                $("#comment-section").html('');
                var template = $('#commentTemplate').html();

                $.each(data, function (i, val) {
                    var temp = template;

                    $.each(val, function (key, value) {
                        temp = temp.replaceAll('{' + key + '}', value);
                    });

                    $("#comment-section").append(temp);
                });
            },
            error: function (data) {
                $("#alert-body").show();
            }
        });
    });
});