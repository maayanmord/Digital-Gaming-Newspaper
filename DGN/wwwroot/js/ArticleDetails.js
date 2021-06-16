$(function () {
    $("#comment-form").keyup(function () {
        var commentBody = $("#comment-body").val();

        if (commentBody.length < 5) {
            $("#comment-body").removeClass("is-valid");
            $("#comment-body").addClass("is-invalid");
            $("#comment-validation").removeClass("valid-feedback");
            $("#comment-validation").addClass("invalid-feedback");
            $("#comment-validation").html("Please insert at least 5 characters");
        } else {
            $("#comment-body").removeClass("is-invalid");
            $("#comment-body").addClass("is-valid");
            $("#comment-validation").removeClass("invalid-feedback");
            $("#comment-validation").addClass("valid-feedback");
            $("#comment-validation").html("Looks good!");
        }
    });

    $("#comment-form").submit(function (e) {
        e.preventDefault();

        var commentBody = $("#comment-body").val();
        var ArticleId = $("#ArticleId").val();
        $.ajax({
            type: "POST",
            url: "/Comments/Create",
            data: {
                Body: commentBody,
                RelatedArticleId: ArticleId,
                __RequestVerificationToken: gettoken()
            },
            success: function (data) {
                var item = $('#commentTemplate').html();

                $.each(data.comment, function (key, value) {
                    item = item.replaceAll('{' + key + '}', value)
                });

                item = item.replaceAll('{fullName}', data.fullName)

                $("#comment-section").prepend(item);
            },
            error: function (data) {

            }
        }).done(function (data) {

        });
    });
});