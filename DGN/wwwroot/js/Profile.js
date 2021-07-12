
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

    $('#editButton').click(reloadCache);
    
    function mostCommentedPage(id, page) {
        $.ajax({
            type: "POST",
            url: "/Users/GetUserCommentedArticles",
            data: {
                Id: id,
                page: page,
                count: 10,
                __RequestVerificationToken: gettoken()
            },
            success: function (data) {
                alert("here2");
                var item = $('#ArticleTemplate').html();

                $('#MostCommentedArticlePage').empty();

                var thereAreArticles = false;
                $.each(data, function (key, value) {
                    thereAreArticles = true;
                    item = item.replaceAll('{' + key + '}', value)
                });

                if (!thereAreArticles) {
                    if (page == 0) {
                        alert("0")
                        var noArticles = $('#NoArticleTemplate').html();
                        $('#MostCommentedArticlePage').prepend(noArticles);
                    } else {
                        var noMoreArticles = $('#NoMoreArticleTemplate').html()
                        $('#MostCommentedArticlePage').prepend(noMoreArticles);
                    }
                }
                else {
                    alert("there are articles")
                    $('#MostCommentedArticlePage').prepend(item);
                }
            },
            error: function (data) {
                alert("nanana");
                //$("#alert-body").show();
            }
        });
    }

    var counter = 0;
    var profileId = $("#ProfileId").val();
    mostCommentedPage(profileId, counter);


    $("#PrevPage").click(function () {
        counter -= 1;
        mostCommentedPage(profileId, counter);
    });
    $("#NextPage").click(function () {
        counter += 1;
        mostCommentedPage(profileId, counter);
    });

})