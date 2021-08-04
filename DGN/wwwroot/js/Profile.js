$(function () {
    view();

    function edit() {
        $("#ViewProperties").attr("hidden", true);
        $("#EditProperties").removeAttr("hidden");
        $("#ViewAbout").attr("hidden", true);
        $("#EditAbout").removeAttr("hidden");
    }

    function view() {
        $("#EditProperties").attr("hidden", true);
        $("#ViewProperties").removeAttr("hidden");
        $("#EditAbout").attr("hidden", true);
        $("#ViewAbout").removeAttr("hidden");
    }

    function reloadCache() {
        location.reload();
    }

    $('#form').submit(function (e) {
        if ($('#form').valid()) {
            reloadCache();
        }
    });

    $('#Cancel').click(function () {
        $("form").trigger("reset");
        $("#form").find(".field-validation-valid").empty();
        view();
    });

    $("#EditButton").click(function () {
        edit();
    });
    
    function getArticlePage(id, page, getUrl, sectionId) {
        count = 5
        $.ajax({
            type: "GET",
            url: getUrl,
            data: {
                id: id,
                page: page,
                count: count
            },
            success: function (data) {
                countCheck = 0;
                $('#' + sectionId).html('');

                $.each(data, function (i, val) {
                    var item = $('#ArticleTemplate').html();

                    $.each(val, function (key, value) {
                        item = item.replaceAll('{' + key + '}', value)
                    });

                    $('#' + sectionId).prepend(item);
                    countCheck += 1;
                })
                
                if (countCheck < count) {
                    $('#NextPage' + sectionId).hide();
                } else {
                    $('#NextPage' + sectionId).show();
                }
                if (page == 0) {
                    $('#PrevPage' + sectionId).hide();
                } else {
                    $('#PrevPage' + sectionId).show();
                }

                if (countCheck == 0) {
                    if (page == 0) {
                        var noArticles = $('#NoArticleTemplate').html();
                        $('#' + sectionId).html(noArticles);
                    } else {
                        var noMoreArticles = $('#NoMoreArticleTemplate').html()
                        $('#' + sectionId).html(noMoreArticles);
                    }
                }
            },
            error: function (data) {
                $("#ErrorMessage").html("There was an error pulling all your profile information, some data might be missing");
            }
        });
    }

    var counterCommented = 0;
    var counterLiked = 0;
    var counterAuthored = 0;

    var profileId = $("#ProfileId").val();

    getArticlePage(profileId, counterCommented, "/Users/GetUserCommentedArticles/", "MostCommented");
    getArticlePage(profileId, counterLiked, "/Users/GetUserLikedArticles/", "Liked");
    getArticlePage(profileId, counterAuthored, "/Users/GetUserArticles/", "Authored");

    $("#PrevPageMostCommented").click(function () {
        counterCommented -= 1;
        getArticlePage(profileId, counterCommented, "/Users/GetUserCommentedArticles/", "MostCommented");
    });
    $("#NextPageMostCommented").click(function () {
        counterCommented += 1;
        getArticlePage(profileId, counterCommented, "/Users/GetUserCommentedArticles/", "MostCommented");
    });
    $("#PrevPageLiked").click(function () {
        counterLiked -= 1;
        getArticlePage(profileId, counterLiked, "/Users/GetUserLikedArticles/", "Liked");
    });
    $("#NextPageLiked").click(function () {
        counterLiked += 1;
        getArticlePage(profileId, counterLiked, "/Users/GetUserLikedArticles/", "Liked");
    });
    $("#PrevPageAuthored").click(function () {
        counterAuthored -= 1;
        getArticlePage(profileId, counterAuthored, "/Users/GetUserArticles/", "Authored");
    });
    $("#NextPageAuthored").click(function () {
        counterAuthored += 1;
        getArticlePage(profileId, counterAuthored, "/Users/GetUserArticles/", "Authored");
    });
})