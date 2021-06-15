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
});