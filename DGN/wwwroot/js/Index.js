$(function () {
    var mostCommentedArticlesCount = 4;

    $.ajax({
        type: "GET",
        url: "/Articles/GetMostCommentedArticles",
        data: { "count": mostCommentedArticlesCount }
    }).done(function (data) {
        $('.best-articles').html('');

        var template = $('#mostCommentedArticlesTemplate').html();

        $.each(data, function (i, val) {
            var item = template;

            $.each(val, function (key, value) {
                item = item.replaceAll('{' + key + '}', value)
            });

            $('.best-articles').append(item);
        })
    });

    var mostLikedArticlesCount = 4;

    $.ajax({
        type: "GET",
        url: "/Articles/GetMostLikedArticles",
        data: { "count": mostLikedArticlesCount }
    }).done(function (data) {
        $('.hot-articles-list').html('');

        var template = $('#mostLikedArticlesTemplate').html();

        $.each(data, function (i, val) {
            var item = template;

            $.each(val, function (key, value) {
                item = item.replaceAll('{' + key + '}', value)
            });

            $('.hot-articles-list').append(item);
        })
    });
});