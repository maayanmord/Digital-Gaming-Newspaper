
$(function () {

    function viewEdit() {
        var editProperties = $("#EditTemplate").html();
        $("#ViewProperties").html(editProperties);
        var editAbout = $("#EditAboutTemplate").html();
        $("#EditAbout").html(editAbout);
    }

    function reloadCache() {
        $.ajax({
            url: "",
            context: document.body,
            success: function (result) {

                $('html[manifest=saveappoffline.appcache]').attr('content', '');
                $(this).html(result);
            }
        });
    }

    firstNameValid = true
    lastNameValid = true
    emailValid = true

    function updateButtonState() {
        if (firstNameValid && lastNameValid && emailValid) {
            $('#SaveChanges').prop('disabled', false);
        }
    }

    $("#EditButton").click(function () {
        viewEdit();
        $('#SaveChangesButton').click(reloadCache);

        $('#firstName').on('keyup', function () {
            if (!$('#firstName').val().match('^[A-Z][a-z]+$')) {
                firstNameValid = false;
                $('#firstNameMessage').html('First name must begin with capital letter and be only letters');
                $('#SaveChanges').prop('disabled', true);
            } else {
                firstNameValid = true;
                $('#firstNameMessage').html('');
                updateButtonState()
            }
        });

        $('#lastName').on('keyup', function () {
            if (!$('#lastName').val().match('^[A-Z][a-z]+$')) {
                lastNameValid = false;
                $('#lastNameMessage').html('Last name must begin with capital letter and be only letters');
                $('#SaveChanges').prop('disabled', true);
            } else {
                lastNameValid = true;
                $('#lastNameMessage').html('');
                updateButtonState()
            }
        });

        $('#email').on('keyup', function () {
            if (!$('#email').val().match('^[a-zA-Z0-9.!#$,;%&\' *"+\/=?^_`{|}~-]+@[a-zA-Z0-9-]+(?:\.[a-zA-Z0-9-]+)*$')) {
                emailValid = false;
                $('#emailMessage').html('must be valid email');
                $('#SaveChanges').prop('disabled', true);
            } else {
                emailValid = true;
                $('#emailMessage').html('');
                updateButtonState()
            }
        });
    });
    
    function getArticlePage(id, page, getUrl, sectionId) {
        count=5
        $.ajax({
            type: "GET",
            url: getUrl + id,
            data: {
                page: page,
                count: count,
                __RequestVerificationToken: gettoken()
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
                alert(data);
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