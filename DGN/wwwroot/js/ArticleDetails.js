﻿$(function () {
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

                $.each(data, function (key, value) {
                    item = item.replaceAll('{' + key + '}', value)
                });

                $("#comment-section").prepend(item);
                $("#comment-body").val("");
                $("#comment-body").removeClass("is-valid")
                $("#comment-validation").html("");
                $("#comment-validation").removeClass("valid-feedback");
            },
            error: function (data) {
                $("#alert-body").show();
            }
        });
    });

    $('#articleDeleteModal').on('show.bs.modal', function (event) {
        var button = $(event.relatedTarget) // Button that triggered the modal
        var articleId = button.data('id');
        var modal = $(this);
        modal.find('.modal-body #article-delete-id').val(articleId);
    });

    $('#commentDeleteModal').on('show.bs.modal', function (event) {
        var button = $(event.relatedTarget) // Button that triggered the modal
        var commentId = button.data('id');
        var fullname = button.data('fullname');
        var body = $("#comment-section div[name=" + commentId + "] .card-body .card-text").text();
        var modal = $(this);
        modal.find('.modal-body h6').text("Comment by: " + fullname);
        modal.find('.modal-body textarea').text(body);
        modal.find('.modal-body #comment-delete-id').val(commentId);
    });

    $('#commentEditModal').on('show.bs.modal', function (event) {
        var button = $(event.relatedTarget) // Button that triggered the modal
        var commentId = button.data('id');
        var body = $("#comment-section div[name=" + commentId + "] .card-body .card-text").text();
        var modal = $(this);
        modal.find('.modal-body #comment-edit-body').val(body);
        modal.find('.modal-body #comment-edit-id').val(commentId);
    });

    $("#comment-delete").submit(function (e) {
        e.preventDefault();
        var commentId = $("#comment-delete-id").val();

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
                $("#alert-body").show();
            }
        }).done(function (data) {
            $("#commentDeleteModal").modal('hide');
        });
    });

    $("#comment-edit").submit(function (e) {
        e.preventDefault();
        var commentId = $("#comment-edit-id").val();
        var commentBody = $("#comment-edit-body").val();

        $.ajax({
            type: "POST",
            url: "/Comments/Edit",
            data: {
                Id: commentId,
                Body: commentBody,
                __RequestVerificationToken: gettoken()
            },
            success: function (data) {
                $("#comment-section div[name=" + data.id + "] .card-body .card-text").html(data.body);
            },
            error: function (data) {
                $("#alert-body").show();
            }
        }).done(function (data) {
            $("#commentEditModal").modal('hide');
        });
    });

    $('#alert-button').click(function () {
        $("#alert-body").hide();
    });

    $('#likeButton').click(() => {
        articleId = $("#ArticleId").val();

        $.ajax({
            type: "POST",
            url: "/Articles/LikeArticle",
            data: {
                articleId,
                __RequestVerificationToken: gettoken()
            },
            success: function (data) {
                if (data.isLiked) {
                    $('like-button').addClass('liked');
                    $('#likeButtonText').html('Unlike');
                } else {
                    $('like-button').removeClass('liked');
                    $('#likeButtonText').html('Like');
                }
            },
            error: function (data) {
                $("#alert-body").show();
            }
        }).done(function (data) {
            $('#numberOfLikes').html(data.count)
        });
    }); 
});