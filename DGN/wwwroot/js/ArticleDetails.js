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
                $("#comment-body").val("");
                $("#comment-body").removeClass("is-valid")
                $("#comment-validation").html("");
                $("#comment-validation").removeClass("valid-feedback");
            },
            error: function (data) {

            }
        }).done(function (data) {

        });
    });

    $('#commentDeleteModal').on('show.bs.modal', function (event) {
        var button = $(event.relatedTarget) // Button that triggered the modal
        var body = button.data('body');
        var fullname = button.data('fullname');
        var commentId = button.data('id');
        var modal = $(this);
        modal.find('.modal-body h6').text("Comment by: " + fullname);
        modal.find('.modal-body p').text(body);
        modal.find('.modal-footer #comment-delete').val(commentId);
    });

    $("#comment-delete").click(function (e) {
        var commentId = $(this).val();
        $.ajax({
            type: "POST",
            url: "/Comments/Delete",
            data: {
                Id: commentId,
                __RequestVerificationToken: gettoken()
            },
            success: function (data) {
                $("#comment-section div[name=" + data.id + "]").remove();
            },
            error: function (data) {

            }
        }).done(function (data) {

        });
    });
});